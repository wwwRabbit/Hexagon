using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardBasic : MonoBehaviour
{
    public RectTransform target;  // 目标位置的RectTransform
    public float speed = 5f;  // 移动速度
    public float threshold = 0.01f;  // 判定为到达目标的阈值
    public bool isMoving = false;  // 追踪移动是否进行中
    public bool enableMovement = true;  // 控制移动是否被允许


    //以下五行变量在非进行UI panel的情况下不能使用或者赋值
    public bool isTransfromUI;
    public MapSceneUiController mapSceneUiController;

    [SerializeField] protected float transformTime;//UI移动的时间。
    private float startTime;


    //用于UI移动的位置参数
    public RectTransform enterPos;
    public RectTransform exitPos;

    internal RectTransform rectTransform;  // 本元素的RectTransform

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();  // 获取本元素的RectTransform
    }

    void Start()
    {

    }

    internal virtual void Update()
    {
        if (target != null && enableMovement && transformTime != 0f)
        {
            if (!isMoving)
            {
                // 开始移动
                isMoving = true;
            }

            // 计算从开始移动到现在的时间进度
            float timeSinceStarted = Time.time - startTime;
            float percentageComplete = timeSinceStarted / transformTime;
            //Debug.Log(percentageComplete+ gameObject.name);
            // 检查是否完成移动
            float distance = Vector3.Distance(rectTransform.position, target.position);
            if (distance < threshold)
            {
                rectTransform.position = target.position;
                if (mapSceneUiController != null)
                {
                    mapSceneUiController.isMovingIn = false;
                    mapSceneUiController.isMovingOut = false;
                }
                isMoving = false;
                //enableMovement = false;
            }

            else
            {
                // 更新位置，向目标位置平滑移动
                rectTransform.position = Vector3.Lerp(rectTransform.position, target.position, percentageComplete);
            }
        }

        else if(target != null && enableMovement)
        {
            // 计算当前位置与目标位置的距离
            float distance = Vector3.Distance(rectTransform.position, target.position);

            // 检查距离是否足够小
            if (distance > 0.01f)  // 设定一个小阈值以避免微小的抖动
            {
                isMoving = true;
                // 每帧更新位置，向目标位置平滑移动
                rectTransform.position = Vector3.Lerp(rectTransform.position, target.position, Time.fixedDeltaTime * speed);
            }
            else if (isMoving)
            {
                // 若已接近目标位置，则标记移动结束，并直接设置位置
                rectTransform.position = target.position;
                isMoving = false;
                enableMovement = false;
            }
        }
    }

    // 公共函数返回移动是否完成
    public bool IsMovementComplete()
    {
        return !isMoving;
    }

    // 公共方法控制移动开关
    public void SetMovementEnabled(bool enabled)
    {
        enableMovement = enabled;
        if (!enabled)
        {
            isMoving = false;
        }
    }

    public void SetPos(RectTransform tar)
    {
        if (rectTransform == null)
            rectTransform = GetComponent<RectTransform>();  // 获取本元素的RectTransform
        if (tar != null && rectTransform != null)
        {
            // 将目标的世界坐标转换为当前RectTransform的父对象的局部坐标
            Vector3 worldPosition = tar.position;  // 获取目标的世界坐标
            Vector3 localPosition = rectTransform.parent.InverseTransformPoint(worldPosition);  // 将世界坐标转换为局部坐标

            // 设置本元素的局部位置
            rectTransform.localPosition = localPosition;
            Debug.Log(localPosition);
        }
    }

    //设定移动目标，update运行
    public void SetTar(RectTransform tar)
    {
        startTime = Time.fixedTime;
        target = tar;
        enableMovement = true;
        isMoving = true;
    }
}
