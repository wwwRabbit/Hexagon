using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEngine;

public class MapSceneUiController : MonoBehaviour, IMapUiController
{
    [Header("��Ƭ��������Ϣ")]
    [SerializeField] protected bool currentState;//��ǰ�����ʾ״��
    public bool isMovingOut;//��ʾ�Ƿ������ƶ�
    public bool isMovingIn;//��ʾ�Ƿ������ƶ�

    //�����ﴢ�浱ǰ�����е�UI Panel
    public List<CardBasic> cardPanels = new List<CardBasic>();

    public List<RectTransform> CardPosDowns;
    public List<RectTransform> CardPosUps;

    

    public FeeChanger fc;

    public haoGanDu haoGanObj;

    private void Awake()
    {
        //FindAllCardPanle();
    }

    private void FindAllCardPanle()
    {
        CardBasic[] cards = FindObjectsOfType<CardBasic>();
        cardPanels.Clear();
        foreach (CardBasic card in cards)
        {
            cardPanels.Add(card);
        }
        Debug.Log("Found " + cardPanels.Count + " Card Panels");
    }

    // Start is called before the first frame update
    void Start()
    {
        //UiAllExit();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UiAllExit()
    {
        isMovingOut = true;
        foreach (CardBasic cardPanel in cardPanels)
        {
            cardPanel.SetTar(cardPanel.exitPos);
        }
        CSAScript._instance.ManageCardsFollow(true);
        currentState = false;
    }

    public bool UiExitEnd()
    {
        /*
        if (!isMovingOut) return true;
        else return false;*/
        foreach (CardBasic cardPanel in cardPanels)
        {
            if (!cardPanel.IsMovementComplete())
                return false;
        }
        return true;
    }
    //�ú�����ʱ��ʹ��
    public void EnterEndUi()
    {

    }

    public void UiAllCome()
    {
        Debug.Log("here3");
        isMovingIn = true;
        foreach (CardBasic cardPanel in cardPanels)
        {
            cardPanel.SetTar(cardPanel.enterPos);
        }
        currentState = true;
    }
    public bool UiComeEnd()
    {
        /*
        if (!isMovingIn) return true;
        else return false;*/

        foreach (CardBasic cardPanel in cardPanels)
        {
            if (!cardPanel.IsMovementComplete())
            {
                //Debug.Log("Not Complete:" + cardPanel.gameObject.name);
                return false;
            }   
        }
        CSAScript._instance.ManageCardsFollow(false);
        return true;
    }


    public void changeFee(int n)
    {
        fc.ChangeFeeDisplay(n);
    }

    public void ModifyHGAppearance(bool tf)
    {
        if (tf)
        {
            haoGanObj.SetTar(haoGanObj.enterPos);
        }
        else
        {
            haoGanObj.SetTar(haoGanObj.exitPos);
        }
    }

    public void ModifyHGAppearance(bool tf, Moniter m)
    {
        ModifyHGAppearance(tf);
        haoGanObj.SetHaoganTar(m);
    } 

    public void MakeConfirm()
    {
        haoGanObj.Confirm();
    }
}
