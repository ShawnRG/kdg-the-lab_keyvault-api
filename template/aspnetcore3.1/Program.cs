using Handler;
using Microsoft.Extensions.Hosting;

namespace Server
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            Startup.CreateHostBuilder(args).Build().Run();
        }

        
    }
}
