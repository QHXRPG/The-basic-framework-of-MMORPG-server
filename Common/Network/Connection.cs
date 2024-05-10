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
using System.IO;

namespace Summer.Network
{
    // 网络连接类，每一个Connection代表一个客户端
    // 功能：发送、接收网络消息，关闭连接，断开通知
    public class Connection
    {
        private Socket _socket;
        public Socket Socket
        {
            get
            {
                return _socket;
            }
        }

        public delegate void DataReceivedCallback(Connection sender, byte[] data);
        public delegate void DisConnectedCallback(Connection sender);

        public DataReceivedCallback OnDataReceived; // 接收到数据
        public DisConnectedCallback OnDisconnected;  // 连接断开

        public Connection(Socket socket)
        {
            this._socket = socket;

            // 创建一个解码器  
            // 参数： socket， 缓冲区大小， 长度字段的位置下标、长度字段本身长度、长度字节和内容中间隔了几个字节、舍弃前面几个字节
            var lfd = new LengthFieldDecoder(socket, 64 * 1024, 0, 4, 0, 4);
            lfd.DataReceived += _received;  //接收到完整的消息时触发DataReceived事件
            lfd.Disconnected += () => OnDisconnected?.Invoke(this);  //lfd的Disconnected事件触发时被调用的
            lfd.Start();  // 启动解码器
        }

        private void _received(byte[] data)
        {
            OnDataReceived?.Invoke(this, data);
        }

        public void Close() // 主动关闭连接
        {
            try
            {
                _socket.Shutdown(SocketShutdown.Both);
            }
            catch { }
            _socket.Close();
            _socket = null;
            OnDisconnected?.Invoke(this);
        }

        #region 发送网络数据包的相关代码

        public void Send(Google.Protobuf.IMessage message)
        {
            byte[] data = null;
            using (MemoryStream ms = new MemoryStream())
            {
                message.WriteTo(ms);  // 把传来的对象写入内存流当中，转为字节数组
                int len = (int)ms.Length; // 数据本身长度

                // 编码
                data = new byte[4 + len];
                byte[] lenbytes = BitConverter.GetBytes(len);
                if (BitConverter.IsLittleEndian) //如果当前是小端平台，翻转成大端
                    Array.Reverse(lenbytes);

                // 数据拼接
                Buffer.BlockCopy(lenbytes, 0, data, 0, 4); // 把消息的长度填充到消息头当中
                Buffer.BlockCopy(ms.GetBuffer(), 0, data, 4, len); //把消息内容加到消息头中
            }
            Send(data, 0, data.Length);  //  从0开始，发data的长度的数据
        }

        public void Send(byte[] data, int offset, int count)  // 异步发送消息
        {
            lock(this)  //加锁
            {
                if (_socket.Connected)  // 如果socket已连接
                {
                    // 把消息放到发送缓冲区
                    _socket.BeginSend(data, offset, count, SocketFlags.None, new AsyncCallback(SendCallback), _socket);
                }
            }
        }

        private void SendCallback(IAsyncResult ar)
        {
            int len = _socket.EndSend(ar);  //发送字节数
        }

        #endregion
    }
}
