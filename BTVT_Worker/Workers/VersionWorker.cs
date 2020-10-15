using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BNetLib.Networking;
using BNetLib.Networking.Commands;
using BTSharedCore.Data;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Summary = BTSharedCore.Data.Summary;
using Version = BTSharedCore.Models.Version;

namespace BTVT_Worker.Workers
{
    internal class VersionWorker : IHostedService
    {
        private readonly IHostApplicationLifetime _appLifetime;
        private readonly ILogger<VersionWorker> _logger;
        private readonly BNetClient _bNetClient;
        private readonly Versions _versions;
        private readonly Summary _summary;
        private CancellationToken _cancellationToken;
        private bool _running;

        public VersionWorker(IHostApplicationLifetime appLifetime, ILogger<VersionWorker> logger, BNetClient bNetClient, Versions versions, Summary summary)
        {
            _appLifetime = appLifetime;
            _logger = logger;
            _bNetClient = bNetClient;
            _versions = versions;
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
                        foreach (var item in localLatest.Value.Where(x => x.Flags == "versions"))
                        {
                            var latestVersion = await _versions.Latest(item.Product);
                            if (latestVersion?.Seqn != item.Seqn)
                            {
                                _logger.LogInformation($"Inserting version for {item.Product}");
                                var (value, seqn) = await _bNetClient.Do<List<BNetLib.Models.Version>>(
                                    new VersionCommand(item.Product.ToLower()));

                                await _versions.Insert(new Version()
                                {
                                    Seqn = seqn,
                                    Value = value,
                                    Product = item.Product,
                                });
                            }
                        }
                    }
                    catch(Exception ex)
                    {
                        _logger.LogError($"{ex}");
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
