using System;
using System.Collections.Generic;
using System.Data;
using CCProductService.Interface;
using Microsoft.EntityFrameworkCore;

namespace CCProductService.Data;

public partial class AramarkDbProduction20210816Context : DbContext, IApplicationDbContext
{
    public AramarkDbProduction20210816Context()
    {
    }

    public AramarkDbProduction20210816Context(DbContextOptions<AramarkDbProduction20210816Context> options)
        : base(options)
    {
    }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<CategoryString> CategoryStrings { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductBarcode> ProductBarcodes { get; set; }

    public virtual DbSet<ProductString> ProductStrings { get; set; }

    public IDbConnection Connection => throw new NotImplementedException();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=tcp:ccdemo-dbserver.database.windows.net,1433;Initial Catalog=AramarkDbProduction_20210816;Persist Security Info=False;User ID=Demo;Password=js4ACAKsw5Gf2B5F;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_dbo.Category");

            entity.ToTable("Category");

            entity.HasIndex(e => new { e.CategoryPoolId, e.CategoryKey }, "IX_CategoryPool_CategorieKey")
                .IsUnique()
                .HasFillFactor(80);

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");

            entity.HasMany(d => d.Products).WithMany(p => p.Categories)
                .UsingEntity<Dictionary<string, object>>(
                    "ProductCategory",
                    r => r.HasOne<Product>().WithMany()
                        .HasForeignKey("ProductId")
                        .HasConstraintName("FK_dbo.ProductCategories_dbo.Product_Product_Id"),
                    l => l.HasOne<Category>().WithMany()
                        .HasForeignKey("CategoryId")
                        .HasConstraintName("FK_dbo.ProductCategories_dbo.Category_Category_Id"),
                    j =>
                    {
                        j.HasKey("CategoryId", "ProductId").HasName("PK_dbo.ProductCategory");
                        j.ToTable("ProductCategory");
                        j.HasIndex(new[] { "CategoryId" }, "IX_CategoryId").HasFillFactor(80);
                        j.HasIndex(new[] { "ProductId" }, "IX_ProductId").HasFillFactor(80);
                    });
        });

        modelBuilder.Entity<CategoryString>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_dbo.CategoryString");

            entity.ToTable("CategoryString");

            entity.HasIndex(e => new { e.CategoryId, e.Culture }, "IX_CategoryString_Culture")
                .IsUnique()
                .HasFillFactor(80);

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.CategoryName)
                .IsRequired()
                .HasMaxLength(256);
            entity.Property(e => e.Culture).HasMaxLength(10);

            entity.HasOne(d => d.Category).WithMany(p => p.CategoryStrings)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("FK_dbo.CategoryString_dbo.Category_Category_Id");
        });

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
        });

        modelBuilder.Entity<ProductBarcode>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_dbo.ProductBarcode");

            entity.ToTable("ProductBarcode");

            entity.HasIndex(e => e.ProductId, "IX_ProductId").HasFillFactor(80);

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.Barcode)
                .IsRequired()
                .HasMaxLength(50);

            entity.HasOne(d => d.Product).WithMany(p => p.ProductBarcodes)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK_dbo.ProductBarcode_dbo.Product_Product_Id");
        });

        modelBuilder.Entity<ProductString>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_dbo.ProductString");

            entity.ToTable("ProductString");

            entity.HasIndex(e => new { e.ProductId, e.Language }, "IX_ProductString_Language")
                .IsUnique()
                .HasFillFactor(80);

            entity.HasIndex(e => e.LongName, "IX_ProductString_LongName");

            entity.HasIndex(e => e.ShortName, "IX_ProductString_ShortName");

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.Language)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.LongName)
                .IsRequired()
                .HasMaxLength(256);
            entity.Property(e => e.ShortName)
                .IsRequired()
                .HasMaxLength(20);

            entity.HasOne(d => d.Product).WithMany(p => p.ProductStrings)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK_dbo.ProductString_dbo.Product_Product_Id");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
