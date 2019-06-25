﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using OSApiInterface;

namespace OSApiInterface.Migrations
{
    [DbContext(typeof(EntityCoreContext))]
    [Migration("20190624224152_deprecated FileEntity and add OssEntity and User")]
    partial class deprecatedFileEntityandaddOssEntityandUser
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .HasAnnotation("ProductVersion", "2.1.0-rtm-30799")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("OSApiInterface.Models.FileMeta", b =>
                {
                    b.Property<string>("Global")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Acl");

                    b.Property<string>("Checksum");

                    b.Property<DateTime>("Date");

                    b.Property<string>("Mime");

                    b.Property<long>("Size");

                    b.Property<long>("Version");

                    b.HasKey("Global")
                        .HasName("pk_fm_global");

                    b.HasIndex("Checksum")
                        .HasName("idx_checksum");

                    b.ToTable("file_metas");
                });

            modelBuilder.Entity("OSApiInterface.Services.OssEntity", b =>
                {
                    b.Property<int>("OssEntityId")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("IsDirectory");

                    b.Property<string>("Name");

                    b.Property<string>("ObjectId");

                    b.Property<string>("Path");

                    b.Property<int>("UserId");

                    b.HasKey("OssEntityId")
                        .HasName("pk_oss_id");

                    b.HasIndex("ObjectId");

                    b.HasIndex("Path")
                        .HasName("idx_oss_path");

                    b.HasIndex("UserId");

                    b.ToTable("OssEntities");
                });

            modelBuilder.Entity("OSApiInterface.Services.User", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Email");

                    b.Property<string>("PasswordHash");

                    b.HasKey("UserId")
                        .HasName("pk_user_id");

                    b.HasIndex("Email")
                        .IsUnique()
                        .HasName("idx_user_email");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("OSApiInterface.Services.OssEntity", b =>
                {
                    b.HasOne("OSApiInterface.Models.FileMeta")
                        .WithMany()
                        .HasForeignKey("ObjectId");

                    b.HasOne("OSApiInterface.Services.User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
