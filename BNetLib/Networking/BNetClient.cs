using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using BNetLib.Models;
using BNetLib.Networking.Commands;

namespace BNetLib.Networking
{
    public class BNetClient
    {
        private readonly string _serverUrl;

        public BNetClient(ServerRegion region = ServerRegion.US)
        {
            _serverUrl = $"{region.ToString().ToLower()}.version.battle.net";
        }

        public async Task<(T Value, int Seqn)> Do<T>(AbstractCommand command) => await Do<T>(command.ToString());

        public async Task<(T Value, int Seqn)> Do<T>(string command)
        {
            using var client = new TcpClient();
            await client.ConnectAsync(_serverUrl, 1119);

            await using var ms = client.GetStream();

            var payload = Encoding.UTF8.GetBytes($"{command}\r\n");
            await ms.WriteAsync(payload, 0, payload.Length);

            if (ms.CanRead)
            {
                using var reader = new StreamReader(ms, Encoding.UTF8);
                var output = Activator.CreateInstance<BNetTools<T>>();

                try
                {
                    var result = await reader.ReadToEndAsync();

                    var text = result.Split("\n");
                    var boundary = text.FirstOrDefault(x => x.Trim().StartsWith("Content-Type:"))?.Split(';').FirstOrDefault(x => x.Trim().StartsWith("boundary="))?.Split('"')[1].Trim();
                    var data = text.SkipWhile(x => x.Trim() != "--" + boundary).Skip(1).TakeWhile(x => x.Trim() != "--" + boundary).Skip(1);

                    return output.Parse(data);
                }
                finally
                {
                    client.Close();
                    ms.Close();
                }
            }

            client.Close();
            ms.Close();
            return default;
        }
    }
}
