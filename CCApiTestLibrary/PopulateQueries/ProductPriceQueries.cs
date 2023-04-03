
namespace CCApiTestLibrary.PopulateQueries
{
    public static class ProductPriceQueries
    {
        public static string PopulateSingleProductPriceList(string name, int key, int priority)
        {
            return $"INSERT INTO ProductPriceList([Name], [Key], Priority, SystemSettingsId) " +
                $"OUTPUT INSERTED.Id " +
                $"VALUES('{name}', {key}, {priority}, '{StaticTestGuids.SystemSettingsId}')";
        }

        public static string DeleteProductPriceLists()
        {
            return "DELETE FROM ProductPriceList";
        }

        public static string PopulateSingleProductPricePool(string name, string description)
        {
            return $"INSERT INTO ProductPricePool([Name], Description, CreatedDate, CreatedUser, LastUpdatedDate, LastUpdatedUser, CurrencyId, SystemSettingsId) " +
                $"OUTPUT INSERTED.Id " +
                $"VALUES('{name}', '{description}', GetDate(), '{StaticTestGuids.UserId}', GetDate(), '{StaticTestGuids.UserId}', '{StaticTestGuids.CurrencyId}', '{StaticTestGuids.SystemSettingsId}')";
        }

        public static string DeleteProductPricePools()
        {
            return "DELETE FROM ProductPricePool";
        }

        public static string PopulateProductPrice(Guid productId, Guid poolId, Guid listId)
        {
            return $"INSERT INTO ProductPrice(ProductId, ProductPricePoolId, ProductPriceListId, ManualPrice) " +
                $"OUTPUT INSERTED.Id " +
                $"VALUES('{productId}', '{poolId}', '{listId}', 0)";
        }

        public static string DeleteProductPrices()
        {
            return "DELETE FROM ProductPrice";
        }

        public static string PopulateProductPriceDate()
        {
            return $"INSERT INTO ProductPriceDate(ProductPriceId, StartDate, [Value]) " +
                $"VALUES(@priceId, @startDate, @value)";
        }

        public static string DeleteProductPriceDates()
        {
            return "DELETE FROM ProductPriceDate";
        }
    }
}
