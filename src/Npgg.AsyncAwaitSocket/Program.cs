using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using BinaryEncoding;

namespace Npgg.AsyncAwaitSocket
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var listener = new TcpListener(new IPEndPoint(IPAddress.Any, 8088));
            listener.Start();

            Log("listener started");
            
            while (true)
            {
                var client = await listener.AcceptTcpClientAsync().ConfigureAwait(false);
                _ = Handle(client.Client);
                Log("connected");
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

                Log(message);
            }
        }

        static void Log(string message)
        {
            Console.WriteLine($"[{DateTime.Now:HHmmss}] : {message}");
        }

    }
}
