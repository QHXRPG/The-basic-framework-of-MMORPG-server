﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Database
{
    public class Db
    {

        /// <summary>
        /// 可配置的数据库参数
        /// </summary>
        static string host = "127.0.0.1";
        static int port = 3306;
        static string user = "root";
        static string passwd = "Qhx990615";
        static string dbname = "qhxgame";



        static string connectionString =
            $"Data Source={host};Port={port};User ID={user};Password={passwd};" +
            $"Initial Catalog={dbname};Charset=utf8;SslMode=none;Max pool size=10";

        public static IFreeSql fsql = new FreeSql.FreeSqlBuilder()
            .UseConnectionString(FreeSql.DataType.MySql, connectionString)
            .UseAutoSyncStructure(true) //自动同步实体结构到数据库
            .Build(); //定义成单例模式



    }
}
