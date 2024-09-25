using Dialog;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using static Unity.Burst.Intrinsics.X86.Avx;

public class DialogSystem : MonoBehaviour
{
    private VerticalLayoutGroup layoutGroup;

    private DialogData dialogData;

    private TextMeshProUGUI txtName;
    private TextMeshProUGUI txtInfo;
    private Image imgRole;

    private int index;

    [SerializeField, Header("打字机速度"), Range(0, 1)] float textSpeed = 0.1f;
    private float nowTextSpeed;
    private bool isDialog;

    private int optionCount;
    private int optionIndex;
    private bool haveOption;
    private  RectTransform arr;
    private List<GameObject> optionList = new List<GameObject>();
    private Camera uiCamera;
    [SerializeField, Header("箭头与选项的偏移量"), Range(0, 100)] private float arrowOffSet = 35f;

    private void Awake()
    {
        txtName = this.transform.Find("TxtName").GetComponent<TextMeshProUGUI>();
        txtInfo = this.transform.Find("TxtInfo").GetComponent<TextMeshProUGUI>();
        imgRole = this.transform.Find("ImgRole").GetComponent<Image>();

        arr = this.transform.Find("ImgArrow") as RectTransform;
        uiCamera = this.transform.parent.Find("UICamera").GetComponent<Camera>();
    }

    private void OnEnable()
    {
        ///arr.SetParent(transform.GetChild(0));
        arr.gameObject.SetActive(false);

        isDialog = false;
        nowTextSpeed = textSpeed;

        //初始化下标
        index = 0;
        //初始化对话数据
        dialogData = Dialogue.Instance.Data;

        Play();
    }

    private void Update()
    {
        Dialog_Update();
    }

    void Dialog_Update()
    {
        //正常对话状态
        if (Input.GetKeyDown(KeyCode.R) && dialogData != null && !haveOption)
        {
            //如果正在打字中，再次按下，就显示全部字
            nowTextSpeed = isDialog ? 0 : textSpeed;
            if (isDialog) return;

            ControlPlay();
        }
        //选择对话选项状态
        else if (haveOption && optionList.Count != 0)
        {
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                optionIndex--;
                if(optionIndex < 0)
                {
                    optionIndex = optionCount - 1;
                }

                //获取选项的世界坐标
                Vector3 pos = optionList[optionIndex].transform.position;
                //将选项世界坐标转为屏幕坐标
                Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(uiCamera, pos);
                //将选项屏幕坐标转为ui坐标
                RectTransformUtility.ScreenPointToLocalPointInRectangle(arr.parent as RectTransform, screenPos, uiCamera, out Vector2 localPos);
                arr.localPosition = new Vector3(localPos.x + (optionList[optionIndex].transform as RectTransform).rect.width / 2 + arrowOffSet, localPos.y, 0f);
            }
            else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                optionIndex++;
                if (optionIndex >= optionCount)
                {
                    optionIndex = 0;
                }

                //获取选项的世界坐标
                Vector3 pos = optionList[optionIndex].transform.position;
                //将选项世界坐标转为屏幕坐标
                Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(uiCamera, pos);               
                //将选项屏幕坐标转为ui坐标
                RectTransformUtility.ScreenPointToLocalPointInRectangle(arr.parent as RectTransform, screenPos, uiCamera, out Vector2 localPos);
                arr.localPosition = new Vector3(localPos.x +(optionList[optionIndex].transform as RectTransform).rect.width / 2 + arrowOffSet, localPos.y, 0f);
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                index = int.Parse(optionList[optionIndex].name);

                haveOption = false;
                optionCount = 0;
                optionIndex = 0;

                foreach (var item in optionList)
                {
                    Destroy(item.gameObject);
                }
                optionList.Clear();

                arr.gameObject.SetActive(false);

                ControlPlay();
            }
        }
    }

    /// <summary>
    /// 控制继续还是结束对话
    /// </summary>
    void ControlPlay()
    {
        //索引达到文本最后，格式化索引和对话数据
        if (index == dialogData.dialogs.Length)
        {
            index = 0;
            dialogData = null;
            txtInfo.text = null;
            txtName.text = null;
            imgRole.sprite = null;

            imgRole.preserveAspect = true;
            isDialog = false;

            Dialogue.Instance.ShowOrHideDialogPanel(false);
        }
        //否则继续播放
        else
        {
            BuildOption();
            if (!haveOption)
            {
                Play();
            }
        }
    }

    /// <summary>
    /// 显示正常对话句子
    /// </summary>
    void Play()
    {        
        //获取当前索引对应的对话结点
        DialogNode node = dialogData.dialogs[index];
        index = node.nextId;

        //如果名称为空就不显示
        txtName.text = node.name is null ? null : node.name;

        //如果图片为空就不显示图片
        if (node.sprite != null)
        {
            //可能有其它逻辑

            imgRole.gameObject.SetActive(true);
            imgRole.sprite = node.sprite;
        }
        else
        {
            imgRole.gameObject.SetActive(false);
        }

        //对话内容应该不会有空的情况
        //txtInfo.text = node.context;
        StopCoroutine(TypeWriter(node));
        StartCoroutine(TypeWriter(node));
    }

    /// <summary>
    /// 生成选项
    /// </summary>
    void BuildOption()
    {
        while (dialogData.dialogs[index].isOption)
        {
            haveOption = true;
            arr.gameObject.SetActive(true);

            GameObject option = Resources.Load<GameObject>("ImgOptionBack");
            option = Instantiate(option);
            option.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = dialogData.dialogs[index].context;
            option.name = dialogData.dialogs[index].nextId.ToString();
            option.transform.SetParent(this.transform.GetChild(0), false);
            option.transform.localScale = Vector3.one;
            option.transform.localRotation = Quaternion.identity;

            LayoutRebuilder.ForceRebuildLayoutImmediate(option.transform as RectTransform);
            
            StartCoroutine(AdjustArrowPosition(option));

            optionList.Add(option);
            index++;
            optionCount++;
            optionIndex = optionCount - 1;
        }
    }

    IEnumerator AdjustArrowPosition(GameObject option)
    {
        // 等待一帧，确保垂直布局组件已经完成调整
        yield return null;

        // 设置箭头的位置
        Vector3 pos = option.transform.position;
        //选项世界坐标系转为屏幕坐标系
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(uiCamera, pos);
        //屏幕坐标系转为ui坐标系
        RectTransformUtility.ScreenPointToLocalPointInRectangle(arr.parent as RectTransform, screenPos, uiCamera, out Vector2 localPos);
        arr.localPosition = new Vector3(localPos.x + (option.transform as RectTransform).rect.width / 2 + arrowOffSet, localPos.y, 0f);
    }

    IEnumerator TypeWriter(DialogNode node)
    {
        isDialog = true;
        txtInfo.text = null;

        for (int i = 0; i < node.context.Length; ++i)
        {
            txtInfo.text += node.context[i];

            yield return new WaitForSeconds(nowTextSpeed);
        }

        isDialog = false;
    }
}
