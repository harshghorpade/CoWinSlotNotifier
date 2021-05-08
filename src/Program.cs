// =====================================================================
// CoWin Service Main class : This program fetches available vaccination 
// slots for given district/pincode using exposed CoWin API by Govt. of
// India and delivers SMS to subscribe mobile numbers. 
// ====================================================================

using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace CoWinService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseUrls("http://+:5000")
                .UseStartup<Startup>();
    }
}
