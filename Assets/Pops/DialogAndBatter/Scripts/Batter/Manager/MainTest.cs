using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainTest : MonoBehaviour
{
    public GameObject TestPanel;
    bool isOpen;

    public static bool stopEnd;
    string currentCell;

    public Toggle togZXL;
    public Toggle togTCCell;
    public Toggle togJSCell;

    public Toggle togMapUp;
    public Toggle togMapDown;

    public Toggle togStopEnd;

    public Toggle togM;

    public Button btnGenerate;

    bool isMapUp;
    bool isMapDown = true;
    bool isZXL = true;
    bool isTC;
    bool isJS;
    bool isM;
    public static bool stopAction;
    
    private void Start()
    {
        togStopEnd.onValueChanged.AddListener((value) =>
        {
            stopEnd = value;
        });

        togMapUp.onValueChanged.AddListener((value) => 
        {
            isMapUp = value;
        });

        togMapDown.onValueChanged.AddListener((value) => 
        {
            isMapDown = value;
        }); 
        
        togZXL.onValueChanged.AddListener((value) => 
        {
            isZXL = value;
        });

        togTCCell.onValueChanged.AddListener((value) => 
        {
            isTC = value;
        });

        togJSCell.onValueChanged.AddListener((value) => 
        {
            isJS = value;
        });

        togM.onValueChanged.AddListener((value) => 
        {
            isM = value;
        });

        btnGenerate.onClick.AddListener(() =>
        {
            string name = "Cell_";

            if (isTC)
            {
                //name += "TC_";
                name += "J_";
            }
            else if (isZXL)
            {
                name += "ZXL_";
            }
            else if (isJS)
            {
                name += "JS_";
            }

            if (isM)
            {
                name += "M_";
            }

            if (isMapDown)
            {
                name += "Solider_Unit";
                BatterManager.Instance.GenerateBelowMap(name);
                Debug.Log("加入友方细胞");
            }
            if (isMapUp)
            {
                name += "Enemy_Unit";
                BatterManager.Instance.GenerateAboveMap(name);
                Debug.Log("加入敌方细胞");
            }
        });
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            isOpen = !isOpen;
            TestPanel.SetActive(isOpen);

            if (!isOpen)
            {
                stopAction = false;
                BatterManager.Instance.CanStartBatter();
            }
            else
            {
                stopAction = true;
            }
        }

    }
}
