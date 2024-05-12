using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Proto.Message;
using Summer;

namespace GameServer.Model
{
    // 在MMO世界当中同步的实体
    public class Entity
    {
        private int _entityId;
        private Vector3Int position;  // 位置
        private Vector3Int direction; //方向
        private int spaceId; // 所在地图ID

        public int SpaceId
        {
            get { return spaceId; }
            set { spaceId = value; }    
        }

        public int entityId { get { return _entityId; } }

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
            this._entityId = id;
        }

        public Entity(int id, Vector3Int position, Vector3Int direction)
        {
            this.position = position;
            this.direction = direction; 
            this._entityId = id;
        }

        // 返回一个用于网络传输的 NEntity 对象
        public NEntity GetData() 
        {
            var data = new NEntity();
            data.Id = this._entityId;
            data.Position = new NVector3() { X = position.x, Y = position.y, Z = position.z };
            data.Direction = new NVector3() { X = direction.x, Y = -direction.y, Z = -direction.z };
            return data;
        }

        // 把网络数值覆盖到本地去
        public void SetEntityData(NEntity entity)
        {
            position.x = entity.Position.X;
            position.y = entity.Position.Y;
            position.z = entity.Position.Z;
            direction.x = entity.Direction.X;
            direction.y = entity.Direction.Y;
            direction.z = entity.Direction.Z;
        }
    }
}
