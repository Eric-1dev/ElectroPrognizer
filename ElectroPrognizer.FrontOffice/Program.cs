using System.Text;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using BundlerMinifier.TagHelpers;
using ElectroPrognizer.DataModel.Models;
using ElectroPrognizer.FrontOffice.Controllers;
using ElectroPrognizer.IoC;
using ElectroPrognizer.Utils.Helpers;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory(builder =>
{
    builder.RegisterType<HomeController>().PropertiesAutowired();

    builder.InitContainer();
}));

var connectionString = builder.Configuration.GetConnectionString("Database");
ConfigurationHelper.SetConnectionString(connectionString);

var configSection = builder.Configuration.GetSection("AppConfiguration").Get<AppConfiguration>();

builder.Services.Configure<AppConfiguration>(option => builder.Configuration.GetSection("AppConfiguration"));
ConfigurationHelper.SetConfiguration(configSection);

builder.Services.AddMvc().AddControllersAsServices();
builder.Services.AddControllersWithViews();

builder.Services.AddBundles(options =>
{
    options.AppendVersion = true;
    options.UseMinifiedFiles = false;
    options.UseBundles = false;
});

var app = builder.Build();

Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

app.UseDeveloperExceptionPage();

app.UseHsts();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
