using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using System.Threading;
using System.Threading.Tasks;

namespace HOMMS.API
{
    public class PrintUrlsHostedService : IHostedService
    {
        private readonly ILogger<PrintUrlsHostedService> _logger;
        private readonly IServer _server;
        private readonly IHostApplicationLifetime _lifetime;

        public PrintUrlsHostedService(
            ILogger<PrintUrlsHostedService> logger,
            IServer server,
            IHostApplicationLifetime lifetime)
        {
            _logger = logger;
            _server = server;
            _lifetime = lifetime;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _lifetime.ApplicationStarted.Register(() =>
            {
                var addresses = _server.Features.Get<IServerAddressesFeature>()?.Addresses;
                if (addresses != null)
                {
                    foreach (var address in addresses)
                    {
                        _logger.LogInformation("Now listening on: {Address}", address);
                    }
                }
            });
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
} 