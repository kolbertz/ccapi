using CCAuthServer.Services;
using CCAuthServer.Services.CodeService;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddControllersWithViews();
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddSingleton<ICodeStoreService, CodeStoreService>();
        builder.Services.AddScoped<IAuthorizeResultService, AuthorizeResultService>(); 
        var app = builder.Build();
        app.UseStaticFiles();
        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            _ = endpoints.MapDefaultControllerRoute();
        });
        app.Run();
    }
}