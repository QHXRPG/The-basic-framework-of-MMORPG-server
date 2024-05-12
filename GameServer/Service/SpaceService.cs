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

namespace GameServer.Service
{
    // 地图同步服务
    public class SpaceService : Singleton<SpaceService>  // 设置为单例
    {
        // 地图字典
        private Dictionary<int, Space> spaceDict = new Dictionary<int, Space>();    
        public void Start()
        {
            // 订阅 位置同步请求 的消息
            MessageRouter.Instance.Subscribe<SpaceEntitySyncRequest>(_SpaceEntitySyncRequest);

            //创建空间对象:新手村场景
            Space space = new Space();
            space.Name = "新手村";
            space.Id = 6; // 新手村id
            spaceDict[space.Id] = space;
        }

        public Space GetSpace(int spaceId)
        { return spaceDict[spaceId]; }


        private void _SpaceEntitySyncRequest(Connection conn, SpaceEntitySyncRequest msg)
        {
            // 通过conn拿到角色所在的地图
            var sp = conn.Get<Space>();
            if (sp == null) return;
            sp.UpdataEntity(msg.EntitySync);

        }
    }
}
