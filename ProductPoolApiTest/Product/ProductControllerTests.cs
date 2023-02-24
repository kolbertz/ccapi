using Azure;
using CCProductPoolService.Interface;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data;
using System.Data.Common;
using System.Net;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using CCApiTest.Base;
using CCApiLibrary.Interfaces;

namespace CCApiTest.Product
{
    public class ProductControllerTests : ProductControllerBase, IClassFixture<CCApiTestStart>
    {
        public ProductControllerTests() { }

        [Fact]
        public async void Get_returns_401_Unauthorized_if_not_authenticated()
        {
            var application = GetWebApplication();

            var client = application.CreateClient();
            var respone = await client.GetAsync("/api/v2/product");
            Assert.Equal(HttpStatusCode.Unauthorized, respone.StatusCode);
        }

        [Fact]
        public async void Get_returns_200_Authorized()
        {
            var application = GetWebApplication();
            var client = application.CreateClient();
            var base64EncodedAuthString = Convert.ToBase64String(Encoding.UTF8.GetBytes("Test:test"));
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", base64EncodedAuthString);

            var respone = await client.GetAsync("/api/v2/product");
            string message = await respone.Content.ReadAsStringAsync();
            Assert.Equal(HttpStatusCode.OK, respone.StatusCode);
        }

        [Fact]
        public async void Get_All_returns_200_And_List_if_successful()
        {
            var application = GetWebApplication();
            using (var services = application.Services.CreateScope())
            {
                IApplicationDbConnection dbConnection = null;
                try
                {
                    dbConnection = services.ServiceProvider.GetService<IApplicationDbConnection>();
                    dbConnection.Init("TestDatabase");
                    // Populate DB
                    await PopulateDatabaseWithList(dbConnection);
                    var client = CreateClientWithAuth(application);
                    var respone = await client.GetAsync("/api/v2/product");
                    string messsage = await respone.Content.ReadAsStringAsync();
                    dynamic products = JArray.Parse(messsage);
                    Assert.Equal(HttpStatusCode.OK, respone.StatusCode);
                    Assert.Equal(2, products.Count);
                    Assert.Equal(1, (int)products[0].key);
                    Assert.Equal("Pool 2", (string)products[1].name);
                }
                catch (Exception ex)
                {

                }
                finally
                {
                    await DePopulateDatabase(dbConnection);
                }
            }
        }
    }
}
