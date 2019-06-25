namespace OSApiInterface.Services
{
    public class OssEntity
    {
        public int OssEntityId { get; set; }
        /// <summary>
        /// ID of the FileMeta
        /// Can be null IsDirectory is true
        /// </summary>
        public string ObjectId { get; set; }
        
        /// <summary>
        /// Name of the file
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Path of the file, contains an Index
        /// </summary>
        public string Path { get; set; }
        
        public bool IsDirectory { get; set; }
        
        /// <summary>
        /// Id of the User
        /// </summary>
        public int UserId { get; set; }
        
        /// <summary>
        /// The access authority of the file.
        /// 0: Only the User of this tool can use it.
        /// 1: All user can see the link.
        /// </summary>
        public int Access { get; set; }
    }
}