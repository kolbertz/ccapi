using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCApiLibrary.Helper
{
    public static class ProgramMainHelper
    {
        public static void AddSwaggerToServiceCollection(IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v2", new OpenApiInfo
                {
                    Title = "CashControl Product Service Version 2",
                    Description = "This is an CashControl Product Service using Microsofts Minimal API with Dapper and EF Core as ORM Mapper"
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
                            TokenUrl = new Uri(@"http://20.103.171.17:80/token"),
                            AuthorizationUrl = new Uri(@"http://20.103.171.17:80/Authorize"),
                        }
                    },
                    Scheme = "globalAuth",
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
        }

        public static void AddAuthenticationToServiceCollection(IServiceCollection services, IConfiguration configuration)
        {
            SecurityKey signingKey = new SymmetricSecurityKey(Convert.FromBase64String(configuration["TokenAuthentication:SecretKey"]));

            services.AddAuthentication(o => {
                o.DefaultScheme = "monolithAuth";
            })
            .AddJwtBearer("monolithAuth", options =>
            {
                options.Audience = "all";
                options.ClaimsIssuer = "cashcontrol";
                options.Authority = "ccapi";
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
            }).AddJwtBearer("globalAuth", options =>
            {
                options.Audience = "all";
                options.ClaimsIssuer = "cashcontrol";
                options.Authority = "ccapi";
                options.Configuration = new Microsoft.IdentityModel.Protocols.OpenIdConnect.OpenIdConnectConfiguration
                {
                    //AuthorizationEndpoint = @"https://localhost:7092/Home/Authorize\",
                    TokenEndpoint = @"http://20.103.171.17:80/Token",
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
        }

        public static void AddAuthorizationToServiceCollection(IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.DefaultPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .AddAuthenticationSchemes("monolithAuth", "globalAuth")
                    .Build();
            });
        }

        public static void AddSwaggerUi(WebApplication app, IConfiguration configuration)
        {
            app.UseSwagger();
            app.UseSwaggerUI(opt =>
            {
                opt.SwaggerEndpoint("/swagger/v2/swagger.json", "v2");
                opt.OAuthClientId(configuration["Authentication:ClientId"]);
                opt.OAuthClientSecret(configuration["Authentication:ClientSecret"]);
                opt.OAuthUsePkce();
            });
        }

        public static void AddCors(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAllHeaders",
                    builder => {
                        builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                    });
            });
        }

        public static void UseCors(WebApplication app)
        {
            app.UseCors("AllowAllHeaders");
        }
    }
}
