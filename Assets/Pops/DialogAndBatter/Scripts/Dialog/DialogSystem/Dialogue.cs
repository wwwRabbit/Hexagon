using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dialogue : MonoBehaviour
{
    private static Dialogue instance;
    public static Dialogue Instance => instance;

    private DialogData data;

    public DialogData Data { get => data; }

    [SerializeField] private GameObject dialogPanel;                 //对话窗口引用

    [SerializeField] private bool canDialog;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
        DontDestroyOnLoad(gameObject);
    }

    //此类，负责开关界面，并获得新的对话文件
    //做成接口，后续类进行重写
    //挂载在玩家身上

    /// <summary>
    /// 外部刷新对话文件方法，可能会需要
    /// </summary>
    /// <param name="data"></param>
    public void RefreshDialogData(DialogData data)
    {
        this.data = data;
    }

    /// <summary>
    /// 控制对话界面显隐
    /// </summary>
    /// <param name="isOpne"></param>
    public void ShowOrHideDialogPanel(bool isOpne = true)
    {
        //正常要从ui管理器内调用api去加载,然后记录引用，暂时写成这样，后边改
        dialogPanel.gameObject.SetActive(isOpne);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && data != null && canDialog)
        {
            ShowOrHideDialogPanel();
        }
    }
}
