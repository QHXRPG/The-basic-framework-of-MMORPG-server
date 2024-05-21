using Summer.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Proto;
using Proto.Message;
using Summer;
using GameServer.Model;
using System.Reflection;
using GameServer.Mgr;
using GameServer.Core;

namespace GameServer.Service
{
    // 地图同步服务
    public class SpaceService : Singleton<SpaceService>  // 设置为单例
    {

        public void Start()
        {
            // 初始化地图
            SpaceManager.Instance.Init();

            // 订阅 位置同步请求 的消息, 有角色移动便会触发
            MessageRouter.Instance.Subscribe<SpaceEntitySyncRequest>(_SpaceEntitySyncRequest);


        }

        public Space GetSpace(int spaceId)
        { return SpaceManager.Instance.GetSpace(spaceId); }

        // 场景实体的同步请求
        private void _SpaceEntitySyncRequest(Connection conn, SpaceEntitySyncRequest msg)
        {
            // 通过conn拿到角色所在的地图
            var Space = conn.Get<Session>().Character?.Space;
            if (Space == null) return;

            NEntity netEntity = msg.EntitySync.Entity;
            Entity serEntity = EntityManager.Instance.GetEntity(netEntity.Id);
            float dist = Vector3Int.Distance(netEntity.Position, serEntity.Position);
            
            // 使用服务器速度
            netEntity.Speed = serEntity.Speed;

            // 计算时间差
            float dt = Math.Min(serEntity.PositionTime, 1.0f);

            // 计算限额
            float limit = serEntity.Speed * dt * 2 *1.5f;
            if (float.IsNaN(dist) || dist> limit)
            { 
                // 把角色拉回原位
                SpaceEntitySyncResponse resp = new SpaceEntitySyncResponse();
                resp.EntitySync = new NEntitySync();
                resp.EntitySync.Entity = serEntity.EntityData;  // 采用服务器这边的数据
                resp.EntitySync.Force = true;
                conn.Send(resp);
                return;
            }

            // 广播同步信息
            Space.UpdateEntity(msg.EntitySync);
        }
    }
}
