using System.Collections.ObjectModel;
using OSApiInterface.Models;

namespace OSApiInterface.Controllers.ReqBody
{
    /// <summary>
    /// Return of GetFile, which will List all the file in a Root
    /// </summary>
    public class FileList
    {
        /// <summary>
        /// Result List of all the file entity
        /// </summary>
        public Collection<FileEntity> Results { get; set; }
    }
}