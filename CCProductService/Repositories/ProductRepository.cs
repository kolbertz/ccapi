﻿using CCApiLibrary.Interfaces;
using CCApiLibrary.Models;
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

        public async Task<IEnumerable<ProductStandardPrice>> GetAllProducts(int? take, int? skip, UserClaim userClaim) 
        {
            string query;
            string productPoolQuery = string.Empty;
            string sysIdQuery = string.Empty;
            var paramObj = new ExpandoObject();

            if (userClaim.SystemId.HasValue)
            {
                sysIdQuery = " WHERE ProductPool.SystemSettingsId = @SysId";
                paramObj.TryAdd("SysId", userClaim.SystemId);
            }

            if (userClaim.ProductPoolIds != null && userClaim.ProductPoolIds.Count() > 0)
            {
                productPoolQuery = " AND Product.ProductPoolId in @poolIds";
                paramObj.TryAdd("poolIds", userClaim.ProductPoolIds.ToArray());
            } 

            if (take.HasValue && skip.HasValue)
            {
                query = $"SELECT temp.Id, ProductPoolId, ProductKey, IsBlocked, Balance, BalanceTare, BalancePriceUnit, BalancePriceUnitValue, [Value] AS Standardprice," +
                    $" ProductString.ProductId, ProductString.Language, ProductString.ShortName, ProductString.LongName, ProductString.Description " +
                    $"FROM (SELECT prod.Id, prod.ProductPoolId, prod.ProductKey, prod.IsBlocked, prod.Balance, prod.BalanceTare, prod.BalancePriceUnit, prod.BalancePriceUnitValue, ProductPriceDate.Value, " +
                    $"ROW_NUMBER() OVER (Partition by prod.Id order by StartDate desc) as row " +
                    $"FROM (SELECT Product.Id, Product.ProductKey, Product.ProductPoolId, Product.IsBlocked, Product.Balance, Product.BalanceTare, Product.BalancePriceUnit, Product.BalancePriceUnitValue " +
                    $"FROM Product LEFT JOIN ProductPool ON ProductPool.Id = Product.ProductPoolId{sysIdQuery}{productPoolQuery} order by Product.ProductKey OFFSET @offset Rows Fetch next @fetch Rows only) as prod " +
                    $"LEFT JOIN ProductPrice ON prod.Id = ProductPrice.ProductId LEFT JOIN ProductPricePoolToPriceList ON ProductPricePoolToPriceList.ProductPricePoolId = ProductPrice.ProductPricePoolId " +
                    $"LEFT JOIN ProductPriceDate ON ProductPriceDate.ProductPriceId = ProductPrice.Id WHERE ProductPricePoolToPriceList.IsDefault = 1 OR ProductPricePoolToPriceList.IsDefault IS NULL) as temp " +
                    $"LEFT JOIN ProductString on ProductString.ProductId = temp.Id where temp.row = 1";

                paramObj.TryAdd("offset", skip.Value);
                paramObj.TryAdd("fetch", take.Value);
            }
            else
            {
                query = $"SELECT temp.Id, ProductPoolId, ProductKey, IsBlocked, Balance, BalanceTare, BalancePriceUnit, BalancePriceUnitValue, [Value] AS Standardprice," +
                    $" ProductString.ProductId, ProductString.Language, ProductString.ShortName, ProductString.LongName, ProductString.Description " +
                    $"FROM (SELECT prod.Id, prod.ProductPoolId, prod.ProductKey, prod.IsBlocked, prod.Balance, prod.BalanceTare, prod.BalancePriceUnit, prod.BalancePriceUnitValue, ProductPriceDate.Value, " +
                    $"ROW_NUMBER() OVER (Partition by prod.Id order by StartDate desc) as row " +
                    $"FROM (SELECT Product.Id, Product.ProductKey, Product.ProductPoolId, Product.IsBlocked, Product.Balance, Product.BalanceTare, Product.BalancePriceUnit, Product.BalancePriceUnitValue " +
                    $"FROM Product LEFT JOIN ProductPool ON ProductPool.Id = Product.ProductPoolId{sysIdQuery}{productPoolQuery}) as prod " +
                    $"LEFT JOIN ProductPrice ON prod.Id = ProductPrice.ProductId LEFT JOIN ProductPricePoolToPriceList ON ProductPricePoolToPriceList.ProductPricePoolId = ProductPrice.ProductPricePoolId " +
                    $"LEFT JOIN ProductPriceDate ON ProductPriceDate.ProductPriceId = ProductPrice.Id WHERE ProductPricePoolToPriceList.IsDefault = 1 OR ProductPricePoolToPriceList.IsDefault IS NULL) as temp " +
                    $"LEFT JOIN ProductString on ProductString.ProductId = temp.Id where temp.row = 1";
            }


            var stringMap = new Dictionary<Guid, ProductStandardPrice>();
            IEnumerable<ProductStandardPrice> result = await _dbContext.QueryAsync<InternalProduct, InternalProductString, ProductStandardPrice>(query, (p, ps) =>
            {
                ProductStandardPrice dto;
                if (!stringMap.TryGetValue(p.Id, out dto))
                {
                    dto = new ProductStandardPrice(p);
                    stringMap.Add(p.Id, dto);
                }
                dto.SetMultilanguageText(ps);
                return dto;
            }, splitOn: "Id, ProductId", param: paramObj).ConfigureAwait(false);
            return stringMap.Values.ToList().AsReadOnly();
        }

        public async Task<ProductBase> GetProductById(Guid id, UserClaim userClaim)
        {
            ProductBase dto = null;
            var paramObj = new ExpandoObject();
            string productPoolQuery = string.Empty;
            string sysIdQuery = string.Empty;

            if (userClaim.SystemId.HasValue)
            {
                sysIdQuery = " AND ProductPool.SystemSettingsId = @SysId";
                paramObj.TryAdd("SysId", userClaim.SystemId);
            }

            if (userClaim.ProductPoolIds != null && userClaim.ProductPoolIds.Count() > 0)
            {
                productPoolQuery = " AND Product.ProductPoolId IN @poolIds";
                paramObj.TryAdd("poolIds", userClaim.ProductPoolIds.ToArray());
            }            

            string query = $"SELECT Product.Id, ProductKey, ProductPoolId, Product.IsBlocked, Product.Balance, Product.BalanceTare, Product.BalancePriceUnit, Product.BalancePriceUnitValue, ProductString.ProductId, ProductString.Language, ProductString.ShortName, " +
                $"ProductString.LongName, ProductString.Description FROM Product JOIN ProductString on Product.Id = ProductString.ProductId " +
                $"WHERE Product.Id = @ProductId{sysIdQuery}{productPoolQuery}";

            paramObj.TryAdd("ProductId", id);


            return (await _dbContext.QueryAsync<InternalProduct, InternalProductString, ProductBase>(query, (p, ps) =>
            {
                if (dto == null)
                {
                    dto = new ProductBase(p);
                }
                dto.SetMultilanguageText(ps);
                return dto;
            }, splitOn: "Id, ProductId", param: paramObj).ConfigureAwait(false)).FirstOrDefault();
        }

        public async Task<Guid> AddProductAsync(ProductBase productDto, UserClaim userClaim)
        {
            string productInsertQuery = "INSERT INTO [dbo].[Product] ([ProductKey],[IsBlocked],[Balance],[BalanceTare],[BalanceTareBarcode],[BalancePriceUnit],[BalancePriceUnitValue],[CreatedDate],[CreatedUser],[LastUpdatedUser],[LastUpdatedDate],[ProductPoolId],[ProductType])"
               + " OUTPUT INSERTED.Id"
                + " VALUES(@ProductKey,@IsBlocked,@Balance,@BalanceTare,@BalanceTareBarcode,@BalancePriceUnit,@BalancePriceUnitValue,@CreatedDate,@CreatedUser,@LastUpdatedUser,@LastUpdatedDate,@ProductPoolId,@ProductType)";

            InternalProduct product = new InternalProduct();
            ProductHelper.ParseDtoToProduct(productDto, product);
            product.CreatedUser = product.LastUpdatedUser = userClaim.UserId;
            product.CreatedDate = product.LastUpdatedDate = DateTimeOffset.Now;
            try
            {
                _dbContext.BeginTransaction();
                Guid id = await _dbContext.ExecuteScalarAsync<Guid>(productInsertQuery, product);
                await InsertBarCode(productDto, userClaim);
                await InsertProductString(productDto, userClaim);
                _dbContext.CommitTransaction();
                return id;
            }
            catch (Exception ex)
            {
                // Rollback

                throw;
            }
        }

        public Task<bool> UpdateProductAsync(ProductBase productDto, UserClaim userClaim)
        {
            InternalProduct product = new InternalProduct();
            ProductHelper.ParseDtoToProduct(productDto, product);
            product.LastUpdatedDate = DateTimeOffset.Now;
            product.LastUpdatedUser = userClaim.UserId;
            return Update(product,productDto, userClaim);
        }

        public async Task<ProductBase> PatchProductAsync(Guid id,JsonPatchDocument jsonPatchDocument, UserClaim userClaim)
        {
            var query = "SELECT * FROM Product WHERE Id = @ProductId";
            var p = new {ProductId = id };
            InternalProduct product = await _dbContext.QuerySingleAsync<InternalProduct>(query, p);
            if (product != null)
            {
                ProductBase productDto = new ProductBase(product);
                product.LastUpdatedDate = DateTimeOffset.Now;
                product.LastUpdatedUser = userClaim.UserId;
                if (await Update(product, productDto, userClaim).ConfigureAwait(false))
                {
                    return productDto;
                }
            }
            return new ProductBase();
        }

        public Task<int> DeleteProductAsync(Guid id, UserClaim userClaim)
        {
            var query = "DELETE FROM [dbo].[Product] WHERE Id = @Id";

            return _dbContext.ExecuteAsync(query, param: new { Id = id });

        }

        public async Task<IEnumerable<ProductCategoryDto>> GetCategoriesAsnyc(Guid id, UserClaim userClaim)
        {
            var query = "SELECT CategoryString.CategoryId, CategoryName, Culture FROM ProductCategory JOIN CategoryString ON ProductCategory.CategoryId = CategoryString.CategoryId " +
                "WHERE ProductCategory.ProductId = @ProductId";
            IEnumerable<InternalCategoryString> categoryStrings = await _dbContext.QueryAsync<InternalCategoryString>(query, param: new { ProductId = id }).ConfigureAwait(false);
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

        public async Task<bool> Update(InternalProduct product,ProductBase productDto, UserClaim userClaim)
        {
            var productUpdateQuery = "UPDATE Product Set ProductKey = @ProductKey, [IsBlocked] = @IsBlocked, [Balance] = @Balance, [BalanceTareBarcode] = @BalanceTareBarcode, " +
                "[BalancePriceUnit] = @BalancePriceUnit, [BalancePriceUnitValue] = @BalancePriceUnitValue, [CreatedUser] = @CreatedUser, [ProductPoolId] = ProductPoolId, [ProductType] = ProductType WHERE Id = @Id";
            //var barcodeUpdateQuery = "UPDATE ProductBarcode Set Barcode = @Barcode, [Refund] = @Refund, [ProductId] = @ProductId, [ParentProductId] = @ParentProductId WHERE ProductId = @Id";
            //var productStringUpdateQuery = "UPDATE ProductString Set Language = @Language, [ShortName] = @ShortName, [Description] = @Description, [ProductId] = @ProductId, " +
            //    "LongName = @LongName WHERE ProductId = @Id";
            try
            {
                _dbContext.BeginTransaction();
                if (await _dbContext.ExecuteAsync(productUpdateQuery, product) > 0)
                {
                    //Delete ProductString, Bardcode
                    await DeleteProductStringAsync(product.Id, userClaim);
                    await DeleteBarCodeAsync(product.Id, userClaim);
                    //Insert ProductString, Barcode
                    await InsertProductString(productDto, userClaim);
                    await InsertBarCode(productDto, userClaim);
                }
               
                //await _dbContext.ExecuteAsync(barcodeUpdateQuery, pool.ProductBarcodes);
                //await _dbContext.ExecuteAsync(productStringUpdateQuery, pool.ProductStrings);
                _dbContext.CommitTransaction();
                return false;
            }
            catch (Exception ex)
            {
                // Rollback

                throw;
            }

            //return _dbContext.ExecuteAsync(query, param: pool);
        }
        private async Task<bool> InsertProductString(ProductBase productDto, UserClaim userClaim)
        {
            string productStringInsertQuery = "INSERT INTO [dbo].[ProductString]([Language],[ShortName],[LongName],[Description],[ProductId])" +
               " VALUES(@Language,@ShortName,@LongName,@Description,@ProductId)";
            InternalProduct product = new InternalProduct();

            ProductHelper.ParseDtoToProduct(productDto, product);
            product.CreatedUser = product.LastUpdatedUser = userClaim.UserId;
            product.CreatedDate = product.LastUpdatedDate = DateTimeOffset.Now;
            try
            {
                await _dbContext.ExecuteAsync(productStringInsertQuery, product.ProductStrings);

                return true;
            }
            catch (Exception ex)
            {
                // Rollback

                throw;
            }
        }

        private async Task<bool> InsertBarCode(ProductBase productDto, UserClaim userClaim)
        {
            string barcodeInsertQuery = "INSERT INTO [dbo].[ProductBarcode]([Barcode],[ProductId],[Refund])" +
            " VALUES(@Barcode,@ProductId,@Refund)";

            InternalProduct product = new InternalProduct();
            ProductHelper.ParseDtoToProduct(productDto, product);
            product.CreatedUser = product.LastUpdatedUser = userClaim.UserId;
            product.CreatedDate = product.LastUpdatedDate = DateTimeOffset.Now;
            try
            {
                await _dbContext.ExecuteAsync(barcodeInsertQuery, product.ProductBarcodes);

                return true;
            }
            catch (Exception ex)
            {
                // Rollback

                throw;
            }
        }

        public Task<int> DeleteProductStringAsync(Guid id, UserClaim userClaim)
        {
            var query = "DELETE FROM [dbo].[ProductString] WHERE ProductId = @ProductId";

            return _dbContext.ExecuteAsync(query, param: new { ProductId = id });

        }

        public Task<int> DeleteBarCodeAsync(Guid id, UserClaim userClaim)
        {
            var query = "DELETE FROM [dbo].[ProductBarcode] WHERE ProductId = @ProductId";

            return _dbContext.ExecuteAsync(query, param: new { ProductId = id });

        }

        private static (string sysId, ExpandoObject paramObj) GetClaimsQuery(UserClaim userClaim)
        {
            string sysIdQuery = string.Empty;
            var paramObj = new ExpandoObject();

            if (userClaim != null)
            {
                sysIdQuery = " where SystemSettingsId = @sysId";
                paramObj.TryAdd("sysId", userClaim.SystemId);
            }
            return (sysIdQuery, paramObj);
        }

    }
}
