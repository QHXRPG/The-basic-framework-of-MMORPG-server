using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.ObjectiveC;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class Log
    {
        public const int DEBUG = 0; //调试信息
        public const int INFO = 1;  //普通信息
        public const int WARN = 2;  //警告信息
        public const int ERROR = 3; //错误信息

        public static int Level = INFO;  // 日志级别

        public delegate void PrintCallback(string text, params object?[]? args);
        public static event PrintCallback Print;

        public static void Debug(string text, params object?[]? args)
        {
            Print?.Invoke(text, args);
        }

        public static void Info(string text, params object?[]? args)
        {
            if(Level <= INFO)
            {
                Print?.Invoke(text, args);
            }
        }

        public static void Warn(string text, params object?[]? args)
        {
            if (Level <= WARN)
            {
                Print?.Invoke(text, args);
            }
        }

        public static void Error(string text, params object?[]? args)
        {
            if (Level <= ERROR)
            {
                Print?.Invoke(text, args);
            }
        }

    }
}
