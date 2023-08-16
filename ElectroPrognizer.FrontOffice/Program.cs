using System.Reflection;
using System.Text;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using BundlerMinifier.TagHelpers;
using ElectroPrognizer.IoC;
using ElectroPrognizer.SchedulerServices.Extensions;
using ElectroPrognizer.Utils.Helpers;
using Microsoft.AspNetCore.Mvc;
using Quartz;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory(builder =>
{
    var controllersAssembly = Assembly.GetExecutingAssembly();
    var controllers = controllersAssembly.GetTypes().Where(type => !type.IsAbstract && type.IsPublic && type.IsAssignableTo<ControllerBase>());

    foreach (var controller in controllers)
    {
        builder.RegisterType(controller).PropertiesAutowired();
    }

    var jobsAssembly = Assembly.GetAssembly(typeof(QuartzExtensions));
    var jobs = jobsAssembly.GetTypes().Where(x => !x.IsAbstract && x.IsAssignableTo<IJob>());

    foreach (var job in jobs)
    {
        builder.RegisterType(job).InstancePerLifetimeScope().PropertiesAutowired();
    }

    builder.InitContainer();
}));

var connectionString = builder.Configuration.GetConnectionString("Database");
ConfigurationHelper.SetConnectionString(connectionString);

builder.Services.AddQuartz(config =>
{
    config.RegisterJobs();
});

builder.Services.AddQuartzHostedService();

builder.Services
    .AddMvc()
    .AddControllersAsServices();

builder.Services.AddControllersWithViews();

builder.Services.AddHttpContextAccessor();

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
