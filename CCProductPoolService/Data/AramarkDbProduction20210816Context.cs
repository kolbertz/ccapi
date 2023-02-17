//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Data.SqlClient;
//using CCProductPoolService.Interface;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Configuration;

//namespace CCProductPoolService.Data;

//public partial class AramarkDbProduction20210816Context : DbContext
//{
//    public AramarkDbProduction20210816Context(DbContextOptions<AramarkDbProduction20210816Context> options)
//        : base(options)
//    {
//    }

//    public virtual DbSet<Product> Products { get; set; }

//    public virtual DbSet<ProductPool> ProductPools { get; set; }

//    protected override void OnModelCreating(ModelBuilder modelBuilder)
//    {
//        modelBuilder.Entity<Product>(entity =>
//        {
//            entity.HasKey(e => e.Id).HasName("PK_dbo.Product");

//            entity.ToTable("Product");

//            entity.HasIndex(e => new { e.ProductKey, e.ProductPoolId }, "IX_ProductPool_ProductKey")
//                .IsUnique()
//                .HasFillFactor(80);

//            entity.HasIndex(e => e.ProductPoolId, "IX_Product_ProductPoolId");

//            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
//            entity.Property(e => e.BalancePriceUnit).HasMaxLength(2);
//            entity.Property(e => e.ClientBlacklist).HasMaxLength(50);
//            entity.Property(e => e.Comment).HasMaxLength(4000);
//            entity.Property(e => e.Image).IsUnicode(false);

//            entity.HasOne(d => d.ProductPool).WithMany(p => p.Products)
//                .HasForeignKey(d => d.ProductPoolId)
//                .HasConstraintName("FK_dbo.Product_dbo.ProductPool_ProductPool_Id");
//        });

//        modelBuilder.Entity<ProductPool>(entity =>
//        {
//            entity.HasKey(e => e.Id).HasName("PK_dbo.ProductPool");

//            entity.ToTable("ProductPool");

//            entity.HasIndex(e => e.ParentProductPoolId, "IX_ParentProductPoolId").HasFillFactor(80);

//            entity.HasIndex(e => e.SystemSettingsId, "IX_SystemSettingsId");

//            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
//            entity.Property(e => e.Description).HasMaxLength(4000);
//            entity.Property(e => e.Name).HasMaxLength(256);

//            entity.HasOne(d => d.ParentProductPool).WithMany(p => p.InverseParentProductPool)
//                .HasForeignKey(d => d.ParentProductPoolId)
//                .HasConstraintName("FK_dbo.ProductPool_dbo.ProductPool_ParentProductPool_Id");
//        });

//        OnModelCreatingPartial(modelBuilder);
//    }

//    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
//}
