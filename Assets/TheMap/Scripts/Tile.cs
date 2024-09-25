using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//一言以蔽之，这个是挂载在一个个地图瓦片上面的方法
public class Tile : MonoBehaviour,IMapUnit
{
    public bool Judian=false;//判断是不是据点的方法

    private bool attack = false;
    private bool isSelected = false;//确认战斗的选择状态

    UnitData Data=new UnitData();//这个类是用来传输所有要往战斗方面过去的数据的类

    NeedData SendMess = new NeedData();

    List<IMapUnit> AllTileDatas = new List<IMapUnit>();//这个是用来存储周围所有格子信息的
    List<IMapUnit> AttackDatas = new List<IMapUnit>();//这个是用来存储涉及到战斗的格子信息的


   // UnitType ThisUnitType;//这个标识代表的是，判断这个格子上的

    public bool ifcell = false;//判断上面是不是有单位的bool

    public unitControl ThisUnit;//记录下当前上面的单位

    [Header("这个是来记录格子上面的敌人类型的")]
    public UnitType ThisTileType;

    [Header("每个地块的六边形坐标")]
    public Vector2 HexPos;

    public UnitBuff currentBuff;

    public HexManager manager;

    public SpriteRenderer SpriteRenderer;//整这么个东西是为了更好的控制每个地图组件

    public bool canwalk;//这个是用来判断，这个地块有没有障碍
    public bool haveFriend;//用来判断，这地方有没有右方单位

    //[Header("这个数字来表示上面的细胞类型")]
    //[Header("：0是敌人 1是中性粒细胞，2是巨噬细胞，3是tc细胞，4是肥大细胞，5是浆细胞")]

    //不暂定的，现在规定，

    public int unittag;//使用个简单的int值来存储格子上面的单位信息，先暂定1是敌人 2是我方单位

   // [Header("下面是要给不同的单位设置不同的层级，层级跟上面细胞类型编号一一对应")]
    //public LayerMask oblayerMask;//敌人层级吧
    //public LayerMask oblayerMask1;//我方单位的层级
    //public LayerMask oblayerMask2;
    //public LayerMask oblayerMask3;
    //public LayerMask oblayerMask4;
    //public LayerMask oblayerMask5;

    public LayerMask[] oblay;


    public bool canBig;//这个是控制，地块能不能变大的
    public bool canClick;//这个是控制能不能点击这个地块的，毕竟移动的方法是写在这个瓦片上面的，这个需要小心控制

    public bool canclickwalk;//用来判定是不是点击的时候需要移动


    public Color SimpleColor;//普通颜色

    public Color InfectColor;//感染细胞的颜色

    public Color highlightcolor;//这是瓦片处于可以选中的状态中时，高亮的颜色

    private Color PriColor;//这个记录瓦片一开始的颜色，用于高亮后还原

    private Vector3 PriSize;//记录一开始的正常瓦片大小

    

    public Vector3 pos;//每个格子的位置

    [Header("正式选择状态相关子物体")]
    public GameObject inBoundObj;
    public GameObject tempSelectObj;
    public GameObject fixedSelectedObj;

    [Header("Buff管理器")]
    public GameObject buffMaintainerObj;
    BuffMaintainer bfMaintainer;

    public BuffMaintainer getMaintainer() { return bfMaintainer; }
    SelectMode curMode;//状态机用的状态

    private void Awake()
    {
        SpriteRenderer = GetComponent<SpriteRenderer>();//拿到相关组件
    }
    void Start()
    {

        pos = this.transform.position;
        PriSize = transform.localScale;
        bfMaintainer = buffMaintainerObj.GetComponent<BuffMaintainer>();
       
        //canwalk = true;
       // CheckObstacle();//执行检测附近障碍物的方法
        canClick = false;//初始设定，所有的瓦片都不能点击

        PriColor = SpriteRenderer.color;//记录一下最初的颜色
        //SpriteRenderer.color = SimpleColor;
        canclickwalk = false;//这里，初始，所有的瓦片都不能走，只有物体在检测之后才能走

        

    }

    // Update is called once per frame
    void Update()
    {
       
       // Debug.Log(currentBuff);
        checkclick();
        CheckObstacle();

        CheckUnitType();//
        theData();//格子上面这个信息类需要实时更新
    }



    public void theData()
    {
     
        if (ThisUnit!=null)
        {
            //List<GeneralBuff> toBuff = bfMaintainer.getBuffs();
            Data.flag = HexPos;//这个是，格子当前的六边形坐标
            
            //if(ThisUnit.MyType == )
            Data.unitType = ThisTileType;//这个代表的是格子的种类
            Data.cellType = ThisUnit.MyType;//这个代表的是，这个格子上的细胞的类型
            
            List<UnitBuff> toBuff = bfMaintainer.getSideBuff();//两个辅助buff

            Data.cellNumber = ThisUnit.num;//这个单位格子上面的格子里的细胞数量，取决于单位里面的细胞数量

            Data.sideBuffs = toBuff;
            Data.mainBuff = bfMaintainer.returnMain();

            if (ThisUnit.boss)
            {
                //Debug.Log(HexPos);
                Judian = true;
            }

        }




    }


    //
    public UnitData UpdateUnitForBattle()
    {
        if (ThisUnit != null)
        {
            bool canTrans = false;
            //List<GeneralBuff> toBuff = bfMaintainer.getBuffs();
            Data.flag = HexPos;//这个是，格子当前的六边形坐标

            //if(ThisUnit.MyType == )
            Data.unitType = ThisTileType;//这个代表的是格子的种类
            Debug.Log("将开始找buff");
            if (bfMaintainer.returnMain() != null)
            {
                Debug.Log("有buff");
                if (bfMaintainer.returnMain().returnEffectCellType() == buffTagForEffectType.both)
                {
                    canTrans = true;
                }
                else if (ThisTileType == UnitType.Enemy_Unit && bfMaintainer.returnMain().returnEffectCellType() == buffTagForEffectType.enmey)
                {
                    canTrans = true;
                }
                else if (ThisTileType == UnitType.Solider_Unit && bfMaintainer.returnMain().returnEffectCellType() == buffTagForEffectType.friendly)
                {
                    canTrans = true;
                }
                if (canTrans)
                {
                    if (bfMaintainer.returnMain().returnTransFormTag(ThisUnit.MyType) != CellType.Cell_Null)
                    {
                        Data.cellType = bfMaintainer.returnMain().returnTransFormTag(ThisUnit.MyType);
                        bfMaintainer.UseMain();

                    }
                    else
                    {
                        Data.cellType = ThisUnit.MyType;
                    }
                }

            }
            else
            {
                Data.cellType = ThisUnit.MyType;//这个代表的是，这个格子上的细胞的类型
            }
            Debug.Log(Data.cellType.ToString());

            List<GeneralBuff> toBuff = bfMaintainer.getBuffs();//两个辅助buff
            List<GeneralBuff> usedBuff = new List<GeneralBuff>();

            int cellNum = ThisUnit.num;
            foreach (GeneralBuff gbf in toBuff)
            {
                switch (gbf)
                {
                    case buff_NumUp:
                        cellNum += 5;
                        usedBuff.Add(gbf);
                        break;
                }
            }

            Data.cellNumber = ThisUnit.num;//这个单位格子上面的格子里的细胞数量，取决于单位里面的细胞数量

            foreach (GeneralBuff gbf in usedBuff)
            {
                if (toBuff.Contains(gbf)) toBuff.Remove(gbf);
            }

            //Data.allBuffs = toBuff;
            throw new System.Exception("使用了未完善的方法");
            return Data;
        }
        return null;
    }


    //这个是实时监测格子上面的单位种类的，从而给格子进行一个敌我的区分
    void CheckUnitType()
    {
        if (ThisUnit!=null)
        {
            if (ThisUnit.FrE==cellTag.friendly)
            {
                ThisTileType = UnitType.Solider_Unit;
                //Data.unitType = ThisTileType;
            }
            else if (ThisUnit.FrE==cellTag.enemy)
            {
                ThisTileType = UnitType.Enemy_Unit;
               // Data.unitType = ThisTileType;
            }
           
        }
    }
    //这个是为了准备进入战斗而收集周围这些格子信息的
   public void AttackTrigger()
    {
        int range = 1;

        //现在需要一个方法，来拿到所有地图格子的信息，然后把这些格子信心都存起来，然后再把涉及到战斗部分的格子信息退回来，单独进行处理
        //List<IMapUnit> AllTileDatas = new List<IMapUnit>();//这个List来存所有的格子信息

        Vector2 center;
        center.x = 0;
        center.y = 0;
        //所有的格子上的信息在这先拿一遍，用下面封装好的方法，我门格子间的最大距离是4吧，保险起见我写成5,直接从最中间进行拿取
        AllTileDatas = GetNerTile(center, 5);

        
        List<IMapUnit> NerTile = new List<IMapUnit>();//这个是，用来存附近所有符合条件的信息的



        NerTile = GetNerTile(HexPos, range);//封装好的一个拿取周围所有格子的方法
        AttackDatas = NerTile;//附近所有的格子都是涉及到战斗场景的
        //好，现在我们拿到了周围的所有的格子

        //把触发战斗周围的这些格子都退出来
        for (int i = 0; i < NerTile.Count; i++)
        {
            AllTileDatas.Remove(NerTile[i]);
        }

        //下面，在changscenemanager里面储存一个值来把这些信息报存下来然后在回来之后把他们都赋回来

       

        //现在，基于格子上面的属性来进行操作，我们来填充往战斗方面传入信息的组

        List<UnitData> TheGoodDatas = new();

        for (int i = 0; i < NerTile.Count; i++)
        {
            //&&NerTile[i].preUnit()!=null
            if (NerTile[i].returnTileType()==UnitType.Solider_Unit && NerTile[i].preUnit() != null)
            {
                //(NerTile[i] as Tile).UpdateUnitForBattle();
                TheGoodDatas.Add((NerTile[i] as Tile).returnData());//这里是添加友方单位的信息
            }
        }

        List<UnitData> TheBadDatas = new();
        for (int i = 0; i < NerTile.Count; i++)
        {
            if (NerTile[i].returnTileType()==UnitType.Enemy_Unit&&NerTile[i].preUnit()!=null)
            {
                //(NerTile[i] as Tile).UpdateUnitForBattle();
                TheBadDatas.Add((NerTile[i] as Tile).returnData());
            }
        }
        //TheBadDatas.Add(this.Data);
        //上面，把包括自己在内的所有的周围的地方格子也参与了战斗



        //Debug.Log(TheGoodDatas.Count);
        SendMess.goodDatas = TheGoodDatas;//将周围所有有我方单位的格子信息赋值给SendMess中的我方信息组
        SendMess.badDatas = TheBadDatas;//这个跟上面一样，是把周围所有敌人单位给赋过去的

        //下面这行本来没用了，我怕出现点啥报错就留着了
      //  SendMess.badData = this.Data;//将自身的信息赋值给了sendmess的敌人信息组，这个方法是在有敌人的格子上面调用的


        if (TheGoodDatas.Count==0)
        {
            Debug.Log("附近没有我方单位，无法进入战斗");
            return;
        }
        else
        {
            Debug.Log("检测到符合战斗条件，再次点击进入战斗");
            Debug.Log("附近参战地块有" + TheGoodDatas.Count+"个");
            attack = true;

        }



    }

    public UnitData decodeBuff(UnitData toDecode)
    {
        if (toDecode != null)
        {
            UnitData newData = new UnitData(); 
            bool canTrans = false;
            //List<GeneralBuff> toBuff = bfMaintainer.getBuffs();
            newData.flag = toDecode.flag;//这个是，格子当前的六边形坐标

            //if(ThisUnit.MyType == )
            newData.unitType = toDecode.unitType;//这个代表的是格子的种类
            newData.mainBuff = toDecode.mainBuff;
            newData.sideBuffs = toDecode.sideBuffs;
            Debug.Log("将开始找buff");
            if (toDecode.mainBuff != null)
            {
                Debug.Log("有buff");
                if (toDecode.mainBuff.returnEffectCellType() == buffTagForEffectType.both)
                {
                    canTrans = true;
                }
                else if (toDecode.unitType == UnitType.Enemy_Unit && toDecode.mainBuff.returnEffectCellType() == buffTagForEffectType.enmey)
                {
                    canTrans = true;
                }
                else if (toDecode.unitType == UnitType.Solider_Unit && toDecode.mainBuff.returnEffectCellType() == buffTagForEffectType.friendly)
                {
                    canTrans = true;
                }
                if (canTrans)
                {
                    Debug.Log("到这了");
                    Debug.Log("主要buff"+ toDecode.mainBuff.returnMessage());
                    Debug.Log("还有："+ toDecode.cellType.ToString());
                    if (toDecode.mainBuff.returnTransFormTag(toDecode.cellType) != CellType.Cell_Null)
                    {
                        Debug.Log("难道这了？");
                        newData.cellType = toDecode.mainBuff.returnTransFormTag(toDecode.cellType);
                        newData.mainBuff = null;

                    }
                    else
                    {
                        newData.cellType = toDecode.cellType;
                    }
                }

            }
            else
            {
                newData.cellType = toDecode.cellType;//这个代表的是，这个格子上的细胞的类型
            }
            Debug.Log(newData.cellType.ToString());

            List<UnitBuff> toBuff = toDecode.sideBuffs;//两个辅助buff
            List<UnitBuff> usedBuff = new List<UnitBuff>();

            int cellNum = toDecode.cellNumber;
            foreach (GeneralBuff gbf in toBuff)
            {
                switch (gbf)
                {
                    case buff_NumUp:
                        cellNum += 5;
                        usedBuff.Add(gbf);
                        break;
                }
            }

            newData.cellNumber = cellNum;//这个单位格子上面的格子里的细胞数量，取决于单位里面的细胞数量

            foreach (UnitBuff gbf in usedBuff)
            {
                if (toBuff.Contains(gbf)) toBuff.Remove(gbf);
            }

            newData.sideBuffs = toBuff;
            return newData;
        }
        return null;
    }

    void EnterAttack()
    {
        //GameObject.Find("主要幕布").GetComponent<Canvas>().gameObject.GetComponent<MapSceneUiController>().useFee();
        GameStageControllerScript._instance.UseOneFee();
        List<UnitData> checkedData = new List<UnitData>();
        foreach (UnitData uD in SendMess.badDatas)
        {
            checkedData.Add(decodeBuff(uD));
        }

        List<UnitData> checkedGoodData = new List<UnitData>();
        foreach (UnitData uD in SendMess.goodDatas)
        {
            checkedGoodData.Add(decodeBuff(uD));
        }
        foreach (UnitData ud in checkedGoodData)
        {
            Debug.Log(ud.cellNumber);
        }
        ChangeSceneManager.Instance.SetMapUnitDatas(checkedGoodData, checkedData);//输送过去战斗必须的值
        //ChangeSceneManager.Instance.SetMapUnitDatas(TheGoodDatas, Data);
        ChangeSceneManager.Instance.SetOtherTileData(AllTileDatas);//存下去非战斗的格子信息
        ChangeSceneManager.Instance.SetAttackTileData(AttackDatas);//存下去战斗的格子的信息
        CSAScript._instance.StoreCurCardsForAfterBatter();
       // Debug.Log(AllTileDatas.Count);
        ChangeSceneManager.Instance.SwitchScene(0);
    }

    /// <summary>
    /// 负责在战斗之后，更新周围格子信息的
    /// </summary>
    /// <param name="Getmess">这个接受到的是战斗之后传过来的这个NeedData</param>
    public void ReData(List<UnitData> goodDatas, UnitData badData)
    {
        List<UnitData> TheGoodDatas = new();//这个是先把传回来的我方单位信息的存起来
        UnitData BadDatas = new UnitData();//敌人格子（也就是本格子）的信息存起来
        TheGoodDatas =goodDatas;
        BadDatas = badData;

        //这个方法说到底是上面的逆向操作吧，我在这里，再把周围格子的信息拿过来，然后根据六边形坐标信息来把处理过的值给一一赋过去

        //跟上面拿到的一样的方法，代码重复了，，，，封装一下？
        int range = 1;

        List<IMapUnit> NerTile = new List<IMapUnit>();//现在再定义一个数组来存储周围所有的格子
        NerTile = GetNerTile(HexPos, range);
        //好，现在我们拿到了周围的所有的格子

        //然后我们根据周围这些格子的位置信息来一一更新这些格子上面的Data值，主要还是更新上面的细胞数量
        //不应该再用循环了，我都知道六边形坐标了，我直接用字典找
        for (int i = 0; i < TheGoodDatas.Count; i++)
        {
           // HexManager._instance.allMapUnits[TheGoodDatas[i].flag].preUnit().num = TheGoodDatas[i].cellNumber;
            HexManager._instance.allMapUnits[TheGoodDatas[i].flag].preUnit().changeNum(TheGoodDatas[i].cellNumber);
            //上面这个太套娃了，意思就是，用拿回来的处理过的我方单位的格子坐标，找到对应的我方单位，然后把上面细胞的数量，更新成处理过的细胞数量的
        }
        ThisUnit.changeNum(BadDatas.cellNumber);
        //this.ThisUnit.num = BadDatas.cellNumber;


    }

    //好，我现在来封装下那个通过maptool里的方法拿到周围所有格子的东西
    /// <summary>
    /// 我想了下，这个方法还是是可以写在maptoolbox里面的
    /// </summary>
    /// <param name="_pos">这个是中心点的位置</param>
    /// <param name="ran">这个是检测半径</param>
    /// <returns></returns>
    List<IMapUnit> GetNerTile(Vector2 _pos, int ran)
    {
        //_pos = this.HexPos;
        List<Vector2> ner = new List<Vector2>();//这个List来存储自己周围距符合距离条件的格子坐标
        ner = MapToolBox.FindAllPointsForFight(_pos, ran, HexManager._instance.allMapUnits);
        List<IMapUnit> NerTile = new List<IMapUnit>();//这个是，用来存附近所有符合条件的信息的
        for (int i = 0; i < ner.Count; i++)
        {
            NerTile.Add(HexManager._instance.allMapUnits[ner[i]]);//根据格子坐标来拿到格子元素
        }

        Debug.Log("中心是："+ner + "，找到了:"+ner.Count);
        return NerTile;
        //这个方法的返回值就是一个存着周围所有符合距离条件的格子的方法
    }

    //确定点击事件
    private void OnMouseDown()
    {
        if (!isSelected && ThisTileType == UnitType.Enemy_Unit)//当确定选择了，且这个格子上面有敌方单位
        {
            SelectItem();

        }
        else
        {
            if (attack)
            {
                EnterAttack();
            }
            DeselectItem();
        }


    }
    //战斗时候选择单位的方法
    private void SelectItem()
    {
        isSelected = true;
        // 在这里可以实现选择状态的效果，例如改变颜色或显示选择框等
        //ChangeColor(1);
        canBig = true;
        Debug.Log("Item selected!");
        AttackTrigger();
    }
    private void DeselectItem()
    {
        isSelected = false;
        // 在这里可以实现取消选择状态的效果，恢复默认状态
        //ChangeColor(0);
        Debug.Log("Item deselected!");
    }



    //鼠标进入
    private void OnMouseEnter()
    {
        
        if (canBig)
        {
            transform.localScale += Vector3.one * 0.05f;//鼠标悬浮上去的时候，给瓦片一个稍微变大的效果
        }

        if (curMode == SelectMode.rangeMode)
        {
            manager.OnSelectShape(this);
        }

    }

    //鼠标离开
    private void OnMouseExit()
    {

        transform.localScale = PriSize;//鼠标只要离开，就强制把大小变成原始大小
        if (curMode == SelectMode.rangeMode)
        {
            manager.DownSelectShapeWhenMouseLeave(this);
        }

    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (oblay[1] == (oblay[1] | (1 << collision.gameObject.layer)))
        {
            
        }
    }
    //根据物体的层级来拿取上面物体的信息
    void CheckObstacle()
    {
        Collider2D[] test = new Collider2D[6];

        //test[0] = Physics2D.OverlapCircle(transform.position, SpriteRenderer.bounds.extents.x, oblayerMask);
        //test[1] = Physics2D.OverlapCircle(transform.position, SpriteRenderer.bounds.extents.x, oblayerMask1);
        //test[2] = Physics2D.OverlapCircle(transform.position, SpriteRenderer.bounds.extents.x, oblayerMask2);
        //test[3] = Physics2D.OverlapCircle(transform.position, SpriteRenderer.bounds.extents.x, oblayerMask3);
        //test[4] = Physics2D.OverlapCircle(transform.position, SpriteRenderer.bounds.extents.x, oblayerMask4);
        //test[5] = Physics2D.OverlapCircle(transform.position, SpriteRenderer.bounds.extents.x, oblayerMask5);
        for (int i = 0; i < oblay.Length; i++)
        {
            test[i] = Physics2D.OverlapCircle(transform.position, SpriteRenderer.bounds.extents.x, oblay[i]);
        }

        //for (int i = 0; i < test.Length; i++)
        //{
        //    Debug.Log(test[i]);
        //}


        int t = 0;
        for (int i = 0; i < test.Length; i++)
        {
            if (test[i]!=null)
            {
                unittag = i;
                t++;
            }
        }
        if (t>0)//只要有一种单位在，这个地方就走不了
        {
            canwalk = false;//这块地就不能走
        }
        else
        {
            canwalk = true;//这个时候能走
        }

        //if (colider!=null)
        //{
        //    unittag = 0;
        //}
        //if (colider1!=null)
        //{
        //    unittag = 1;
        //}

    }



    //这个涉及的东西太多，不想用了，改成下面那个方法
   //地块发亮和可以点击的绑定的
    public void HighLightTiles()
    {
        //通过看unitControl那发现已经对 canwalk进行检测了，这里关于能不能走，已经完全可以不用检测了
        //检测地块能不能亮的前提是能不能走，能走的地块才会亮
        if (canwalk)
        {
            SpriteRenderer.color = highlightcolor;//这里，改变为高亮色
            canBig = true;//打开了图片变大的开关
            canClick = true;//同时，打开了可以点击的开关
        }
        else
        {
            ResetTile();//点击之后恢复原来的方法
            //SpriteRenderer.color = PriColor;
            canClick = false;
        }

        //SpriteRenderer.color = highlightcolor;
    }

    //这里我重新写个方法，用来检测物体能不能走的点击移动的，为了和点击部署，点击索敌来分隔开
    public void ShowWhereCanWalk()
    {
        ChangeColor(1);//这里输入1，改成把颜色改成高亮
        canclickwalk = true;//这里才让瓦片变得通过点击让物体移动
        canBig = true;//这里的是，瓦片可以稍微变大的效果的开关

    }

    //写一个专门用来改变颜色的方法
    public void ChangeColor(int x)
    {

        if (x==0)
        {
            SpriteRenderer.color = SimpleColor;//输入0,就是普通颜色
        }
        else if (x==1)
        {
            SpriteRenderer.color = highlightcolor;//这里，改变为高亮色
        }
        else if (x == 2)
        {
            SpriteRenderer.color = InfectColor;//这里。是改变成染病的淡紫色
        }


    }

    //让地块恢复原来的颜色
    public void ResetTile()
    {
        SpriteRenderer.color = SimpleColor;
    }

    //点击砖块的时候，符合条件的话，就直接将这个瓦块的位置坐标传入到我方的移动方法上
    //private void OnMouseDown()
    //{
    //    if (canclickwalk)
    //    {
    //        GameManager.instance.selectUnit.Move( this.transform.position);
    //        ResetTile();//点击之后恢复原来的方法
    //        ChangeColor(0);
    //        canclickwalk = false;
    //    }

        
    //}

    //检测有没有触发点击事件，当颜色不是高亮的状态下，将点击和变大两个trigger进行还原
    void checkclick()
    {
        if (SpriteRenderer.color!=highlightcolor)
        {
            canClick = false;
            canBig = false;
           // Debug.Log("11111");
        }
    }

    public Vector3 thisPos()
    {
        //throw new System.NotImplementedException();
        return this.transform.position;
    }
    public Vector2 returnPos()
    {
        //throw new System.NotImplementedException();
        return HexPos;
    }

    public void changePos(Vector2 pos)
    {
        throw new System.NotImplementedException();
    }

    public HexManager returnManager()
    {
        throw new System.NotImplementedException();
    }

    public void changeManager(HexManager manager)
    {
        //throw new System.NotImplementedException();
    }

    public GameObject returnGmo()
    {
        return gameObject;
       // throw new System.NotImplementedException();
    }

    //表示这个单元暂时被选中
    public void Highlight()
    {
        //被选中的状态现在暂时就是 （1）鼠标放上去会有个变大的效果 （2）现在暂时先是变为高亮色
        //canBig = true;//变大就是打开这个开关，鼠标进入的时候自然就会变大
        //ChangeColor(1);//变为高亮色的参数就是1
        //throw new System.NotImplementedException();
        if(curMode == SelectMode.rangeMode)tempSelectObj.SetActive(true);
    }

    //取消暂时被选中
    public void DeHighlight()
    {
        //取消选中效果就是与上面相反 （1）取消放上去会变大的效果 （2）就是把颜色变为正常
        //canBig = false;
        //ChangeColor(0);
        tempSelectObj.SetActive(false);
        //throw new System.NotImplementedException();
    }

    //确定选择
    public void SetSelected()
    {
        fixedSelectedObj.SetActive(true);
        //ChangeColor(2);
        //throw new System.NotImplementedException();
    }

    //处于范围内
    public void SetInBound()
    {
        //canBig = true;
        //ChangeColor(1);
        //throw new System.NotImplementedException();
        inBoundObj.SetActive(true);
    }

    //取消处于范围内
    public void DownBound()
    {
        //canBig = true;
        //ChangeColor(1);
        inBoundObj.SetActive(false);
        //throw new System.NotImplementedException();
    }

    //取消确定选择
    public void DownSelection()
    {
        //ChangeColor(0);
        //curMode = SelectMode.zeroMode;
        //throw new System.NotImplementedException();
        fixedSelectedObj.SetActive(false);
    }

    //开启选择模式
    public void OnMouseSelect()
    {
        curMode = SelectMode.rangeMode;
        // throw new System.NotImplementedException();
        //canBig = true;
    }

    //取消选择模式
    public void DownMouseSelect()
    {
        curMode = SelectMode.zeroMode;
        //canBig = false;
        //throw new System.NotImplementedException();
    }

    public SelectMode returnMode()
    {
        return curMode;
        //throw new System.NotImplementedException();
    }

    public void AddBuff(UnitBuff buff)
    {
        if (buff == null) ;
        // Debug.Log("111111");
        bfMaintainer.enBuff(buff);

        //throw new System.NotImplementedException();
    }

    public unitControl preUnit()
    {
        return ThisUnit;
       
        //throw new System.NotImplementedException();
    }

    

    public void ChangeUnit(unitControl _this)
    {
        ThisUnit = _this;
        if (_this == null) 
        { ifcell = false; }
        else 
        { ifcell = true; }
        //throw new System.NotImplementedException();
    }

    public bool IfHaveFriendCell()
    {

        return ifcell;
        //throw new System.NotImplementedException();
    }

    public void setNoUnit()
    {
        ifcell = false;
       // throw new System.NotImplementedException();
    }

    public bool canBeMove()
    {
        return !ifcell;
        //throw new System.NotImplementedException();
    }

    public UnitData returnData()
    {
        return Data;
        //throw new System.NotImplementedException();
    }

    public UnitType returnTileType()
    {
        return ThisTileType;
        //throw new System.NotImplementedException();
    }

    public void DestroyCurUnit()
    {
        Debug.Log(ThisUnit+"被删除了");
        if(ThisUnit != null)Destroy(ThisUnit.gameObject);
        
        ThisUnit = null;
        ifcell = false;
        ThisTileType = UnitType.Solider_Unit;
    }
}
