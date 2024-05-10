using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.ObjectiveC;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    #region 简单模拟Serilog实现
/*    public class Log
    {
        public const int DEBUG = 0; //调试信息
        public const int INFO = 1;  //普通信息
        public const int WARN = 2;  //警告信息
        public const int ERROR = 3; //错误信息

        public static int Level = INFO;  // 日志级别

        public delegate void PrintCallback(string text);
        public static event PrintCallback Print;

        static Log()
        {
            Log.Print += (text) =>
            {
                Console.WriteLine(text);
            };
        }

        static string[] LevelName = { "DEBUG", "INFO", "WARN", "ERROR" };
        private static void WriteLine(int lev, string text, params object[] args)
        {
            if(Level <= lev)
            {
                text = String.Format(text, args);
                text = String.Format("[{0}]\t -- {1}", LevelName[lev], text);
                Print?.Invoke(text);
            }
        }

        public static void Debug(string text, params object?[]? args)
        {
            WriteLine(0, text, args);
        }

        public static void Info(string text, params object?[]? args)
        {
            WriteLine(1, text, args);
        }

        public static void Warn(string text, params object?[]? args)
        {
            WriteLine(2, text, args);
        }

        public static void Error(string text, params object?[]? args)
        {
            WriteLine(3, text, args);
        }
    }*/
    #endregion

}
