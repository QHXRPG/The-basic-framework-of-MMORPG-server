using Summer.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Summer;
using Serilog;

namespace GameServer.Model
{
    // 空间、地图、场景
    public class Space
    {
        public int Id { get; set; }
        public string Name { get; set; }

        // 角色进入场景
        public Dictionary<int, Character> CharacterDict = new Dictionary<int, Character>();

        public void CharacterJoin(Connection conn, Character character)
        {
            Log.Information("角色进入场景：" + character.Id);
            CharacterDict[character.Id] = character;
            // 广播给场景其它玩家

        }
    }
}
