using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameServer.Model;
using Serilog;
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

        // 判断这个实体是否存在
        public bool Exist(int entityId)
        {
            return AllEntities.ContainsKey(entityId);
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

        // 查找Entity对象
        public List<T> GetEntityList<T>(int spaceId, Predicate<T> match) where T : Entity
        {
            return SpaceEntities[spaceId]
                    .OfType<T>()                           // 筛选符合类型的 entity
                    .Where(entity=> match.Invoke(entity))  // 筛选符合match条件的 entity
                    .ToList();                             // 返回一个列表
        }

        // 查找一定范围内最接近的对象
        public T GetNearest<T>(int spaceId, Vector3Int center, int range) where T : Entity
        {
            // 创建 match条件
            Predicate<T> match = (e) =>
            {
                return Vector3Int.Distance(center * Monster.XZ1000, e.Position * Monster.XZ1000) <= range;
            };

            var entity = GetEntityList<T>(spaceId, match)
                        .OrderBy(e => Vector3Int.Distance(center * Monster.XZ1000, e.Position * Monster.XZ1000))
                        .FirstOrDefault();

            return entity;
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

        public void Update()
        {
            foreach(var entity in AllEntities) 
            {
                entity.Value.Update();
            }
        }
    }
}
