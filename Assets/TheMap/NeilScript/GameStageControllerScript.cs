using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameStageControllerScript : MonoBehaviour
{

    #region 单例
    public static GameStageControllerScript _instance;

    void Awake()
    {
        // 确保只有一个实例存在
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            // 如果已存在一个实例而且不是当前实例，则销毁当前GameObject
            if (this != _instance)
            {
                Destroy(this.gameObject);
            }
        }
    }
    #endregion

    public gameStage curStage = gameStage.zero;

    List<ICardUi> cards = new List<ICardUi>(); //这个是用来存储零时需要的牌的集合，不是全部牌
    ICardSystemAssistant csAssistant;
    IMapUiController MUController;
    IVirusExpandController VRController;


    int curTurn;
    bool haveDealCard = false;
    bool haveRearrangeAfterBurn = false;

    [Header("相关物品")]
    public EndTurnButton turnBtn;
    public MapSceneUiController MapSceneUiControl;

    public gameStage returnStage() { return curStage; }

    public int curFee = 4;

    // Start is called before the first frame update
    void Start()
    {
        csAssistant = CSAScript._instance;
        MUController = MapSceneUiControl;
        Debug.Log("是null吗" + (MUController == null));
        StartStage(gameStage.LevelSetUp);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateStage();
    }

    public void StartStage(gameStage newStage)
    {
        EndStage(curStage);
        switch (newStage)
        {
            case gameStage.LevelSetUp:
                //Debug.Log("1");
                csAssistant.levelSetUp(1);
                turnBtn.SetEndTurnEnabled(false);
                break;

            case gameStage.PreAction:
                //开始阶段，发牌
                //Debug.Log("here");
                haveDealCard = false;
                MUController.UiAllCome();
                curTurn += 1;
                //VRController.UpdateBar(curTurn);
                turnBtn.SetEndTurnEnabled(false);
                break;

            case gameStage.Action:
                csAssistant.ManageCardInteraction(true);
                turnBtn.SetEndTurnEnabled(true);
                break;

            case gameStage.EndActionCheck:
                turnBtn.SetEndTurnEnabled(false);
                cards = new List<ICardUi>();
                foreach (ICardUi cui in csAssistant.GetAllCards())
                {
                    if (cui.ifTempCard())
                    {
                        cards.Add(cui);
                        cui.getBurned();
                    }
                }
                foreach (ICardUi cui in cards)
                {
                    CSAScript._instance.UseOneCard(cui);
                }
                haveRearrangeAfterBurn = false;
                break;

            case gameStage.EnemyAction:
                turnBtn.SetEndTurnEnabled(false);
                MUController.UiAllExit();
                //VRController.InstantiateEnemy();
                break;

            case gameStage.Teach:
                turnBtn.SetEndTurnEnabled(false);
                break;

            case gameStage.Lose:
                turnBtn.SetEndTurnEnabled(false);
                break;
        }
        curStage = newStage;
    }

    public void EndStage(gameStage oldStage)
    {
        switch (oldStage)
        {
            case gameStage.PreAction:
                cards = new List<ICardUi>();
                break;

            case gameStage.Action:
                csAssistant.ManageCardInteraction(false);
                break;

            case gameStage.EndActionCheck:
                cards = new List<ICardUi>();
                Debug.Log("是null吗" + (MUController == null));
                MUController.UiAllExit();
                break;

            case gameStage.EnemyAction:

                break;
        }
    }

    public void UpdateStage()
    {
        switch (curStage)
        {
            case gameStage.LevelSetUp:
                StartStage(gameStage.PreAction);
                break;

            case gameStage.PreAction:
                //检测到所有牌运行结束，结束发牌阶段
                if (!haveDealCard)
                {
                    /*
                    DealCard();
                    haveDealCard = true;*/

                    if (MUController.UiComeEnd())
                    {
                        DealCard();
                        haveDealCard = true;
                    }
                }
                else
                {
                    if (CheckDealEnd())
                    {
                        StartStage(gameStage.Action);
                    }
                }
                break;

            case gameStage.EndActionCheck:
                if (!haveRearrangeAfterBurn)
                {
                    if (CheckBurnEnd())
                    {
                        csAssistant.Rearrange();
                        haveRearrangeAfterBurn = true;
                    }
                }
                else
                {
                    if (csAssistant.FinishRA())
                        StartStage(gameStage.EnemyAction);
                }
                break;

            case gameStage.Action:
                break;

            case gameStage.EnemyAction:
                /*
                if (VRController.GameEnd())
                {
                    StartStage(gameStage.Lose);
                }
                if (VRController.InstantiateEnd())
                {
                    StartStage(gameStage.PreAction);
                }*/
                if (MUController.UiExitEnd())
                {
                    StartStage(gameStage.PreAction);
                }
                //Debug.Log("Now here， successful");
                break;
        }
    }

    public void DealCard()
    {
        cards = csAssistant.DealCurCard();
        Debug.Log('2');
    }

    public bool CheckDealEnd()
    {
        foreach (ICardUi cui in cards)
        {
            if (!cui.ifMoveEnd())
                return false;
        }
        return true;
    }

    public int returnFee() { return curFee; }
    public void UseOneFee()
    {
        curFee = Mathf.Clamp(curFee - 1, 0, 4);
        GameObject.Find("主要幕布").GetComponent<Canvas>().gameObject.GetComponent<MapSceneUiController>().changeFee(curFee);
    }

    public void UpdateFee()
    {
        GameObject.Find("主要幕布").GetComponent<Canvas>().gameObject.GetComponent<MapSceneUiController>().changeFee(curFee);
    }

    public bool CheckBurnEnd()
    {
        foreach (ICardUi cui in cards)
        {
            if (!cui.ifBurnEnd())
                return false;
        }
        return true;
    }

    public void EndActionTurn()
    {
        MUController = GameObject.Find("主要幕布").GetComponent<Canvas>().gameObject.GetComponent<MapSceneUiController>();
        Debug.Log("结束回合");
        StartStage(gameStage.EndActionCheck);
    }

    public void ModifyEndTurnEnableForHexManager(bool toState, FunctionMode mState)
    {
        if(mState == FunctionMode.zero && curStage == gameStage.Action)
            turnBtn.SetEndTurnEnabled(toState);
    }
}

public enum gameStage
{
    LevelSetUp,
    Teach,
    PreAction,
    Action,
    EndActionCheck,
    EnemyAction,
    Lose,
    zero
}

public interface ICardUi
{
    bool ifMoveEnd();
    bool ifTempCard();
    GameObject getUiObj();
    void getBurned();
    bool ifBurnEnd();
    void SetPos(RectTransform rts);
    void SetTar(RectTransform rts);
    void enCard(GeneralCard card);
    GeneralCard getCard();
    void useOneTime();
}

public interface ICardSystemAssistant
{
    void levelSetUp(int n);
    List<ICardUi> DealCurCard();
    List<ICardUi> GetAllCards();
    void Rearrange();
    bool FinishRA();
    void ManageCardInteraction(bool toState);
}

public interface IMapUiController
{
    void UiAllExit();
    bool UiExitEnd();
    //先不进行调用
    void EnterEndUi();
    void UiAllCome();
    bool UiComeEnd();
}

public interface IVirusExpandController
{
    void UpdateBar(int curTurn);
    void InstantiateEnemy();
    bool InstantiateEnd();
    bool GameEnd();
}