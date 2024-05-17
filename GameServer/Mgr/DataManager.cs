using Summer;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;


public class DataManager : Singleton<DataManager>
{
    // 场景字典
    public Dictionary<int, SpaceDefine> Spaces;

    // 单元字典
    public Dictionary<int, UnitDefine> Units;
    public void Init()
    {
        // 反序列化
        Spaces = Load<SpaceDefine>("Data/SpaceDefine.json");
        Units = Load<UnitDefine>("Data/UnitDefine.json");
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
        Console.WriteLine(content);
        // 反序列化
        return JsonConvert.DeserializeObject<Dictionary<int, T>>(content);
    }
}


