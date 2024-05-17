using GameServer.Database;
using Proto.Message;
using Summer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Model
{
    public class Monster : Actor
    {
        public Monster(int tid, int level, Vector3Int pos, Vector3Int dir)
            : base(EntityType.Monster, tid, level, pos, dir)
        {
        }
    }
}
