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

        // <EntityId, Entity>
        private Dictionary<int, Entity> AllEntities = new Dictionary<int, Entity>();

        // 记录每个场景对应的实体 <SpaceID, EntityList>
        private Dictionary<int, List<Entity>> SpaceEntities = new Dictionary<int, List<Entity>>();


        // 添加 Entity
        public void AddEntity(int spaceID, Entity entity)
        {
            lock(this)
            {
                // 统一管理的对象分配id
                entity.EntityData.Id = NewEntityId;
                AllEntities[entity.entityId] = entity;

                //  把对应的角色放进对应的 SpaceEntities 中
                if (!SpaceEntities.ContainsKey(spaceID))
                {
                    SpaceEntities[spaceID] = new List<Entity>();
                }
                SpaceEntities[spaceID].Add(entity);
            }
        }

        public void RemoveEntity(int spaceId, Entity entity)
        {
            lock(this) 
            {
                AllEntities.Remove(entity.entityId);
                SpaceEntities[spaceId].Remove(entity);
            }
        }

        public Entity GetEntity(int entityId)
        {
            return AllEntities.GetValueOrDefault(entityId, null);
        }

        public int NewEntityId
        {
            get 
            { 
                lock(this) 
                { 
                    return index++; 
                }
            }
        }

        public EntityManager() { }
    }
}
