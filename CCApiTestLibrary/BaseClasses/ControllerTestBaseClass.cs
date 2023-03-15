using Bazinga.AspNetCore.Authentication.Basic;
using CCApiLibrary.DbConnection;
using CCApiLibrary.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Data.Common;
using System.Text;

namespace CCApiTestLibrary.BaseClasses
{
    public class ControllerTestBaseClass
    {
        private IConfiguration GetTestConfiguration()
        {
            var inMemorySettings = new Dictionary<string, string>
            {
                { "ConnectionStrings:DefaultDatabase", "Server=tcp:kolbertz.database.windows.net,1433;Initial Catalog=CCServiceApiTestDatabase;Persist Security Info=False;User ID=cc_user;Password=!1cc#2§44ef!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;" }
            };

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();
            return configuration;
        }

        private ServiceDescriptor GetDbServiceDescriptor(IServiceCollection services)
        {
            return services.SingleOrDefault(d => d.ServiceType == typeof(DbConnection));
        }

        protected void PrepareServiceCollectionForTest(IServiceCollection services)
        {
            ServiceDescriptor dbConnectionDescriptor = GetDbServiceDescriptor(services);
            if (dbConnectionDescriptor != null)
            {
                services.Remove(dbConnectionDescriptor);
            }
            services.AddSingleton<IConfiguration>(GetTestConfiguration());
            services.AddSingleton<IApplicationDbConnection, ApplicationDbConnection>();
            services.AddAuthentication()
                .AddBasicAuthentication(credentials => Task.FromResult(credentials.username == "Test" && credentials.password == "test"));
            services.AddAuthorization(config => {
                config.DefaultPolicy = new Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder(config.DefaultPolicy)
                                            .AddAuthenticationSchemes(BasicAuthenticationDefaults.AuthenticationScheme)
                                            .Build();
            });
        }

        protected void CreateBasicClientWithAuth(HttpClient client)
        {
            var base64EncodedAuthString = Convert.ToBase64String(Encoding.UTF8.GetBytes("Test:test"));
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", base64EncodedAuthString);
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
