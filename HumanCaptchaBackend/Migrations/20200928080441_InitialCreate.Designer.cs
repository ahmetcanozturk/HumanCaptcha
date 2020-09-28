﻿// <auto-generated />
using System;
using HumanCaptchaBackend.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace HumanCaptchaBackend.Migrations
{
    [DbContext(typeof(HumanCaptchaContext))]
    [Migration("20200928080441_InitialCreate")]
    partial class InitialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.8");

            modelBuilder.Entity("HumanCaptchaBackend.Data.Captcha", b =>
                {
                    b.Property<uint>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("Expires")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("Guid")
                        .HasColumnType("TEXT");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.ToTable("Captchas");
                });

            modelBuilder.Entity("HumanCaptchaBackend.Data.Exception", b =>
                {
                    b.Property<uint>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("ExceptionTime")
                        .HasColumnType("datetime");

                    b.Property<string>("InnerStack")
                        .HasColumnType("TEXT")
                        .IsUnicode(false);

                    b.Property<string>("Message")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .IsUnicode(false);

                    b.Property<string>("StackTrace")
                        .HasColumnType("TEXT")
                        .IsUnicode(false);

                    b.Property<string>("TypeName")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .IsUnicode(false);

                    b.HasKey("ID");

                    b.ToTable("Exceptions");
                });

            modelBuilder.Entity("HumanCaptchaBackend.Data.Token", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("Expires")
                        .HasColumnType("TEXT");

                    b.Property<bool>("Used")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.ToTable("Tokens");
                });
#pragma warning restore 612, 618
        }
    }
}
