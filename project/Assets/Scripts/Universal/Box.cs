using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : Interactive
{
    [SerializeField] Bag[] items;
    protected override void Intertact()
    {
        if (items.Length == 0) InfoTip_Manager.Instance.Show_InfoTip("什么都没有......");
        for (int i = 0; i < items.Length; i++)
            EventCenter.Notify_GetItem(items[i].itemID, items[i].num);
        isInteracting = false;
        GameManager.Remove_InteractableList(this);
    }
}
