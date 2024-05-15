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
        private Vector3Int position;  // 位置
        private Vector3Int direction; //方向
        private int spaceId; // 所在地图ID 
        private NEntity netObj;  // 网络对象

        public int SpaceId
        {
            get { return spaceId; }
            set { spaceId = value; }
        }

        public int entityId { get { return netObj.Id; } }

        // 设置 Entity 时， NEntity的数值也会跟着更新
        public Vector3Int Position { 
            get { return position; }
            set 
            { 
                position = value;
                netObj.Position = value;
            }
        }

        public Vector3Int Direction { 
            get { return direction;}
            set 
            { 
                direction = value;
                netObj.Direction = value;
            }
        }


        public Entity(Vector3Int pos, Vector3Int dir)
        {
            netObj = new NEntity();
            Position = pos;
            Direction = dir;
        }

        // 把网络数值覆盖到本地去
        public NEntity EntityData
        {
            get { return netObj; }
            set
            {
                netObj = value;
                position = netObj.Position;
                direction = netObj.Direction;
            }
        }
    }
}
