using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SortCard : MonoBehaviour
{
    public GameObject prefabToInstantiate; // 在Inspector中设置要实例化的预制体 

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

        // 使用LINQ对子物体进行排序，根据它们的y坐标  
        childGameObjects.Sort((go1, go2) => go1.transform.position.x.CompareTo(go2.transform.position.x));

        // 根据排序后的列表重新设置子物体的顺序  
        for (int i = 0; i < childGameObjects.Count; i++)
        {
            childGameObjects[i].transform.SetSiblingIndex(i);
        }
    }



}


