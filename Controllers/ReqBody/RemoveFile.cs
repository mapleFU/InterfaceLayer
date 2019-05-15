using System.Collections.ObjectModel;

namespace OSApiInterface.Controllers.ReqBody
{
    public class RemoveFile
    {
        public string Path { get; set; }
        
        public Collection<string> Items { get; set; }
    }
}