using System.ComponentModel.DataAnnotations.Schema;

namespace OSApiInterface.Models
{
    /// <summary>
    /// This class is a Relational class
    /// Represents the relationship between the file and directory
    /// </summary>
    public class FileDirectory
    {
        // the directory of the file
        [ForeignKey("Directory")]
        public int DirectoryId { get; set; }
        
        public FileEntity Directory { get; set; }
        
        // the child of the file
        [ForeignKey("Child")]
        public int ChildId { get; set; }
        
        public FileEntity Child { get; set; }
    }
}