﻿// <auto-generated />
using System;
using CourseZero.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CourseZero.Migrations.ProfileComments
{
    [DbContext(typeof(ProfileCommentsContext))]
    [Migration("20190302065303_Update23")]
    partial class Update23
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.1-servicing-10028")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

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
#pragma warning restore 612, 618
        }
    }
}