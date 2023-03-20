
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
    }
}
