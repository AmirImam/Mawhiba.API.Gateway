using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Mawhiba.API.Gateway.Models;

public partial class ContentServicesDbContext : DbContext
{
    public ContentServicesDbContext()
    {
    }

    public ContentServicesDbContext(DbContextOptions<ContentServicesDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AppVariable> AppVariables { get; set; }

    public virtual DbSet<AppVersion> AppVersions { get; set; }

    public virtual DbSet<ContentType> ContentTypes { get; set; }

    public virtual DbSet<ExceptionLog> ExceptionLogs { get; set; }

    public virtual DbSet<GatewaySetting> GatewaySettings { get; set; }

    public virtual DbSet<HomeBanner> HomeBanners { get; set; }

    public virtual DbSet<Service> Services { get; set; }

    public virtual DbSet<ServicesAppresource> ServicesAppresources { get; set; }

    public virtual DbSet<ServicesContent> ServicesContents { get; set; }

    public virtual DbSet<ServicesFeature> ServicesFeatures { get; set; }

    public virtual DbSet<SignalRconnection> SignalRconnections { get; set; }

    public virtual DbSet<UserFeedback> UserFeedbacks { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=sqlmi-stg-uaenorth.8296b33d5bbf.database.windows.net,1433;Database=ContentServicesDB;User Id=SQLAdmin;Password=P@ssw0rd@123@123;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AppVariable>(entity =>
        {
            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.VariableKey).HasMaxLength(50);
            entity.Property(e => e.VariableValue).HasMaxLength(50);
        });

        modelBuilder.Entity<AppVersion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_AppVersion");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.AndroidVersionNumber).HasMaxLength(50);
            entity.Property(e => e.IosversionNumber)
                .HasMaxLength(50)
                .HasColumnName("IOSVersionNumber");
            entity.Property(e => e.VersionDate).HasColumnType("datetime");
            entity.Property(e => e.VersionNumber).HasMaxLength(50);
        });

        modelBuilder.Entity<ContentType>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.ContentTypeName).HasMaxLength(50);
        });

        modelBuilder.Entity<ExceptionLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_ExceptionLogV2");

            entity.ToTable("ExceptionLog");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID");
        });

        modelBuilder.Entity<GatewaySetting>(entity =>
        {
            entity.ToTable("GatewaySetting");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
        });

        modelBuilder.Entity<HomeBanner>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.BannerClickPath).HasMaxLength(500);
            entity.Property(e => e.BannerHoverText).HasMaxLength(500);
            entity.Property(e => e.BannerHoverTitle).HasMaxLength(200);
            entity.Property(e => e.BannerUrl).HasMaxLength(255);
            entity.Property(e => e.PublishedFrom).HasColumnType("datetime");
            entity.Property(e => e.PublishedTo).HasColumnType("datetime");
        });

        modelBuilder.Entity<Service>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.ServiceApplicationUrl).HasMaxLength(255);
            entity.Property(e => e.ServiceHomeImageUrl).HasMaxLength(255);
            entity.Property(e => e.ServiceName).HasMaxLength(250);
            entity.Property(e => e.ServiceUrl).HasMaxLength(255);
        });

        modelBuilder.Entity<ServicesAppresource>(entity =>
        {
            entity.ToTable("ServicesAPPResources");

            entity.Property(e => e.FileData).HasColumnType("image");
            entity.Property(e => e.LangCode).HasMaxLength(50);
            entity.Property(e => e.LastModified).HasColumnType("datetime");

            entity.HasOne(d => d.Service).WithMany(p => p.ServicesAppresources)
                .HasForeignKey(d => d.ServiceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ServicesAPPResources_Services");
        });

        modelBuilder.Entity<ServicesContent>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_ServicesContents_1");

            entity.Property(e => e.LastUpdated).HasColumnType("datetime");

            entity.HasOne(d => d.ContentType).WithMany(p => p.ServicesContents)
                .HasForeignKey(d => d.ContentTypeId)
                .HasConstraintName("FK_ServicesContents_ContentTypes");

            entity.HasOne(d => d.Service).WithMany(p => p.ServicesContents)
                .HasForeignKey(d => d.ServiceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ServicesContents_Services");
        });

        modelBuilder.Entity<ServicesFeature>(entity =>
        {
            entity.Property(e => e.FeatureName).HasMaxLength(50);
            entity.Property(e => e.FeatureText).HasMaxLength(150);
            entity.Property(e => e.FeatureTextEn).HasMaxLength(150);

            entity.HasOne(d => d.Service).WithMany(p => p.ServicesFeatures)
                .HasForeignKey(d => d.ServiceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ServicesFeatures_Services");
        });

        modelBuilder.Entity<SignalRconnection>(entity =>
        {
            entity.ToTable("SignalRConnection");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.ConnectionId).HasMaxLength(100);
            entity.Property(e => e.UserName).HasMaxLength(50);
        });

        modelBuilder.Entity<UserFeedback>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_UserFeedback");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.AppVersion).HasMaxLength(50);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.DeviceModel).HasMaxLength(50);
            entity.Property(e => e.DeviceName).HasMaxLength(50);
            entity.Property(e => e.Feedback).HasMaxLength(500);
            entity.Property(e => e.MoreInfo).HasMaxLength(500);
            entity.Property(e => e.Ostype)
                .HasMaxLength(50)
                .HasColumnName("OSType");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
