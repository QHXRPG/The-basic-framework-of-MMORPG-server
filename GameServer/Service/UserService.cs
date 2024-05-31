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
using GameServer.Database;
using System.Runtime.Intrinsics.Arm;
using GameServer.Core;

namespace Common.Network.Server
{
    // 玩家服务： 注册登录，创建角色，进入游戏
    internal class UserService : Singleton<UserService>
    {
        public void Start()
        {
            // 订阅进入游戏的消息
            MessageRouter.Instance.Subscribe<GameEnterRequest>(_GameEnterRequest);

            // 用户登录请求
            MessageRouter.Instance.Subscribe<UserLoginRequest>(_UserLoginRequest);

            // 注册请求
            MessageRouter.Instance.Subscribe<UserRegisterRequest>(_UserRegisterRequest);

            // 创建角色请求
            MessageRouter.Instance.Subscribe<CharacterCreateRequest>(_CharacterCreateRequest);

            // 查询角色列表的请求
            MessageRouter.Instance.Subscribe<CharacterListRequest>(_CharacterListRequest);

            // 删除角色的请求
            MessageRouter.Instance.Subscribe<CharacterDeleteRequest>(_CharacterDeleteRequest);
        }

        // 用户注册请求
        private void _UserRegisterRequest(Connection conn, UserRegisterRequest msg)
        {
            UserRegisterReponse userRegisterReponse = new UserRegisterReponse();

            // 查询数据库中是否已存在相同的用户名
            var existingPlayer = Db.fsql.Select<DbPlayer>().Where(p => p.Username == msg.Username).First();

            if (existingPlayer == null)
            {
                // 不存在相同的用户名，执行插入操作
                var DbNewPlayer = new DbPlayer()
                {
                    Username = msg.Username,
                    Password = msg.Password,
                };
                Db.fsql.Insert(DbNewPlayer).ExecuteAffrows();
                userRegisterReponse.Code = 1;
                userRegisterReponse.Message = "注册成功";
            }
            else
            {
                userRegisterReponse.Code = 0;
                userRegisterReponse.Message = "用户名已存在，请使用其他用户名";
            }
            conn.Send(userRegisterReponse);
        }

        // 删除角色的请求
        private void _CharacterDeleteRequest(Connection conn, CharacterDeleteRequest msg)
        {
            Log.Information($"删除角色:{msg.CharacterId}");
            var player = conn.Get<Session>().DbPlayer; // 通过连接在数据库中拿到玩家的信息
            Db.fsql.Delete<DbCharacter>()
                .Where(t => t.Id == msg.CharacterId)
                .Where(t => t.PlayerId == player.Id)
                .ExecuteAffrows();

            // 给客户端响应
            CharacterDeleteResponse response = new CharacterDeleteResponse();
            response.Success = true;
            response.Message = "执行完成";
            conn.Send(response);
        }

        // 查询角色列表的请求
        private void _CharacterListRequest(Connection conn, CharacterListRequest msg)
        {
            try
            {
                var player = conn.Get<Session>().DbPlayer; // 通过连接在数据库中拿到玩家的信息

                // 从数据库中查询到玩家的全部角色
                var list = Db.fsql.Select<DbCharacter>().Where(t => t.PlayerId == player.Id).ToList();
                CharacterListResponse characterListResponse = new CharacterListResponse();
                foreach (var item in list)
                {
                    // Entity 以及 EntityId 需要 开始游戏 后才给值
                    characterListResponse.CharacterList.Add(new NetActor()
                    {
                        Id = item.Id,
                        Tid = item.JobId,
                        Name = item.Name,
                        Level = item.Level,
                        Exp = item.Exp,
                        SpaceId = item.SpaceId,
                        Gold = item.Gold,
                    });
                }
                conn.Send(characterListResponse);
            }
            catch { }
        }

        private void _CharacterCreateRequest(Connection conn, CharacterCreateRequest msg)
        {
            Log.Information("创建角色请求:{0}", msg);
            CharacterCreateResponse response = new CharacterCreateResponse();
            var player = conn.Get<Session>().DbPlayer;  // 把player从数据库中取出来
            if(player == null)
            {
                Log.Information("未登录不能创建角色");
                response.Success = false;
                response.Message = "未登录不能创建角色";
                conn.Send(response);    
                return;
            }
            // 获取当前玩家拥有角色的数量
            long Count = Db.fsql.Select<DbCharacter>().Where(t => t.PlayerId.Equals(player.Id)).Count();
            if(Count >= 4)
            {
                // 角色数量最多四个
                Log.Information("角色数量最多四个");
                response.Success = false;
                response.Message = "角色数量最多四个";
                conn.Send(response);
                return;
            }
            if(string.IsNullOrWhiteSpace(msg.Name))
            {
                Log.Information("创建角色失败：角色名不为空");
                response.Success = false;
                response.Message = "创建角色失败：角色名不为空";
                conn.Send(response);
                return;
            }
            string name = msg.Name.Trim();  // 把首位的空格去掉
            if(name.Length > 7)
            {
                Log.Information("创建角色失败：角色名最为7");
                response.Success = false;
                response.Message = "创建角色失败：角色名最为7";
                conn.Send(response);
                return;
            }
            if(Db.fsql.Select<DbCharacter>().Where(t => t.Name.Equals(name)).Count() > 0)
            {
                Log.Information("创建角色失败：角色名已被占用");
                response.Success = false;
                response.Message = "创建角色失败：角色名已被占用";
                conn.Send(response);
                return;
            }

            DbCharacter dc = new DbCharacter()
            {
                Name = msg.Name,
                JobId = msg.JobType,
                Hp = 100,
                Mp = 100,
                Level = 1,
                Exp = 0,
                SpaceId = 1,
                Gold = 0,
                PlayerId = player.Id,
            };
            int aff = Db.fsql.Insert(dc).ExecuteAffrows();
            if(aff > 0) // 已成功入库
            {
                Log.Information("创建角色成功");
                response.Success = true;
                response.Message = "创建角色成功";
                conn.Send(response);
            }
        }

        private void _UserLoginRequest(Connection conn, UserLoginRequest msg)
        {
            var dbPlayer = Db.fsql.Select<DbPlayer>()
                .Where(p => p.Username == msg.Username)
                .Where(p => p.Password == msg.Password)
                .First();

            UserLoginReponse response = new UserLoginReponse();
            if (dbPlayer != null) 
            {
                response.Success = true;
                response.Message = "登录成功";

                // 登录成功后，在conn连接里记录玩家信息
                conn.Get<Session>().DbPlayer = dbPlayer;
            }
            else
            {
                response.Success = false;
                response.Message = "用户名或密码不正确";
            }
            conn.Send(response);
        }

        private void _GameEnterRequest(Connection conn, GameEnterRequest msg)
        {
            Log.Information($"有玩家进入游戏,ID:{msg.CharacterId}");
            // 申请一个EntityId
            int entityId = EntityManager.Instance.NewEntityId; 

            // 获取玩家
            var player = conn.Get<Session>().DbPlayer;

            // 查询数据库的角色
            var DbCharacter = Db.fsql.Select<DbCharacter>()
                        .Where(t => t.PlayerId == player.Id)
                        .Where(t => t.Id == msg.CharacterId)
                        .First();

            // 把数据库角色 转换为 游戏对象
            Character character = CharacterManager.Instance.CreateCharacter(DbCharacter);

            //通知玩家登录成功
            GameEnterResponse response = new GameEnterResponse();
            response.Success = true;
            response.Entity = character.EntityData;
            response.Charater = character.Info;  // 把创建好的角色传给客户端
            conn.Send(response);

            //将新角色加入到地图
            var space = SpaceService.Instance.GetSpace(DbCharacter.SpaceId);  
            space.CharacterJoin(conn, character); //地图广播
        }


    }
}
