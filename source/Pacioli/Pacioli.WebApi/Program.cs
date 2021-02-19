using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Pacioli.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>()
                        //This has to be explicitly set to pass the test.
                        .UseSetting("https_port", "443");
                });
    }
}
