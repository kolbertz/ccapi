using System;
using System.Collections.Generic;
using System.Data;
using CCProductPoolService.Interface;
using Microsoft.EntityFrameworkCore;

namespace CCProductPoolService.Data;

public partial class AramarkDbProduction20210816Context : DbContext
{
    public AramarkDbProduction20210816Context()
    {
    }

    public AramarkDbProduction20210816Context(DbContextOptions<AramarkDbProduction20210816Context> options)
        : base(options)
    {
    }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductPool> ProductPools { get; set; }
    public IDbConnection Connection { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=tcp:ccdemo-dbserver.database.windows.net,1433;Initial Catalog=AramarkDbProduction_20210816;Persist Security Info=False;User ID=Demo;Password=js4ACAKsw5Gf2B5F;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_dbo.Product");

            entity.ToTable("Product");

            entity.HasIndex(e => new { e.ProductKey, e.ProductPoolId }, "IX_ProductPool_ProductKey")
                .IsUnique()
                .HasFillFactor(80);

            entity.HasIndex(e => e.ProductPoolId, "IX_Product_ProductPoolId");

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.BalancePriceUnit).HasMaxLength(2);
            entity.Property(e => e.ClientBlacklist).HasMaxLength(50);
            entity.Property(e => e.Comment).HasMaxLength(4000);
            entity.Property(e => e.Image).IsUnicode(false);

            entity.HasOne(d => d.ProductPool).WithMany(p => p.Products)
                .HasForeignKey(d => d.ProductPoolId)
                .HasConstraintName("FK_dbo.Product_dbo.ProductPool_ProductPool_Id");
        });

        modelBuilder.Entity<ProductPool>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_dbo.ProductPool");

            entity.ToTable("ProductPool");

            entity.HasIndex(e => e.ParentProductPoolId, "IX_ParentProductPoolId").HasFillFactor(80);

            entity.HasIndex(e => e.SystemSettingsId, "IX_SystemSettingsId");

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.Description).HasMaxLength(4000);
            entity.Property(e => e.Name).HasMaxLength(256);

            entity.HasOne(d => d.ParentProductPool).WithMany(p => p.InverseParentProductPool)
                .HasForeignKey(d => d.ParentProductPoolId)
                .HasConstraintName("FK_dbo.ProductPool_dbo.ProductPool_ParentProductPool_Id");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
