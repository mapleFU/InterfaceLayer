using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NetMQ;
using NetMQ.Sockets;
using org.apache.zookeeper;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using Google.Protobuf.WellKnownTypes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OSApiInterface.Services;

namespace OSApiInterface.Controllers
{
    public class UserInput
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
    
    [Route("api/[controller]")]
    [ApiController]
    public class UserController: ControllerBase
    {
        private IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }
        
        [Route("login")]
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] UserInput userInput)
        {
            var usr = await _userService.FindUserBymail(userInput.Email);
            if (usr == null)
            {
                
                return NotFound("User with email not found");
            }

            string s = await _userService.Login();
            if (s == null)
            {
                return BadRequest("User password error.");
            }

            return Ok(s);
        }

        [Route("register")]
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] UserInput userInput)
        {

            if (await _userService.ExistsUserWithEmail(userInput.Email))
            {
                return BadRequest("user with email already exists");
            }

            return Ok(_userService.Register(userInput.Email, userInput.Password));
        }
    }
}