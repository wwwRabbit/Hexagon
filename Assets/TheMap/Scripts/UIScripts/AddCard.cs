using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddCard : MonoBehaviour
{

    public GameObject uiParent; // 在Inspector中设置UI父物体  
    public GameObject[] uiPrefab; // 在Inspector中设置要实例化的UI预制体  

    private void Start()
    {
    
    }

    public void DealCard(int x)
    {
        
        // 实例化预制体  
        GameObject newUiChild = Instantiate(uiPrefab[x]);

        // 确保新实例化的UI元素有RectTransform组件  
        RectTransform newUiChildRectTransform = newUiChild.GetComponent<RectTransform>();

        if (newUiChildRectTransform != null)
        {
            // 将新实例化的UI元素设置为UI父物体的子物体  
            newUiChildRectTransform.SetParent(uiParent.transform as RectTransform, false);
        }
        else
        {
            Debug.LogError("The instantiated prefab does not have a RectTransform component!");
        }
    }

    public void PracticeNumber(int x)
    {
        if (x==1)
        {
            Debug.Log("测试成功1");
        }
        else if (x==2)
        {
            Debug.Log("测试成功2");
        }
        
    }



}
