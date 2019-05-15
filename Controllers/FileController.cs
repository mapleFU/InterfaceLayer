using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OSApiInterface.Controllers.ReqBody;
using OSApiInterface.Models;

namespace OSApiInterface.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {

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
            var fileStream = System.IO.File.Create(name);
//            myOtherObject.InputStream.Seek(0, SeekOrigin.Begin);
            await stream.CopyToAsync(fileStream);
            fileStream.Close();

            return name;
        }
    }
    
}