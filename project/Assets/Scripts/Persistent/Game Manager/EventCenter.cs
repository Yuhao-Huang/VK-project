using System;
using UnityEngine;

public static class EventCenter
{
    //对话结束
    public static event Action<string> event_DialogComplete;
    public static void Notify_DialogComplete(string dialogID) => event_DialogComplete?.Invoke(dialogID);

    //获得物品
    public static event Action<string, int> event_GetItem;
    public static void Notify_GetItem(string itemID,int itemNum) => event_GetItem?.Invoke(itemID,itemNum);

    //场景转换
    public static event Action<string,string> event_Transition;
    public static void Notify_Transition(string from,string to) => event_Transition?.Invoke(from, to);

    //任务完成
    public static event Action<string> event_CompleteQuest;
    public static void Notify_CompleteQuest(string questID) => event_CompleteQuest?.Invoke(questID);

}
