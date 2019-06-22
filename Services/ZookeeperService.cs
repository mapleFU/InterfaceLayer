using System;
using System.Threading.Tasks;
using org.apache.zookeeper;

namespace OSApiInterface.Services
{
    public class ZookeeperService: IZookeeperService
    {
        private readonly ZooKeeper _zk;
        private Random rnd = new Random();
        
        public ZookeeperService(ZooKeeper zk)
        {
            _zk = zk;
        }
        
        public async Task<ChildrenResult> GetChildrenAsync()
        {
            return await _zk.getChildrenAsync("/fs");
        }

        public async Task<Server> RandomChooseChildrenAsync()
        {
            var childrenResult = await GetChildrenAsync(); 
            int randPos = rnd.Next(0, childrenResult.Children.Count);
            string chooseChild = childrenResult.Children[randPos];
//            _logger.LogInformation("Choose {} as child", chooseChild);
            // TODO: make clear if something bad happened, what will we do
            var dataResult = await _zk.getDataAsync("/fs/" + chooseChild, false);
            return Server.Parser.ParseFrom(dataResult.Data);
        }
    }
}