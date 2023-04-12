using CCApiLibrary.CustomAttributes;
using CCApiLibrary.DbConnection;
using CCApiLibrary.Helper;
using CCApiLibrary.Interfaces;
using CCProductPoolService.Interface;
using CCProductPoolService.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;

public class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
        IdentityModelEventSource.ShowPII = true;
        ConfigurationManager configuration = builder.Configuration;

        // Add services to the container.

        builder.Services.AddControllers().AddNewtonsoftJson();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        ProgramMainHelper.AddSwaggerToServiceCollection(builder.Services);

        builder.Services.AddScoped<IApplicationDbConnection, ApplicationDbConnection>();
        builder.Services.AddScoped<IProductPoolRepository, ProductPoolRepository>();
        builder.Services.AddScoped<ValidateModelAttribute>();
        builder.Services.Configure<ApiBehaviorOptions>(Options=> Options.SuppressModelStateInvalidFilter = true);

        ProgramMainHelper.AddAuthenticationToServiceCollection(builder.Services, configuration);
        ProgramMainHelper.AddAuthorizationToServiceCollection(builder.Services);

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        ProgramMainHelper.AddSwaggerUi(app, configuration);

        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();
        app.Run();
    }
}