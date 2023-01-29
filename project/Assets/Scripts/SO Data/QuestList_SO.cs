using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Quest List", menuName = "Data/Quest List")]
public class QuestList_SO : ScriptableObject
{
    public List<Quest> questList = new List<Quest>();
    public Quest Get_QuestInfo(string s)
    {
        Quest quest = questList.Find(i => i.questID == s || i.questName == s);
        if (quest == null) Debug.Log($"Can not find quest {s} !");
        return quest;
    }
}


//»ŒŒÒ¿‡
[System.Serializable]
public class Quest
{
    public string questID;
    public string questName;
    [TextArea] public string decription;
    public Bag[] rewards;
    public Condition unlockCondition;
    public Condition completeCondition;
}

