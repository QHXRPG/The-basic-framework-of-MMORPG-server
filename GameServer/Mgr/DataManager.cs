using Summer;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Serilog;


public class DataManager : Singleton<DataManager>
{
    // 场景字典
    public Dictionary<int, SpaceDefine> Spaces;

    // 单元字典
    public Dictionary<int, UnitDefine> Units;

    //刷怪字典
    public Dictionary<int, SpawnDefine> Spawns;

    public Dictionary<int, SkillDefine> Skills;

    public void Init()
    {
        // 反序列化
        Spaces = Load<SpaceDefine>("Data/SpaceDefine.json");
        Units = Load<UnitDefine>("Data/UnitDefine.json");
        Spawns = Load<SpawnDefine>("Data/SpawnDefine.json");
        Skills = Load<SkillDefine>("Data/技能设定.json");
        Log.Information("Skills = Load<SkillDefine>(Data/技能设定.json);     " + Skills.Values.ToList());
    }

    public Dictionary<int, T> Load<T>(string filePath)
    {
        // 获取exe文件所在目录的绝对路径
        string exePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
        string exeDirectory = Path.GetDirectoryName(exePath);
        // 构建文件的完整路径
        string txtFilePath = Path.Combine(exeDirectory, filePath); // @"Data/SpaceDefine.json"
        // 读取文件的内容
        string content = File.ReadAllText(txtFilePath);
        // 反序列化
        return JsonConvert.DeserializeObject<Dictionary<int, T>>(content);
    }
}


