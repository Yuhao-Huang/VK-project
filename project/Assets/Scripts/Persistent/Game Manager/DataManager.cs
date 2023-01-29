using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Newtonsoft.Json;
using UnityEngine;

public class DataManager : Singleton<DataManager>
{
    [SerializeField] private Data data =new();
    public static Data Data => Instance.data;

    private void OnEnable()
    {
        EventCenter.event_GetItem += Add_Bag;
        EventCenter.event_Transition += On_Transition;
    }


    void Add_Bag(string id, int num)
    {
        Bag bag = data.playerBag.Find(i => i.itemID == id);
        if (bag == null) data.playerBag.Add(new Bag(id, num));
        else bag.num += num;
    }
    void On_Transition(string from, string to) => data.sceneName = to;

    //保存/加载 数据
    public void Save_Data<T>(in T obj, string path)
    {
        if (path[0] is not '\\' and not '/') path = "/" + path;
        path = "/Game Data" + path;
        string parentPath = path;
        int i = parentPath.Length - 1;
        while (parentPath[i] is not '/' and not '\\') { i--; }
        parentPath = parentPath.Substring(0, i);
        if (!Directory.Exists(Application.dataPath + parentPath))
            Directory.CreateDirectory(Application.dataPath + parentPath);
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream file = File.Create(Application.dataPath + path);
        formatter.Serialize(file, JsonConvert.SerializeObject(obj));
        file.Close();
    }
    public void Load_Data<T>(ref T obj, string path)
    {
        if (path[0] != '\\' && path[0] != '/') path = "/" + path;
        path = "/Game Data" + path;
        if (!File.Exists(Application.dataPath + path)) { Debug.Log("Can not Find  " + path);  return; }
        FileStream file = File.Open(Application.dataPath + path, FileMode.Open);
        BinaryFormatter formatter = new BinaryFormatter();
        obj = JsonConvert.DeserializeObject<T>((string)formatter.Deserialize(file));
        file.Close();
    }
}

 

//数据类
[System.Serializable]
public class Data
{
    //玩家状态
    //----------
    //玩家背包
    public List<Bag> playerBag = new List<Bag>();
    //玩家目前的任务列表和进度
    public List<QuestState> playerQuests =new List<QuestState>();
    //当前场景名字
    public string sceneName = "";

    //游戏状态
    public Dictionary<string, int> objState = new Dictionary<string, int>();
    //一次性物体(Binary)      编号1开头       没触发-0,触发过-1
    //背包物品                编号2开头       在ItemList里记录
    //对话                    编号3开头       locked-0 , waiting-1 , completed-2
    //任务                    编号4开头       locked-0 , accepted-1 , completed-2
    //NPC                    编号5开头  
    //特殊物体                编号9开头       这类物体由于特殊不属于上面几类,需要单独写代码处理

    public int Get_BagNum(string id) { Bag bag = playerBag.Find(i => i.itemID == id);  return bag == null ? 0 : bag.num; }
    public QuestState Get_QuestState(string id) => playerQuests.Find(i => i.questID == id);
    public int Get_ObjState(string id) { if (objState.ContainsKey(id)) return objState[id]; else return 0; }

}

//背包类
[System.Serializable]
public class Bag
{
    public string itemID;
    public int num;

    public Bag(string itemID, int num)
    {
        this.itemID = itemID;
        this.num = num;
    }
}

//任务进度类
[System.Serializable]
public class QuestState
{
    public string questID;
    public int nowValue; //当前的进度

    public QuestState(string questID, int nowValue)
    {
        this.questID = questID;
        this.nowValue = nowValue;
    }
}