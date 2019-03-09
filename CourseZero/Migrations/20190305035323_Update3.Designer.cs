﻿// <auto-generated />
using System;
using CourseZero.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CourseZero.Migrations
{
    [DbContext(typeof(AllDbContext))]
    [Migration("20190305035323_Update3")]
    partial class Update3
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.1-servicing-10028")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("CourseZero.Models.AuthToken", b =>
                {
                    b.Property<string>("Token")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Last_access_Browser");

                    b.Property<string>("Last_access_Device");

                    b.Property<string>("Last_access_IP");

                    b.Property<string>("Last_access_Location");

                    b.Property<DateTime>("Last_access_Time");

                    b.Property<int>("userID");

                    b.HasKey("Token");

                    b.ToTable("AuthTokens");
                });

            modelBuilder.Entity("CourseZero.Models.Course", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Course_Code");

                    b.Property<string>("Course_Title");

                    b.Property<string>("Prefix");

                    b.Property<string>("Subject_Name");

                    b.HasKey("ID");

                    b.ToTable("Courses");
                });

            modelBuilder.Entity("CourseZero.Models.FileComment", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Text")
                        .HasMaxLength(2048);

                    b.Property<int>("file_ID");

                    b.Property<DateTime>("posted_dateTime");

                    b.Property<int>("sender_UserID");

                    b.HasKey("ID");

                    b.HasIndex("file_ID");

                    b.ToTable("FileComments");
                });

            modelBuilder.Entity("CourseZero.Models.ProfileComment", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Text")
                        .HasMaxLength(2048);

                    b.Property<DateTime>("posted_dateTime");

                    b.Property<int>("receiver_UserID");

                    b.Property<int>("sender_UserID");

                    b.HasKey("ID");

                    b.HasIndex("receiver_UserID");

                    b.ToTable("ProfileComments");
                });

            modelBuilder.Entity("CourseZero.Models.Rating", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("fileID");

                    b.Property<int>("userID");

                    b.Property<int>("user_Rating");

                    b.HasKey("ID");

                    b.HasIndex("userID", "fileID")
                        .IsUnique();

                    b.ToTable("Ratings");
                });

            modelBuilder.Entity("CourseZero.Models.Report", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("RelatedID");

                    b.Property<DateTime>("ReportTime");

                    b.Property<int>("Report_Type");

                    b.Property<bool>("Resovled");

                    b.Property<string>("Text")
                        .HasMaxLength(10240);

                    b.Property<int>("UserID");

                    b.HasKey("ID");

                    b.HasIndex("Report_Type");

                    b.ToTable("Reports");
                });

            modelBuilder.Entity("CourseZero.Models.Subscription", b =>
                {
                    b.Property<int>("UserID");

                    b.Property<int>("CourseID");

                    b.HasKey("UserID", "CourseID");

                    b.HasIndex("UserID");

                    b.ToTable("Subscriptions");
                });

            modelBuilder.Entity("CourseZero.Models.UploadHist", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("File_Description")
                        .HasMaxLength(10240);

                    b.Property<string>("File_Name")
                        .HasMaxLength(256);

                    b.Property<string>("File_typename");

                    b.Property<int>("Procesed_ErrorMsg");

                    b.Property<bool>("Processed");

                    b.Property<int>("Processed_FileID");

                    b.Property<bool>("Processed_Success");

                    b.Property<int>("Related_courseID");

                    b.Property<DateTime>("Upload_Time");

                    b.Property<int>("Uploader_UserID");

                    b.HasKey("ID");

                    b.ToTable("UploadHistories");
                });

            modelBuilder.Entity("CourseZero.Models.UploadedFile", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<byte[]>("Binary");

                    b.Property<string>("Course_Prefix");

                    b.Property<int>("DisLikes");

                    b.Property<string>("File_Description")
                        .HasMaxLength(10240);

                    b.Property<string>("File_Name")
                        .HasMaxLength(256);

                    b.Property<string>("File_Typename")
                        .HasMaxLength(10);

                    b.Property<int>("Likes");

                    b.Property<int>("Related_courseID");

                    b.Property<bool>("Stored_Internally");

                    b.Property<DateTime>("Upload_Time");

                    b.Property<int>("Uploader_UserID");

                    b.Property<string>("Words_for_Search");

                    b.HasKey("ID");

                    b.HasIndex("Likes");

                    b.HasIndex("Upload_Time");

                    b.ToTable("UploadedFiles");
                });

            modelBuilder.Entity("CourseZero.Models.User", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("email");

                    b.Property<DateTime>("email_verification_issue_datetime");

                    b.Property<bool>("email_verified");

                    b.Property<string>("email_verifying_hash");

                    b.Property<string>("password_change_hash");

                    b.Property<string>("password_change_new_password");

                    b.Property<DateTime>("password_change_request_datatime");

                    b.Property<string>("password_hash");

                    b.Property<string>("password_salt");

                    b.Property<string>("username");

                    b.HasKey("ID");

                    b.HasIndex("email")
                        .IsUnique()
                        .HasFilter("[email] IS NOT NULL");

                    b.HasIndex("username")
                        .IsUnique()
                        .HasFilter("[username] IS NOT NULL");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("CourseZero.Models.WatchLater", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("FileID");

                    b.Property<int>("UserID");

                    b.HasKey("ID");

                    b.ToTable("watchLaters");
                });
#pragma warning restore 612, 618
        }
    }
}
