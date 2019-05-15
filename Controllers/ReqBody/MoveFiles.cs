using System.Collections.ObjectModel;

namespace OSApiInterface.Controllers.ReqBody
{
    public class MoveFiles
    {
        // Item path
        public string OldPath { get; set; }
        
        public string NewPath { get; set; }
        
        // the items we need to move
        public Collection<string> Items { get; set; }
        
        // the single file to be moved
        public string FileName { get; set; }
        
    }
}