using Fungus;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GZBuff : MonoBehaviour
{
    [Header("检测半径"), SerializeField] float detectRadius = 5f;
    [Header("检测图层"), SerializeField] LayerMask detectLayer;
    public bool openDetect = false;                                 // 是否开启检测
    public bool drawGizmos = true;                                  // 是否绘制检测范围的 Gizmos

    List<GameObject> detectedCells = new List<GameObject>();        // 保存检测到的cell列表

    private void Update()
    {
        if (openDetect)
        {
            detectedCells = DetectCells();
            openDetect = false;
        }
    }

    /// <summary>
    /// 检测目标
    /// </summary>
    List<GameObject> DetectCells()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, detectRadius, detectLayer);

        List<GameObject> detectedCells = new List<GameObject>();

        foreach (Collider2D collider in colliders)
        {
            if(collider.GetComponent<CellController>())
            {
                CellController buffCell = collider.GetComponent<CellController>();
                if(!collider.GetComponent<GZCellController>())
                {
                    Debug.Log($"{buffCell.transform.name}的攻击速度增加");
                    buffCell.data.atkSpeed += 0.5f;
                }
            }
        }

        return detectedCells;
    }

    /// <summary>
    /// 清除检测到的所有cell
    /// </summary>
    public void ClearDetectedCells()
    {
        detectedCells.Clear();
    }

    private void OnDrawGizmos()
    {
        if (drawGizmos)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, detectRadius);
        }
    }
}
