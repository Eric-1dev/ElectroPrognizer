using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using ElectroPrognizer.DataModel.Models;
using ElectroPrognizer.IoC;
using ElectroPrognizer.Utils.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Main;

public class Program
{
    [STAThread]
    public static void Main()
    {
        var builder = Host.CreateDefaultBuilder();

        builder.ConfigureServices((hostContext, services) =>
            {
                var connectionString = hostContext.Configuration.GetConnectionString("Database");
                ConfigurationHelper.SetConnectionString(connectionString);

                var config = new AppConfiguration();
                hostContext.Configuration.GetSection("AppConfiguration").Bind(config);
                ConfigurationHelper.SetConfiguration(config);

                services.Configure<AppConfiguration>(option => hostContext.Configuration.GetSection("AppConfiguration"));
            });

        builder.UseServiceProviderFactory(new AutofacServiceProviderFactory(builder =>
        {
            builder.RegisterType<App>().PropertiesAutowired().SingleInstance();
            builder.RegisterType<MainWindow>().PropertiesAutowired().SingleInstance();
            builder.InitContainer();
        }));

        var host = builder.Build();

        var app = host.Services.GetService<App>();

        app?.Run();
    }
}
