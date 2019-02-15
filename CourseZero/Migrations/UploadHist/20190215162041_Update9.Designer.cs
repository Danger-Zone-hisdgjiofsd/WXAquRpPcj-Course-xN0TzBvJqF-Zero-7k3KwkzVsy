﻿// <auto-generated />
using System;
using CourseZero.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CourseZero.Migrations.UploadHist
{
    [DbContext(typeof(UploadHistContext))]
    [Migration("20190215162041_Update9")]
    partial class Update9
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.1-servicing-10028")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("CourseZero.Models.UploadHist", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("File_typename");

                    b.Property<bool>("Processed");

                    b.Property<int>("Processed_FileID");

                    b.Property<bool>("Processed_Success");

                    b.Property<DateTime>("Upload_Time");

                    b.Property<int>("Uploader_UserID");

                    b.HasKey("ID");

                    b.ToTable("UploadHistories");
                });
#pragma warning restore 612, 618
        }
    }
}
