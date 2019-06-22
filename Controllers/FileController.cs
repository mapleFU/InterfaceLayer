using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NetMQ;
using NetMQ.Sockets;
using org.apache.zookeeper;
using System.Net.Http;

using OSApiInterface.Controllers.ReqBody;
using OSApiInterface.Models;

namespace OSApiInterface.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private ZooKeeper _zk;
        private Random rnd = new Random();
        private readonly ILogger _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private PublisherSocket _publisherSocket;
        private PullSocket _pullSocket;
        
        public FileController(ZooKeeper zk, ILogger<FileController> logger, IHttpClientFactory httpClientFactory, 
            PublisherSocket publisherSocket, PullSocket pullSocket)
        {
            _zk = zk;
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _publisherSocket = publisherSocket;
            _pullSocket = pullSocket;
        }

        [Route("object/{objectId}")]
        [HttpGet]
        public async Task<IActionResult> GetFile([FromRoute] string objectId)
        {
            
            _publisherSocket.SendMoreFrame("object ").SendFrame(objectId);
            
//            return NotFound();
            byte[] msgData;
            
            if (!_pullSocket.TryReceiveFrameBytes(TimeSpan.FromSeconds(5), out msgData))
            {
                return NotFound();
            }

            var parsedMsg = Server.Parser.ParseFrom(msgData);
            var httpClient = _httpClientFactory.CreateClient();
            var httpUrl = new UriBuilder("http", parsedMsg.Host, System.Convert.ToUInt16(parsedMsg.HttpPort), "data/" + objectId);
            
            var resp = await httpClient.GetStreamAsync(httpUrl.ToString());
            
            return File(resp, "application/octet-stream");
        }
        
        private void Locate()
        {
            
        }
        
        [Route("object/{objectId?}")]
        [HttpPut]
        public async Task<string> PostProfilePicture(IFormFile file, [FromRoute] string objectId)
        {
            var infileStream = file.OpenReadStream();
            if (objectId == null)
            {
                objectId = file.FileName;
            }
//            Console.WriteLine(name);
//            string ret = "";
//            var fileStream = System.IO.File.Create(name);
            var childrenResult = await _zk.getChildrenAsync("/fs");
           
            _logger.LogInformation("{}", childrenResult.Children.Count);

            int randPos = rnd.Next(0, childrenResult.Children.Count);
            string chooseChild = childrenResult.Children[randPos];
            _logger.LogInformation("Choose {} as child", chooseChild);
            // TODO: make clear if something bad happened, what will we do
            var dataResult = await _zk.getDataAsync("/fs/" + chooseChild, false);
            var server = Server.Parser.ParseFrom(dataResult.Data);
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