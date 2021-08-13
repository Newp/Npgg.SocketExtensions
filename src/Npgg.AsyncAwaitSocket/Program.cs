using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using BinaryEncoding;
using Npgg.SocketExtensions;

namespace Npgg.AsyncAwaitSocket
{


    class Program
    {
        static readonly int Port = 8088;
        static async Task Main(string[] args)
        {
            var listener = Listen();

            await Task.Delay(1000);
            for(int i =0;i<100;i++)
            {
                _ = StartClient(i);
            }

            await listener;
        }

        static async Task StartClient(int id)
        {
            TcpClient tcpClient = new TcpClient();
            tcpClient.Connect(IPAddress.Parse("127.0.0.1"), Port);

            var socket = tcpClient.Client;

            for (int i = 0; i < 1000000; i++)
            {
                var payload = Encoding.UTF8.GetBytes($"CLIENT {id} MESSAGE STEP >> {i}");
                var length = payload.Length;

                var message = Binary.LittleEndian.GetBytes((UInt16)length)
                    .Concat(payload)
                    .ToArray();

                await socket.SendAsync(message);

                await Task.Delay(100);
            }
        }

        static async Task Listen()
        {
            var listener = new TcpListener(new IPEndPoint(IPAddress.Any, Port));
            listener.Start();

            ServerLog("listener started");

            while (true)
            {
                var client = await listener.AcceptTcpClientAsync().ConfigureAwait(false);
                _ = Handle(client.Client);
                ServerLog("connected");
            }
        }

        static async Task Handle(Socket socket)
        {
            while (true)
            {
                var header = await socket.ReceiveAsync(2);

                var payloadLength = Binary.LittleEndian.GetInt16(header);

                var payload = await socket.ReceiveAsync(payloadLength);

                var message = Encoding.UTF8.GetString(payload).Trim();

                ServerLog(message);
            }
        }

        static void ServerLog(string message)
        {
            Console.WriteLine($"[{DateTime.Now:HHmmss}] Server : {message}");
        }
        static void ClientLog(string message)
        {
            Console.WriteLine($"[{DateTime.Now:HHmmss}] Client : {message}");
        }

    }
}
