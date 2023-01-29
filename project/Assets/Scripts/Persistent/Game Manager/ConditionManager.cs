using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionManager : Singleton<ConditionManager>
{
    [SerializeField] QuestList_SO questList;
    [SerializeField] NpcInfoList_SO npcInfoList;

    private void OnEnable()
    {
        EventCenter.event_GetItem += Check_ItemNum;
        EventCenter.event_DialogComplete += Complete_Dialog;
        EventCenter.event_CompleteQuest += Complete_Qustt;
        EventCenter.event_Transition += Check_SceneName;
    }



    //触发事件时 检查条件状态
    void Check_ItemNum(string id,int num)
    {
        //检查 现有任务
        for (int i = DataManager.Data.playerQuests.Count-1; i >= 0; i--)
        {
            QuestState questState = DataManager.Data.playerQuests[i];
            Quest questinfo = questList.Get_QuestInfo(questState.questID);
            if (questinfo == null) return;
            Condition condition = questinfo.completeCondition;
            if (condition.type == ConditionType.ItemNum && condition.id == id)
            {
                questState.nowValue = DataManager.Data.Get_BagNum(id);
                if (condition.Check()) EventCenter.Notify_CompleteQuest(questState.questID);
            }
        }
        Check_Npc(ConditionType.ItemNum, id);
    }
    void Check_ObjStates(string id)
    {
        for(int i= DataManager.Data.playerQuests.Count-1;i>=0;i--)
        {
            QuestState questState = DataManager.Data.playerQuests[i];
            Quest questinfo = questList.Get_QuestInfo(questState.questID);
            if (questinfo == null) return;
            Condition condition = questinfo.completeCondition;
            if (condition.type == ConditionType.ObjState && condition.id == id && condition.Check())
                EventCenter.Notify_CompleteQuest(questState.questID);
        }
        Check_Npc(ConditionType.ObjState, id);
    }
    void Check_SceneName(string from,string to)
    {
        //检查 现有任务
        for (int i = DataManager.Data.playerQuests.Count-1; i >= 0; i--)
        {
            QuestState questState = DataManager.Data.playerQuests[i];
            Quest questinfo = questList.Get_QuestInfo(questState.questID);
            if (questinfo == null) return;
            Condition condition = questinfo.completeCondition;
            if (condition.type == ConditionType.SceneName && condition.Check())
                EventCenter.Notify_CompleteQuest(questState.questID);
        }
        Check_Npc(ConditionType.SceneName, "");
    }



    //检查当前场景NPC 的 任务 和 对话
    void Check_Npc(ConditionType type ,string id)
    {
        if (GameObject.FindGameObjectWithTag("Npc") == null) { Debug.Log("Can not Find Tag \"Npc\"!"); return; }
        foreach (GameObject npc in GameObject.FindGameObjectsWithTag("Npc")!)
        {
            NpcInfo npcinfo = npcInfoList.Get_NpcInfo(npc.name);
            if (npcinfo == null) { Debug.Log($"There is no Npcinfo of the npc {npc.name}"); return; }
            //检查 任务列表
            foreach (var questID in npcinfo.questIDList)
            {
                Condition condition = questList.Get_QuestInfo(questID).unlockCondition;
                if (!DataManager.Data.objState.ContainsKey(questID)) DataManager.Data.objState.Add(questID, 0);
                if (condition.type == type && condition.id == id && DataManager.Data.objState[questID]==0 && condition.Check())
                    Accept_Quest(questID);
            }
            //检查 对话列表
            foreach (var dialog in npcinfo.dialogList)
            {
                Condition condition = dialog.triggerCondition;
                if (!DataManager.Data.objState.ContainsKey(dialog.dialogID)) DataManager.Data.objState.Add(dialog.dialogID, 0);
                if (condition.type == type && condition.id == id && DataManager.Data.objState[dialog.dialogID] == 0 && condition.Check())
                    Change_NpcDialog(npc, dialog);
            }
            //如果当前对话触发过且不重复,回归重复的默认对话
            if (DataManager.Data.objState[npc.GetComponent<Npc>().dialogID] == 2 && !npc.GetComponent<Npc>().isRepeat && npc.GetComponent<Npc>().repeatDialogID != "")
                Change_NpcDialog(npc, npcinfo.Get_dialogInfo(npc.GetComponent<Npc>().repeatDialogID));
        }
    }
    //接取 任务
    void Accept_Quest(string id)
    {
        DataManager.Data.objState[id] = 1;

        int nowValue = 0;
        Condition condition = questList.Get_QuestInfo(id).completeCondition;
        if (condition.Check()) EventCenter.Notify_CompleteQuest(id);
        else if(condition.type==ConditionType.ItemNum) nowValue = DataManager.Data.Get_BagNum(id); ;
        DataManager.Data.playerQuests.Add(new QuestState(id,nowValue));
    }
    //完成 任务
    void Complete_Qustt(string id)
    {
        Quest quest = questList.Get_QuestInfo(id);
        foreach (var reward in quest.rewards)
            EventCenter.Notify_GetItem(reward.itemID, reward.num);
        DataManager.Data.playerQuests.Remove(DataManager.Data.Get_QuestState(id));
        DataManager.Data.objState[id] = 2;
        Check_ObjStates(id);
    }
    //改变 Npc对话
    void Change_NpcDialog(GameObject npc, Dialog dialog)
    {
        npc.GetComponent<Npc>().dialogID = dialog.dialogID;
        npc.GetComponent<Npc>().isInteracted = false;
        npc.GetComponent<Npc>().isRepeat = dialog.isRepeat;
        if(dialog.isRepeat) npc.GetComponent<Npc>().repeatDialogID = dialog.dialogID;
        DataManager.Data.objState[dialog.dialogID] = 1;
    }
    //完成 Npc对话
    void Complete_Dialog(string id)
    {
        DataManager.Data.objState[id] = 2;
        Check_ObjStates(id);
    }
}


//条件类型                   物品数量  物体状态   场景名称
public enum ConditionType { ItemNum, ObjState, SceneName }
//比较方式
public enum CompareType { Greater, Less, Equal, GreaterOrEqual, LessOrEqual }

//条件类
[System.Serializable]
public class Condition
{
    public ConditionType type;
    public string id;
    public CompareType compare;
    public string target;
    public bool Check()
    {
        switch (type)
        {
            case ConditionType.ItemNum: { return Compare(DataManager.Data.Get_BagNum(id), Convert.ToInt32(target), compare); }
            case ConditionType.ObjState: { return Compare(DataManager.Data.Get_ObjState(id), Convert.ToInt32(target), compare); }
            case ConditionType.SceneName: { return Compare(DataManager.Data.sceneName, target, compare); }
        }
        return false;
    }
    bool Compare<T>(T obj, T target, CompareType type) where T : IComparable
    {
        switch (type)
        {
            case CompareType.Greater: { return obj.CompareTo(target) > 0; }
            case CompareType.GreaterOrEqual: { return obj.CompareTo(target) >= 0; }
            case CompareType.Less: { return obj.CompareTo(target) < 0; }
            case CompareType.LessOrEqual: { return obj.CompareTo(target) <= 0; }
            case CompareType.Equal: { return obj.CompareTo(target) == 0; }
        }
        return false;
    }
}
