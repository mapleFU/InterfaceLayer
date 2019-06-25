using System.Threading.Tasks;

namespace OSApiInterface.Services
{
    public interface IUserService
    {
        /// <summary>
        /// User register api
        /// </summary>
        /// <param name="email">Email of </param>
        /// <param name="password"></param>
        /// <returns></returns>
        Task<User> Register(string email, string password);
        
        /// <summary>
        /// Check if the User exists in the system.
        /// </summary>
        /// <param name="email">The email of the user</param>
        /// <returns></returns>
        Task<bool> ExistsUserWithEmail(string email);

        /// <summary>
        /// Get User by token
        /// </summary>
        /// <param name="token">Token</param>
        /// <returns></returns>
        Task<User> FindUserByToken(string token); 
            
        /// <summary>
        /// Return token
        /// If string is null, then login failed.
        /// It will be used if a user exists
        /// </summary>
        /// <returns></returns>
        Task<string> Login(string email, string password);


        /// <summary>
        /// Find User by email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        Task<User> FindUserBymail(string email);
    }
}