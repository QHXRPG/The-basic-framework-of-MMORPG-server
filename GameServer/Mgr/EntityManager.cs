using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameServer.Model;
using Summer;

namespace GameServer.Mgr
{
    // Entity 管理器
    public class EntityManager :Singleton<EntityManager>
    {
        private int index = 1;
        private Dictionary<int, Entity> AllEntity = new Dictionary<int, Entity>();

        public Entity CreateEntity()
        {
            lock(this)
            {
                var entity = new Entity(index++, Vector3Int.zero, Vector3Int.zero);
                AllEntity[entity.Id] = entity;
                return entity;
            }
        }

        public EntityManager() { }
    }
}
