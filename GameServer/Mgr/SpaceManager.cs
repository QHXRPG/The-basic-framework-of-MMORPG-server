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

        // 获取场景，如果不存在，就返回空
        public Space GetSpace(int spaceId) 
        {
            return spaceDict.GetValueOrDefault(spaceId, null);
        }

        public void Update()
        {
            foreach(var space in spaceDict.Values) 
            {
                space.Update();
            }
        }

    }
}
