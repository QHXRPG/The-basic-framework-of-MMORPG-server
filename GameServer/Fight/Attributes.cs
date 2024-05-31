using Google.Protobuf.WellKnownTypes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Battle
{
    // 属性数据
    public class Attributes
    {
        // 属性数据项
        public float AD;
        public float AP;
        public float DEF;
        public float MDEF;
        public float CRI;
        public float CRD;
        public float STR;
        public float INT;
        public float AGI;
        public float GSTR;
        public float GINT;
        public float GAGI;
        public float Speed;
        public float HPMax;
        public float MPMax;


        // 属性融合
        public void Add(Attributes data)
        {
            this.AP += data.AP;
            this.DEF += data.DEF;
            this.MDEF += data.MDEF;
            this.CRI += data.CRI;
            this.CRD += data.CRD;
            this.STR += data.STR;
            this.INT += data.INT;
            this.AGI += data.AGI;
            this.GSTR += data.GSTR;
            this.GINT += data.GINT;
            this.Speed += data.Speed;
            this.HPMax += data.HPMax;
            this.MPMax += data.MPMax;
        }


        // 属性移除
        public void Sub(Attributes data) 
        {
            this.AP -= data.AP;
            this.DEF -= data.DEF;
            this.MDEF -= data.MDEF;
            this.CRI -= data.CRI;
            this.CRD -= data.CRD;
            this.STR -= data.STR;
            this.INT -= data.INT;
            this.AGI -= data.AGI;
            this.GSTR -= data.GSTR;
            this.GINT -= data.GINT;
            this.Speed -= data.Speed;
            this.HPMax -= data.HPMax;
            this.MPMax -= data.MPMax;
        }

        // 属性重置
        public void Reset()
        {
            this.AP = 0;
            this.DEF = 0;
            this.MDEF = 0;
            this.CRI = 0;
            this.CRD = 0;
            this.STR = 0;
            this.INT = 0;
            this.AGI = 0;
            this.GSTR = 0;
            this.GINT = 0;
            this.Speed = 0;
            this.HPMax = 0;
            this.MPMax = 0;
        }
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
