namespace OSApiInterface.Controllers.ReqBody
{
    public class Rename
    {
        // the path of the file
        public string Path { get; set; }
        
        // the old file name
        public string FileName { get; set; }
        
        // the new file name 
        public string NewFileName { get; set; }
        
        
    }
}