using System.Collections;
using System.Collections.Generic;
using Google.Protobuf;
using System.IO;
using System;
using Proto.Message;
using Proto;
using System.Reflection;
using Google.Protobuf.Reflection;
using Serilog;

namespace Summer
{
    /// <summary>
    /// Protobuf序列化与反序列化
    /// 序列化：把一个对象转成一组序列
    /// 反序列化：把一个二维序列转成对象
    /// </summary>
    public class ProtoHelper
    {
        /// <summary>
        /// 序列化protobuf
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static byte[] Serialize(IMessage msg)
        {
            using (MemoryStream rawOutput = new MemoryStream())
            {
                msg.WriteTo(rawOutput);
                byte[] result = rawOutput.ToArray();
                return result;
            }
        }
        /// <summary>
        /// 解析
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataBytes"></param>
        /// <returns></returns>
        public static T Parse<T>(byte[] dataBytes) where T : IMessage, new()
        {
            T msg = new T();
            msg = (T)msg.Descriptor.Parser.ParseFrom(dataBytes);
            return msg;
        }

        // 消息类型列表
        private static Dictionary<string, Type> _registry = new Dictionary<string, Type>();

        static ProtoHelper()
        {
            // 用反射原理查询出当前程序集中所有的类型
            var q = from t in Assembly.GetExecutingAssembly().GetTypes() select t;
            q.ToList().ForEach(t =>
            {
                //找出属于IMessage的派生类
                if (typeof(Google.Protobuf.IMessage).IsAssignableFrom(t))
                {
                    //从类型t中获取名为Descriptor的属性的值。
                    var desc = t.GetProperty("Descriptor").GetValue(t) as MessageDescriptor;

                    //Descriptor是一个接口，用于描述Protobuf消息类型的元数据信息
                    _registry.Add(desc.FullName, t); 
                    Log.Debug("类型注册：{0}", desc.FullName);
                }
            });
        }

        // 打包
        public static Proto.Package Pack(Google.Protobuf.IMessage message)
        {
            Proto.Package package = new Proto.Package();
            package.Fullname = message.Descriptor.FullName;  // 取出message的名字
            package.Data = message.ToByteString();    // 取出message的二进制字符串
            return package;
        }

        //解包
        public static IMessage Unpack(Proto.Package package)
        {
            // 从传入的Proto.Package对象中获取Fullname属性值，即消息类型的全名
            string fullName = package.Fullname;

            //检查 _registry 字典中是否包含该消息类型的全名
            if (_registry.ContainsKey(fullName))
            {
                // 从 _registry 字典中获取对应的消息类型 Type t
                Type t = _registry[fullName];

                //从类型t中获取名为Descriptor的属性的值。
                var desc = t.GetProperty("Descriptor").GetValue(t) as MessageDescriptor;
                return desc.Parser.ParseFrom(package.Data);
            }
            return null;
        }
    }
}
