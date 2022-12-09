using CCProductPoolService.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using ProductPoolApiTest.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductPoolApiTest.ProductPool
{
    public class ProductPoolControllerBase : ControllerTestBaseClass, IPopulateDbControllerTest
    {
        public async Task PopulateDbWithSystemSetting(DbContext ctx, IDbContextTransaction transaction, bool commit = false)
        {
            var commandText = "INSERT INTO SystemSettings (Id, InternalName, [Name], DistributorId, IsBlocked, IsHosted, SystemType, AddressName1, " +
                "AddressStreet, AddressPostalCode, AddressCity, DefaultTimeZone, MaxCustomCurrencyExchangeRateDiff, MinPriceUnit, NoDeleteRange, " +
                "CreatedDate, CreatedUser, LastUpdatedDate, LastUpdatedUser, [Key]) " +
                $"VALUES('{systemSettingsId}', 'TestSystem', 'TestSystem', 0, 0, 0, 0, 'Test', 'Test', 'Test', 'Test', 'Test', 0, 0, 0, GETDATE(), '313de81f-a37c-422b-8e6d-fbff6c02eb6f', GETDATE(), '313de81f-a37c-422b-8e6d-fbff6c02eb6f', 0);";

            await PopulateDatabase(commandText, ctx, transaction);
            if (commit)
            await transaction.CommitAsync();
        }

        public async Task DePopulateDatabase(IDbContextTransaction transaction, DbContext ctx)
        {
            if (transaction != null)
            {
                transaction?.Rollback();
            }
            else
            {
                transaction = ctx.Database.BeginTransaction();
                await PopulateDatabase("delete from ProductPool", ctx, transaction);
                await PopulateDatabase("delete from SystemSettings", ctx, transaction);
                await transaction.CommitAsync();
            }
        }

        public async Task PopulateDatabaseWithList(IDbContextTransaction transaction, DbContext ctx)
        {
            await PopulateDbWithSystemSetting(ctx, transaction);
            var query = "INSERT INTO ProductPool(ProductPoolKey, [Name], SystemSettingsId, CreatedDate, CreatedUser, LastUpdatedDate, LastUpdatedUser) " +
                "VALUES(1, 'Pool 1', 'fab8c985-6147-4eba-b2c7-5f7012c4aeeb', GetDate(), '1f11e600-4b51-4ae5-9feb-d372d096acb4', GetDate(), '1f11e600-4b51-4ae5-9feb-d372d096acb4'), " +
                "(2, 'Pool 2', 'fab8c985-6147-4eba-b2c7-5f7012c4aeeb', GetDate(), '1f11e600-4b51-4ae5-9feb-d372d096acb4', GetDate(), '1f11e600-4b51-4ae5-9feb-d372d096acb4');";
            await PopulateDatabase(query, ctx, transaction).ConfigureAwait(false);
            await transaction?.CommitAsync();
        }

        public async Task<Guid> PopulateDatabaseWithSingleEntity(IDbContextTransaction transaction, DbContext ctx)
        {
            await PopulateDbWithSystemSetting(ctx, transaction);
            var query = "INSERT INTO ProductPool(ProductPoolKey, [Name], SystemSettingsId, CreatedDate, CreatedUser, LastUpdatedDate, LastUpdatedUser) " +
                "OUTPUT Inserted.Id " +
                "VALUES(1, 'Pool 1', 'fab8c985-6147-4eba-b2c7-5f7012c4aeeb', GetDate(), '1f11e600-4b51-4ae5-9feb-d372d096acb4', GetDate(), '1f11e600-4b51-4ae5-9feb-d372d096acb4')";
            Guid productPoolId = await PopulateDatabaseAndReturnIdentity(query, ctx, transaction).ConfigureAwait(false);
            await transaction?.CommitAsync();
            return productPoolId;
        }
    }
}
