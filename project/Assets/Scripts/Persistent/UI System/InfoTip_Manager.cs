using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoTip_Manager : Singleton<InfoTip_Manager>
{
    [Header("信息提示")]
    [SerializeField] Text infoTipBar;
    [SerializeField] ItemList_SO itemInfo;
    bool isShow_InfoTip;
    bool isRefresh_InfoTip;

    private void OnEnable()
    {
        EventCenter.event_GetItem += On_GetItem;
    }



    void On_GetItem(string id, int num)
    {
        string classifier = itemInfo.Get_ItemInfo(id).classifier;
        if (classifier == null) classifier = "个";
        Show_InfoTip($"你获得了 {num} {classifier} {itemInfo.Get_ItemInfo(id).itemName}");
    }


    //显示 信息提示
    public void Show_InfoTip(string info)
    {
        if (Instance.isShow_InfoTip) Instance.isRefresh_InfoTip = true;
        isShow_InfoTip = true;
        infoTipBar.enabled = true;
        infoTipBar.text = info;
        Invoke("Disappear_InfoTip", 2f);
    }
    void Disappear_InfoTip()
    {
        if (isRefresh_InfoTip) isRefresh_InfoTip = false;
        else
        {
            infoTipBar.enabled = false;
            isShow_InfoTip = false;
        }
    }
}
