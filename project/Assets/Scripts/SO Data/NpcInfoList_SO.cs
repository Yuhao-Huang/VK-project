using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NpcInfo List", menuName = "Data/NpcInfo List")]
public class NpcInfoList_SO : ScriptableObject
{

        public List<NpcInfo> npcInfoList =new List<NpcInfo>();
        public NpcInfo Get_NpcInfo(string s)
        {
            NpcInfo npcInfo = npcInfoList.Find(i => i.npcID == s || i.npcName == s);
            if (npcInfo == null) Debug.Log($"Can not find npcinfo {s} !");
            return npcInfo;
        }
}

[System.Serializable]
public class NpcInfo
{
    public string npcID;
    public string npcName;
    public List<Dialog> dialogList = new List<Dialog>();
    public List<string> questIDList = new List<string>();
    public Dialog Get_dialogInfo(string s)
    {
        Dialog dialog = dialogList.Find(i => i.dialogID == s || i.dialogName == s);
        if (dialog == null) Debug.Log($"Can not find dialog {s} !");
        return dialog;
    }
}



//∂‘ª∞¿‡
[System.Serializable]
public class Dialog
{
    public string dialogID;
    public string dialogName;
    public bool isRepeat;
    public Condition triggerCondition;
}


