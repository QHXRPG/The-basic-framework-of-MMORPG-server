using Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using Proto.Message;
using System.Threading.Tasks;
using static Network.LengthFieldDecoder;

namespace GameServer.Network
{
    // 网络连接类，每一个NetConnection代表一个客户端
    // 功能：发送、接收网络消息，关闭连接，断开通知
    public class NetConnection
    {
        public Socket socket;
        public delegate void DataReceivedCallback(NetConnection sender, byte[] data);
        public delegate void DisConnectedCallback(NetConnection sender);

        private DataReceivedCallback datareceivedCallback;
        private DisConnectedCallback disconnectedCallback;

        public NetConnection(Socket socket, DataReceivedCallback cb1, DisConnectedCallback cb2)
        {
            this.socket = socket;
            this.datareceivedCallback = cb1;
            this.disconnectedCallback = cb2;

            // 创建一个解码器  
            // 参数： socket， 缓冲区大小， 长度字段的位置下标、长度字段本身长度、长度字节和内容中间隔了几个字节、舍弃前面几个字节
            var lfd = new LengthFieldDecoder(socket, 64 * 1024, 0, 4, 0, 4);
            lfd.DataReceived += OnDataReceived;
            lfd.Disconnected += OnDisconnected;
            lfd.Start();  // 启动解码器
        }

        private void OnDisconnected(Socket soc)
        {
            disconnectedCallback(this);
        }
              
        private void OnDataReceived(object? sender, byte[] buffer)
        {
            datareceivedCallback(this, buffer);
        }

        public void Close() // 主动关闭连接
        {
            try
            {
                socket.Shutdown(SocketShutdown.Both);
            }
            catch { }
            socket.Close();
            socket = null;
            disconnectedCallback(this);
        }
    }
}
