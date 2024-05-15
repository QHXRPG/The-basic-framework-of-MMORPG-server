using Summer;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;


public class DataManager : Singleton<DataManager>
{
    // �����ֵ�
    public Dictionary<int, SpaceDefine> Spaces;
    public void Init()
    {
        // �����л�
        Spaces = Load<SpaceDefine>("Data/SpaceDefine.json");
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

public class SpaceDefine
{
    public int SID; // �������
    public string Name; // ����
    public string Resource; // ��Դ
    public string Kind; // ����
    public int AllowPK; // ����PK��1����0������
}
