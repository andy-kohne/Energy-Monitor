using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Brultech.ECM1240
{
    public class TcpReceiver
    {
        public EventHandler<PacketReceivedArgs> PacketReceived;

        private TcpListener _listener;
        private TcpClient _tcpClient;
        private readonly ByteBuffer _processing = new ByteBuffer(1024);
        private readonly byte[] _packetBytes = new byte[PacketLength];
        private readonly byte[] _receiveBuffer = new byte[1024];
        private const int PacketLength = 65;

        private readonly List<DateTime> _badPackets = new List<DateTime>();

        public async Task ListenAsync(string[] allowedRemotes, int port, CancellationToken cancellationToken)
        {
            Helpers.RetryOnException(5, TimeSpan.FromSeconds(20), () =>
            {
                _listener = new TcpListener(IPAddress.Any, port);
                _listener.Start();
            });

            while (!cancellationToken.IsCancellationRequested)
            {
                _processing.Clear();
                _tcpClient = await GetConnectionAsync(allowedRemotes, cancellationToken);
                var stream = _tcpClient.GetStream();

                try
                {
                    while (!cancellationToken.IsCancellationRequested)
                    {
                        var newBytes = await stream.ReadAsync(_receiveBuffer, 0, _receiveBuffer.Length, cancellationToken)
                            .WithTimeout(TimeSpan.FromSeconds(30));
                        Console.WriteLine($"{newBytes} bytes");
                        _processing.Enqueue(_receiveBuffer, 0, newBytes);
                        ProcessBuffer();
                    }
                }
                catch (PacketException)
                {
                    _tcpClient.Dispose();
                    _badPackets.Clear();
                }
                catch (TimeoutException)
                {
                    _tcpClient.Dispose();
                    continue;
                }
            }
        }

        private void ProcessBuffer()
        {
            var pos = 0;
            while (_processing.Size - pos >= PacketLength)
            {
                if (!IsPacket(pos))
                {
                    pos++;
                    continue;
                }
                if (pos > 0)
                {
                    Console.WriteLine($"discarding {pos} bytes before");
                    _processing.Discard(pos);
                    pos = 0;
                }

                var head = _processing.Head;
                _processing.Dequeue(_packetBytes, PacketLength);
                Console.WriteLine("PACKET");

                try
                {
                    var packet = new Packet(_packetBytes);
                    PacketReceived?.Invoke(this, new PacketReceivedArgs(packet));
                }
                catch (PacketException)
                {
                    Console.WriteLine($"BAD CHECKSUM - from head {head}");
                    for (var i = _badPackets.Count - 1; i >= 0; i--)
                    {
                        if (_badPackets[i] < DateTime.UtcNow.AddMinutes(-2))
                            _badPackets.RemoveAt(i);
                    }
                    _badPackets.Add(DateTime.UtcNow);
                    if (_badPackets.Count > 5)
                        throw;
                }
            }
        }

        private bool IsPacket(int offset)
        {
            return _processing[offset] == 0xFE &&
                   _processing[offset + 1] == 0xFF &&
                   _processing[offset + 2] == 0x03 &&
                   _processing[offset + 62] == 0xFF &&
                   _processing[offset + 63] == 0xFE;
        }

        public void StopListening()
        {
            _tcpClient?.Dispose();
            _listener.Stop();
        }

        private async Task<TcpClient> GetConnectionAsync(string[] allowedRemotes, CancellationToken cancellationToken)
        {
            Console.WriteLine("getting connection");
            while (!cancellationToken.IsCancellationRequested)
            {
                var client = await _listener.AcceptTcpClientAsync();
                Console.WriteLine("checking connection");

                if (ConnectionAuthorized(allowedRemotes, client))
                {
                    Console.WriteLine($"got client {client.Client.RemoteEndPoint}");
                    return client;
                }
                Console.WriteLine($"{client.Client.RemoteEndPoint} not authorized");
                client.Dispose();
            }
            return null;
        }

        private bool ConnectionAuthorized(string[] allowedRemotes, TcpClient client)
        {
            if (allowedRemotes == null || !allowedRemotes.Any())
                return true;

            var address = ((IPEndPoint)client.Client.RemoteEndPoint).Address;

            foreach (var item in allowedRemotes)
            {
                var u = new UriBuilder { Host = item };
                switch (u.Uri.HostNameType)
                {
                    case UriHostNameType.Dns:
                        try
                        {
                            var he = Dns.GetHostAddresses(u.Uri.DnsSafeHost);
                            foreach (var h in he)
                            {
                                if (address.Equals(h))
                                    return true;
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                            // ignored
                        }
                        break;

                    case UriHostNameType.IPv4:
                        if (address.Equals(IPAddress.Parse(u.Uri.Host)))
                            return true;
                        break;

                    default:
                        throw new ArgumentException();
                }

            }
            return false;
        }
    }
}
