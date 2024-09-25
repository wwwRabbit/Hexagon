using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class CountDownDisplay : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI txtCountDown;

    private void Awake()
    {
        //txtCountDown = transform.Find("TxtCountDown").GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        EventCenter.GetInstance().AddEventListener<StringBuilder>("CountDownUI", DisPlay);
    }

    void DisPlay(StringBuilder txt)
    {
        txtCountDown.text = txt.ToString();
    }

    private void OnDisable()
    {
        EventCenter.GetInstance().RemoveEventListener<StringBuilder>("CountDownUI", DisPlay);      
    }
}
