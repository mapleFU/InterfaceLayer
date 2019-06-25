using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Extensions.Internal;
using OSApiInterface.Models;

namespace OSApiInterface.Services
{
    public class FileMetaService: IFileMetaService
    {
        private EntityCoreContext _context;

        public FileMetaService(EntityCoreContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Load Meta with objectId of file
        /// </summary>
        /// <param name="objectId"></param>
        /// <returns></returns>
        public async Task<FileMeta> LoadMetaWithId(string objectId)
        {
//            return await _context.Fi.FindAsync(objectId);
            return await _context.FileMetas.FindAsync(objectId);
        }

        public async Task<bool> ExistsDataWithId(string objectId)
        {
            return await _context.FileMetas.AnyAsync(o => o.Global == objectId);
        }

        public async Task<bool> ExistsDataWithHash(string shaHash)
        {
            return await _context.FileMetas.AnyAsync(o => o.Checksum == shaHash);
        }


        public async Task<FileMeta> CreateFileMeta(string objectId, Int64 size, 
            string checksum, string fileContentType)
        {
            var currentMeta = new FileMeta();
            currentMeta.Date = DateTime.Now;
            currentMeta.Size = size;
            currentMeta.Global = objectId;
            currentMeta.Mime = fileContentType;
            currentMeta.Checksum = checksum;
            // TODO: set these
//            currentMeta.Acl = "allow-all";
            currentMeta.Version = 0;
            
            
            await _context.FileMetas.AddAsync(currentMeta);
            await _context.SaveChangesAsync();
            return currentMeta;

        }

        public async Task<IEnumerable<FileMeta>> LoadFileMetaAsync(int page, int pageSize = 10)
        {
            return await _context.FileMetas.OrderBy(t => t.Date).Skip(page * pageSize).ToListAsync();
        }
    }
}