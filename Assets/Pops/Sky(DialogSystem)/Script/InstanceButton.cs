using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InstanceButton : MonoBehaviour
{
    public Button button;

    private void Start()
    {
        if(button!=null)
        {
            button.onClick.AddListener(DestroyButton);
        }
        void DestroyButton()
        {
            Debug.Log("Destroy this button");
            Destroy(button.gameObject);
        }
    }
}
