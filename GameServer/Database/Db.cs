using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Database
{
    public class Db
    {
        static string connectionString = "Data Source=127.0.0.1;Port=3306;User ID=root;Password=Qhx990615;" +
                                         "Initial Catalog=QhxGame;Charset=utf8;SslMode=none;Max pool size=10";

        public static IFreeSql fsql = new FreeSql.FreeSqlBuilder()
            .UseConnectionString(FreeSql.DataType.MySql, connectionString)
            .UseAutoSyncStructure(true) // 自动同步实体结构到数据库
            .Build();                   // 定义成 Singleton 单例模式
    }
}
