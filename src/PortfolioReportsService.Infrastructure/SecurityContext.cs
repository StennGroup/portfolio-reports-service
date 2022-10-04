using System.Diagnostics.CodeAnalysis;
using System.Security.Principal;
using PortfolioReportsService.Application.Port;

namespace PortfolioReportsService.Infrastructure
{
    public sealed class SecurityContext : IUserContext
    {
        public SecurityContext([NotNull] IIdentity identity)
        {
            UserName = identity.Name;
        }

        public string UserName { get; private set; }
    }
}