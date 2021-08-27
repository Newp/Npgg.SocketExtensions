using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Npgg.SocketExtensions
{
    public static class SocketExtensions
    {
        public static async Task<byte[]> ReceiveAsync(this Socket socket, int length)
        {
            byte[] buffer = new byte[length];

            int rest = length;
            int receiveCount = 0;
            
            while (rest > 0)
            {
                var begin = socket.BeginReceive(buffer, receiveCount, rest, SocketFlags.None, null, null);
                int recv = await Task.Factory.FromAsync(begin,
                           socket.EndReceive);

                if (recv == 0) throw new Exception("recv 0");

                rest -= recv;
                receiveCount += recv;
            }
            return buffer;
        }
        public static async Task<int> SendAsync(this Socket socket, byte[] buffer)
        {
            int sendCount = await Task.Factory.FromAsync(
                       socket.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, null, socket),
                       socket.EndSend);

            if (sendCount != buffer.Length)
            {

            }
            return sendCount;
        }
    }
}
