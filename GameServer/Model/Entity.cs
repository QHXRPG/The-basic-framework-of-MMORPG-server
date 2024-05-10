using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Summer;

namespace GameServer.Model
{
    // 在MMO世界当中同步的实体
    public class Entity
    {
        private int id;
        private Vector3Int position;  // 位置
        private Vector3Int direction; //方向
        public int Id { get { return id; } }

        public Vector3Int Position { 
            get { return position; }
            set { position = value; }
        }

        public Vector3Int Direction { 
            get { return direction;}
            set { direction = value; }
        }

        public Entity(int id)
        {
            this.id = id;
        }

        public Entity(int id, Vector3Int position, Vector3Int direction)
        {
            this.position = position;
            this.direction = direction; 
            this.id = id;
        }
    }
}
