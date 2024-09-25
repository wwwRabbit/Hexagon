using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CSAScript : MonoBehaviour, ICardSystemAssistant
{
    public List<RectTransform> CardPosDowns;
    public List<RectTransform> CardPosUps;
    public Canvas canvas;
    public bool ifFollow;

    List<ICardUi> curCards;
    List<ICardUi> allCards = new List<ICardUi>();//当前的所有卡牌

    List<GeneralCard> cardToDealFixed;
    List<GeneralCard> curCardToDeal;

    List<GeneralCard> storedCardsForAfterBatter;

    public static CSAScript _instance;

    private System.Random random = new System.Random();  // 随机数生成器


    void Awake()
    {
        transform.parent = null;
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

    // Start is called before the first frame update
    void Start()
    {
        //levelSetUp(1);
        //DealCurCard();
    }

    // Update is called once per frame
    void Update()
    {
        if (ifFollow)
        {
            Rearrange();
        }

        if (Input.GetKeyDown(KeyCode.U))
        {
            StoreCurCardsForAfterBatter();
            CopyCardsAfterBatter();
        }
    }

    public List<ICardUi> DealCurCard()
    {
        if (curCardToDeal == null)
            curCardToDeal = new List<GeneralCard>();
        if (cardToDealFixed == null)
            cardToDealFixed = new List<GeneralCard>();

        //添加必发牌到需要发的集合里

        int a = random.Next(0, 14);
        switch (a)
        {
            case 1:
                curCardToDeal.Add(new NormalAdd_JS(HexManager._instance, InterActController._instance, 5));
                break;

            case 2:
                curCardToDeal.Add(new NormalAdd_JXB(HexManager._instance, InterActController._instance, 5));
                break;

            case 3:
                curCardToDeal.Add(new NormalAdd_FD(HexManager._instance, InterActController._instance, 5));
                break;

            case 4:
                curCardToDeal.Add(new DoubleBuffer(HexManager._instance, InterActController._instance));
                break;

            case 5:
                curCardToDeal.Add(new ExtraVitaminBooster(HexManager._instance, InterActController._instance));
                break;

            case 6:
                curCardToDeal.Add(new ImmuneCheckLocker(HexManager._instance, InterActController._instance));
                break;

            case 7:
                curCardToDeal.Add(new BCellForcedSplit(HexManager._instance, InterActController._instance, 5));
                break;

            case 8:
                curCardToDeal.Add(new FKAllergySignal(HexManager._instance, InterActController._instance, 8));
                break;

            case 9:
                curCardToDeal.Add(new OxidativeBurst(HexManager._instance, InterActController._instance));
                break;

            case 10:
                curCardToDeal.Add(new BloodActivator(HexManager._instance, InterActController._instance));
                break;

            case 11:
                curCardToDeal.Add(new TCGeneEditing(HexManager._instance, InterActController._instance));
                break;

            case 12:
                curCardToDeal.Add(new ZXLActivator(HexManager._instance, InterActController._instance));
                break;

            case 13:
                curCardToDeal.Add(new BCellForcedSplit(HexManager._instance, InterActController._instance,5));
                break;

            case 14:
                curCardToDeal.Add(new CellBindingCapsule(HexManager._instance, InterActController._instance));
                break;

        }
        foreach (GeneralCard gc in cardToDealFixed)
            curCardToDeal.Add(gc);

        List<ICardUi> newCards = new List<ICardUi>();
        foreach (GeneralCard gc in curCardToDeal)
        {
            if (allCards.Count >= 10) continue;
            GameObject newCard = Instantiate(GCtoCardUiFactory._instance.generalCardToUiFactory(gc), canvas.gameObject.transform);
            ICardUi cui = newCard.GetComponent<ICardUi>();
            newCards.Add(cui);
            int index = allCards.Count;
            allCards.Add(cui);
            cui.SetPos(CardPosDowns[index]);
            cui.SetTar(CardPosUps[index]);
            GeneralCard newCardG = CardFoundation.CardSpecificCopy(gc);
            cui.enCard(newCardG);
            newCardG.enUi(cui);
            (cui as CargDrag).StartStage(cardState.Initializing);
        }
        curCardToDeal = new List<GeneralCard>();
        return newCards;
    }
    public List<ICardUi> GetAllCards()
    {
        return allCards;
    }
    public void Rearrange()
    {
        for (int i = 0; i< Mathf.Min(CardPosUps.Count,allCards.Count ); i++)
        {
            allCards[i].SetTar(CardPosUps[i]);
        }
    }
    public bool FinishRA()
    {
        return true;
    }
    public void ManageCardInteraction(bool toState)
    {

    }

    public void levelSetUp(int n)
    {
        switch (n)
        {
            case 1:
                cardToDealFixed = new List<GeneralCard>();
                cardToDealFixed.Add(new NormalMove_Level01_01(HexManager._instance,InterActController._instance));
                cardToDealFixed.Add(new NormalMove_Level01_02(HexManager._instance, InterActController._instance));
                //cardToDealFixed.Add(new DoubleBuffer(HexManager._instance, InterActController._instance));
                //cardToDealFixed.Add(new ExtraVitaminBooster(HexManager._instance,InterActController._instance));
                //cardToDealFixed.Add(new ImmuneCheckLocker(HexManager._instance, InterActController._instance));
                //cardToDealFixed.Add(new BCellForcedSplit(HexManager._instance, InterActController._instance, 5));
                cardToDealFixed.Add(new NormalAdd_ZXL(HexManager._instance, InterActController._instance,2));
                cardToDealFixed.Add(new NormalAdd_TC(HexManager._instance, InterActController._instance, 2));
                //cardToDealFixed.Add(new NormalAdd_JS(HexManager._instance, InterActController._instance, 5));
                //cardToDealFixed.Add(new NormalAdd_JXB(HexManager._instance, InterActController._instance, 5));
                //cardToDealFixed.Add(new NormalAdd_FD(HexManager._instance, InterActController._instance, 5));
                //cardToDealFixed.Add(new ImmuneCheckLocker(HexManager._instance, InterActController._instance));
                //cardToDealFixed.Add(new FKAllergySignal(HexManager._instance, InterActController._instance,8));
                //cardToDealFixed.Add(new OxidativeBurst(HexManager._instance, InterActController._instance));
                //cardToDealFixed.Add(new BloodActivator(HexManager._instance, InterActController._instance));
                break;
        }
    }

    public void SetOtherCardsWhenOneIsDragged(CargDrag dragedCard)
    {
        foreach (ICardUi cui in allCards)
        {
            if ((cui as CargDrag) == dragedCard) continue;
            (cui as CargDrag).StartStage(cardState.NormalWhileOtherAreDraged);
        }
    }

    public void ReSetOtherCardsAfterOneIsDragged(CargDrag dragedCard)
    {
        foreach (ICardUi cui in allCards)
        {
            if ((cui as CargDrag) == dragedCard) continue;
            (cui as CargDrag).StartStage(cardState.Normal);
        }
    }

    public void UseOneCard(ICardUi cui)
    {
        allCards.Remove(cui);
        Debug.Log("第一次count:"+allCards.Count);
        Rearrange();
    }

    public void AddOneCardAndMinusUse(GeneralCard gc)
    {
        if (gc.canBeUsed <= 1) return;
        GameObject newCard = Instantiate(GCtoCardUiFactory._instance.generalCardToUiFactory(gc), canvas.gameObject.transform);
        int index = allCards.Count;
        Debug.Log("第二次count:" + allCards.Count);
        ICardUi cui = newCard.GetComponent<ICardUi>();
        allCards.Add(cui);
        cui.SetPos(CardPosUps[index]);
        cui.SetTar(CardPosUps[index]);
        gc.canBeUsed -= 1;
        for (int i = 0; i < (gc.returnCUI() as CargDrag).useTime; i++)
        {
            cui.useOneTime();
        }
        cui.enCard(gc); //这里是复制
        cui.getCard().enUi(cui);
        (cui as CargDrag).StartStage(cardState.Normal);
        //allCards.Add(cui);
        cui.useOneTime();
        Rearrange();
    }

    public void ManageCardsFollow(bool follow)
    {
        ifFollow = follow;
    }

    public void StoreCurCardsForAfterBatter()
    {
        storedCardsForAfterBatter = new List<GeneralCard>();
        foreach (ICardUi cui in allCards)
        {
            storedCardsForAfterBatter.Add(cui.getCard());
        }
    }

    public List<ICardUi> CopyCardsAfterBatter()
    {
        if (storedCardsForAfterBatter == null) return null;
        RefreshCardPosList();
        allCards = new List<ICardUi>();
        List<ICardUi> newCards = new List<ICardUi>();
        for(int i = 0; i< storedCardsForAfterBatter.Count; i++)
        {
            GeneralCard gc = storedCardsForAfterBatter[i];
            GameObject newCard = Instantiate(GCtoCardUiFactory._instance.generalCardToUiFactory(gc), canvas.gameObject.transform);
            ICardUi cui = newCard.GetComponent<ICardUi>();
            newCards.Add(cui);
            int index = allCards.Count;
            allCards.Add(cui);
            cui.SetPos(CardPosUps[index]);
            cui.SetTar(CardPosUps[index]);
            cui.enCard(gc); //这里是复制
            gc.enUi(cui);
            (cui as CargDrag).StartStage(cardState.Normal);
        }
        return newCards;
    }

    public void RefreshCardPosList()
    {
        if (canvas == null) canvas = GameObject.Find("主要幕布").GetComponent<Canvas>();
        CardPosDowns = canvas.GetComponent<MapSceneUiController>().CardPosDowns;
        CardPosUps = canvas.GetComponent<MapSceneUiController>().CardPosUps;
    }
}
