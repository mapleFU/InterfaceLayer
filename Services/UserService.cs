using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

namespace OSApiInterface.Services
{
    public class UserService: IUserService
    {
        private EntityCoreContext _context;
        private ConnectionMultiplexer _redis;
        
        public UserService(EntityCoreContext context, ConnectionMultiplexer redis)
        {
            _context = context;
            _redis = redis;
        }
        
        static string ComputeSha256Hash(string rawData)  
        {  
            // Create a SHA256   
            using (SHA256 sha256Hash = SHA256.Create())  
            {  
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));  
  
                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();  
                for (int i = 0; i < bytes.Length; i++)  
                {  
                    builder.Append(bytes[i].ToString("x2"));  
                }  
                return builder.ToString();  
            }  
        }


        public async Task<User> Register(string email, string password)
        {
            var usr = new User();
            usr.Email = email;
            usr.PasswordHash = ComputeSha256Hash(password);
            await _context.Users.AddAsync(usr);
            await _context.SaveChangesAsync();
            return usr;
        }

        public async Task<bool> ExistsUserWithEmail(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }

        public async Task<User> FindUserByToken(string token)
        {
            var redisVal = await _redis.GetDatabase().StringGetAsync(token);
            if (redisVal.IsNullOrEmpty)
            {
                // User None Exists
                return null;
            }

            var email = redisVal.ToString();
            User usr = await FindUserBymail(email);
            return usr;
        }

        
        public async Task<string> Login(string email, string password)
        {
            // find user by email
            User usr = await FindUserBymail(email);
            if (usr == null)
            {
                // User Non exists
                return null;
            }

            if (usr.PasswordHash != ComputeSha256Hash(password))
            {
                // Password vertify error
                return null;
            }

            string token = Guid.NewGuid().ToString();
            // set email pair
            // TODO: make we need to set an expire
            await _redis.GetDatabase().StringSetAsync(token, usr.Email);
            return token;
        }

        public async Task<User> FindUserBymail(string email)
        {
            // TODO: make clear if this is ok
            return await _context.Users.FindAsync(email);
        }
    }
}