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
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Primitives;
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

            
            var parsedMsg = Locate(objectId);
            if (parsedMsg == null)
            {
                return NotFound();
            }
            
            var httpClient = _httpClientFactory.CreateClient();
            var httpUrl = new UriBuilder("http", parsedMsg.Host, System.Convert.ToUInt16(parsedMsg.HttpPort), "data/" + objectId);
            
            var resp = await httpClient.GetStreamAsync(httpUrl.ToString());
            
            return File(resp, "application/octet-stream");
        }
        
        private Server Locate(string objectId) 
        {
            _publisherSocket.SendMoreFrame("object ").SendFrame(objectId);
            
//            return NotFound();
            byte[] msgData;
            
            if (!_pullSocket.TryReceiveFrameBytes(TimeSpan.FromSeconds(5), out msgData))
            {
                return null;
            }

            return Server.Parser.ParseFrom(msgData);
        }

        [Route("object/{objectId?}")]
        [HttpPut]
        public async Task<string> PutFileBody(IFormFile file, string objectId)
        {
            if (await _fileMetaService.ExistsDataWithId(objectId))
            {
                return "Duplicated";
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
                        digest = Convert.ToBase64String(SHA256.ComputeHash(fstream));
                }
                infileStream = file.OpenReadStream();
            }

            
            
            Int64 length = (Int64) file.Length;
            string fileContentType = file.ContentType;
            await _fileMetaService.CreateFileMeta(objectId, length, digest, fileContentType);
            Server server = await _zkService.RandomChooseChildrenAsync();
            var url = new System.UriBuilder("http", server.Host, System.Convert.ToUInt16(server.HttpPort), "/data/" + objectId);

            var httpClient = _httpClientFactory.CreateClient();
            if ((await httpClient.PutAsync(url.ToString(), new StreamContent(infileStream))).IsSuccessStatusCode)
            {
                return "";
            }
            else
            {
                return "Fault";
            }
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

//            using (SHA256 SHA256 = SHA256Managed.Create())
//            {
//                using (Stream fileStream = infileStream)
//                    return Convert.ToBase64String(SHA256.ComputeHash(fileStream));
//            }           
//            Console.WriteLine(name);
//            string ret = "";
//            var fileStream = System.IO.File.Create(name);
//            var childrenResult = await _zk.getChildrenAsync("/fs");
//            
//            _logger.LogInformation("{}", childrenResult.Children.Count);
//
//            int randPos = rnd.Next(0, childrenResult.Children.Count);
//            string chooseChild = childrenResult.Children[randPos];
//            _logger.LogInformation("Choose {} as child", chooseChild);
//            // TODO: make clear if something bad happened, what will we do
//            var dataResult = await _zk.getDataAsync("/fs/" + chooseChild, false);
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