
using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using EmailSender.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.Reflection;

namespace EmailSender
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
            => Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, config) =>
                {
                    var builtConfiguration = config.Build();
                    config.SetBasePath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
                    config.AddJsonFile("appsettings.json", optional: false, false);
                    config.AddEnvironmentVariables();

                    string kvUrl = builtConfiguration["KeyVaultConfig:KeyVaultUrl"];
                    string tenantId = builtConfiguration["KeyVaultConfig:TenantId"];
                    string clientId = builtConfiguration["KeyVaultConfig:ClientId"];
                    string clientSecret = builtConfiguration["KeyVaultConfig:ClientSecret"];

                    var credential = new ClientSecretCredential(tenantId, clientId, clientSecret);

                    var client = new SecretClient(new Uri(kvUrl), credential);
                    config.AddAzureKeyVault(client, new AzureKeyVaultConfigurationOptions());
                })
                .ConfigureServices((hostContext, services) =>
                {
                    var configuration = hostContext.Configuration;
                    var uri = new Uri(configuration["postgre-connection"]);
                    var db = uri.AbsolutePath.Trim('/');
                    var user = uri.UserInfo.Split(':')[0];
                    var passwd = uri.UserInfo.Split(':')[1];
                    var port = uri.Port > 0 ? uri.Port : 5432;
                    var connStr = string.Format("Server={0};Database={1};User Id={2};Password={3};Port={4}",
                        uri.Host, db, user, passwd, port);

                    services.AddDbContext<AppDbContext>(options =>
                        options.UseNpgsql(connStr));
                    services.AddHostedService<Worker>();
                });
    }
}
