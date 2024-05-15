using Summer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Model
{
    internal class Monster : Actor
    {
        public Monster(Vector3Int position, Vector3Int direction) : base(position, direction)
        {
        }
    }
}
