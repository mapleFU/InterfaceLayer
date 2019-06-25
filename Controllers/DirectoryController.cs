using System;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OSApiInterface.Controllers.ReqBody;
using OSApiInterface.Models;

namespace OSApiInterface.Controllers
{
    
    [Route("api/directory")]
    [ApiController]
    public class DirectoryController : ControllerBase
    {
        private EntityCoreContext _context;

        public DirectoryController(EntityCoreContext dbContext)
        {
            _context = dbContext;
        }
        
//        /// <summary>
//        /// List file in a directory
//        /// if the path is null, turn to page "null"
//        /// TODO: add authority for it
//        /// </summary>
//        [HttpGet("{path}")]
//        public ActionResult<FileList> ListDir([FromRoute] string path)
//        {
//            
//            FileList fileList = new FileList();
//            fileList.Results = new Collection<FileEntity>();
//           
//            using (var context = _context)
//            {
//                var files = context.FileEntities.ToList();
//                foreach (var fe in files)
//                {
//                    fileList.Results.Add(fe);
//                }
//            }
//
//            return fileList;
//        }
        
       
    }
}