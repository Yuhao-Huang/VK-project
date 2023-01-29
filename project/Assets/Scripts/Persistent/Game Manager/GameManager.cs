using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    protected override void Awake()
    {
        base.Awake();
    }
    private void OnEnable()
    {
        EventCenter.event_Transition += Build_SceneObj;
    }
    private void Start()
    {
        TransitionManager.Instance.Transition("Persistent", "1");
    }


    //周围的可交互物品列表
    public List<Interactive> interactableList = new List<Interactive>();
    public static void Add_InteractableList(Interactive obj)
    {
        Instance.interactableList.Add(obj);
        if (Instance.interactableList.Count == 1)
            Instance.StartCoroutine(Instance.interactableList[0].Set_Interactable());
    }
    public static void Remove_InteractableList(Interactive obj)
    {
        if(Instance.interactableList.Contains(obj))
            Instance.interactableList.Remove(obj);
        if (Instance.interactableList.Count > 0)
            Instance.StartCoroutine(Instance.interactableList[0].Set_Interactable());
    }


    //转换场景时,通过数据构建物体
    void Build_SceneObj(string from="",string to="")
    {
        GameObject[] obj = GameObject.FindGameObjectsWithTag("Binary");
        for (int i = obj.Length-1; i >=0; i--)
        {
            if (!DataManager.Data.objState.ContainsKey(obj[i].name)) DataManager.Data.objState.Add(obj[i].name, 0);
            else if (DataManager.Data.Get_ObjState(obj[i].name) == 0) Destroy(obj[i]);
        }
    }

}
