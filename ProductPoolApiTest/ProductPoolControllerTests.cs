using CCProductPoolService.Data;
using CCProductPoolService.Dtos;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Text;

namespace ProductPoolApiTest
{
    public class ProductPoolControllerTests : ControllerTestBaseClass, IClassFixture<TestRunStart>
    {
        public ProductPoolControllerTests() {}

        [Fact]
        public async Task Get_returns_401_Unauthorized_if_not_authenticated()
        {
            var application = GetWebApplication();

            var client = application.CreateClient();
            var respone = await client.GetAsync("/api/v2/productpool");
            Assert.Equal(HttpStatusCode.Unauthorized, respone.StatusCode);
        }

        [Fact]
        public async Task Get_returns_200_Authorized()
        {
            var application = GetWebApplication();
            var client = application.CreateClient();
            var base64EncodedAuthString = Convert.ToBase64String(Encoding.UTF8.GetBytes("Test:test"));
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", base64EncodedAuthString);

            var respone = await client.GetAsync("/api/v2/productpool");
            var message = await respone.Content.ReadAsStringAsync();
            Assert.Equal(HttpStatusCode.OK, respone.StatusCode);
        }

        [Fact]
        public async Task Post_returns_201_if_successful_Get_returns_200_and_Pool()
        {
            var application = GetWebApplication();
             
            using (var services = application.Services.CreateScope())
            {
                IDbContextTransaction transaction = null;
                try
                {
                    var ctx = services.ServiceProvider.GetService<AramarkDbProduction20210816Context>();
                    transaction = ctx.Database.BeginTransaction();

                    // Populate DB with a systemSetting
                    Guid systemSettingsId = new Guid("fab8c985-6147-4eba-b2c7-5f7012c4aeeb");
                    var commandText = "INSERT INTO SystemSettings (Id, InternalName, [Name], DistributorId, IsBlocked, IsHosted, SystemType, AddressName1, " +
                        "AddressStreet, AddressPostalCode, AddressCity, DefaultTimeZone, MaxCustomCurrencyExchangeRateDiff, MinPriceUnit, NoDeleteRange, " +
                        "CreatedDate, CreatedUser, LastUpdatedDate, LastUpdatedUser, [Key]) " +
                        $"VALUES('{systemSettingsId}', 'TestSystem', 'TestSystem', 0, 0, 0, 0, 'Test', 'Test', 'Test', 'Test', 'Test', 0, 0, 0, GETDATE(), '313de81f-a37c-422b-8e6d-fbff6c02eb6f', GETDATE(), '313de81f-a37c-422b-8e6d-fbff6c02eb6f', 0);";

                    await PopulateDatabase(commandText, ctx, transaction);
                    //transaction.Commit();
                    //transaction = ctx.Database.BeginTransaction();
                    var client = CreateClientWithAuth(application);
                    ProductPoolDto productPool = new ProductPoolDto
                    {
                        Description = "ApiController Test Pool",
                        Name = "ApiController Test Pool",
                        Key = 1,
                        SystemSettingsId = systemSettingsId
                    };
                    HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(productPool), Encoding.UTF8, "application/json");

                    var response = await client.PostAsync("/api/v2/productpool", httpContent);
                    var message = await response.Content.ReadAsStringAsync();
                    Assert.Equal(HttpStatusCode.Created, response.StatusCode);

                    var poolId = Guid.Parse(response.Headers.Location!.PathAndQuery.Substring(response.Headers.Location!.PathAndQuery.LastIndexOf("/") + 1));

                    response = await client.GetAsync("/api/v2/productpool/" + poolId);
                    message = await response.Content.ReadAsStringAsync();
                    dynamic pool = JObject.Parse(message);
                    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                    Assert.Equal(1, (int)pool.key);
                    Assert.Equal("ApiController Test Pool", (string)pool.description);
                    Assert.Equal("ApiController Test Pool", (string)pool.name);
                    Assert.Equal(poolId, (Guid)pool.id);
                }
                finally
                {
                    transaction?.Rollback();
                }
            }
        }
    }
}
