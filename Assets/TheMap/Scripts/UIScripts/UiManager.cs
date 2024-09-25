using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiManager : MonoBehaviour
{
    public static UiManager instance;

    public GameObject attackUiPrefab;//显示要战斗的UI
    private void Awake()
    {
        if (instance==null)
        {
            instance = this;
        }
        else if (instance!=this)
        {
            Destroy(instance);
        }
    }
    private void Start()
    {
        
    }
    public void ShowWillAttack(Vector3 pos)
    {
        Vector3 screenPos;//屏幕坐标
        screenPos = Camera.main.WorldToScreenPoint(pos);
        GameObject newImage = Instantiate(attackUiPrefab, screenPos, Quaternion.identity);

    }


}
