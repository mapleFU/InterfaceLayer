using System;
using System.Threading.Tasks;
using OSApiInterface.Models;

namespace OSApiInterface.Services
{
    public interface IFileMetaService
    {
        Task<FileMeta> LoadMetaWithId(string objectId);

        /// <summary>
        /// Create FileMeta and store it to database
        /// </summary>
        /// <param name="objectId">MetaID of object</param>
        /// <param name="size">Size of object</param>
        /// <param name="checksum">SHA-512 of object</param>
        /// <param name="fileContentType"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        Task<FileMeta> CreateFileMeta(string objectId, Int64 size,
            string checksum, string fileContentType);

        /// <summary>
        /// check if data exists with the uid
        /// </summary>
        /// <param name="objectId">The uid of the object in a bucket</param>
        /// <returns>If the data exists</returns>
        Task<bool> ExistsDataWithId(string objectId);

        /// <summary>
        /// Return if the storage system exists the hashCode
        /// </summary>
        /// <param name="shaHash"></param>
        /// <returns></returns>
        Task<bool> ExistsDataWithHash(string shaHash);
    }
}