using CCProductService.Data;
using CCProductService.DTOs;
using CCProductService.Helper;
using CCProductService.Interface;
using Microsoft.AspNetCore.JsonPatch;
using System.Dynamic;

namespace CCProductService.Repositories
{
    public class ProductRepository : IProductRepository
    {
        public IApplicationDbConnection _dbContext { get; }

        public ProductRepository(IApplicationDbConnection writeDbConnection)
        {
            _dbContext = writeDbConnection;
        }

        public void Init(string database)
        {
            _dbContext.Init(database);
        }

        public void Dispose()
        {
            _dbContext?.Dispose();
        }

        public async Task<IEnumerable<ProductDto>> GetAllProducts(int? take, int? skip, UserClaim userClaim)
        {
            string query;
            string productPoolQuery = string.Empty;
            var paramObj = new ExpandoObject();

            if (userClaim.ProductPoolIds != null && userClaim.ProductPoolIds.Count() > 0)
            {
                productPoolQuery = " where Product.ProductPoolId in @poolIds";
                paramObj.TryAdd("poolIds", userClaim.ProductPoolIds.ToArray());
            }

            if (take.HasValue && skip.HasValue)
            {
                query = $"Select t.Id, t.ProductPoolId, t.ProductKey, t.IsBlocked, t.Balance, t.BalanceTare, t.BalancePriceUnit, t.BalancePriceUnitValue, ProductString.ProductId, ProductString.Language, ProductString.ShortName, ProductString.LongName, " +
                    $"ProductString.Description FROM (Select Id, ProductKey, ProductPoolId, IsBlocked, Balance, BalanceTare, BalancePriceUnit, BalancePriceUnitValue From Product{productPoolQuery} ORDER BY ProductKey " +
                    $"OFFSET @offset ROWS FETCH NEXT @fetch ROWS ONLY) as t LEFT JOIN ProductString on t.Id = ProductString.ProductId";
                paramObj.TryAdd("offset", skip.Value);
                paramObj.TryAdd("fetch", take.Value);
            }
            else
            {
                query = "SELECT Product.Id, ProductKey, Product.ProductPoolId, Product.IsBlocked, Product.Balance, Product.BalanceTare, Product.BalancePriceUnit, Product.BalancePriceUnitValue, " +
                    "ProductString.ProductId, ProductString.Language, ProductString.ShortName, ProductString.LongName, ProductString.Description " +
                    $"from Product JOIN ProductString on Product.Id = ProductString.ProductId{productPoolQuery} " +
                    "ORDER BY ProductKey";
                paramObj.TryAdd("usergroup", userClaim.UserGroupId);
            }
            var stringMap = new Dictionary<Guid, ProductDto>();
            IEnumerable<ProductDto> result = await _dbContext.QueryAsync<Product, ProductString, ProductDto>(query, (p, ps) =>
            {
                ProductDto dto;
                if (!stringMap.TryGetValue(p.Id, out dto))
                {
                    dto = new ProductDto(p);
                    stringMap.Add(p.Id, dto);
                }
                dto.SetMultilanguageText(ps);
                return dto;
            }, splitOn: "Id, ProductId", param: paramObj).ConfigureAwait(false);
            return stringMap.Values.ToList().AsReadOnly();
        }

        public async Task<ProductDto> GetProductById(Guid id, UserClaim userClaim)
        {
            ProductDto dto = null;
            var paramObj = new ExpandoObject();
            string productPoolQuery = string.Empty;

            if (userClaim.ProductPoolIds != null && userClaim.ProductPoolIds.Count() > 0)
            {
                productPoolQuery = " and Product.ProductPoolId in @poolIds";
                paramObj.TryAdd("poolIds", userClaim.ProductPoolIds.ToArray());
            }


            string query = $"SELECT Product.Id, ProductKey, ProductPoolId, Product.IsBlocked, Product.Balance, Product.BalanceTare, Product.BalancePriceUnit, Product.BalancePriceUnitValue, ProductString.ProductId, ProductString.Language, ProductString.ShortName, " +
                $"ProductString.LongName, ProductString.Description from Product JOIN ProductString on Product.Id = ProductString.ProductId " +
                $"WHERE Product.Id = @ProductId{productPoolQuery}";

            paramObj.TryAdd("ProductId", id);


            return (await _dbContext.QueryAsync<Product, ProductString, ProductDto>(query, (p, ps) =>
            {
                if (dto == null)
                {
                    dto = new ProductDto(p);
                }
                dto.SetMultilanguageText(ps);
                return dto;
            }, splitOn: "Id, ProductId", param: paramObj).ConfigureAwait(false)).FirstOrDefault();
        }

        public async Task<Guid> AddProductAsync(ProductDto productDto, UserClaim userClaim)
        {
            string productInsertQuery = "INSERT INTO [dbo].[Product] ([ProductKey],[IsBlocked],[Balance],[BalanceTare],[BalanceTareBarcode],[BalancePriceUnit],[BalancePriceUnitValue],[CreatedDate],[CreatedUser],[LastUpdatedUser],[LastUpdatedDate],[ProductPoolId],[ProductType])"
               + " OUTPUT INSERTED.Id"
                + " VALUES(@ProductKey,@IsBlocked,@Balance,@BalanceTare,@BalanceTareBarcode,@BalancePriceUnit,@BalancePriceUnitValue,@CreatedDate,@CreatedUser,@LastUpdatedUser,@LastUpdatedDate,@ProductPoolId,@ProductType)";

            string barcodeInsertQuery = "INSERT INTO [dbo].[ProductBarcode]([Barcode],[ProductId],[Refund])" +
                " VALUES(@Barcode,@ProductId,@Refund)";

            string productStringInsertQuery = "INSERT INTO [dbo].[ProductString]([Language],[ShortName],[LongName],[Description],[ProductId])" +
                " VALUES(@Language,@ShortName,@LongName,@Description,@ProductId)";
            Product product = new Product();
            ProductHelper.ParseDtoToProduct(productDto, product);
            product.CreatedUser = product.LastUpdatedUser = userClaim.UserId;
            product.CreatedDate = product.LastUpdatedDate = DateTimeOffset.Now;
            try
            {
                _dbContext.BeginTransaction();
                Guid id = await _dbContext.ExecuteScalarAsync<Guid>(productInsertQuery, product);
                await _dbContext.ExecuteAsync(barcodeInsertQuery, product.ProductBarcodes);
                await _dbContext.ExecuteAsync(productStringInsertQuery, product.ProductStrings);
                _dbContext.CommitTransaction();
                return id;
            }
            catch (Exception ex)
            {
                // Rollback
                
                throw;
            }
        }

        public async Task UpdateProductAsync(ProductDto productDto, UserClaim userClaim)
        {
            //using (var context = new AramarkDbProduction20210816Context())
            //{
            //    Product product = await context.Products
            //        .Include(p => p.ProductBarcodes)
            //        .Include(p => p.ProductStrings)
            //        .Where(p => p.Id == productDto.Id).FirstOrDefaultAsync();
            //    ProductHelper.ParseDtoToProduct(productDto, product);
            //    product.LastUpdatedUser = userClaim.UserId;
            //    product.LastUpdatedDate = DateTimeOffset.Now;
            //    await context.SaveChangesAsync(new CancellationToken()).ConfigureAwait(false);
            //}
        }

        public async Task<ProductDto> PatchProductAsync(Guid id, JsonPatchDocument productPatch)
        {
            //Product product = await _dbContext.Products
            //    .Include(p => p.ProductBarcodes)
            //    .Include(p => p.ProductStrings)
            //    .Where(p => p.Id == id).FirstOrDefaultAsync();
            //if (product != null)
            //{
            //    ProductDto productDto = new ProductDto();
            //    ProductHelper.ParseProductToDto(product, productDto);
            //    productPatch.ApplyTo(productDto);
            //    ProductHelper.ParseDtoToProduct(productDto, product);
            //    await _dbContext.SaveChangesAsync(new CancellationToken()).ConfigureAwait(false);
            //}
            return new ProductDto();
        }

        public async Task DeleteProductAsync(Guid id, UserClaim userClaim)
        {
            //Product product = await _dbContext.Products.Where(p => p.Id == id).FirstOrDefaultAsync();
            //_dbContext.Products.Remove(product);
            //await _dbContext.SaveChangesAsync(new CancellationToken()).ConfigureAwait(false);
        }

        public async Task<IEnumerable<ProductCategoryDto>> GetCategoriesAsnyc(Guid id, UserClaim userClaim)
        {
            var query = "SELECT CategoryString.CategoryId, CategoryName, Culture FROM ProductCategory JOIN CategoryString ON ProductCategory.CategoryId = CategoryString.CategoryId " +
                "WHERE ProductCategory.ProductId = @ProductId";
            IEnumerable<CategoryString> categoryStrings = await _dbContext.QueryAsync<CategoryString>(query, param: new { ProductId = id }).ConfigureAwait(false);
            return categoryStrings.GroupBy(cs => cs.CategoryId).Select(c => new ProductCategoryDto
            {
                CategoryId = c.Key,
                CategoryNames = c.Select(x => new MultilanguageText
                {
                    Culture = x.Culture,
                    Text = x.CategoryName
                })
            });
        }

        public Task<IEnumerable<string>> GetBarcodesAsync(Guid id, UserClaim userClaim)
        {
            var query = "SELECT Barcode FROM ProductBarcode WHERE ProductId = @ProductId";
            return _dbContext.QueryAsync<string>(query, param: new { ProductId = id });
        }

        public async Task<IEnumerable<ProductPriceDto>> GetProductPrices(Guid id, UserClaim userClaim)
        {
            var query = "Select ppp.Name as PricePool, ppl.Name as PriceList, [Value] From " +
                "(Select pp.ProductPriceListId, pp.ProductPricePoolId, ppd.Value, " +
                "ROW_NUMBER() Over (Partition by pp.Id Order by ppd.StartDate desc) as row from Product p " +
                "join ProductPrice pp on pp.ProductId = p.Id join ProductPriceDate ppd on ppd.ProductPriceId = pp.Id " +
                "where p.Id = @ProductId) as tmp " +
                "join ProductPriceList ppl on ppl.Id = tmp.ProductPriceListId " +
                "join ProductPricePool ppp on ppp.Id = tmp.ProductPricePoolId Where tmp.row = 1";

            var result = await _dbContext.QueryAsync<(string pool, string list, decimal value)>(query, param: new { ProductId = id });
            return result.GroupBy(r => r.pool)
                .Select(r => new ProductPriceDto
                {
                    PoolName = r.Key,
                    PriceListPrice = r.Select(x => new PriceListPrice { PriceList = x.list, Value = x.value })
                });
        }
    }
}
