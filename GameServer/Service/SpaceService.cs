using Summer.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Proto;
using Proto.Message;

namespace GameServer.Service
{
    // 地图同步服务
    public class SpaceService
    {
        public void Start()
        {
            // 订阅 位置同步请求 的消息
            MessageRouter.Instance.Subscribe<SpaceEntitySyncRequest>(_SpaceEntitySyncRequest);
        }

        private void _SpaceEntitySyncRequest(Connection sender, SpaceEntitySyncRequest msg)
        {

        }
    }
}
