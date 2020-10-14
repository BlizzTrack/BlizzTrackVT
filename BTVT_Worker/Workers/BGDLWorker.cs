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
using BGDL = BTSharedCore.Data.BGDL;
using Summary = BTSharedCore.Data.Summary;
using Version = BTSharedCore.Models.Version;

namespace BTVT_Worker.Workers
{
    internal class BGDLWorker : IHostedService
    {
        private readonly IHostApplicationLifetime _appLifetime;
        private readonly ILogger<BGDLWorker> _logger;
        private readonly BNetClient _bNetClient;
        private readonly BGDL _bgdl;
        private readonly Summary _summary;
        private CancellationToken _cancellationToken;
        private bool _running;

        public BGDLWorker(IHostApplicationLifetime appLifetime, ILogger<BGDLWorker> logger, BNetClient bNetClient, BGDL bgdl, Summary summary)
        {
            _appLifetime = appLifetime;
            _logger = logger;
            _bNetClient = bNetClient;
            _bgdl = bgdl;
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
                        foreach (var item in localLatest.Value.Where(x => x.Flags == "bgdl"))
                        {
                            var latestVersion = await _bgdl.Latest(item.Product);
                            if (latestVersion?.Seqn != item.Seqn)
                            {
                                var (value, seqn) = await _bNetClient.Do<List<BNetLib.Models.Version>>(
                                    new BGDLCommand(item.Product.ToLower()));

                                await _bgdl.Insert(new BTSharedCore.Models.BGDL()
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
