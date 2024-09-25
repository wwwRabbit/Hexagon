using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FeeChanger : MonoBehaviour
{
    public Sprite Fee_0;
    public Sprite Fee_1;
    public Sprite Fee_2;
    public Sprite Fee_3;
    public Sprite Fee_4;
    public Image selfImage;

    // 设置检测范围的长和宽
    public float detectionWidth = 100f;
    public float detectionHeight = 100f;

    private RectTransform rectTransform;
    private bool isMouseInside = false;

    int defaultN = 0;

    bool endEnabled = true;

    void Start()
    {
        // 初始设置或检查
        if (selfImage == null)
        {
            selfImage = GetComponent<Image>();
        }

        rectTransform = GetComponent<RectTransform>();
        ChangeFeeDisplay(4);
    }

    void Update()
    {
        // 获取鼠标在世界坐标中的位置
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPosition.z = 0; // 忽略Z轴，因为这是2D检测

        // 获取UI元素的中心位置和检测范围大小
        Vector2 rectCenter = rectTransform.position;
        Vector2 rectSize = new Vector2(detectionWidth, detectionHeight);

        bool isInside = (mouseWorldPosition.x > rectCenter.x - rectSize.x / 2 &&
                         mouseWorldPosition.x < rectCenter.x + rectSize.x / 2 &&
                         mouseWorldPosition.y > rectCenter.y - rectSize.y / 2 &&
                         mouseWorldPosition.y < rectCenter.y + rectSize.y / 2);

        // 处理鼠标进入和离开事件
        if (isInside && !isMouseInside)
        {
            isMouseInside = true;
            Highlight();
        }
        else if (!isInside && isMouseInside)
        {
            isMouseInside = false;
            CancelHighlight();
        }

        // 检查鼠标点击事件
        if (isInside && Input.GetMouseButtonDown(0))
        {
            EndTurn();
        }

        if (GameStageControllerScript._instance.curStage == gameStage.PreAction) endEnabled = true; 
    }

    public void ChangeFeeDisplay(int n)
    {
        Debug.Log("here");
        switch (n)
        {
            case 0:
                selfImage.sprite = Fee_0;
                defaultN = 0;
                break;
            case 1:
                selfImage.sprite = Fee_1;
                defaultN = 1;
                break;
            case 2:
                selfImage.sprite = Fee_2;
                defaultN = 2;
                break;
            case 3:
                selfImage.sprite = Fee_3;
                defaultN = 3;
                break;
            case 4:
                selfImage.sprite = Fee_4;
                defaultN = 4;
                break;
        }
    }

    // 你可以在此实现“高亮”效果
    private void Highlight()
    {
        Debug.Log("Highlight");
        selfImage.sprite = Fee_0;
        // 实现你的高亮效果
    }

    // 你可以在此实现“取消高亮”效果
    private void CancelHighlight()
    {
        Debug.Log("Cancel Highlight");
        ChangeFeeDisplay(defaultN);
        // 实现你的取消高亮效果
    }

    // 你可以在此实现“结束回合”效果
    private void EndTurn()
    {
        if (!endEnabled) return;
        Debug.Log("End Turn");
        // 实现你的结束回合效果
        GameStageControllerScript._instance.gameObject.GetComponent<EndTurnButton>().EndTurn();
        endEnabled = true;
    }

    // 使用Gizmos绘制检测范围
    private void OnDrawGizmos()
    {
        if (rectTransform == null)
        {
            rectTransform = GetComponent<RectTransform>();
        }

        // 获取UI元素中心点和尺寸
        Vector2 rectCenter = rectTransform.position;
        Vector2 rectSize = new Vector2(detectionWidth, detectionHeight);

        // 设置Gizmos的颜色
        Gizmos.color = Color.yellow;

        // 绘制长方形检测范围
        Gizmos.DrawWireCube(rectCenter, rectSize);
    }

    
}