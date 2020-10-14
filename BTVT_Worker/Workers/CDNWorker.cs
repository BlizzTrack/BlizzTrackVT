using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BNetLib.Networking;
using BNetLib.Networking.Commands;
using BTSharedCore.Data;
using BTSharedCore.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using CDN = BTSharedCore.Data.CDN;
using Summary = BTSharedCore.Data.Summary;
using Version = BTSharedCore.Models.Version;

namespace BTVT_Worker.Workers
{
    internal class CDNWorker : IHostedService
    {
        private readonly IHostApplicationLifetime _appLifetime;
        private readonly ILogger<CDNWorker> _logger;
        private readonly BNetClient _bNetClient;
        private readonly CDN _cdn;
        private readonly Summary _summary;
        private CancellationToken _cancellationToken;
        private bool _running;

        public CDNWorker(IHostApplicationLifetime appLifetime, ILogger<CDNWorker> logger, BNetClient bNetClient, CDN cdn, Summary summary)
        {
            _appLifetime = appLifetime;
            _logger = logger;
            _bNetClient = bNetClient;
            _cdn = cdn;
            _summary = summary;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _appLifetime.ApplicationStarted.Register(OnStarted);
            _appLifetime.ApplicationStopping.Register(OnStopping);
            _appLifetime.ApplicationStopped.Register(OnStopped);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _cancellationToken = cancellationToken;

            return Task.CompletedTask;
        }

        private void OnStarted()
        {
            _running = true;
            _logger.LogInformation("OnStarted has been called.");

            Task.Run(async () =>
            {
                while (_running)
                {
                    try
                    {
                        var localLatest = await _summary.Latest();
                        foreach (var item in localLatest.Value.Where(x => x.Flags == "cdn"))
                        {
                            var latestVersion = await _cdn.Latest(item.Product);
                            if (latestVersion?.Seqn != item.Seqn)
                            {
                                var (value, seqn) = await _bNetClient.Do<List<BNetLib.Models.CDN>>(
                                    new CDNCommand(item.Product.ToLower()));

                                await _cdn.Insert(new BTSharedCore.Models.CDN()
                                {
                                    Seqn = seqn,
                                    Value = value,
                                    Product = item.Product,
                                });
                            }
                        }
                    }
                    catch
                    {
                        // ignored
                    }

                    await Task.Delay(TimeSpan.FromMinutes(1), _cancellationToken);
                }
            }, _cancellationToken);
        }

        private void OnStopping()
        {
            _running = false;
            _logger.LogInformation("OnStopping has been called.");
        }

        private void OnStopped()
        {
            _logger.LogInformation("OnStopped has been called.");
        }
    }
}
