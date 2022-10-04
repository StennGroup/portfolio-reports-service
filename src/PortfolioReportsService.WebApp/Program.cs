using System.Threading.Tasks;

namespace PortfolioReportsService.WebApp
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            await new PortfolioReportsServiceServiceHost().RunAsync(args);
        }
    }
}