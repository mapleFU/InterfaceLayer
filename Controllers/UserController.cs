using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace OSApiInterface.Controllers
{
    public class UserInput
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
    
    [Route("api/[controller]")]
    [ApiController]
    public class UserController
    {
        
        [Route("login")]
        public async void Login()
        {
            throw new NotImplementedException();
        }

        [Route("register")]
        public async void Register([FromBody] UserInput userInput)
        {
            
            throw new NotImplementedException();
        }
    }
}