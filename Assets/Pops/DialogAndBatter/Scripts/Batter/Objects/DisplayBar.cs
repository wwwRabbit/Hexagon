using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DisplayBar : MonoBehaviour
{
    public Image imgInfoA;
    public TextMeshProUGUI txtInfoA;

    public Image imgInfoB;
    public TextMeshProUGUI txtInfoB;

    int nowCountA;
    int nowCountB;
    int maxCountA;
    int maxCountB;

    private void Update()
    {
        nowCountA = BatterManager.Instance.nowSoldierNumber;
        nowCountB = BatterManager.Instance.nowEnemyNumber;
        maxCountA = BatterManager.Instance.BatterData.soldierNumber;
        maxCountB = BatterManager.Instance.BatterData.enemyNumber;
        if(maxCountA != 0 && maxCountB != 0)
        {
            float percentage = ((float)nowCountA / maxCountA) * 100;
            txtInfoA.text = Mathf.RoundToInt(percentage).ToString() + "%";
            imgInfoA.fillAmount = (float)nowCountA / maxCountA;

            percentage = ((float)nowCountB / maxCountB) * 100;
            txtInfoB.text = Mathf.RoundToInt(percentage).ToString() + "%";
            imgInfoB.fillAmount = (float)nowCountB / maxCountB;
            //yield return null;
            //StartCoroutine(RefreshDisplay());
        }
    }

    private void Start()
    {
        StartCoroutine(RefreshDisplay());
        Debug.Log("here");
    }

    

    IEnumerator RefreshDisplay()
    {
        if (maxCountA != 0 && maxCountB != 0)
        {
            float percentage = ((float)nowCountA / maxCountA) * 100;
            txtInfoA.text = Mathf.RoundToInt(percentage).ToString() + "%";
            imgInfoA.fillAmount = (float)nowCountA / maxCountA;

            percentage = ((float)nowCountB / maxCountB) * 100;
            txtInfoB.text = Mathf.RoundToInt(percentage).ToString() + "%";
            imgInfoB.fillAmount = (float)nowCountB / maxCountB;
            yield return null;
            StartCoroutine(RefreshDisplay());
        }
        else
        {
            ////������
            //Debug.Log("ϸ����������ȷ����ǰ����δ����");
            //Debug.Log($"����ʿ��������{maxCountA}���������������{maxCountB}");
            //yield return new WaitForSeconds(5);
            //StartCoroutine(RefreshDisplay());
        }
    }
}
