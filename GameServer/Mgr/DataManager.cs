using Summer;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Serilog;


public class DataManager : Singleton<DataManager>
{
    // �����ֵ�
    public Dictionary<int, SpaceDefine> Spaces;

    // ��Ԫ�ֵ�
    public Dictionary<int, UnitDefine> Units;

    //ˢ���ֵ�
    public Dictionary<int, SpawnDefine> Spawns;

    public Dictionary<int, SkillDefine> Skills;

    public void Init()
    {
        // �����л�
        Spaces = Load<SpaceDefine>("Data/SpaceDefine.json");
        Units = Load<UnitDefine>("Data/UnitDefine.json");
        Spawns = Load<SpawnDefine>("Data/SpawnDefine.json");
        Skills = Load<SkillDefine>("Data/�����趨.json");
        Log.Information("Skills = Load<SkillDefine>(Data/�����趨.json);     " + Skills.Values.ToList());
    }

    public Dictionary<int, T> Load<T>(string filePath)
    {
        // ��ȡexe�ļ�����Ŀ¼�ľ���·��
        string exePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
        string exeDirectory = Path.GetDirectoryName(exePath);
        // �����ļ�������·��
        string txtFilePath = Path.Combine(exeDirectory, filePath); // @"Data/SpaceDefine.json"
        // ��ȡ�ļ�������
        string content = File.ReadAllText(txtFilePath);
        // �����л�
        return JsonConvert.DeserializeObject<Dictionary<int, T>>(content);
    }
}


