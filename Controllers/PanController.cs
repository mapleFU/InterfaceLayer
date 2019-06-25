using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OSApiInterface.Services;

namespace OSApiInterface.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PanController : ControllerBase
    {
        
        /// <summary>
        /// Traverse Directory
        /// If dirId is not given, then the sys will return the root directory.
        /// </summary>
        /// <param name="dirId"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        [Route("directory/{dirId}")]
        public async Task<IEnumerable<OssEntity>> TraverseDir([FromRoute] int? dirId)
        {
            
            throw new NotImplementedException(); 
        }
        
        
    }
}