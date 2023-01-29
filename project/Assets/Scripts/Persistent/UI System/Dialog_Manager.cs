using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class Dialog_Manager : Singleton<Dialog_Manager>
{

    [Header("对话")]
    string dialogID;    //对话的id
    string[] lines;     //每行的内容
    int lineIndex;      //行的下标
    [SerializeField] Window[] windows = new Window[26];   //每个角色对应的聊天窗口
    int winIndex;       //当前窗口的下标
    bool isSetWin;

    [SerializeField] GameObject dialogCanves;  //对话窗口的预制体
    [SerializeField] float prientInterval;   //输出间隔

    //一些判定
    bool isStop;
    bool isSkip;
    bool isDialoging;
    bool isPrinting;


    private void Update()
    {
        Dialogue();
    }




    //编译 对话文本
    void Compile_Dialog()
    {
        if (lineIndex == lines.Length) return;
        string line = lines[lineIndex].Replace(" ","").Trim();
        //处理 空行
        if (line == "") { lineIndex++; Compile_Dialog(); return; }
        //处理 正文角色转换
        if (line.Length == 1 && line[0] is >= 'A' and <= 'Z')
            { Switch_DialogBar(line[0]); lineIndex++; Compile_Dialog();  return; }
        //处理 正文
        if (line[0] is not '#' and not '$') return;
        //处理 命令行
        if (line[0] is '#' or '$')
        {
            string temp = ""; int i = 1;
            while(i<line.Length)
            {
                if (line[i] is '(' or '（') break;
                if (line[i] is ':' or '：')
                {
                    i++;
                    //角色声明
                    if( temp.Length == 1 && temp[0] is >= 'A' and <= 'Z')
                    {
                        winIndex = temp[0] - 'A';
                        temp = "";
                        while (i < line.Length && line[i] is not '(' and not '（') { temp += line[i];i++; }
                        GameObject obj = GameObject.Find(temp);
                        if (obj == null) { Debug.Log($"In dialog {dialogID} ,can not find {temp} !"); break; };
                        isSetWin = true;
                        windows[winIndex].bar = Instantiate(dialogCanves, obj.transform.position, obj.transform.rotation, obj.transform);
                        windows[winIndex].text = windows[winIndex].bar.transform.GetChild(0).GetComponentInChildren<Text>();
                        break;
                    }
                    //对话框偏移
                    if(temp=="Offset")
                    {
                        temp = ""; float x, y;
                        while (i < line.Length && line[i] is not ',' and not '，' ) 
                        {
                            if (line[i] is '(' or '（') { Debug.Log($"In dialog {dialogID} ,there is a wrong '(' !"); lineIndex++; Compile_Dialog(); return; }
                            temp += line[i]; i++;
                        }
                        x = Convert.ToSingle(temp);
                        temp = ""; i++;
                        while (i < line.Length && line[i] is not '(' and not '（') { temp += line[i]; i++; }
                        y = Convert.ToSingle(temp);
                        windows[winIndex].bar.GetComponent<RectTransform>().anchoredPosition += new Vector2(x, y);
                        break;
                    }
                }
                temp += line[i];
                i++;
            }
            lineIndex++;
            Compile_Dialog();
            return;
        }
    }
    //开始 对话
    public void Start_Dialogue(string id,Transform trans = null)
    {
        TextAsset textFile = Resources.Load<TextAsset>("Text/" + id);
        lines = textFile.text.Split('\n');
        dialogID = id;
        lineIndex = 0;
        Compile_Dialog();
        if (!isSetWin)
        {
            windows[0].bar = Instantiate(dialogCanves, trans.position, trans.rotation, trans);
            windows[0].text = windows[winIndex].bar.transform.GetChild(0).GetComponentInChildren<Text>();
        }
        winIndex = 0;
        windows[0].bar.SetActive(true);
        isDialoging = true;
        isStop = false;
        Dialogue();
    }
    //终止 对话
    public void Stop_Dialogue()
    {
        isStop = true;
        isDialoging = false;
        isSkip = false;
        isPrinting = false;
        for(int i=0;i<windows.Length;i++)
            if(windows[i].bar!=null)
                Destroy(windows[i].bar);
    }
    //对话是否结束
    public bool Is_FinishDialog => !Instance.isDialoging;
    //进行 对话
    void Dialogue()
    {
        if (!isDialoging) return;
        if (!Input.GetButtonDown("Interact")) return;
        if (isPrinting) { isSkip = true; return; }

        Compile_Dialog();

        if (lineIndex == lines.Length)
        {
            EventCenter.Notify_DialogComplete(dialogID);
            Stop_Dialogue();
            return;
        }

        StartCoroutine(Instance.Print_DialogWords());

    }
    //切换 对话框
    void Switch_DialogBar(char c)
    {
        if (windows[c - 'A'].bar == null) { Debug.Log($"In dialog {dialogID} ,there is an no character {c} !"); return; }
        windows[winIndex].bar.SetActive(false);
        winIndex = c-'A';
        windows[winIndex].bar.SetActive(true);
    }
    //输出 对话文字
    IEnumerator Print_DialogWords()
    {
        isPrinting = true;
        windows[winIndex].text.text = "";
        for (int i = 0; i < lines[lineIndex].Length; i++)
        {
            if (isSkip)
            {
                windows[winIndex].text.text = lines[lineIndex];
                isSkip = false;
                break;
            }
            if (isStop) break;
            windows[winIndex].text.text += lines[lineIndex][i];
            yield return new WaitForSeconds(prientInterval);
        }
        isPrinting = false;
        lineIndex++;
    }


}

//对话窗口类
[System.Serializable]
public class Window
{
    public Text text;
    public GameObject bar;

    public Window(Text text = null, GameObject bar = null)
    {
        this.text = text;
        this.bar = bar;
    }
}
