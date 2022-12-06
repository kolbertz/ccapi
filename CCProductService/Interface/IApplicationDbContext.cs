using System.Data;
using CCProductService.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace CCProductService.Interface
{
    public interface IApplicationDbContext
    {
        public IDbConnection Connection { get; }
        DatabaseFacade Database { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);

        public DbSet<Category> Categories { get; set; }
        public DbSet<CategoryString> CategoryStrings { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductString> ProductStrings { get; set; }
        public DbSet<ProductBarcode> ProductBarcodes { get; set; }
    }
}
