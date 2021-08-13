using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Npgg.AsyncAwaitSocket
{
    public static class SocketExtensions
    {
        public static async Task<byte[]> ReceiveAsync(this Socket socket, int length)
        {
            byte [] buffer = new byte[length];
            int receiveCount = await Task.Factory.FromAsync(
                       socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, null, socket),
                       socket.EndReceive);

            if(receiveCount == 0)
            {
                throw new Exception("recv 0");
            }

            if(receiveCount != length)
            {

            }
            return buffer;
        }
    }
}
