namespace OSApiInterface.Services
{
    public class User
    {
        /// <summary>
        /// Id of the User
        /// </summary>
        public int UserId { get; set; }
        
        /// <summary>
        /// Email of the user
        /// </summary>
        public string Email { get; set; }
        
        /// <summary>
        /// Password Hash of the user    
        /// </summary>
        public string PasswordHash { get; set; }
    }
}