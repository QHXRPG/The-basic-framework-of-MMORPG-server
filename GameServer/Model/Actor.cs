using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Summer;
using GameServer.Mgr;
using Proto.Message;

namespace GameServer.Model
{
    public class Actor : Entity
    {
        public int Id;

        public string Name { get; set; }

        public NCharacter Info { get; set; } = new NCharacter();

        public Actor(int id, Vector3Int position, Vector3Int direction)
            : base(id, position, direction)
        {
            
        }
    }
}
