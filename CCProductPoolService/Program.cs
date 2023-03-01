using CCApiLibrary.CustomAttributes;
using CCApiLibrary.DbConnection;
using CCApiLibrary.Interfaces;
using CCProductPoolService;
using CCProductPoolService.Data;
using CCProductPoolService.Interface;
using CCProductPoolService.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v2", new OpenApiInfo
            {
                Title = "CashControl ProductPool Service Version 2",
                Description = "This is an CashControl ProductPool Service with Dapper and EF Core as ORM Mapper"
            });
            c.EnableAnnotations();

            c.AddSecurityDefinition("cclive", new OpenApiSecurityScheme()
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
                Scheme = "monolithAuth",
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
                            Id = "cclive"
                        }
                    },
                    new string[] {}
                }
                    });

            c.AddSecurityDefinition("ccauthService", new OpenApiSecurityScheme()
            {
                Name = HeaderNames.Authorization,
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows
                {
                    AuthorizationCode = new OpenApiOAuthFlow
                    {
                        TokenUrl = new Uri(@"https://localhost:7092/Home/token"),
                        AuthorizationUrl = new Uri(@"https://localhost:7092/Home/Authorize"),
                    }
                },
                Scheme = "gloabalAuth",
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
                            Id = "ccauthService"
                        }
                    },
                    new string[] {}
                }
            });
        });

        builder.Services.AddDbContext<AramarkDbProduction20210816Context>(options =>
            options.UseSqlServer(configuration.GetConnectionString("AramarkStaging")));
        builder.Services.AddScoped<IApplicationDbConnection, ApplicationDbConnection>();
        builder.Services.AddScoped<IProductPoolRepository, ProductPoolRepository>();
        builder.Services.AddScoped<ValidateModelAttribute>();
        builder.Services.Configure<ApiBehaviorOptions>(Options=> Options.SuppressModelStateInvalidFilter = true);

        SecurityKey signingKey = new SymmetricSecurityKey(Convert.FromBase64String(configuration["TokenAuthentication:SecretKey"]));
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer("monolithAuth", options =>
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
        }).AddJwtBearer("gloabalAuth", options =>
            {
                options.Audience = "all";
                options.ClaimsIssuer = "localhost";
                options.Authority = "https://localhost:7092";
                options.Configuration = new Microsoft.IdentityModel.Protocols.OpenIdConnect.OpenIdConnectConfiguration
                {
                //AuthorizationEndpoint = @"https://localhost:7092/Home/Authorize\",
                            TokenEndpoint = @"https://localhost:7092/Home/Token",
                        };
                        options.RequireHttpsMetadata = false;
                        SecurityKey signingKey = new SymmetricSecurityKey(Convert.FromBase64String(configuration["TokenAuthentication:SecretKey"]));
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateAudience = true,
                            ValidateIssuer = true,
                            ValidateIssuerSigningKey = false,
                            ValidateActor = false,
                            ValidateLifetime = false,
                            ValidateTokenReplay = false,
                            ValidIssuer = configuration["TokenAuthentication:Issuer"],
                            ValidAudience = configuration["TokenAuthentication:Audience"],
                            IssuerSigningKey = signingKey,
                            SignatureValidator = delegate (string token, TokenValidationParameters validationParameters)
                            {
                                var jwt = new JwtSecurityToken(token);
                                return jwt;
                            }
                        };
                    });


        builder.Services.AddAuthorization(options =>
        {
            options.DefaultPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .AddAuthenticationSchemes("monolithAuth", "gloabalAuth")
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
        app.UseCors(builder => builder
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader());
        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();
        app.Run();
    }
}