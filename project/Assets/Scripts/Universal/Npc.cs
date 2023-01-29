using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Npc : Interactive
{
    public string dialogID;
    public string repeatDialogID ="";
    bool isdialoging;

    protected override void Awake()
    {
        base.Awake();
        if (isRepeat) repeatDialogID = dialogID;
    }

    protected override void Intertact()
    {
        EventCenter.event_DialogComplete += On_DialogComplete;
        Dialog_Manager.Instance.Start_Dialogue(dialogID, transform);
        isdialoging = true;
    }

    protected override void OnTriggerExit2D(Collider2D collision)
    {
        base.OnTriggerExit2D(collision);
        if (isdialoging) //对话被中断
        {
            Dialog_Manager.Instance.Stop_Dialogue();
            EventCenter.event_DialogComplete -= On_DialogComplete;
            isInteracted = false;
        }
    }

    void On_DialogComplete(string id)
    {
        GameManager.Remove_InteractableList(this);
        isdialoging = false;
        isInteracting = false;
        isIntertactable = false;
        EventCenter.event_DialogComplete -= On_DialogComplete;
        if (!isInteracted || isRepeat) //Invoke("GM_Add_Interactable", 0.1f);
            GM_Add_Interactable();
    }
    void GM_Add_Interactable()
    {
        GameManager.Add_InteractableList(this);
    }
}
