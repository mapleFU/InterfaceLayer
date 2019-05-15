using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OSApiInterface.Models
{
    public class FileEntity
    {
        // Id of the File
        [Key]
        public int Id { get; set; }
        
        public Int64 Global { get; set; }
        
        // name of the file
        public string Name { get; set; }
        
        // Type of the file, which can be 
        // folder or simple file 
        public string Type { get; set; }
        
        // True if the file is a directory
        public bool IsDirectory { get; set; }
        
        /// <summary>
        /// The directory of the file
        /// </summary>
        public int? DirectoryId { get; set; }
        
        /// <summary>
        /// Children of the directory
        /// </summary>
        public ICollection<FileEntity> Children { get; set; }
        
        public FileEntity Directory { get; set; }
        
        // Share/Private acl of the file
        public bool Share { get; set; }
    }
}