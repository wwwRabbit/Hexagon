using Fungus;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GZBuff : MonoBehaviour
{
    [Header("���뾶"), SerializeField] float detectRadius = 5f;
    [Header("���ͼ��"), SerializeField] LayerMask detectLayer;
    public bool openDetect = false;                                 // �Ƿ������
    public bool drawGizmos = true;                                  // �Ƿ���Ƽ�ⷶΧ�� Gizmos

    List<GameObject> detectedCells = new List<GameObject>();        // �����⵽��cell�б�

    private void Update()
    {
        if (openDetect)
        {
            detectedCells = DetectCells();
            openDetect = false;
        }
    }

    /// <summary>
    /// ���Ŀ��
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
                    Debug.Log($"{buffCell.transform.name}�Ĺ����ٶ�����");
                    buffCell.data.atkSpeed += 0.5f;
                }
            }
        }

        return detectedCells;
    }

    /// <summary>
    /// �����⵽������cell
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
