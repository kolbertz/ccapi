using CCProductPoolService.DapperDbConnection;
using CCProductPoolService.Data;
using CCProductPoolService.Interface;
using CCProductPoolService.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

public class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        ConfigurationManager configuration = builder.Configuration;

        // Add services to the container.

        builder.Services.AddControllers().AddNewtonsoftJson();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v2", new OpenApiInfo
            {
                Title = "CashControl ProductPool Service Version 2",
                Description = "This is an CashControl ProductPool Service with Dapper and EF Core as ORM Mapper"
            });
            c.EnableAnnotations();
            c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme()
            {
                Name = "Authorization",
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows
                {
                    AuthorizationCode = new OpenApiOAuthFlow
                    {
                        TokenUrl = new Uri(@"https://staging-signin.cashcontrol.com/OAuth/token"),
                        AuthorizationUrl = new Uri(@"https://staging-signin.cashcontrol.com/OAuth/Authorize"),
                    }
                },
                Scheme = JwtBearerDefaults.AuthenticationScheme,
                In = ParameterLocation.Header,
                Description = "JSON Web Token based security"
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
                    {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "oauth2"
                        }
                    },
                    new string[] {}
                }
                    });
        });

        builder.Services.AddDbContext<AramarkDbProduction20210816Context>(options =>
            options.UseSqlServer(configuration.GetConnectionString("AramarkStaging")));
        //builder.Services.AddScoped<IApplicationDbContext>(provider => provider.GetService<AramarkDbProduction20210816Context>());
        builder.Services.AddScoped<IApplicationWriteDbConnection, ApplicationWriteDbConnection>();
        builder.Services.AddScoped<IApplicationReadDbConnection, ApplicationReadDbConnection>();
        builder.Services.AddScoped<IProductPoolRepository, ProductPoolRepository>();

        SecurityKey signingKey = new SymmetricSecurityKey(Convert.FromBase64String(configuration["TokenAuthentication:SecretKey"]));
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
        {
            options.Audience = "all";
            options.ClaimsIssuer = "localhost";
            options.Authority = "http://localhost:7298";
            options.Configuration = new Microsoft.IdentityModel.Protocols.OpenIdConnect.OpenIdConnectConfiguration
            {
                AuthorizationEndpoint = @"https://staging-signin.cashcontrol.com/OAuth/Authorize\",
                TokenEndpoint = @"https://staging-signin.cashcontrol.com/OAuth/token",
            };
            options.RequireHttpsMetadata = false;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                ValidIssuer = configuration["TokenAuthentication:Issuer"],
                ValidAudience = configuration["TokenAuthentication:Audience"],
                IssuerSigningKey = signingKey
            };
        });


        builder.Services.AddAuthorization(options =>
        {
            options.DefaultPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                .Build();
        });


        var app = builder.Build();

        // Configure the HTTP request pipeline.
        app.UseSwagger();
        app.UseSwaggerUI(opt =>
        {
            opt.SwaggerEndpoint("/swagger/v2/swagger.json", "v2");
            opt.OAuthClientId(configuration["Authentication:ClientId"]);
            opt.OAuthClientSecret(configuration["Authentication:ClientSecret"]);
            opt.OAuthUsePkce();
        });
        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();
        app.Run();
    }
}