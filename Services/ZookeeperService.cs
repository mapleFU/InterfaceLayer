using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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
            if (childrenResult.Children.Count == 0)
            {
                return null;
            }
            int randPos = rnd.Next(0, childrenResult.Children.Count);
            string chooseChild = childrenResult.Children[randPos];
//            _logger.LogInformation("Choose {} as child", chooseChild);
            // TODO: make clear if something bad happened, what will we do

            return await LoadServerByNameAsync(chooseChild);
        }

        public async Task<Server> LoadServerByNameAsync(string chooseChild)
        {
            var dataResult =  await _zk.getDataAsync("/fs/" + chooseChild, false);
            return Server.Parser.ParseFrom(dataResult.Data);
        }
        public async Task<IEnumerable<Server>> LoadChildServers()
        {
            // TODO: use parrell to handle this
            var childrenResult = await GetChildrenAsync();
//            Task.WaitAll(childrenResult.Children.Select(s => await LoadServerByNameAsync(s)));
           IEnumerable<Server> servers = new List<Server>();
            foreach (var s in childrenResult.Children)
            {
                servers.Append(await LoadServerByNameAsync(s));
            }

            return servers;

        }
    }
}