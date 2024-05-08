using Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using Proto.Message;
using System.Threading.Tasks;
using static Network.LengthFieldDecoder;
using Google.Protobuf;

namespace Summer.Network
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
            disconnectedCallback?.Invoke(this);

        }
              
        private void OnDataReceived(object? sender, byte[] buffer)
        {
            datareceivedCallback?.Invoke(this, buffer); // datareceivedCallback为空不执行
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

        #region 发送网络数据包的相关代码
        private package _package = null;
        public Request Request
        {
            get
            {
                if(_package == null)
                {
                    _package = new package();
                }
                if(_package.Request == null)
                {
                    _package.Request = new Request();
                }
                return _package.Request;
            }
        }

        public Response Response
        {
            get
            {
                if (_package == null)
                {
                    _package = new package();
                }
                if (_package.Response == null)
                {
                    _package.Response = new Response();
                }
                return _package.Response;
            }
        }

        public void Send()
        {
            if(_package != null)
                Send(_package);
            _package = null;
        }

        public void Send(package package)
        {
            byte[] data = null;
            using (MemoryStream ms = new MemoryStream())
            {
                package.WriteTo(ms);  // 把传来的对象写入内存流当中，转为字节数组

                // 编码
                data = new byte[4 + ms.Length];
                Buffer.BlockCopy(BitConverter.GetBytes(ms.Length), 0, data, 0, 4); // 把消息的长度填充到消息头当中
                Buffer.BlockCopy(ms.GetBuffer(), 0, data, 4, (int)ms.Length); //把消息内容加到消息头中
            }
            Send(data, 0, data.Length);  //  从0开始，发data的长度的数据
        }

        public void Send(byte[] data, int offset, int count)  // 异步发送消息
        {
            lock(this)  //加锁
            {
                if (socket.Connected)  // 如果socket已连接
                {
                    // 把消息放到发送缓冲区
                    socket.BeginSend(data, offset, count, SocketFlags.None, new AsyncCallback(SendCallback), socket);
                }
            }
        }

        private void SendCallback(IAsyncResult ar)
        {
            int len = socket.EndSend(ar);  //发送字节数
        }

        #endregion
    }
}
