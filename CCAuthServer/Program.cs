using CCAuthServer.Context;
using CCAuthServer.Services;
using CCAuthServer.Services.CodeService;
using Microsoft.EntityFrameworkCore;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        ConfigurationManager configuration = builder.Configuration;

        builder.Services.AddDbContext<UserDBContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("UserDB"));
        });

        builder.Services.AddControllersWithViews();
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddSingleton<ICodeStoreService, CodeStoreService>();
        builder.Services.AddScoped<IAuthorizeResultService, AuthorizeResultService>();
        builder.Services.AddScoped<IUserRepository, UserRepository>();

        var app = builder.Build();
        app.UseStaticFiles();
        app.UseRouting();

        app.UseCors(builder => builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());

        app.UseAuthentication();
        app.UseAuthorization();

        _ = app.UseEndpoints(endpoints =>
        {
            _ = endpoints.MapDefaultControllerRoute();
        });
        app.Run();
    }
}