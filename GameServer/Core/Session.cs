using GameServer.Database;
using GameServer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Core
{
    public class Session
    {
        //当前登录的角色
        public Character Character;

        // 当前所在的地图
        public Space Space => Character.Space;

        public DbPlayer DbPlayer;
    }
}
