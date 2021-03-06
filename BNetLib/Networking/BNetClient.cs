﻿using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
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
            /*
            TODO: Make this throw an error if the input is not a IEnumerable
            if (typeof(T) != typeof(List<>))
            {
                throw new Exception("Must be List");
            }
            */

            using var client = new TcpClient();
            await client.ConnectAsync(_serverUrl, 1119);

            await using var ms = client.GetStream();

            var payload = Encoding.UTF8.GetBytes($"{command}\r\n");
            await ms.WriteAsync(payload, 0, payload.Length);

            if (ms.CanRead)
            {
                using var reader = new StreamReader(ms, Encoding.UTF8);
                try
                {
                    var result = await reader.ReadToEndAsync();

                    /// From TactLib -> https://github.com/overtools/TACTLib/blob/7d2ecbc98b83a315ea599fd519403fa0d8b24dce/TACTLib/Protocol/Ribbit/RibbitClient.cs
                    var text = result.Split("\n");
                    var boundary = text.FirstOrDefault(x => x.Trim().StartsWith("Content-Type:"))?.Split(';').FirstOrDefault(x => x.Trim().StartsWith("boundary="))?.Split('"')[1].Trim();
                    var data = text.SkipWhile(x => x.Trim() != "--" + boundary).Skip(1).TakeWhile(x => x.Trim() != "--" + boundary).Skip(1);

                    return BNetTools<T>.Parse(data);
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
