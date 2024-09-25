using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;  // 使用TextMeshPro

public class TextFollow : MonoBehaviour
{
    public Transform target;  // 要跟随的目标单位
    //public Transform target;  // 要跟随的目标单位
    public Vector3 offset = new Vector3(0, 2, 0);  // UI相对于目标的偏移
    public Camera gameCamera;  // 游戏使用的主摄像机
    public TextMeshProUGUI unitCountText;  // 如果使用TextMeshPro

    private RectTransform rectTransform;  // RectTransform组件

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();  // 获取RectTransform组件
        if (gameCamera == null)
            gameCamera = Camera.main;  // 如果没有指定相机，使用主相机
    }

    void LateUpdate()
    {
        if (target != null)
        {
            // 将目标单位的世界坐标转换为屏幕坐标
            Vector3 screenPosition = gameCamera.WorldToScreenPoint(target.position + offset);

            // 转换屏幕坐标为Canvas坐标
            Vector2 movePos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform.parent as RectTransform, screenPosition, gameCamera, out movePos);
            rectTransform.localPosition = movePos;
        }
    }

    // 方法用于更新显示的数量
    public void UpdateUnitCount(int count)
    {
        unitCountText.text = count.ToString();
    }
}
