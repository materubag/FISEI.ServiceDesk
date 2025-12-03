using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace FISEI.ServiceDesk.Web.Services;

public class AuthHeaderHandler : DelegatingHandler
{
    private readonly AuthClientService _auth;
    public AuthHeaderHandler(AuthClientService auth) => _auth = auth;

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = _auth.GetToken();
        if (!string.IsNullOrWhiteSpace(token) && request.Headers.Authorization is null)
        {
            request.Headers.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }
        return base.SendAsync(request, cancellationToken);
    }
}