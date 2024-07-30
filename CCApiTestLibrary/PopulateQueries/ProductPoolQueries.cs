
namespace CCApiTestLibrary.PopulateQueries
{
    public static class ProductPoolQueries
    {
        public static string PopulateSingleProductPool(int key = 1, string name = "Test", string description = "Beschreibung")
        {
            return "INSERT INTO ProductPool(ProductPoolKey, [Name], Description, SystemSettingsId, CreatedDate, CreatedUser, LastUpdatedDate, LastUpdatedUser) " +
                "OUTPUT Inserted.Id " +
                $"VALUES({key}, '{name}', '{description}', 'fab8c985-6147-4eba-b2c7-5f7012c4aeeb', GetDate(), '1f11e600-4b51-4ae5-9feb-d372d096acb4', GetDate(), '1f11e600-4b51-4ae5-9feb-d372d096acb4')";
        }

        public static string PopulateProductPoolList()
        {
            return "INSERT INTO ProductPool(ProductPoolKey, [Name], SystemSettingsId, CreatedDate, CreatedUser, LastUpdatedDate, LastUpdatedUser) " +
                "VALUES(1, 'Pool 1', 'fab8c985-6147-4eba-b2c7-5f7012c4aeeb', GetDate(), '1f11e600-4b51-4ae5-9feb-d372d096acb4', GetDate(), '1f11e600-4b51-4ae5-9feb-d372d096acb4'), " +
                "(2, 'Pool 2', 'fab8c985-6147-4eba-b2c7-5f7012c4aeeb', GetDate(), '1f11e600-4b51-4ae5-9feb-d372d096acb4', GetDate(), '1f11e600-4b51-4ae5-9feb-d372d096acb4');";
        }

        public static string DeleteProductPools()
        {
            return "DELETE FROM ProductPool";
        }
    }
}
