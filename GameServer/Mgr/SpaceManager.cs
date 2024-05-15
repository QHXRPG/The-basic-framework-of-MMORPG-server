using GameServer.Model;
using Serilog;
using Summer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Mgr
{
    // 场景管理器
    public class SpaceManager : Singleton<SpaceManager>
    {

        // 地图字典
        private Dictionary<int, Space> spaceDict = new Dictionary<int, Space>();

        public SpaceManager()
        {

        }

        public void Init()
        {
            foreach (var kv in DataManager.Instance.Spaces) 
            {
                spaceDict[kv.Key] = new Space(kv.Value);
                Log.Information("初始化地图：{0}", kv.Value.Name);
            }
        }

        public Space GetSpace(int spaceId) 
        {
            return spaceDict[spaceId];  
        }

    }
}
