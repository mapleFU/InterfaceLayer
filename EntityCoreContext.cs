using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OSApiInterface.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using org.apache.zookeeper;
using OSApiInterface.Services;

namespace OSApiInterface
{
    public class EntityCoreContext: DbContext
    {
        IConfiguration Configuration { get; }
                 
        public EntityCoreContext(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        
//        public virtual DbSet<FileEntity> FileEntities { get; set; }
        public virtual DbSet<FileMeta> FileMetas { get; set; }
        
        public virtual DbSet<OssEntity> OssEntities { get; set; }
        
        public virtual DbSet<User> Users { get; set; }
//        public virtual DbSet<FileDirectory> FileDirectories { get; set; }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            
            //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
            optionsBuilder.UseNpgsql(Configuration["ConnectionStrings:DevLocalDatabase"]);
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
//            modelBuilder.Entity<FileEntity>()
//                .Property(f => f.Id)
//                .ValueGeneratedOnAdd();
//            
//            modelBuilder.Entity<FileEntity>().ToTable("file_entities")
//                .HasKey(fe => fe.Id)
//                .HasName("pk_file_entities");
//            
//            modelBuilder.Entity<FileEntity>().HasIndex(fe => fe.DirectoryId).HasName("idx_fe_directory");
//            
//
//            modelBuilder.Entity<FileEntity>().HasMany(fe => fe.Children)
//                .WithOne(fe => fe.Directory)
//                .HasForeignKey(fe => fe.DirectoryId);
//
//            modelBuilder.Entity<FileEntity>().HasIndex(fe => fe.Name).HasName("idx_fe_name");
//            
//            // mother fuck
//            modelBuilder.Entity<FileEntity>().HasData(
//                new FileEntity
//                {
//                    DirectoryId = null,
//                    Share = true,
//                    Type = "folder",
//                    Name = "/",
//                    Id = -1
//                }
//            );

            

            modelBuilder.Entity<FileMeta>()
                .Property(f => f.Global)
                .ValueGeneratedOnAdd();
            
            modelBuilder.Entity<FileMeta>().ToTable("file_metas")
                .HasKey(fe => fe.Global)
                .HasName("pk_fm_global");
            
            modelBuilder.Entity<FileMeta>().HasIndex(e => e.Checksum).HasName("idx_checksum");


            modelBuilder.Entity<User>()
                .Property(f => f.UserId)
                .ValueGeneratedOnAdd();
            
            modelBuilder.Entity<User>().HasKey(u => u.UserId).HasName("pk_user_id");

            modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique().HasName("idx_user_email");


            modelBuilder.Entity<OssEntity>()
                .Property(oss => oss.OssEntityId).ValueGeneratedOnAdd();
            
            modelBuilder.Entity<OssEntity>().HasKey(u => u.OssEntityId).HasName("pk_oss_id");

            modelBuilder.Entity<OssEntity>().HasIndex(p => p.Path).HasName("idx_oss_path");

            modelBuilder.Entity<OssEntity>().HasOne<FileMeta>().WithMany().HasForeignKey(oe => oe.ObjectId);

            modelBuilder.Entity<OssEntity>().HasOne<User>().WithMany().HasForeignKey(oe => oe.UserId);
        }
    }
}
