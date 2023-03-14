//using CCApiLibrary.Interfaces;
//using CCApiTest.Base;
//using CCApiTest.Interface;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace CCApiTest.Category
//{
//    public class CategoryControllerBase : ControllerTestBaseClass, IPopulateDbControllerTest
//    {
//        public async Task PopulateDbWithSystemSetting(IApplicationDbConnection ctx)
//        {
//            var commandText = "INSERT INTO SystemSettings (Id, InternalName, [Name], DistributorId, IsBlocked, IsHosted, SystemType, AddressName1, " +
//              "AddressStreet, AddressPostalCode, AddressCity, DefaultTimeZone, MaxCustomCurrencyExchangeRateDiff, MinPriceUnit, NoDeleteRange, " +
//              "CreatedDate, CreatedUser, LastUpdatedDate, LastUpdatedUser, [Key]) " +
//              $"VALUES('{systemSettingsId}', 'TestSystem', 'TestSystem', 0, 0, 0, 0, 'Test', 'Test', 'Test', 'Test', 'Test', 0, 0, 0, GETDATE(), '313de81f-a37c-422b-8e6d-fbff6c02eb6f', GETDATE(), '313de81f-a37c-422b-8e6d-fbff6c02eb6f', 0);";

//            await PopulateDatabase(commandText, ctx);
//        }

//        public async Task DePopulateDatabase(IApplicationDbConnection ctx)
//        {
//            await PopulateDatabase("delete from Category", ctx);
//            await PopulateDatabase("delete from SystemSettings", ctx);
//        }

//        public async Task PopulateDatabaseWithList(IApplicationDbConnection ctx)
//        {
//            await PopulateDbWithSystemSetting(ctx);
//            var query = "INSERT INTO Product(CategoryKey, CreatedDate, CreatedUser, LastUpdatedDate, LastUpdatedUser, ProductPoolId) " +
//                "VALUES(1, GetDate(), '1f11e600-4b51-4ae5-9feb-d372d096acb4', GetDate(), '1f11e600-4b51-4ae5-9feb-d372d096acb4', 'fab8c985-6147-4eba-b2c7-5f7012c4aeeb'), " +
//                      "(2, GetDate(), '1f11e600-4b51-4ae5-9feb-d372d096acb4', GetDate(), '1f11e600-4b51-4ae5-9feb-d372d096acb4', 'fab8c985-6147-4eba-b2c7-5f7012c4aeeb');";
//            await PopulateDatabase(query, ctx);
//        }

//        public async Task<Guid> PopulateDatabaseWithSingleEntity(IApplicationDbConnection ctx)
//        {
//            await PopulateDbWithSystemSetting(ctx);
//            throw new NotImplementedException();
//        }

       
//    }
//}
