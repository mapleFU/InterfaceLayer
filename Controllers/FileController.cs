using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NetMQ;
using NetMQ.Sockets;
using org.apache.zookeeper;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using OSApiInterface.Controllers.ReqBody;
using OSApiInterface.Models;
using OSApiInterface.Services;

namespace OSApiInterface.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private PublisherSocket _publisherSocket;
        private PullSocket _pullSocket;
        private IFileMetaService _fileMetaService;
        private IZookeeperService _zkService;
        
        public FileController(ZooKeeper zk, ILogger<FileController> logger, IHttpClientFactory httpClientFactory, 
            PublisherSocket publisherSocket, PullSocket pullSocket, IFileMetaService fileMetaService, IZookeeperService zkService)
        {
            _zkService = zkService;
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _publisherSocket = publisherSocket;
            _pullSocket = pullSocket;
            _fileMetaService = fileMetaService;
        }

        
        
        [Route("object/{objectId}")]
        [HttpGet]
        public async Task<IActionResult> GetFile([FromRoute] string objectId)
        {
            _logger.LogInformation("Call GetFile with objectId {}", objectId);
            var meta = await _fileMetaService.LoadMetaWithId(objectId);
            if (meta == null)
            {
                return NotFound(objectId);
            }
            
            var parsedMsg = await Locate(meta.Checksum);
            if (parsedMsg == null)
            {
                return NotFound(string.Format("Locate not find file with checksum {0}", meta.Checksum));
            }
            
            var httpClient = _httpClientFactory.CreateClient();
            var httpUrl = new UriBuilder("http", parsedMsg.Host, System.Convert.ToUInt16(parsedMsg.HttpPort), "data/" + meta.Checksum);
            
            var resp = await httpClient.GetStreamAsync(httpUrl.ToString());
            
            return File(resp, "application/octet-stream");
        }
        
        private async Task<Server> Locate(string checksum) 
        {
//            for (int i = 0; i < 10; i++)
//            {
//                _publisherSocket.SendMoreFrame("hash").SendFrame(checksum);    
//            }
////            return NotFound();
//            byte[] msgData;
//            if (!_pullSocket.TryReceiveFrameBytes(TimeSpan.FromSeconds(5), out msgData))
//            {
//                return null;
//            }
//            return Server.Parser.ParseFrom(msgData);
            var childrenResult = await _zkService.GetChildrenAsync();
            var httpClient = _httpClientFactory.CreateClient();
            foreach (var child in childrenResult.Children)
            {
                var parsedMsg = await _zkService.LoadServerByNameAsync(child);
                var httpUrl =  new UriBuilder("http", parsedMsg.Host, System.Convert.ToUInt16(parsedMsg.HttpPort), "data/exists/" + checksum);
                var resp = await httpClient.GetAsync(httpUrl.ToString());
                if (resp.IsSuccessStatusCode)
                {
                    return parsedMsg;
                }
            }

            return null;
        }

        /// <summary>
        /// Upload File with correspond file and get result
        /// TODO: encapsulate the total procedure as a transaction, and cache the hashcode of the file to avoid duplicate upload
        /// </summary>
        /// <param name="file"></param>
        /// <param name="objectId"></param>
        /// <returns></returns>
        [Route("object/{objectId?}")]
        [HttpPut]
        public async Task<IActionResult> PutFileBody(IFormFile file, string objectId)
        {
            if (await _fileMetaService.ExistsDataWithId(objectId))
            {
                // TODO: considering to create a new version here.
                return BadRequest(new
                {
                    error = "exists object id,"
                });
            }
            
            var infileStream = file.OpenReadStream();
            if (objectId == null)
            {
                objectId = file.FileName;
            }

            // Get Digest in Header
            string digest;
            StringValues stringValue;
            if (HttpContext.Request.Headers.TryGetValue("Digest", out stringValue))
            {
                digest = stringValue.ToString();
            }
            else
            {
                using (SHA256 SHA256 = SHA256Managed.Create())
                {

                    using (Stream fstream = infileStream)
                    {
                        digest = Convert.ToBase64String(SHA256.ComputeHash(fstream));
                    }
                        
                }
                infileStream = file.OpenReadStream();
            }

            bool ifUpload = await _fileMetaService.ExistsDataWithHash(digest);
            
            Int64 length = (Int64) file.Length;
            string fileContentType = file.ContentType;
            await _fileMetaService.CreateFileMeta(objectId, length, digest, fileContentType);
            Server server = await _zkService.RandomChooseChildrenAsync();

            // The hash code not exists and need to be upload
            if (ifUpload)
            {
                var url = new System.UriBuilder("http", server.Host, System.Convert.ToUInt16(server.HttpPort), "/temp/" + digest);

                var httpClient = _httpClientFactory.CreateClient();
                var stringPayload = await Task.Run(() => JsonConvert.SerializeObject(
                new {
                    size = length,
                }));
                var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");
                // Post first
                var resp = await httpClient.PostAsync(url.ToString(), httpContent);
                if (!resp.IsSuccessStatusCode)
                {
                    return BadRequest(resp.StatusCode.ToString());

                }
                string uid = await resp.Content.ReadAsStringAsync();
                _logger.LogInformation("Receive uuid {}", uid);
                
                var uid_url = new System.UriBuilder("http", server.Host, System.Convert.ToUInt16(server.HttpPort), "/temp/" + uid);
                // Patch after post with uuid
                resp = await httpClient.PatchAsync(uid_url.ToString(), new StreamContent(infileStream));
                if (!resp.IsSuccessStatusCode)
                {
                    return BadRequest(await resp.Content.ReadAsStringAsync());
//                    throw new HttpRequestException(await resp.Content.ReadAsStringAsync());
                }
                // Finally Put
                await httpClient.PutAsync(uid_url.ToString(), new StringContent(""));
                
                _logger.LogInformation("Upload to storage layer done.");
            }
            else
            {
                _logger.LogInformation("Didn't upload to storage layer.");
            }

            return Ok();

        }
        
        [Route("object/form/{objectId?}")]
        [HttpPut]
        public async Task<string> PostProfilePicture(IFormFile file, [FromRoute] string objectId)
        {
            var infileStream = file.OpenReadStream();
            if (objectId == null)
            {
                objectId = file.FileName;
            }

            var server = await _zkService.RandomChooseChildrenAsync();
            _logger.LogInformation("Host {0}: http_port {1}; zmq port {2}", server.Host, server.HttpPort, server.ZmqTcpPort);

            

            var url = new System.UriBuilder("http", server.Host, System.Convert.ToUInt16(server.HttpPort), "/data/" + objectId);

            var httpClient = _httpClientFactory.CreateClient();
            if ((await httpClient.PutAsync(url.ToString(), new StreamContent(infileStream))).IsSuccessStatusCode)
            {
                return "";
            }
            else
            {
                _logger.LogInformation("Something bad happened");
                return "";
            }
            
        }
        
        
    }
    
}