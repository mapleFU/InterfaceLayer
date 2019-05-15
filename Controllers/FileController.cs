using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NetMQ;
using NetMQ.Sockets;
using org.apache.zookeeper;
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

        public FileController(ZooKeeper zk, ILogger<FileController> logger)
        {
            _zk = zk;
            _logger = logger;
        }

        [HttpGet]
        [Route("hey")]
        public ActionResult<string> Hey()
        {
            return "hey";
        }

        [Route("upload")]
        [HttpPost]
        public async Task<string> PostProfilePicture(IFormFile file)
        {
            var stream = file.OpenReadStream();
            var name = file.FileName;
            Console.WriteLine(name);
            string ret = "";
            var fileStream = System.IO.File.Create(name);
            var childrenResult = await _zk.getChildrenAsync("/fs");
           
            _logger.LogInformation("{}", childrenResult.Children.Count);

            int randPos = rnd.Next(0, childrenResult.Children.Count);
            string chooseChild = childrenResult.Children[randPos];
            _logger.LogInformation("Choose {} as child", chooseChild);
            // TODO: make clear if something bad happened, what will we do
            var dataResult = await _zk.getDataAsync("/fs/" + chooseChild, false);
            var server = Server.Parser.ParseFrom(dataResult.Data);
            _logger.LogInformation("{0}:{1}", server.ZmqHost, server.ZmqTcpPort);
            using (var client = new RequestSocket(string.Format("tcp://{0}:{1}", server.ZmqHost, server.ZmqTcpPort)))  // connect
            {
                // TODO: send file here
            }
            
            
            await stream.CopyToAsync(fileStream);
            fileStream.Close();

            return ret;
        }
    }
    
}