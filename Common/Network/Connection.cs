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
using Serilog;
using Google.Protobuf.Reflection;

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

        public delegate void DataReceivedCallback(Connection sender, Google.Protobuf.IMessage data);
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
            // 解包。把package -》msg
            using(var buf = ByteBuffer.Allocate(data, true))
            {
                //获取消息序列号
                ushort code = buf.ReadUshort();
                Type t = ProtoHelper.SeqType(code);

                var desc = t.GetProperty("Descriptor").GetValue(t) as MessageDescriptor;
                var msg = desc.Parser.ParseFrom(data, 2, data.Length - 2);
                Log.Information("解析消息：code={0} - {1}", code, msg);
                OnDataReceived?.Invoke(this, msg);
            }
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
        // 4(报文长度) + 2（消息类型） + n（消息数据）
        public void Send(Google.Protobuf.IMessage message)
        {
            using(ByteBuffer buf = ByteBuffer.Allocate(1024, true))
            {
                int code = ProtoHelper.SeqCode(message.GetType());  // 通过类型获取序号
                buf.WriteUshort((ushort)code);
                buf.WriteBytes(message.ToByteArray());
                this.Send(buf.ToArray());
            }
        }

        public void Send(byte[] data)  // 异步发送消息
        {
            this.Send(data, 0, data.Length);
        }

        private void Send(byte[] data, int offset, int len)
        {
            lock(this)
            {
                if(_socket.Connected)
                {
                    Log.Debug("发送消息：len={0}", data.Length);
                    byte[] buffer = new byte[4 + len];
                    byte[] lenBytes = BitConverter.GetBytes(len);

                    if (BitConverter.IsLittleEndian) //如果当前是小端平台，翻转成大端
                        Array.Reverse(lenBytes);

                    // 数据拼接
                    Buffer.BlockCopy(lenBytes, 0, data, 0, 4); // 把消息的长度填充到消息头当中
                    Buffer.BlockCopy(buffer, offset, data, 4, len); //把消息内容加到消息头中
                    _socket.BeginSend(buffer, 0, buffer.Length, SocketFlags.None,
                                        new AsyncCallback(SendCallback), _socket);
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
