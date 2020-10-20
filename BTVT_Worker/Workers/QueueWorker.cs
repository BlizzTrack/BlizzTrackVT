using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BNetLib.Networking;
using BNetLib.Networking.Commands;
using BTSharedCore.Data;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Summary = BNetLib.Models.Summary;

namespace BTVT_Worker.Workers
{
    public class QueueWorker : IHostedService
    {
        private readonly ILogger<QueueWorker> _logger;
        private readonly Versions _versions;
        private readonly BGDL _bgdl;
        private readonly CDN _cdn;
        private readonly BNetClient _bNetClient;
        private readonly ConcurrentQueue<Summary> _queue;
        private readonly IHostApplicationLifetime _applicationLifetime;
        private CancellationToken _cancellationToken;

        private bool _running;

        public QueueWorker(ILogger<QueueWorker> logger, Versions versions, BGDL bgdl, CDN cdn, ConcurrentQueue<Summary> queue, IHostApplicationLifetime applicationLifetime, BNetClient bNetClient)
        {
            _logger = logger;
            _versions = versions;
            _bgdl = bgdl;
            _cdn = cdn;
            _queue = queue;
            _applicationLifetime = applicationLifetime;
            _bNetClient = bNetClient;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _cancellationToken = cancellationToken;

            _applicationLifetime.ApplicationStarted.Register(OnStarted);
            _applicationLifetime.ApplicationStopping.Register(OnStopping);
            _applicationLifetime.ApplicationStopped.Register(OnStopped); 
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private void OnStarted()
        {
            _running = true;

            _logger.LogInformation("OnStarted has been called.");

            _running = true;
            Task.Run(async () =>
            {
                while (_running)
                {
                   
                        if (_queue.Count == 0)
                        {
                            await Task.Delay(TimeSpan.FromSeconds(1));
                            continue;
                        }

                        if (!_queue.TryDequeue(out var item))
                        {
                            await Task.Delay(TimeSpan.FromSeconds(1));
                            continue;
                        }

                        try
                        {
                            _logger.LogInformation($"Updating {item.Product} to {item.Seqn}");
                            switch (item.Flags.ToLower())
                            {
                                case "cdn":
                                    var (cdn_value, cdn_seqn) = await _bNetClient.Do<List<BNetLib.Models.CDN>>(
                                        new CDNCommand(item.Product.ToLower()));
                                    await _cdn.Insert(new BTSharedCore.Models.CDN()
                                    {
                                        Seqn = cdn_seqn,
                                        Value = cdn_value,
                                        Product = item.Product,
                                    });
                                    break;
                                case "versions":
                                    var (version_value, version_seqn) =
                                        await _bNetClient.Do<List<BNetLib.Models.Version>>(
                                            new VersionCommand(item.Product.ToLower()));

                                    await _versions.Insert(new BTSharedCore.Models.Version()
                                    {
                                        Seqn = version_seqn,
                                        Value = version_value,
                                        Product = item.Product,
                                    });
                                    break;
                                case "bgdl":
                                    var (bgdl_value, bgdl_seqn) = await _bNetClient.Do<List<BNetLib.Models.Version>>(
                                        new BGDLCommand(item.Product.ToLower()));

                                    await _bgdl.Insert(new BTSharedCore.Models.BGDL()
                                    {
                                        Seqn = bgdl_seqn,
                                        Value = bgdl_value,
                                        Product = item.Product,
                                    });
                                    break;
                            }
                        }
                        catch (Exception)
                        {
                            _queue.Enqueue(item);
                            await Task.Delay(TimeSpan.FromSeconds(1));
                        }
                }
            }, _cancellationToken);
        }
        private void OnStopping()
        {
            _running = false;
        }

        private void OnStopped()
        {
           
        }
    }
}
