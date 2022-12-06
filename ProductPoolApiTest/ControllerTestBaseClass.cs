using Bazinga.AspNetCore.Authentication.Basic;
//using Castle.Core.Configuration;
using CCProductPoolService.Data;
using CCProductPoolService.Interface;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using static System.Net.Mime.MediaTypeNames;
using System.Text;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection.Extensions;
using CCProductPoolService.Repositories;
using CCProductPoolService.DapperDbConnection;

namespace ProductPoolApiTest
{
    public class ControllerTestBaseClass
    {
        protected WebApplicationFactory<Program> GetWebApplication()
        {
            var inMemorySettings = new Dictionary<string, string>
            {
                { "ConnectionStrings:AramarkStaging", "Server=tcp:kolbertz.database.windows.net,1433;Initial Catalog=CCServiceApiTestDatabase;Persist Security Info=False;User ID=cc_user;Password=!1cc#2§44ef!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;" }
            };

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            return new WebApplicationFactory<Program>().WithWebHostBuilder(builder => {
                builder.ConfigureTestServices(services => {
                    var oldOptions = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<AramarkDbProduction20210816Context>));
                    if (oldOptions != null)
                    {
                        services.Remove(oldOptions);
                    }
                    var options = new DbContextOptionsBuilder<AramarkDbProduction20210816Context>()
                    .UseSqlServer(configuration.GetConnectionString("AramarkStaging"))
                                    .Options;
                    services.AddSingleton<IConfiguration>(configuration);
                    services.AddSingleton(options);
                    services.AddSingleton<AramarkDbProduction20210816Context>();
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

        protected HttpClient CreateClientWithAuth(WebApplicationFactory<Program> application)
        {
            HttpClient client = application.CreateClient();
            var base64EncodedAuthString = Convert.ToBase64String(Encoding.UTF8.GetBytes("Test:test"));
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", base64EncodedAuthString);
            return client;
        }

        protected async Task PopulateDatabase(string commandText, AramarkDbProduction20210816Context ctx, IDbContextTransaction transaction)
        {
            var conn = ctx.Database.GetDbConnection();
            using (var cmd = conn.CreateCommand())
            {
                cmd.Transaction = transaction.GetDbTransaction();
                cmd.CommandText = commandText;
                await cmd.ExecuteNonQueryAsync();
            }
        }
    }
}
