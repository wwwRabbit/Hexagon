using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class RangeSensor : MonoBehaviour
{
    [Header("攻击半径"), SerializeField] public float  atkRadius = 1f;
    [Header("攻击半径偏移"), SerializeField] float offset = 0.5f; 
    [Header("检测半径"), SerializeField] float detectRadius = 5f;
    [Header("检测图层"), SerializeField] LayerMask detectLayer;    
    public bool openDetect = false;                                 // 是否开启检测
    public bool drawGizmos = true;                                  // 是否绘制检测范围的 Gizmos
    [SerializeField] bool alreadyDetected = false;                  // 是否已经检测到了物体

    CellController cell;
    GameObject target;

    float distance;
    [SerializeField] private bool canAtk;
    public bool CanAtk => canAtk;

    private void Awake()
    {
        cell = this.transform.parent.GetComponent<CellController>();
    }

    private void Start()
    {
               
    }

    private void Update()
    {
        //这个在全部单位生成完后，正式开局就会打开
        if (!openDetect)
        {
            return;
        }

        if (!alreadyDetected)
        {
            DetectObjects();
        }
        DetectATKObject();
    }

    /// <summary>
    /// 打开或关闭目标探测系统
    /// </summary>
    /// <param name="open"></param>
    public void OpenOrCloseDetect(bool open)
    {
        openDetect = open;

        if (open is false)
        {
            ClearTarget();
        }
    }

    /// <summary>
    /// 检测目标
    /// </summary>
    void DetectObjects()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(this.transform.position, detectRadius, detectLayer);

        float closeDis = float.PositiveInfinity;
        GameObject closeObject = null;

        foreach (Collider2D item in colliders)
        {
            float distance = Vector2.Distance(transform.position, item.transform.position);

            //更新距离最小物体
            if (distance < closeDis)
            {
                closeDis = distance;
                closeObject = item.gameObject;
            }
        }

        if (!alreadyDetected && closeObject != null)
        {
            alreadyDetected = true;
            //更新目标
            cell.target = closeObject;
            target = closeObject;
        }
    }

    /// <summary>
    /// 清除单位的目标以及检测状态
    /// </summary>
    public void ClearTarget()
    {
        target = null;
        cell.target = target;
        alreadyDetected = false;
    }


    /// <summary>
    /// 检测目标是否进入了攻击范围
    /// </summary>
    void DetectATKObject()
    {
        if (target != null)
        {
            distance = Vector2.Distance(transform.position, target.transform.position);
            //offset存在优化的空间，应该是target物体的offset值，当然，也可以理解为offset是指hitbar的长度。
            if ((distance - offset) <= atkRadius)
            {
                canAtk = true;//设置预备进攻状态
            }
            else
            {
                canAtk = false;//取消预备进攻状态
            }
        }
    }

    public bool TargetLeaveRange()
    {
        if (target != null)
        {
            distance = Vector2.Distance(transform.position, target.transform.position);
            if (distance > atkRadius + offset + 1f)
            {
                return true;
            }
            return false;
        }
        return false;
    }

    private void OnDrawGizmos()
    {
        if (drawGizmos)
        {           
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(this.transform.position, detectRadius);

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(this.transform.position, atkRadius);
        }
    }
}
