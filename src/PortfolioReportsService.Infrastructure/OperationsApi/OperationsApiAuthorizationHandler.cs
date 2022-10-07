using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using PortfolioReportsService.Infrastructure.Configuration;

namespace OperationsNotifications.Infrastructure.OperationsApi;

internal class OperationsApiAuthenticationHandler : DelegatingHandler
{
    private readonly PortfolioReportsServiceConfiguration _configDto;
    private string _token;

    public OperationsApiAuthenticationHandler(PortfolioReportsServiceConfiguration configDto)
    {
        _configDto = configDto;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var token = _token ??= await GetToken().ConfigureAwait(false);

        request.Headers.Add("Authorization", token);
        var response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);

        if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
        {
            token = _token = await GetToken().ConfigureAwait(false);

            request.Headers.Remove("Authorization");
            request.Headers.Add("Authorization", token);
            response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        }

        return response;
    }

    private async Task<string> GetToken()
    {
        var clientCredential =
            new ClientCredential(_configDto.OperationsApiAdClientId, _configDto.OperationsApiAdClientSecret);
        var context =
            new AuthenticationContext($"{_configDto.OperationsApiAdInstance}/{_configDto.OperationsApiAdTenantId}",
                false);
        var token = await context.AcquireTokenAsync(_configDto.OperationsApiAdClientId, clientCredential);
        return token.CreateAuthorizationHeader();
    }
}