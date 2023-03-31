
using CCApiLibrary.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Kubernetes;
using System.IdentityModel.Tokens.Jwt;

namespace CCApiGateway
{
    public class Program
    {
        public static void Main(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            IdentityModelEventSource.ShowPII = true;
            ConfigurationManager configuration = builder.Configuration;

            // Add services to the container.

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            ProgramMainHelper.AddSwaggerToServiceCollection(builder.Services);


            ProgramMainHelper.AddAuthenticationToServiceCollection(builder.Services, configuration);
            ProgramMainHelper.AddAuthorizationToServiceCollection(builder.Services);

#if DEBUG
            builder.Configuration.AddJsonFile("ocelotDebug.json", optional: false, reloadOnChange: true);
#else
            builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
#endif
            builder.Services.AddOcelot(builder.Configuration)
                .AddKubernetes();
            builder.Services.AddSwaggerForOcelot(builder.Configuration);

            builder.Services.AddAuthorization();

            var app = builder.Build();
            app.UseSwaggerForOcelotUI(opt =>
            {
                opt.PathToSwaggerGenerator = "/swagger/docs";
            });

            app.UseHttpsRedirection();

            app.UseOcelot().Wait();

            app.UseAuthorization();

            app.Run();
        }
    }
}