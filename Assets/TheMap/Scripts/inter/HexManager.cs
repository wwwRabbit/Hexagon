using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class HexManager : MonoBehaviour
{
    //public GameObject CellPrefb1;//为了实验测试用的细胞1，现在就当是TC细胞了

    //public GameObject CellPrefb2;//预制体2，现在是中性粒细胞

    //public GameObject CellPrefb3;//预制体3，现在是巨噬细胞

    #region 单例
    public static HexManager _instance;

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

    //所有的格子类，在这边提前拿到
    public Tile[] hexs;

    public IMapUnit pathStart { get; set; }
    public Dictionary<Vector2, IMapUnit> allMapUnits = new Dictionary<Vector2, IMapUnit>();

    bool rangeSelected = false;

    List<Vector2> shortPath;
    List<Vector2> selectRange;
    List<Vector2> lastSelectShape;
    List<Vector2> moveRange;

    //private static HexManager instance;
    //public static HexManager Instance => instance; 
    /*
     * 卡牌效果相关变量
     */

    //这是一个需要手动输入的集合，用来记录所有可以生成单位的六边形
    public List<Vector2> startArea = new List<Vector2>();


    FunctionMode curMode = FunctionMode.zero;
    MoveCellsController moveController; //用来执行移动的顺序
    AddCellsController adcController; //用来执行添加细胞的顺序
    AddEffectController adeController; //用来执行添加效果的顺序
    SelectionRangeMaintainer rMaintainer; //用来维护选择的范围
    PreuseRecord puRecorder = new PreuseRecord(); //用来检测鼠标释放

    public FunctionMode returnMode() { return curMode; }

    /// <summary>
    /// 写这个方法，来拿到所有格子的位置，从而存到字典里面
    /// </summary>
    //void GetList()
    //{
    //    for (int i = 0; i < hexs.Length; i++)
    //    {
    //        allMapUnits.Add(hexs[i].HexPos,hexs[i]);
    //    }
    //    Debug.Log(allMapUnits.Count);
    //}

    void GetList1()
    {
        for (int i = 0; i < GameManager.instance.tiles.Length; i++)
        {
            // print(GameManager.instance.tiles[i].HexPos);
            allMapUnits.Add(GameManager.instance.tiles[i].HexPos, GameManager.instance.tiles[i]);
        }

    }
    // NeedData needData;

    public void FindAllTileObjects()
    {
        // 清空已有的Tile对象
        allMapUnits.Clear();

        // 获取场景中的所有根对象
        GameObject[] rootObjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();

        // 遍历每个根对象
        foreach (GameObject rootObject in rootObjects)
        {
            // 遍历根对象及其所有子对象
            Tile[] tiles = rootObject.GetComponentsInChildren<Tile>(true);

            // 添加所有找到的Tile对象
            foreach (Tile tl in tiles)
            {
                allMapUnits.Add(tl.returnPos(), tl);
                tl.manager = this;
            }
        }
    }


    void Start()
    {
        //needData = new NeedData();

        ////从战斗界面那，更新细胞数据
        //needData = ChangeSceneManager.Instance.GetMapUnitDatas();
        //if (allMapUnits[needData.badData.flag]!=null)
        //{
        //    allMapUnits[needData.badData.flag].ReData(needData.goodDatas, needData.badData);
        //}





        GetList1();//在最开始的时候拿到整个地图格子的字典
                   //Debug.Log(hexs.Length);
                   //GetList();
                   //这里人工添加了一个选择范围

        selectRange = new List<Vector2>();
        rMaintainer = new SelectionRangeMaintainer(this);
        selectRange.Add(new Vector2(0, 0));
        selectRange.Add(new Vector2(0, -1));
        selectRange.Add(new Vector2(1, 0));
        selectRange.Add(new Vector2(2, 0));
        rMaintainer = new SelectionRangeMaintainer(this);

        //List<Vector2> moveRange = new List<Vector2>();
        //moveRange.Add(new Vector2(0, 0));

        // TriggerAddEffect(null, selectRange);
        // TriggerMovement(1, moveRange);
        // TriggerAddCell(thecell);
        // TriggerAddCell(GameManager.instance.selectUnit);
        //TriggerAddEffect();

    }

    //public GameObject GetPooledObject()
    //{
    //    // 遍历对象池，找到第一个未激活的物体并返回
    //    for (int i = 0; i < pooledObjects.Count; i++)
    //    {
    //        if (!pooledObjects[i].activeInHierarchy)
    //        {
    //            return pooledObjects[i];
    //        }
    //    }

    //    // 如果没有可用物体，则根据需要创建新的物体并添加到对象池中
    //    GameObject newObj = Instantiate(CellPrefb1);
    //    newObj.SetActive(false);
    //    pooledObjects.Add(newObj);
    //    return newObj;
    //}

    public void AddCell()
    {

        TriggerAddCell(null, 1);

    }



    public void Movement()
    {
        List<Vector2> moveRange = new List<Vector2>();
        moveRange.Add(new Vector2(0, 0));
        moveRange.Add(new Vector2(1, -1));
        TriggerMovement(1, moveRange);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateMode();
        if (Input.GetMouseButtonUp(0))
        {
            //Debug.Log("up");
            puRecorder.RecordUse();
        }
    }

    #region stateMachine
    public void StartMode(FunctionMode newMode)
    {
        EndMode(curMode);
        //这里将mapUnit都调整到了对应的模式
        switch (newMode)
        {
            case FunctionMode.zero:
                GameStageControllerScript._instance.ModifyEndTurnEnableForHexManager(true, newMode);
                break;

            case FunctionMode.moveCells:
                moveController.StartStage(MoveCellsStep.preTrigger);
                break;

            case FunctionMode.generateCell:
                adcController.StartStage(AddCellsStep.preTrigger);
                break;

            case FunctionMode.addEffect:
                adeController.StartStage(AddCellsStep.preTrigger);
                break;
        }
        curMode = newMode;
    }

    //这里就是把所有都复原
    public void EndMode(FunctionMode lastMode)
    {
        switch (lastMode)
        {
            case FunctionMode.moveCells:
                moveController.ExitEffect();
                if (curCard != null && moveController.EffectEnd())
                {
                    doSideEffect();
                }
                break;

            case FunctionMode.generateCell:
                adcController.ExitEffect();
                if (curCard != null && adcController.EffectEnd())
                {
                    doSideEffect();
                }
                break;

            case FunctionMode.addEffect:
                adeController.ExitEffect();
                if (curCard != null && adeController.EffectEnd())
                {
                    doSideEffect();
                }
                break;

            case FunctionMode.zero:
                GameStageControllerScript._instance.ModifyEndTurnEnableForHexManager(false, lastMode); ;
                break;
        }
    }

    public void UpdateMode()
    {
        // print(curMode);
        switch (curMode)
        {
            case FunctionMode.moveCells:
                moveController.UpdateStage();
                if (moveController.EffectEnd())
                {
                    StartMode(FunctionMode.zero);
                }

                break;

            case FunctionMode.generateCell:
                adcController.UpdateStage();
                if (adcController.EffectEnd())
                {
                    StartMode(FunctionMode.zero);
                }
                break;

            case FunctionMode.addEffect:
                adeController.UpdateStage();
                if (adeController.EffectEnd())
                {
                    StartMode(FunctionMode.zero);
                }
                break;

            case FunctionMode.zero:
                //TriggerMovement(1, selectRange);
                //TriggerMovement(1, moveRange);
                //TriggerAddCell(null);
                //TriggerAddEffect(null, selectRange);
                break;
        }
    }
    #endregion


    /*
     * 这个区域代表了卡牌相关
     */
    #region cardRelated

    //注意，statemachine里面加了一句，触发side effect的

    //查找修改可以找到改的地方

    GeneralCard curCard;
    int fee = 1;
    public void enCard(GeneralCard card)
    {
        curCard = card;
    }

    public bool enterPreTrigger()
    {
        if (curCard == null) return false;
        if (!curCard.CheckFee(fee)) return false;
        //makeCardDisappear(curCard);
        fee = curCard.UseFee(fee);
        curCard.TriggerMainFunction();
        return true;

    }

    public void cancelPreTrigger() //这里是，回到选区内
    {
        if (curCard == null) return;
        //makeCardAppear(curCard);
        fee = curCard.ReturnFee(fee);
        curCard.CancelTrigger();
        Debug.Log("取消了");
    }

    public void cancelCardChoose()
    {
        (curCard.returnCUI() as CargDrag).StartStage(cardState.CancelUse);
        curCard.CancelTrigger();
        fee = curCard.ReturnFee(fee);
        curCard = null;
        Debug.Log("在这边");

    }

    public void useChoosedCard()
    {
        //从CSA里移除掉卡牌
        CSAScript._instance.UseOneCard(curCard.returnCUI());
        if (curCard.ifConfirmHG())
        {
            GameObject.Find("主要幕布").GetComponent<MapSceneUiController>().MakeConfirm();
        }
        Debug.Log("用了！");
        

        //让当前卡牌进入使用状态
        (curCard.returnCUI() as CargDrag).StartStage(cardState.Used);
        CSAScript._instance.AddOneCardAndMinusUse(curCard);

    }

    public void doSideEffect()
    {
        if (rMaintainer == null) return;
        curCard.CorrespondingEffect(rMaintainer);
    }

    public void makeCardDisappear(GeneralCard card)
    {
        //让card所在物体零时消失
    }

    public void makeCardAppear(GeneralCard card)
    {
        //让card所在物体零时消失
    }

    #endregion



    /* * * * * * * * *
     * 
     * 3个封装的函数
     * 
     * * * * * * * * *
     */
    public void TriggerMovement(int n, List<Vector2> shapeX) //传入规定范围和距离
    {
        moveController = new MoveCellsController(shapeX, n, this);
        rMaintainer.selectFriRange = new List<Vector2>();
        StartMode(FunctionMode.moveCells);
    }

    public void TriggerAddCell(cellUnit tarCell, int n) //传入需要创建的单位
    {
        adcController = new AddCellsController(tarCell, this, n);
        rMaintainer.selectFriRange = new List<Vector2>();//清空选中友方防止出事。
        StartMode(FunctionMode.generateCell);
    }

    public void TriggerAddEffect(UnitBuff buff, List<Vector2> shapeX) //传入buff和范围
    {
        adeController = new AddEffectController(buff, shapeX, this);
        rMaintainer.selectFriRange = new List<Vector2>();//清空选中友方防止出事。
        StartMode(FunctionMode.addEffect);
    }

    /* * * * * * * * *
     *
     * 需要填充的函数(当前剩余1个)
     * 
     * * * * * * * * *
     */
    public void MoveCell(Vector2 from, Vector2 to)
    {
        //这里需要将from上的单位移动到to上去

        //在这里，先通过我们的六边形位置来拿到这个位置的格子
        IMapUnit TheFrom = allMapUnits[from];//这个是初始的格子地点
        IMapUnit TheTo = allMapUnits[to];//这个是要去的地图格子地点
        TheFrom.preUnit().Move(TheTo.thisPos());
        TheTo.ChangeUnit(TheFrom.preUnit());
        TheFrom.ChangeUnit(null);


    }
    public void MoveCell(List<Vector2> shape, Vector2 sCenter, Vector2 tCenter)
    {
        //这里需要将一组cell进行移动.
        Dictionary<unitControl, Vector3> moveRecord = new Dictionary<unitControl, Vector3>();
        foreach (Vector2 v in shape)
        {
            if (allMapUnits[sCenter + v].preUnit() != null)
            {
                moveRecord.Add(allMapUnits[sCenter + v].preUnit(), allMapUnits[tCenter + v].thisPos());
            }

        }
        Debug.Log(moveRecord.Count);
        Dictionary<Vector2, unitControl> infoRecord = new Dictionary<Vector2, unitControl>();
        foreach (Vector2 v in shape)
        {
            infoRecord.Add(v, allMapUnits[sCenter + v].preUnit());
            allMapUnits[sCenter + v].ChangeUnit(null);
            //Debug.Log("I am here");
        }
        foreach (Vector2 v in shape)
        {
            allMapUnits[tCenter + v].ChangeUnit(infoRecord[v]);
        }
        foreach (unitControl c in moveRecord.Keys)
        {
            c.Move(moveRecord[c]);
            Debug.Log(moveRecord[c]);
        }

    }

    public bool MoveEnd()
    {
        //这里需要在移动结束时，返回true；不然都是false
        return true;
    }
    public void AddCurCell(IMapUnit m, cellUnit inputCell, int n)
    {
        //这里需要给一个Imapunit添加对应的cell
        // Vector3 Addpos = m.thisPos();

        //我想，这边应该加一个新的形参来输入，来供我们选择不同的预制体
        // GameObject newcull = CellPrefb1;//现在测试使用的都是预制体1，所以能生成的都是1号预制体的TC

        // GameObject newcull = GetPooledObject();
        // unitControl n= newcull.GetComponent<unitControl>();
        //GameObject newcull2 = CellFactory.instance.ProducePrefb(inputCell.returnSpcCelltag());
        //m.ChangeUnit(n);
        //GameObject newObject = Instantiate(newcull2, Addpos, Quaternion.identity);

        //注意，这里的 inputCell.returnSpcCelltag() 拿到的是在unitData里面定义的那个CellTag，开头大写的，这个是用来区分具体类型的

        GameObject newObject = CellFactory.instance.ProducePrefb(m, inputCell.returnSpcCelltag());
        newObject.SetActive(true);


        //这里，写破防了，总是检测不到东西，最后逼得用对象池了生成的物体后面（clone）套娃，我才看出来 顺序错了这段代码应该放在生成后面，在前面生成会空
        unitControl d = newObject.GetComponent<unitControl>();
        //  d.hex = m.returnPos();//给新生成的细胞上面赋上所在坐标的格子值
        m.ChangeUnit(d);//把新生成的细胞上面的脚本给到这个格子上
        d.setNum(n);




        //上面，生成的新物体是CellPrefb1,也就是第一个预制体，是普通的中性粒子细胞

        //GameObject newObiect = Instantiate(EnemyPrefab, ProducePosition, Quaternion.identity);



    }

    public bool AddEnd()
    {
        //这里需要在添加结束时，返回true；不然都是false
        return true;
    }

    public void AddEffect(Vector2 pivot, List<Vector2> shape, UnitBuff buff)
    {
        //这里需要给一个IMapUnit添加对应的buff

        foreach (Vector2 v in shape)
        {
            allMapUnits[pivot + v].AddBuff(buff);
        }

    }
    public bool AddEffectEnd()
    {
        //这里需要在添加结束时，返回true；不然都是false
        return true;
    }

    public void SwitchEffect(IMapUnit u, UnitBuff buffToRemove, UnitBuff buffToAdd)
    {
        u.getMaintainer().switchBuff(buffToRemove, buffToAdd);
    }


    #region Foundation
    //调用两次来传入两点，连接成路径
    public void OnSelectPath(IMapUnit u)
    {
        if (pathStart == null)
        {
            pathStart = u;
            if (shortPath != null)
            {
                foreach (Vector2 pos in shortPath)
                {
                    allMapUnits[pos].DeHighlight();
                }
            }
            u.Highlight();
        }
        else
        {
            shortPath = MapToolBox.FindPath(pathStart, u, allMapUnits);
            foreach (Vector2 pos in shortPath)
            {
                allMapUnits[pos].Highlight();
            }
            pathStart = null;
        }
    }

    //基于一个范围进行选择
    public void OnSelectShape(List<Vector2> rPoses, IMapUnit u)
    {
        if (lastSelectShape != null)
        {
            foreach (Vector2 v in lastSelectShape)
            {
                allMapUnits[v].DeHighlight();
            }
        }
        lastSelectShape = new List<Vector2>();
        foreach (Vector2 v in rPoses)
        {
            if (allMapUnits.ContainsKey(u.returnPos() + v))
            {
                allMapUnits[u.returnPos() + v].Highlight();
                lastSelectShape.Add(u.returnPos() + v);
            }
        }
    }

    public void DownSelectShape()
    {
        if (lastSelectShape != null)
        {
            foreach (Vector2 v in lastSelectShape)
            {
                allMapUnits[v].DeHighlight();
            }
        }
        lastSelectShape = new List<Vector2>();
    }

    public void OnSelectShape(List<Vector2> rPoses) //直接位置
    {
        if (lastSelectShape != null)
        {
            foreach (Vector2 v in lastSelectShape)
            {
                allMapUnits[v].DeHighlight();
            }
        }
        lastSelectShape = new List<Vector2>();
        foreach (Vector2 v in rPoses)
        {
            if (allMapUnits.ContainsKey(v))
            {
                allMapUnits[v].Highlight();
                lastSelectShape.Add(v);
                //Debug.Log("Here3");
            }
        }
    }

    //基于自己定义的范围进行选择
    public void OnSelectShape(IMapUnit u)
    {
        List<Vector2> curShape = new List<Vector2>();
        puRecorder.turnOnRecord();
        switch (curMode)
        {
            case FunctionMode.moveCells:
                if (moveController.curStep == MoveCellsStep.preTrigger)
                {
                    Debug.Log("here?realy");
                    curShape = rMaintainer.returnRangeInRMWithAtLeast1Unit(u.returnPos());
                }
                else
                {
                    curShape = rMaintainer.returnRangeInRMWithNoConflict(u.returnPos());
                }
                if (curShape.Count == rMaintainer.selectRange.Count)
                {
                    //这里是正确选择
                    //Debug.Log("Here1");
                    OnSelectShape(curShape);
                }
                else
                {
                    //这里是标红，代表选取有问题
                    //Debug.Log("Here2");
                    OnSelectShape(curShape);
                }

                if (rMaintainer.ifBound)
                {
                    //Debug.Log("Here 4");
                    OnSelectShape(rMaintainer.returnBound(u.returnPos()));
                }
                //Debug.Log("Here3");
                break;

            case FunctionMode.generateCell:
                curShape = rMaintainer.returnRangeInRM(u.returnPos());
                if (rMaintainer.ifEffectiveRange)
                {
                    OnSelectShape(curShape);
                }
                break;

            case FunctionMode.addEffect:
                curShape = rMaintainer.returnRangeInRM(u.returnPos());
                if (rMaintainer.ifEffectiveRange)
                {
                    OnSelectShape(curShape);
                }
                break;
        }
        rangeSelected = true;

    }

    public void DownSelectShapeWhenMouseLeave(IMapUnit u)
    {
        if (rMaintainer.CancelRange(u.returnPos()))
        {
            //puRecorder.reset();
            //puRecorder.turnDownRecord();
        }
    }


    /*
     * 这里下面是卡牌相关的
     */
    public void OnMouseSelectRange(List<Vector2> shape, bool withDistance, int n)
    {
        /*
         * 开启所有格子的鼠标选择范围功能
         */
        foreach (IMapUnit m in allMapUnits.Values)
        {
            m.OnMouseSelect();
        }
        if (withDistance)
            rMaintainer.enRange(shape, n);
        else
            rMaintainer.enRange(shape);

        rangeSelected = false;
    }

    public void OnMouseSelectRangeForMove(List<Vector2> shape, bool withDistance, int n)
    {
        foreach (IMapUnit m in allMapUnits.Values)
        {
            if (m.returnTileType() != UnitType.Enemy_Unit) m.OnMouseSelect();
            else Debug.Log("起作用了");
        }
        if (withDistance)
            rMaintainer.enRange(shape, n);
        else
            rMaintainer.enRange(shape);

        rangeSelected = false;
    }

    public void DownMouseSelectRange()
    {
        /*
         * 关闭所有格子的鼠标选择范围功能
         */
        foreach (IMapUnit m in allMapUnits.Values)
        {
            m.DownMouseSelect();
        }
    }

    public void SetSelected(Vector2 center, List<Vector2> shape)
    {
        /*
         * 使所有六边形变为”选择的“状态
         */
        foreach (Vector2 ad in shape)
        {
            allMapUnits[center + ad].SetSelected();
        }
    }

    public void SetSelected(Vector2 center, List<Vector2> shape, List<Vector2> bound)//bound也是地图上的
    {
        /*
         * 使所有六边形变为”选择的“状态,bound内变成范围内状态
         */
        foreach (Vector2 mu in bound)
        {
            switch (curMode)
            {
                case FunctionMode.moveCells:
                    if (!allMapUnits[mu].IfHaveFriendCell())
                        allMapUnits[mu].SetInBound();
                    break;

                default:
                    allMapUnits[mu].SetInBound();
                    break;
            }
        }
        SetSelected(center, shape);

    }

    public void SetSelectedForMove(Vector2 center, List<Vector2> shape, List<Vector2> bound)//bound也是地图上的
    {
        /*
         * 使所有六边形变为”选择的“状态,bound内变成范围内状态
         */
        foreach (Vector2 mu in bound)
        {
            switch (curMode)
            {
                case FunctionMode.moveCells:
                    if (!allMapUnits[mu].IfHaveFriendCell() && !(allMapUnits[mu].returnTileType() == UnitType.Enemy_Unit))
                        allMapUnits[mu].SetInBound();
                    break;

                default:
                    allMapUnits[mu].SetInBound();
                    break;
            }
        }
        SetSelected(center, shape);

    }

    public void OnMouseSelectRangeWithinRange(List<Vector2> shape, List<Vector2> bound)//这里的bound是地图上的，不是相对的
    {
        /*
         * 使范围内的六边形打开”选择“模式，但是选择是会对周围进行检查
         */
        foreach (Vector2 v in bound)
        {
            allMapUnits[v].OnMouseSelect();
        }
        rMaintainer.enRange(shape);
        rangeSelected = false;
    }

    public void OnMouseSelectRangeWithinRangeWithNoConflict(List<Vector2> shape, List<Vector2> bound)
    {
        foreach (Vector2 v in bound)
        {
            if (!allMapUnits[v].IfHaveFriendCell())
                allMapUnits[v].OnMouseSelect();
        }
        rMaintainer.enRange(shape);
        rangeSelected = false;
        
    }

    public bool ExportSelectedRangeCenter(out Vector2 mailBox)
    {
        //Debug.Log(rMaintainer.ifHaveAnswer());
        if (puRecorder.used && rangeSelected)//鼠标松开，并且有范围，并且没有离开
        {
            
            if (!rMaintainer.ifHaveAnswer())
            {
                if (curMode == FunctionMode.moveCells && moveController.curStep == MoveCellsStep.pivotConfirmed)
                {
                    mailBox = new Vector2(0, 0);
                    puRecorder.reset();
                    return false;
                }
                else
                {
                    //Debug.Log("在这呢2");
                    StartMode(FunctionMode.zero);
                    mailBox = new Vector2(0, 0);
                    return false;
                }
            }
            //Debug.Log("难道在这？");
            if (rMaintainer.ifEffectiveRange)
            {
                if (!CheckRaycastHit(allMapUnits[rMaintainer.curPivot].returnGmo()))
                {
                    puRecorder.reset();
                    mailBox = new Vector2(0, 0);
                    return false;
                }
                /*
                 * 这部分代表了零时取出这些信息,位移动服务的
                 */
                if (rMaintainer.selectFriRange != null)
                {
                    foreach (Vector2 v in rMaintainer.selectFriRange)
                    {
                        allMapUnits[rMaintainer.curPivot + v].setNoUnit();
                    }
                }
                //Debug.Log("here4");
                mailBox = rMaintainer.curPivot;
                puRecorder.turnDownRecord();
                puRecorder.reset();
                return true;
            }
            else
            {
                if (curMode == FunctionMode.moveCells && moveController.curStep == MoveCellsStep.pivotConfirmed)
                {
                    mailBox = new Vector2(0, 0);
                    puRecorder.reset();
                    return false;
                }
                else
                {
                    //Debug.Log("在这呢2");
                    StartMode(FunctionMode.zero);
                    mailBox = new Vector2(0, 0);
                    return false;
                }
            }
        }
        mailBox = new Vector2(0, 0);
        puRecorder.reset();
        return false;
    }

    public bool ExportPossibleRangeAndCenter(out Vector2 mailBoxCenter, out List<Vector2> mailBoxBound)
    {
        if (ExportSelectedRangeCenter(out mailBoxCenter))
        {
            mailBoxBound = new List<Vector2>(rMaintainer.curBound);
            return true;
        }
        else
        {
            mailBoxCenter = new Vector2(0, 0);
            mailBoxBound = new List<Vector2>();
            return false;
        }
    }

    public void DownBound(List<Vector2> bound)
    {
        /*
         * 将范围内的所有处于范围模式的全部关闭。
         */
        foreach (Vector2 mu in bound)
        {
            allMapUnits[mu].DownBound();
        }

    }

    public void DownSelection(List<Vector2> shape, Vector2 center)
    {
        /*
         * 将范围内的所有处于选择模式的全部关闭。
         */
        foreach (Vector2 ad in shape)
        {
            allMapUnits[ad + center].DownSelection();
            //Debug.Log("移动问题");
        }
    }

    /*
     * 这里是移动相关的 过程控制器
     */
    class MoveCellsController
    {
        internal MoveCellsStep curStep = MoveCellsStep.zero;
        List<Vector2> shape;
        int range;
        HexManager father;
        bool ifEnd = false;

        Vector2 selectionCenter = Vector2.zero;
        List<Vector2> selectionBound = new List<Vector2>();
        Vector2 tarCenter;

        public MoveCellsController(List<Vector2> toShape, int toRange, HexManager outer)
        {
            shape = toShape;
            range = toRange;
            father = outer;
        }

        public void StartStage(MoveCellsStep newStage)
        {
            EndStage(curStep);
            switch (newStage)
            {
                case MoveCellsStep.preTrigger:
                    //开启鼠标选择模式
                    father.OnMouseSelectRangeForMove(shape, true, range);
                    father.rMaintainer.selectFriRange = new List<Vector2>();//清空选中友方防止出事。
                    //Debug.Log("Here");
                    father.puRecorder.reset();
                    father.puRecorder.turnOnRecord();
                    break;

                case MoveCellsStep.pivotConfirmed:
                    father.curCard.UseConfirmed();
                    father.curCard.UseNotice();
                    father.OnMouseSelectRangeWithinRangeWithNoConflict(shape, selectionBound);
                    break;

                case MoveCellsStep.endPointConfirmed:
                    father.MoveCell(shape, selectionCenter, tarCenter);
                    //foreach (Vector2 rPos in shape)
                    //{
                    //    father.MoveCell(shape, selectionCenter, tarCenter);
                    //    //father.MoveCell(selectionCenter + rPos, tarCenter + rPos);
                    //    //Debug.Log(selectionCenter + rPos);
                    //   // Debug.Log(tarCenter + rPos);
                    //}
                    break;
            }
            curStep = newStage;
           
        }

        public void EndStage(MoveCellsStep lastStage)
        {
            switch (lastStage)
            {
                case MoveCellsStep.preTrigger:
                    father.DownMouseSelectRange();
                    father.SetSelectedForMove(selectionCenter, shape, selectionBound);
                    break;

                case MoveCellsStep.pivotConfirmed:
                    father.DownBound(selectionBound);
                    father.SetSelected(tarCenter, shape);
                    break;

                case MoveCellsStep.endPointConfirmed:
                    father.DownSelection(shape, selectionCenter);
                    father.DownSelection(shape, tarCenter);
                    father.DownSelectShape();
                    ifEnd = true;
                    break;
            }
        }

        public void UpdateStage()
        {
            switch (curStep)
            {
                case MoveCellsStep.preTrigger:
                    if (father.ExportPossibleRangeAndCenter(out selectionCenter, out selectionBound))
                    {
                        StartStage(MoveCellsStep.pivotConfirmed);
                    }
                    break;

                case MoveCellsStep.pivotConfirmed:
                    if (father.ExportSelectedRangeCenter(out tarCenter))
                    {
                        StartStage(MoveCellsStep.endPointConfirmed);
                    }
                    break;

                case MoveCellsStep.endPointConfirmed:
                    if (father.MoveEnd())
                    {
                        StartStage(MoveCellsStep.zero);
                    }
                    break;
            }
        }

        public bool EffectEnd()
        {
            return ifEnd;
        }

        public void ExitEffect()
        {
            switch (curStep)
            {
                case MoveCellsStep.preTrigger:
                    father.curCard.UseNotice();
                    father.DownMouseSelectRange();
                    father.DownSelectShape();
                    break;

                case MoveCellsStep.zero:
                    CSAScript._instance.ReSetOtherCardsAfterOneIsDragged(null);
                    break;
            }

            /*
             * 这里补一些退出逻辑。
             */
            father.puRecorder.turnDownRecord();
        }
    }

    public class SelectionRangeMaintainer
    {
        internal List<Vector2> selectRange;
        internal List<Vector2> curBound;
        internal List<Vector2> curRange;
        internal List<Vector2> selectFriRange; //这是一个相对的
        internal Vector2 curPivot;
        int r;
        public bool ifBound;
        public bool ifEffectiveRange = false;
        bool getAnswer = false;

        HexManager father;

        public SelectionRangeMaintainer(HexManager m)
        {
            father = m;
        }

        public List<Vector2> returnRange(Vector2 pivot)
        {
            getAnswer = true;
            curPivot = pivot;
            curRange = MapToolBox.FindAllPointsWithinDistance(pivot, 0, selectRange, father.allMapUnits);
            ifEffectiveRange = true;
            return curRange;
        }

        public List<Vector2> returnBound(Vector2 pivot)
        {
            getAnswer = true;
            curPivot = pivot;
            curBound = MapToolBox.FindAllPointsWithinDistance(pivot, r, selectRange, father.allMapUnits);
            return curBound;
        }

        public List<Vector2> returnRangeInRM(Vector2 pivot)
        {
            if (father.allMapUnits[pivot].returnMode() == SelectMode.rangeMode)
            {
                getAnswer = true;
                curPivot = pivot;
                curRange = MapToolBox.FindAllPointsWithinDistanceForRM(pivot, 0, selectRange, father.allMapUnits);
                if (curRange.Count == selectRange.Count)
                    ifEffectiveRange = true;
                else
                {
                    //Debug.Log("Should be here");
                    ifEffectiveRange = false;
                }
            }
            else
            {
                Debug.Log("看见这个说明在一个不处于rangemode的东西上调用了选择");
                return new List<Vector2>();
            }
            return curRange;
        }

        public List<Vector2> returnRangeInRMWithNoConflict(Vector2 pivot)
        {
            if (selectFriRange == null) selectFriRange = new List<Vector2>();
            if (father.allMapUnits[pivot].returnMode() == SelectMode.rangeMode)
            {
                getAnswer = true;
                curPivot = pivot;
                curRange = MapToolBox.FindAllPointsWithinDistanceForRM(pivot, 0, selectRange, father.allMapUnits);
                if (curRange.Count == selectRange.Count)
                {
                    ifEffectiveRange = true;
                    foreach (Vector2 v in selectFriRange)
                    {
                        if (!father.allMapUnits[pivot + v].canBeMove())
                        {
                            ifEffectiveRange = false;
                        }
                    }
                }
                else
                {
                    //Debug.Log("Should be here");
                    ifEffectiveRange = false;
                }
            }
            else
            {
                Debug.Log("看见这个说明在一个不处于rangemode的东西上调用了选择");
                return new List<Vector2>();
            }
            return curRange;
        }

        public List<Vector2> returnRangeInRMWithAtLeast1Unit(Vector2 pivot)
        {
            if (father.allMapUnits[pivot].returnMode() == SelectMode.rangeMode)
            {
                getAnswer = true;
                curPivot = pivot;
                curRange = MapToolBox.FindAllPointsWithinDistanceForRMAndNoEn(pivot, 0, selectRange, father.allMapUnits);
                selectFriRange = new List<Vector2>();
                if (curRange.Count == selectRange.Count)
                {
                    ifEffectiveRange = false;
                    foreach (Vector2 v in curRange)
                    {
                        if (father.allMapUnits[v].IfHaveFriendCell())
                        {
                            ifEffectiveRange = true;
                            selectFriRange.Add(v - pivot); //这里更新了存有友方的。
                        }
                    }
                }
                else
                {
                    //Debug.Log("Should be here");
                    ifEffectiveRange = false;
                }
            }
            else
            {
                Debug.Log("看见这个说明在一个不处于rangemode的东西上调用了选择");
                return new List<Vector2>();
            }
            return curRange;
        }



        public void enRange(List<Vector2> shape)
        {
            selectRange = shape;
            curBound = null;
            curRange = null;
            ifBound = false;
            curPivot = Vector2.zero;
            getAnswer = false;
        }

        public void enRange(List<Vector2> shape, int range)
        {
            enRange(shape);
            r = range;
            ifBound = true;
        }

        public bool CancelRange(Vector2 checkPos)
        {
            if (checkPos != curPivot) return false;
            curBound = new List<Vector2>();
            curRange = new List<Vector2>();
            curPivot = new Vector2(-100, -100);
            getAnswer = false;
            return true;
        }

        public bool ifHaveAnswer() { return getAnswer; }
    }

    class PreuseRecord
    {
        internal bool used = false;
        bool onRecord = false;

        public void reset() { used = false; }
        public void turnOnRecord() { onRecord = true; }
        public void turnDownRecord() { onRecord = false; }
        public void RecordUse()
        {
            if (onRecord)
                used = true;
        }
    }

    /*
     * 这里是生成相关的 过程控制器
     */
    public void ModMouseSelectRangeInFixedArea(List<Vector2> shape, List<Vector2> area, bool ifOn)
    {
        if (ifOn)
        {
            foreach (Vector2 v in area)
            {
                if (allMapUnits.ContainsKey(v)) allMapUnits[v].OnMouseSelect();
            }
            rMaintainer.enRange(shape);
        }
        else
        {
            foreach (Vector2 v in area)
            {
                if (allMapUnits.ContainsKey(v))
                {
                    allMapUnits[v].DownMouseSelect();
                    allMapUnits[v].DeHighlight();
                }
            }
        }

    }

    public void AddCurCell(IMapUnit m)
    {
        AddCurCell(m, adcController.returnCell(),1);
    }

    class AddCellsController
    {
        cellUnit cellToCreate;
        HexManager father;
        bool ifEnd = false;
        AddCellsStep curStep;
        internal List<Vector2> unitShape = new List<Vector2>();
        Vector2 addCenter;
        int cellNum;

        public AddCellsController(cellUnit tarCell, HexManager outer, int n)
        {
            cellToCreate = tarCell;
            father = outer;
            curStep = AddCellsStep.zero;
            unitShape.Add(new Vector2(0, 0));
            cellNum = n;
        }

        public void StartStage(AddCellsStep newStage)
        {
            EndStage(curStep);
            switch (newStage)
            {
                case AddCellsStep.preTrigger:
                    father.ModMouseSelectRangeInFixedArea(unitShape, father.startArea, true);
                    //father.OnAddModeInFixedArea(father.startArea);
                    //Debug.Log("Here");
                    father.puRecorder.reset();
                    father.puRecorder.turnOnRecord();
                    break;

                case AddCellsStep.pivotConfirmed:
                    father.curCard.UseConfirmed();
                    father.curCard.UseNotice();
                    father.AddCurCell(father.allMapUnits[addCenter], cellToCreate, cellNum);
                    break;
            }

            curStep = newStage;
        }

        public void EndStage(AddCellsStep lastStage)
        {
            switch (lastStage)
            {
                case AddCellsStep.preTrigger:
                    father.ModMouseSelectRangeInFixedArea(unitShape, father.startArea, false);
                    break;

                case AddCellsStep.pivotConfirmed:
                    ifEnd = true;
                    break;
            }
        }

        public void UpdateStage()
        {
            switch (curStep)
            {
                case AddCellsStep.preTrigger:
                    if (father.ExportSelectedRangeCenter(out addCenter))
                    {
                        StartStage(AddCellsStep.pivotConfirmed);
                    }
                    break;

                case AddCellsStep.pivotConfirmed:
                    if (father.AddEnd())
                    {
                        StartStage(AddCellsStep.zero);
                    }
                    break;
            }
        }

        public cellUnit returnCell() { return cellToCreate; }

        public bool EffectEnd()
        {
            return ifEnd;
        }

        public void ExitEffect()
        {
            switch (curStep)
            {
                case AddCellsStep.preTrigger:
                    father.curCard.UseNotice();
                    father.DownMouseSelectRange();
                    father.DownSelectShape();
                    break;

                case AddCellsStep.zero:
                    CSAScript._instance.ReSetOtherCardsAfterOneIsDragged(null);
                    break;
            }

            /*
             * 这里补一些退出逻辑。
             */
            father.puRecorder.turnDownRecord();
            Debug.Log("End");
        }

    }

    /*
     * 加效果相关的 过程控制器
     */
    class AddEffectController
    {
        UnitBuff buffToAdd;
        List<Vector2> shape;
        HexManager father;
        bool ifEnd = false;
        AddCellsStep curStep;
        internal List<Vector2> unitShape = new List<Vector2>();
        Vector2 buffCenter;

        public AddEffectController(UnitBuff toAdd, List<Vector2> shapeX, HexManager outer)
        {
            buffToAdd = toAdd;
            shape = shapeX;
            father = outer;
            curStep = AddCellsStep.zero;
            unitShape.Add(new Vector2(0, 0));
        }

        public void StartStage(AddCellsStep newStage)
        {
            EndStage(curStep);
            switch (newStage)
            {
                case AddCellsStep.preTrigger:
                    father.OnMouseSelectRange(shape, false, 0);
                    //father.OnAddModeInFixedArea(father.startArea);
                    //Debug.Log("Here");
                    father.puRecorder.reset();
                    father.puRecorder.turnOnRecord();
                    break;

                case AddCellsStep.pivotConfirmed:
                    father.curCard.UseConfirmed();
                    father.curCard.UseNotice();
                    father.AddEffect(buffCenter,shape, buffToAdd);
                    break;
            }

            curStep = newStage;
        }

        public void EndStage(AddCellsStep lastStage)
        {
            switch (lastStage)
            {
                case AddCellsStep.preTrigger:
                    father.DownMouseSelectRange();
                    break;

                case AddCellsStep.pivotConfirmed:
                    father.DownSelectShape();
                    ifEnd = true;
                    break;
            }
        }

        public void UpdateStage()
        {
            switch (curStep)
            {
                case AddCellsStep.preTrigger:
                    if (father.ExportSelectedRangeCenter(out buffCenter))
                    {
                        StartStage(AddCellsStep.pivotConfirmed);
                    }
                    break;

                case AddCellsStep.pivotConfirmed:
                    if (father.AddEffectEnd())
                    {
                        StartStage(AddCellsStep.zero);
                    }
                    break;
            }
        }

        public bool EffectEnd()
        {
            return ifEnd;
        }

        public void ExitEffect()
        {
            switch (curStep)
            {
                case AddCellsStep.preTrigger:
                    father.curCard.UseNotice();
                    father.DownMouseSelectRange();
                    father.DownSelectShape();
                    break;

                case AddCellsStep.zero:
                    CSAScript._instance.ReSetOtherCardsAfterOneIsDragged(null);
                    break;
            }
            /*
             * 这里补一些退出逻辑。
             */
            father.puRecorder.turnDownRecord();
            Debug.Log("End");
        }

    }
    #endregion

    public bool CheckRaycastHit(GameObject obj)
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D[] hits = Physics2D.RaycastAll(mousePosition, Vector2.zero);

        // 检查所有击中的对象中是否包含目标对象
        foreach (var hit in hits)
        {
            if (hit.collider != null && hit.collider.gameObject == obj)
            {
                return true;  // 射线击中了传入的对象
            }
        }

        return false;  // 射线未击中传入的对象
    }
}

#region enums
public enum FunctionMode
{
    moveCells,
    addEffect,
    generateCell,
    zero,
}

public enum MoveCellsStep
{
    zero,
    preTrigger,
    pivotConfirmed,
    endPointConfirmed
}

public enum AddCellsStep
{
    zero,
    preTrigger,
    pivotConfirmed
}

public interface cellUnit
{
    cellTag returnTag();
    bool compareUnit(cellUnit other);
    int returnNum();
    int changeNum(int modifier);
    int setNum(int toNum);

    //这个是拿来检测，我们这边是TC细胞，是中性粒细胞，还是巨噬细胞的，用的是拼音
    //TheUnitTag returnUnitTag();

    //看清楚，这里这个前面是大写的C，定义在UnitData脚本里
    CellType returnSpcCelltag();

}

public class CellUnitInfoHolder:cellUnit
{
    cellTag relationState;
    CellType cellType;
    int n;

    public CellUnitInfoHolder(cellUnit otherUnit)
    {
        relationState = otherUnit.returnTag();
        cellType = otherUnit.returnSpcCelltag();
        n = otherUnit.returnNum();
    }

    public cellTag returnTag() { return relationState; }
    public bool compareUnit(cellUnit other) { return false; }
    public int returnNum() { return n; }
    public int changeNum(int modifier)
    {
        n = n + modifier;
        return n;
    }
    public int setNum(int toNum)
    {
        n = toNum;
        return n;
    }
    public CellType returnSpcCelltag()
    {
        return cellType;
    }
}

public enum cellTag
{
    enemy,
    friendly,
    zero,

}

//为了和pop已经写好的细胞种类对接，这个不用了，具体的细胞类型在UnitData里面
public enum TheUnitTag
{
    tc,
    zhongxingli,
    jushi

}

//BUFF类，在这里给BUFF类型定义
public interface UnitBuff
{
    bool ifTransform();
    buffTagForEffectType returnEffectCellType();
    string returnMessage();
    CellType returnTransFormTag(CellType inputC);

}

public enum buffTagForEffectType
{
    enmey,
    friendly,
    both,
    zero
}
#endregion


