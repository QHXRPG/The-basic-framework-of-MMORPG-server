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
            var Space = conn.Get<Character>()?.Space;
            if (Space == null) return;
            Space.UpdataEntity(msg.EntitySync);
        }
    }
}
