using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Proto.Message;
using Summer.Network;
using Serilog;
using Summer;
using GameServer.Model;
using GameServer.Mgr;
using GameServer.Service;

namespace Common.Network.Server
{
    // 玩家服务： 注册登录，创建角色，进入游戏
    internal class UserService : Singleton<UserService>
    {


        public void Start()
        {
            // 订阅进入游戏的消息
            MessageRouter.Instance.Subscribe<GameEnterRequest>(_GameEnterRequest);

        }

        private void _GameEnterRequest(Connection conn, GameEnterRequest msg)
        {
            Log.Information("有玩家进入游戏");
            int entityId = EntityManager.Instance.NewEntityId;  // 获得一个独有的实体Id
            Random random = new Random();
            Vector3Int pos = new Vector3Int(500+random.Next(-5, 5), 0, 500 + random.Next(-5, 5));  // 玩家坐标
            Vector3Int r_pos = new Vector3Int(0, 0, 0);
            Character character = new Character(entityId, pos, r_pos); // 分配id、坐标、方向
            
            //通知玩家登录成功
            GameEnterResponse response = new GameEnterResponse();
            response.Success = true;
            response.Entity = character.GetData();
            conn.Send(response);

            //将新角色加入到地图
            var space = SpaceService.Instance.GetSpace(6);  
            space.CharacterJoin(conn, character); //地图广播
        }
    }
}
