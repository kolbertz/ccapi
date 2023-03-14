using Bazinga.AspNetCore.Authentication.Basic;
//using Castle.Core.Configuration;
using CCProductPoolService.Data;
using CCProductPoolService.Interface;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Text;
using CCProductPoolService.Repositories;
using System.Data.Common;
using System.Data;
using CCApiLibrary.DbConnection;
using CCApiLibrary.Interfaces;
using CCProductService.Interface;
using CCProductService.Repositories;

namespace CCApiTest.Base
{
    public class ControllerTestBaseClass
    {
        protected Guid systemSettingsId = new Guid("fab8c985-6147-4eba-b2c7-5f7012c4aeeb");

        protected WebApplicationFactory<IProgram> GetWebApplication()
        {
            var inMemorySettings = new Dictionary<string, string>
            {
                { "ConnectionStrings:TestDatabase", "Server=tcp:kolbertz.database.windows.net,1433;Initial Catalog=CCServiceApiTestDatabase;Persist Security Info=False;User ID=cc_user;Password=!1cc#2§44ef!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;" }
            };

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            return new WebApplicationFactory<IProgram>().WithWebHostBuilder(builder => {
                builder.ConfigureTestServices(services => {
                    var dbConnectionDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbConnection));
                    if (dbConnectionDescriptor != null)
                    {
                        services.Remove(dbConnectionDescriptor);
                    }
                   
                    services.AddSingleton<IConfiguration>(configuration);
                    services.AddSingleton<IApplicationDbConnection, ApplicationDbConnection>();
                    services.AddSingleton<IProductPoolRepository, ProductPoolRepository>();
                    services.AddSingleton<IProductRepository, ProductRepository>();
                    services.AddAuthentication()
                        .AddBasicAuthentication(credentials => Task.FromResult(credentials.username == "Test" && credentials.password == "test"));
                    services.AddAuthorization(config => {
                        config.DefaultPolicy = new Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder(config.DefaultPolicy)
                                                    .AddAuthenticationSchemes(BasicAuthenticationDefaults.AuthenticationScheme)
                                                    .Build();
                    });
                });
            });
        }

        protected HttpClient CreateClientWithAuth(WebApplicationFactory<IProgram> application)
        {
            HttpClient client = application.CreateClient();
            var base64EncodedAuthString = Convert.ToBase64String(Encoding.UTF8.GetBytes("Test:test"));
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", base64EncodedAuthString);
            return client;
        }

        protected Task PopulateDatabase(string commandText, IApplicationDbConnection dbConnection)
        {
            return dbConnection.ExecuteAsync(commandText);
        }

        protected Task<Guid> PopulateDatabaseAndReturnIdentity(string commandText, IApplicationDbConnection dbConnection)
        {
            return dbConnection.ExecuteScalarAsync<Guid>(commandText);
        }
    }
}
