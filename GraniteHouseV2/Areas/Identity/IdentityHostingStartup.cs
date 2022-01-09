using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(GraniteHouseV2.Areas.Identity.IdentityHostingStartup))]
namespace GraniteHouseV2.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
            });
        }
    }
}