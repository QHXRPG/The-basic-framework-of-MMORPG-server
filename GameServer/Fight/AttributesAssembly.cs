using GameServer.Model;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Battle
{
    public class AttributesAssembly
    {
        private Attributes Basic;  // 基础属性 (定义+成长)
        private Attributes Equip;  // 装备属性
        private Attributes Buffs;  // BUFF属性
        public Attributes Final;  // 最终属性


        // 属性初始化
        public void Init(Actor actor)
        {
            Basic = new Attributes();
            Equip = new Attributes();
            Buffs = new Attributes();
            Final = new Attributes();

            var define = actor.Define;
            var level = actor.Info.Level;

            // 初始属性
            var Initial = new Attributes();
            Initial.HPMax = define.HPMax;
            Initial.AP = define.AP;
            Initial.STR = define.STR;
            Initial.DEF = define.DEF;
            Initial.AGI = define.AGI;
            Initial.Speed = define.Speed;
            Initial.MPMax = define.MPMax;
            Initial.AD = define.AD;   
            Initial.MDEF = define.MDEF;
            Initial.CRD = define.CRD;
            Initial.CRI = define.CRI;
            Initial.INT = define.INT;
            Initial.GAGI = define.GAGI;
            Initial.GINT = define.GINT;
            Initial.GSTR = define.GSTR;

            // 成长属性的加持
            var Growth = new Attributes();
            Growth.STR = define.GSTR * level;   // 力量成长
            Growth.INT = define.GINT * level;   // 智力成长
            Growth.AGI = define.GAGI * level;   // 敏捷成长

            // 基础属性 = 初始属性 + 成长属性
            Basic.Add(Initial);
            Basic.Add(Growth);

            // todo 处理装备和buff

            // 合并到最终属性
            Final.Add(Basic);
            Final.Add(Equip);
            Final.Add(Buffs);

            // 计算附加属性
            var Extra = new Attributes();
            Extra.HPMax = Final.STR * 5;  // 每点力量增加5点最大生命值
            Extra.AP = Final.INT * 1.5f;
            Final.Add(Extra);

/*          Log.Information("初始属性：{0}",Initial);
            Log.Information("成长属性：{0}",Growth);
            Log.Information("装备属性：{0}",Equip);
            Log.Information("Buff属性：{0}",Buffs);
            Log.Information("属性附加：{0}",Extra);
            Log.Information("最终属性：{0}",Final);*/
        }
    }
}
