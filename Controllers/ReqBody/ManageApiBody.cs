namespace OSApiInterface.Controllers.ReqBody
{
    public class ManageApiBody
    {
        // the action the method will do
        public string Action { get; set; }
        
        // list : the path of the dir to List
        public string Path { get; set; }
        
    }
}