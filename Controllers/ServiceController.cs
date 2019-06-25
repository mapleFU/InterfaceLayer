using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OSApiInterface.Models;
using OSApiInterface.Services;

namespace OSApiInterface.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceController : ControllerBase
    {
        /// <summary>
        /// Get status of the file
        /// </summary>
        /// <returns></returns>
        [Route("status")]
        [HttpGet]
        private async Task<IEnumerable<Server>> GetFileStatus()
        {
            var childrenResult = await _zkService.GetChildrenAsync();
            var resultList = new List<Server>();
            foreach (var childrenName in childrenResult.Children)
            {
                resultList.Add(await _zkService.LoadServerByNameAsync(childrenName));
            }

            return resultList;
        }

        private IZookeeperService _zkService;
        public ServiceController(IZookeeperService zkService)
        {
            _zkService = zkService;
        }
        
        /// <summary>
        /// Get status of the file
        /// </summary>
        /// <returns></returns>
        [Route("status/")]
        [HttpGet]
        public async Task<IEnumerable<Server>> GetAllStatus()
        {
            var childrenResult = await _zkService.GetChildrenAsync();
            var resultList = new List<Server>();
            foreach (var childrenName in childrenResult.Children)
            {
                resultList.Add(await _zkService.LoadServerByNameAsync(childrenName));
            }

            return resultList;
        }
    }
}