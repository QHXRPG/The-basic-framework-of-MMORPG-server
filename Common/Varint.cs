using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Summer
{
    public class Varint
    {

        public static byte[] VarintEncode(ulong value)
        {
            var list = new List<byte>();  //存储编码后的字节数据
            while (value > 0)      //直到 value 变为0，即所有位数都已经编码完成
            {
                byte b = (byte)(value & 0x7f);  // 取 value 的低7位作为一个字节，并将其转换为 byte 类型，存储在 b 中
                value >>= 7;      //将 value 右移7位，相当于去掉了已经编码的7位
                if (value > 0)   //value 大于0，表示还有剩余的位需要编码
                {
                    b |= 0x80;   //将当前字节的最高位设置为1
                }
                list.Add(b);   // 将编码后的字节 b 添加到列表 list 中
            }
            return list.ToArray();   //返回编码后的字节数组
        }

        //varint 解析
        public static ulong VarintDecode(byte[] buffer)
        {
            ulong value = 0;
            int shift = 0;
            int len = buffer.Length;
            for (int i = 0; i < len; i++)
            {
                byte b = buffer[i];
                value |= (ulong)(b & 0x7F) << shift;
                if ((b & 0x80) == 0)
                {
                    break;
                }
                shift += 7;
            }
            return value;
        }

        public static int VarintSize(ulong value)
        {
            //位置7位，如果前面都为0，说明只有一个有效字节
            if ((value & (0xFFFFFFFF << 7)) == 0)
            {
                return 1;
            }

            if ((value & (0xFFFFFFFF << 14)) == 0)
            {
                return 2;
            }

            if ((value & (0xFFFFFFFF << 21)) == 0)
            {
                return 3;
            }

            if ((value & (0xFFFFFFFF << 28)) == 0)
            {
                return 4;
            }
            return 5;
        }
    }
}
