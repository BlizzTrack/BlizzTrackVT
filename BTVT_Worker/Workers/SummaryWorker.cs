using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
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
        private readonly ConcurrentQueue<BNetLib.Models.Summary> _queue;

        private readonly Versions _versions;
        private readonly BGDL _bgdl;
        private readonly CDN _cdn;

        private CancellationToken _cancellationToken;
        private bool _running;

        public SummaryWorker(IHostApplicationLifetime appLifetime, ILogger<SummaryWorker> logger, BNetClient bNetClient, Summary summary, ConcurrentQueue<BNetLib.Models.Summary> queue, Versions versions, BGDL bgdl, CDN cdn)
        {
            _appLifetime = appLifetime;
            _logger = logger;
            _bNetClient = bNetClient;
            _summary = summary;
            _queue = queue;
            _versions = versions;
            _bgdl = bgdl;
            _cdn = cdn;
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

                        foreach (var item in values)
                        {
                            var previous = localLatest?.Value?.FirstOrDefault(x =>
                                x.Product == item.Product && x.Flags == item.Flags);

                            var exist = await ItemExist(item);

                            if (!exist || previous == null || previous.Seqn != item.Seqn)
                            {
                                _queue.Enqueue(item);
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

        private async Task<bool> ItemExist(BNetLib.Models.Summary item)
        {
            switch (item.Flags.ToLower())
            {
                case "cdn":
                    var c = await _cdn.Get(item.Product.ToLower(), item.Seqn);
                    return c != null;
                case "versions":
                    var v = await _versions.Get(item.Product.ToLower(), item.Seqn);
                    return v != null;
                case "bgdl":
                    var b = await _bgdl.Get(item.Product.ToLower(), item.Seqn);
                    return b != null;
            }

            return false;
        }
    }
}
