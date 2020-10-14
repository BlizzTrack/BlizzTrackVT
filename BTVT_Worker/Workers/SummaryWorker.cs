using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BNetLib.Networking;
using BNetLib.Networking.Commands;
using BTSharedCore.Data;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BTVT_Worker.Workers
{
    internal class SummaryWorker : IHostedService
    {
        private readonly IHostApplicationLifetime _appLifetime;
        private readonly ILogger<SummaryWorker> _logger;
        private readonly BNetClient _bNetClient;
        private readonly Summary _summary;
        private CancellationToken _cancellationToken;
        private bool _running;

        public SummaryWorker(IHostApplicationLifetime appLifetime, ILogger<SummaryWorker> logger, BNetClient bNetClient, Summary summary)
        {
            _appLifetime = appLifetime;
            _logger = logger;
            _bNetClient = bNetClient;
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
                        var (values, seqn) = await _bNetClient.Do<List<BNetLib.Models.Summary>>(new SummaryCommand());

                        if (localLatest?.Seqn != seqn)
                        {
                            await _summary.Insert(new BTSharedCore.Models.Summary()
                            {
                                Seqn = seqn,
                                Value = values
                            });
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
