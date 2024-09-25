using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SortCard : MonoBehaviour
{
    public GameObject prefabToInstantiate; // ��Inspector������Ҫʵ������Ԥ���� 

    private void Start()
    {
        SortChildObjectsByY();
    }

    private void SortChildObjectsByY()
    {
        List<GameObject> childGameObjects = new List<GameObject>();
        foreach (Transform child in transform)
        {
            childGameObjects.Add(child.gameObject);
        }

        // ʹ��LINQ��������������򣬸������ǵ�y����  
        childGameObjects.Sort((go1, go2) => go1.transform.position.x.CompareTo(go2.transform.position.x));

        // �����������б����������������˳��  
        for (int i = 0; i < childGameObjects.Count; i++)
        {
            childGameObjects[i].transform.SetSiblingIndex(i);
        }
    }



}


