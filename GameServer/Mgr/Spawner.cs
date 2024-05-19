using GameServer.Model;
using Org.BouncyCastle.Crypto.Parameters;
using Serilog;
using Summer;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GameServer.Mgr
{
    public class Spawner
    {
        public SpawnDefine define;
        public Space space;
        public Monster monster;
        public Vector3Int Pos { get; private set; } // 刷怪位置
        public Vector3Int Dir { get; private set; }  // 刷怪方向

        private bool reviving;  // 是否正在被复活中

        private float reviveTime; // 复活时间

        public Spawner(SpawnDefine Define, Space space)
        {
            this.define = Define;
            this.space = space;
            Pos = ParsePoint(define.Pos);  // 解析刷怪点
            Dir = Vector3Int.zero;
            Log.Debug("New Spawner:场景：{0}， 坐标：{1}， 单位类型：{2}, 刷怪周期：{3}", 
                                        space.Name, Define.Pos, Define.TID, Define.Period);
            this.spawn();
        }

        // 解析刷怪点字符串
        private Vector3Int ParsePoint(string text)
        {
            string pattern = @"(\d+) (\d+) (\d+)";
            Match match = Regex.Match(text, pattern);
            if (match.Success)
            {
                int x = int.Parse(match.Groups[1].Value);
                int y = int.Parse(match.Groups[2].Value);
                int z = int.Parse(match.Groups[3].Value);
                return new Vector3Int(x, y, z);
            }
            return Vector3Int.zero;
        }

        private void spawn()
        {
            this.monster = this.space.monsterManager.Create(define.TID, define.Level, Pos, Dir);
        }

        public void Update()
        {
            if(monster != null && monster.IsDeath && !reviving) 
            {
                float period = define.Period;
                this.reviveTime = Time.time + period;
                reviving = true;
            }

            if(reviving && reviveTime <= Time.time)  // 复活时间到
            {
                reviving = false;   
                monster?.Revive();
            }
        }
    }
}
