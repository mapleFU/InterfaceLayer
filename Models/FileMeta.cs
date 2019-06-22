using System;
using System.ComponentModel.DataAnnotations;


namespace OSApiInterface.Models
{
    public class FileMeta
    {
        // 全局的 uuid
        [Key]
        public string Global { get; set; }
        
        // MIME Type
        public string Mime { get; set; }
        
        // Size of the file
        public Int64 Size { get; set; }
        
        public string Checksum { get; set; }
        
        // ACL List of the file
        public string Acl { get; set; }
        
        // Version of the FileMeta, default(not use) is zero 
        public Int64 Version { get; set; }
        
        // Date to update the version
        public DateTime Date { get; set; }
    }
}