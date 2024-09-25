using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpDataFromAttack : MonoBehaviour
{
    //这个方法放在我们的大地图场景里面。负责每次从战斗回来之后更新数据
    //先定义两个存储数据的地方

    private NeedData GetNeedData = new NeedData();
    private List<IMapUnit> AllTileData = new List<IMapUnit>();
    private List<IMapUnit> AttacktileData = new List<IMapUnit>();

    // Start is called before the first frame update
    void Start()
    {

        if (ChangeSceneManager.Instance.GetAttackTileData()!=null&& ChangeSceneManager.Instance.GetMapUnitDatas() != null)
        {
            AttacktileData = ChangeSceneManager.Instance.GetAttackTileData();
            GetNeedData = ChangeSceneManager.Instance.GetMapUnitDatas();
            ReturnAttack(GetNeedData, AttacktileData);

        }

        if (ChangeSceneManager.Instance.GetOtherTileData()!=null)
        {
            AllTileData = ChangeSceneManager.Instance.GetOtherTileData();
            ReturnUnAttack(AllTileData);
            // Debug.Log(ChangeSceneManager.Instance.GetOtherTileData().Count);
            //Debug.Log("1111111111");
            CSAScript._instance.CopyCardsAfterBatter();
            InterActController._instance.RefreshCamera();
            GameStageControllerScript._instance.UpdateFee();
        }

        //在上面我们拿到了一个是处理之后的战斗部分的数据，一个是非战斗部分我们存下来的数据
        Time.timeScale = 1;
        
    }

    
    //下面我们定义俩方法把这两部分的值分别赋回去
    //这个方法啊来还原非战斗场景的数据
    void ReturnUnAttack(List<IMapUnit> _tiles)
    {
        List<Vector2> TilePos = new List<Vector2>();//这个来存这些所有的格子的坐标信息
        for (int i = 0; i < _tiles.Count; i++)
        {
            TilePos.Add(_tiles[i].returnPos());
        }
        //下面我们根据这些六边形坐标，来给所有的格子赋上上面的单位
        Dictionary<Vector2, IMapUnit> allTile = new Dictionary<Vector2, IMapUnit>();
        HexManager._instance.FindAllTileObjects();
        allTile = HexManager._instance.allMapUnits;//把这个先拿回来，一会用的时候方便

        /*
        foreach (cellUnit mu in ChangeSceneManager.Instance.getPreUnits())
        {
            Debug.Log("这是null吗？" + (mu == null));
            //Debug.Log("unit是null吗？" + (mu.preUnit() == null));
        }*/
        

        
        for (int i = 0; i < TilePos.Count; i++)
        {
            if (allTile[TilePos[i]].preUnit() == null)
            {
                if (ChangeSceneManager.Instance.getPreUnits()[TilePos[i]] != null && ChangeSceneManager.Instance.getPreUnits()[TilePos[i]].returnNum() != 0)
                {
                    //按照之前老格子上面的标签，来地图上面再次生成
                    GameObject Reobject = CellFactory.instance.ProducePrefb(allTile[TilePos[i]], ChangeSceneManager.Instance.getPreUnits()[TilePos[i]].returnSpcCelltag());
                    unitControl n = Reobject.GetComponent<unitControl>();//拿到细胞上面的脚本
                    allTile[TilePos[i]].ChangeUnit(n);//把新生成的脚本给格子赋上
                                                      //n.num = allTile[TilePos[i]].preUnit().num;//数量也还愿一下
                    //n.num = ChangeSceneManager.Instance.getPreUnits()[TilePos[i]].returnNum();
                    n.setNum(ChangeSceneManager.Instance.getPreUnits()[TilePos[i]].returnNum());
                }

            }
            else
            {
                if (ChangeSceneManager.Instance.getPreUnits()[TilePos[i]] == null)
                {
                    allTile[TilePos[i]].DestroyCurUnit();
                    //Debug.Log("应该删除" + TilePos[i]);
                }
                else
                {
                    allTile[TilePos[i]].preUnit().setNum(ChangeSceneManager.Instance.getPreUnits()[TilePos[i]].returnNum());
                }
            }
        }
    }

    //这个方法用来还原经过战斗处理过的那些格子的信息
    void ReturnAttack(NeedData _needData,List<IMapUnit> _tiles)
    {
        HexManager._instance.FindAllTileObjects();
        
        
        //UnitData enemy = new UnitData();
        //enemy = _needData.badData;//地方信息格子
        List<UnitData> Enemy = new List<UnitData>();
        Enemy = _needData.badDatas;//这个是参战的地方格子信息

        

        Debug.Log("开始汇报敌人信息");
        string ansToShout = "";
        foreach (UnitData ud in Enemy)
        {
            ansToShout += ud.flag;
            ansToShout += " ";
        }
        Debug.Log(ansToShout);
        foreach (UnitData ud in Enemy)
        {
            Debug.Log("格子是：" +ud.flag);
            Debug.Log("种类是："+ ud.cellType);
            Debug.Log("数量是："+ ud.cellNumber);

        }

        List<UnitData> friend = new List<UnitData>();
        friend = _needData.goodDatas;//附近参战的我方信息格子
        
        Debug.Log("开始汇报我方信息");
        Debug.Log(friend.Count);
        string ansToShoutAgain = "";
        foreach (UnitData ud in friend)
        {
            ansToShoutAgain += ud.flag;
            ansToShoutAgain += " ";
        }
        Debug.Log(ansToShoutAgain);
        foreach (UnitData ud in friend)
        {
            Debug.Log("格子是：" + ud.flag);
            Debug.Log("种类是：" + ud.cellType);
            Debug.Log("数量是：" + ud.cellNumber);

        }

        Dictionary<Vector2, IMapUnit> Fir = new Dictionary<Vector2, IMapUnit>();
        for (int i = 0; i < _tiles.Count; i++)
        {
            Fir.Add(_tiles[i].returnPos(), HexManager._instance.allMapUnits[_tiles[i].returnPos()]);
        }

        //Dictionary<Vector2, IMapUnit> Enem = new Dictionary<Vector2, IMapUnit>();
        //for (int i = 0; i < _tiles.Count; i++)
        //{

        //}

        //有了这么一个坐标和格子一一对应的dic,我们就根据传回来的信息来对这个进行修改

        for (int i = 0; i < friend.Count; i++)
        {
            if (Fir[friend[i].flag].preUnit()==null)
            {
                //上面单位是空而且战斗之后还有剩余的就重新生成
                if (friend[i].cellNumber != 0)
                {
                    Debug.Log(friend[i].flag);
                    IMapUnit a = Fir[friend[i].flag];//这个格子，就是存有我方信息单位的格子
                                                     //现在，给它重新按照信心生成我们的单位
                    GameObject NewF = CellFactory.instance.ProducePrefb(a, friend[i].cellType);
                    unitControl n = NewF.GetComponent<unitControl>();
                    a.ChangeUnit(n);
                    //n.num = friend[i].cellNumber;
                    n.setNum(friend[i].cellNumber);
                }
            }
            else
            {
                //不是空的就先赋值，如果赋的值是0说明打没了，销毁掉
                Fir[friend[i].flag].preUnit().setNum(friend[i].cellNumber);
                if (friend[i].cellNumber<=0)
                {
                    Destroy(Fir[friend[i].flag].preUnit());
                    Fir[friend[i].flag].DestroyCurUnit(); //changeunit（null）
                }
            }

            /*
            Fir[friend[i].flag].AddBuff(friend[i].mainBuff);
            foreach (UnitBuff bf in friend[i].sideBuffs)
            {
                Fir[friend[i].flag].AddBuff(bf);
            }*/
        }

        for (int i = 0; i < Enemy.Count; i++)
        {
            if (Fir[Enemy[i].flag].preUnit() == null)
            {
                if (Enemy[i].cellNumber != 0)
                {
                    Debug.Log(Enemy[i].flag);
                    IMapUnit a = Fir[Enemy[i].flag];

                    GameObject NewF = CellFactory.instance.ProducePrefb(a, Enemy[i].cellType);
                    unitControl n = NewF.GetComponent<unitControl>();
                    a.ChangeUnit(n);
                    //n.num = Enemy[i].cellNumber;
                    n.setNum(Enemy[i].cellNumber);
                }
            }
            else
            {
                Fir[Enemy[i].flag].preUnit().setNum( Enemy[i].cellNumber);
                Debug.Log("Just here?:"+ Fir[Enemy[i].flag].preUnit().num);
                if (Enemy[i].cellNumber <= 0)
                {
                    Debug.Log("SHould be here");
                    Destroy(Fir[Enemy[i].flag].preUnit());
                    Fir[Enemy[i].flag].DestroyCurUnit();
                }

            }
            /*
            Fir[Enemy[i].flag].AddBuff(Enemy[i].mainBuff);
            foreach (UnitBuff bf in Enemy[i].sideBuffs)
            {
                Fir[Enemy[i].flag].AddBuff(bf);
            }*/


        }








    }
}
