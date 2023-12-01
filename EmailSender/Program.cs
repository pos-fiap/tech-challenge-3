
using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
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
                    services.AddHostedService<Worker>();
                });
    }
}
