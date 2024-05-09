using System.Collections;
using System.Collections.Generic;
using Google.Protobuf;
using System.IO;
using System;

namespace Summer
{
    /// <summary>
    /// Protobuf序列化与反序列化
    /// 序列化：把一个对象转成一组序列
    /// 反序列化：把一个二维序列转成对象
    /// </summary>
    public class ProtobufTool
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
    }
}
