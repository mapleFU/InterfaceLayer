using System;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using OSApiInterface.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using org.apache.zookeeper;

namespace OSApiInterface
{
    public class EntityCoreContext: DbContext
    {
        IConfiguration Configuration { get; }
                 
        public EntityCoreContext(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        
        public virtual DbSet<FileEntity> FileEntities { get; set; }
//        public virtual DbSet<FileDirectory> FileDirectories { get; set; }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            
            //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
            optionsBuilder.UseNpgsql(Configuration["ConnectionStrings:DevLocalDatabase"]);
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.Entity<FileEntity>()
                .Property(f => f.Id)
                .ValueGeneratedOnAdd();
            
            modelBuilder.Entity<FileEntity>().ToTable("file_entities")
                .HasKey(fe => fe.Id)
                .HasName("pk_file_entities");
            
            modelBuilder.Entity<FileEntity>().HasIndex(fe => fe.DirectoryId).HasName("idx_fe_directory");

            modelBuilder.Entity<FileEntity>().HasMany(fe => fe.Children)
                .WithOne(fe => fe.Directory)
                .HasForeignKey(fe => fe.DirectoryId);

            modelBuilder.Entity<FileEntity>().HasIndex(fe => fe.Name).HasName("idx_fe_name");
            
            // mother fuck
            modelBuilder.Entity<FileEntity>().HasData(
                new FileEntity
                {
                    DirectoryId = null,
                    Share = true,
                    Type = "folder",
                    Name = "/",
                    Id = -1
                }
            );

        }
    }
}
