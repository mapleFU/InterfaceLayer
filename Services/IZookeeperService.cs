using System.Threading.Tasks;
using org.apache.zookeeper;

namespace OSApiInterface.Services
{
    public interface IZookeeperService
    {
        Task<ChildrenResult> GetChildrenAsync();
        Task<Server> RandomChooseChildrenAsync();
    }
}