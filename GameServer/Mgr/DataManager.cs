using Summer;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;


public class DataManager : Singleton<DataManager>
{
    // 场景字典
    public Dictionary<int, SpaceDefine> Spaces;
    public void Init()
    {
        // 反序列化
        Spaces = Load<SpaceDefine>("Data/SpaceDefine.json");
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

public class SpaceDefine
{
    public int SID; // 场景编号
    public string Name; // 名称
    public string Resource; // 资源
    public string Kind; // 类型
    public int AllowPK; // 允许PK（1允许，0不允许）
}
