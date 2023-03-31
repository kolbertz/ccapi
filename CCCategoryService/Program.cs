using CCApiLibrary.DbConnection;
using CCApiLibrary.Interfaces;
using CCCategoryService.Interface;
using CCCategoryService.Repositories;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Logging;
using CCApiLibrary.CustomAttributes;
using Microsoft.AspNetCore.Mvc;
using CCApiLibrary.Helper;

public class Program
{
    private static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
        IdentityModelEventSource.ShowPII = true;

        // Add services to the container.

        ConfigurationManager configuration = builder.Configuration;
        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        ProgramMainHelper.AddSwaggerToServiceCollection(builder.Services);

        builder.Services.AddScoped<IApplicationDbConnection, ApplicationDbConnection>();
        builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
        builder.Services.AddScoped<ICategoryPoolRepository, CategoryPoolRepository>();
        builder.Services.AddScoped<ValidateModelAttribute>();
        builder.Services.Configure<ApiBehaviorOptions>(Options => Options.SuppressModelStateInvalidFilter = true);

        ProgramMainHelper.AddAuthenticationToServiceCollection(builder.Services, configuration);
        ProgramMainHelper.AddAuthorizationToServiceCollection(builder.Services);

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        ProgramMainHelper.AddSwaggerUi(app, configuration);

        ProgramMainHelper.UseCors(app);

        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}