using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TMPro;
using UnityEditor.ShaderGraph.Drawing;
using UnityEngine;


public class BatterData
{
    public int enemyNumber=0;
    public int soldierNumber=0;    

    public List<RangeSensor> soldierList = new List<RangeSensor>();
    public List<RangeSensor> enemyList = new List<RangeSensor>();


}

/// <summary>
/// 管理战斗相关内容
/// </summary>
public class BatterManager : MonoBehaviour
{
    //这里面去做实际的生成，因此需要记录当前对局生成的敌人和士兵数量，所属势力，类型，以及初始化他们的一些属性，可以做成一个类去包装

    //生成的时候可能会有卡顿，对资源进行异步加载包括场景，就在内外界面切换的时候，加载界面里进行。

    //生成的时候是随机生成，需要指定两个生成范围，然后在范围内随机生成

    //敌人或士兵类里，就是cellController里，需要有一个接口去更新数据。对应外部里的炎症效果或者异变效果

    //对于生成的士兵或敌人，在游戏开始的时候，可以去随机选择目标，在idle状态里启动

    //提供一个方法，让士兵，敌人，被控制细胞死亡的时候（可以放进缓存池内），可以更新此类里的士兵或敌人数量，以及清除引用

    //计时系统，计时结束，或者任意一方被歼灭游戏结束，跳转回外界面，内界面里的数据交互给外界面

    //战斗结束，检测友方细胞数量，超过阈值，发卡，次数有限制


    //日志：
    
    //4-19 实现战斗时与胜利失败计算
    //  出现时间显示内容无限叠加的问题
    //4-20 修复了无限叠加的问题，原因在于没有及时清空。显示以叠加的模式增加
    //  实现计时结束以及任意一方单位覆灭后 战斗结束
    //  实现基本的生成双方单位，并且分别随机生成在地图两侧，按照矩形范围随机生成
    //  暂时不能按照传入的类型去生成，只能生成基本类型
    //  测试出现生成位置超出地图边界的情况
    //4-28 更新了开始的单位生成函数
    //  暂时没有做完，需要改资源名称，进行一定的拼接来获取单位
    private static BatterManager instance;
    [HideInInspector] public static BatterManager Instance => instance;

    public GameObject failure;
    public GameObject endObj;
    private BatterData batterData;
    public BatterData BatterData => batterData;

    [Header("补偿机制阈值"), SerializeField] int compensationThreshold;
    [Header("补偿总次数"), SerializeField] int compensationNumber;
    [Header("剩余补偿次数"), SerializeField] int currentNumber;

    [Header("战斗持续时间"), SerializeField] double batterMaxTime;
    double currentBatterTime;
    StringBuilder displayTime = new StringBuilder();
    [SerializeField] bool batterEnd;

    public int nowSoldierNumber;
    public int nowEnemyNumber;


    public TextMeshProUGUI txt;

    [Header("上方生成点")] public Vector2 pointUp;
    [Header("下方生成点")] public Vector2 pointDown;
    [Header("水平生成范围")] public float horizontalRange;
    [Header("竖直生成范围")] public float verticalRange;
    [Header("地图边缘坐标")] public Vector2[] mapEdges = new Vector2[2];     //0 水平 1 竖直 
    public bool generateComplete = false;

    NeedData needData;
    NeedData retNeedData;

    private void Awake()
    {        
        instance = this;
        batterData = new BatterData();
    }

    private void Start()
    {
        batterEnd = false;
        currentBatterTime = batterMaxTime;

        EventCenter.GetInstance().AddEventListener<int>("SubCount", SubMemberCount);
        EventCenter.GetInstance().AddEventListener<Vector2>("SubFlag", Sub2Unit);


        needData = new NeedData();
        retNeedData = new NeedData();

        //从外侧地图界面获取战斗内生成细胞所需的相关数据
        needData = ChangeSceneManager.Instance.GetMapUnitDatas();

        retNeedData.badDatas = needData.badDatas;
        retNeedData.goodDatas = needData.goodDatas;
        Time.timeScale = 1;

        InitBatterManagerData();
    }

    private void Update()
    {

        //调试用
        //if (!MainTest.stopEnd)
        //{
        //    return;
        //}

        BatterEndJudgment();
    }

    /// <summary>
    /// 外部调用，还没写完。不过实际上可以让外部用反射调用
    /// </summary>
    public void InitBatterManagerData()
    {
        int goodN = 0;
        int badN = 0;
        foreach (UnitData item in needData.goodDatas)
        {
            Debug.Log("我没动？");
            goodN += item.cellNumber;
        }

        foreach (UnitData item in needData.badDatas)
        {
            badN += item.cellNumber;
        }


        //badN = needData.badData.cellNumber;

        batterData.enemyNumber = badN;
        batterData.soldierNumber = goodN;

        nowEnemyNumber = 0;
        nowSoldierNumber = 0;
        Debug.Log("enNum"+ batterData.enemyNumber);
        Debug.Log("solNum" + batterData.soldierNumber);
        GenerateAllUnit();
    }


    #region 计时计数模块 
    void BatterEndJudgment()
    {
        if (!batterEnd && generateComplete)
        {
            CountDown();
            BatterSummary();
        }
    }

    void CountDown()
    {
        currentBatterTime -= Time.deltaTime;
        if (currentBatterTime < 0)
        {
            currentBatterTime = 0;
        }

        int hours = (int)(currentBatterTime / 3600);
        int minutes = (int)((currentBatterTime % 3600) / 60);
        int seconds = (int)(currentBatterTime % 60);

        displayTime.Clear();

        //displayTime.AppendFormat("{0:0}:{1:0}:{2:00}", hours, minutes, seconds);
        if (hours > 0) displayTime.AppendFormat("{0:0}:", hours);
        if (minutes > 0) displayTime.AppendFormat("{0:0}:", minutes);
        displayTime.AppendFormat("{0:00}", seconds);

        //通知ui变化
        EventCenter.GetInstance().EventTrigger<StringBuilder>("CountDownUI", displayTime);

        //倒计时结束，统计双方单位数量
        //传出数据
        if (currentBatterTime == 0)
        {
            batterEnd = true;
            Time.timeScale = 0;
            //
            //TODO:
            //
            RefreshData();
        }
    }

    /// <summary>
    /// 计数
    /// </summary>
    void BatterSummary()
    {
        Debug.Log(nowEnemyNumber);
        Debug.Log(nowSoldierNumber);
        if (nowEnemyNumber <= 0 || nowSoldierNumber <= 0)
        {
            batterEnd = true;
            Time.timeScale = 0;
            //
            //TODO:
            //
            RefreshData();
        }
    }

    /// <summary>
    /// 更新外部数据
    /// </summary>
    void RefreshData()
    {
        //txt.gameObject.SetActive(true);
        endObj.SetActive(true);

        ChangeSceneManager.Instance.SetMapUnitDatas(retNeedData.goodDatas, retNeedData.badDatas);

        ChangeSceneManager.Instance.SwitchScene(1);
    }
    #endregion

    #region 单位数量管理模块

    /// <summary>
    /// 减少数量
    /// </summary>
    /// <param name="type">类型，1是友方，0是敌方</param>
    void SubMemberCount(int type)
    {
        if (type ==  0)
        {
            Debug.Log("该看见我了");
            nowEnemyNumber -= 1;
            nowEnemyNumber = Mathf.Clamp(nowEnemyNumber, 0, batterData.enemyNumber);
        }
        else if (type == 1)
        {
            nowSoldierNumber -= 1;
            nowSoldierNumber = Mathf.Clamp(nowSoldierNumber, 0, batterData.soldierNumber);
        }
    }

    void Sub2Unit(Vector2 flag)
    {
        foreach (UnitData item in retNeedData.goodDatas)
        {
            if (item.flag == flag)
            {
                item.cellNumber -= 1;
                Debug.Log("死了一个细胞！现在是：" + item.cellNumber);
            }
        }
        foreach (UnitData item in retNeedData.badDatas)
        {
            if (item.flag==flag)
            {
                item.cellNumber -= 1;
                Debug.Log("死了一个病毒！现在是：" + item.cellNumber);
            }
        }


        //if (retNeedData.badData.flag == flag)
        //{
        //    retNeedData.badData.cellNumber -= 1;
        //}
    }

    #endregion

    #region 单位生成模块

    void GenerateAllUnit()
    {
        //for(int i = 0; i < batterData.enemyNumber / 2; i++)
        //{
        //    GenerateUnit(pointUp, "BaseCell2", 0);
        //}
        //for (int i = 0; i < batterData.soldierNumber / 2; i++)
        //{
        //    GenerateUnit(pointDown, "BaseCell", 1);
        //}
        //for (int i = 0; i < batterData.enemyNumber / 2; i++)
        //{
        //    GenerateUnit(pointUp, "BaseCell2_Arrow", 0);
        //}
        //for (int i = 0; i < batterData.soldierNumber / 2; i++)
        //{
        //    GenerateUnit(pointDown, "BaseCell_Arrow", 1);
        //}

        // Cell_种类_好坏

        foreach (UnitData item in needData.goodDatas)
        {
            for(int i = 0; i < item.cellNumber; ++i)
            {
                Debug.Log("生产了一个好细胞"+ BuildCellNameString(item.cellType.ToString(), item.unitType.ToString()));
                GenerateBelowMap(BuildCellNameString(item.cellType.ToString(), item.unitType.ToString()), false, item.flag);
            }
        }
        foreach (UnitData item in needData.badDatas)
        {
            for (int i = 0; i < item.cellNumber; i++)
            {
                Debug.Log("生产了一个坏细胞");
                GenerateAboveMap(BuildCellNameString(item.cellType.ToString(), item.unitType.ToString()), false, item.flag);
            }
        }


        //for(int i = 0; i < needData.badData.cellNumber; ++i)
        //{
        //    GenerateAboveMap(BuildCellNameString(needData.badData.cellType.ToString(), needData.badData.unitType.ToString()), false, needData.badData.flag);
        //}
    }

    string BuildCellNameString(string type, string cap)
    {
        return type + "_" + cap;
    }

    public void GenerateAboveMap(string path, bool haveFather = false, Vector2 flag = default)//在上方生成敌人单位
    {
        GenerateUnit(pointUp, path, 0, haveFather, flag);
        Debug.Log("hereI Add a js");
    }

    public void GenerateBelowMap(string path, bool haveFather = false, Vector2 flag = default)//在下方生成友方单位
    {
        GenerateUnit(pointDown, path, 1, haveFather, flag);
    }

    #region 肥大细胞死亡时原地随机生成新细胞
    public void GenerateEnmForFD(Vector3 deadPosition, string path, bool haveFather = false, Vector2 flag = default)
    {
        Vector2 position = new Vector2(deadPosition.x, deadPosition.y);
        List<Vector2> positions = GenerateRandomPositions(position, 2, 1f);
        foreach(Vector2 pos in positions) 
        {
            GenerateUnit(pos, path, 0, haveFather, flag);
        }
    }
    public void GenerateSolForFD(Vector3 deadPosition, string path, bool haveFather = false, Vector2 flag = default)
    {
        Vector2 position = new Vector2(deadPosition.x, deadPosition.y);
        List<Vector2> positions = GenerateRandomPositions(position, 2, 1f);
        foreach (Vector2 pos in positions)
        {
            GenerateUnit(pos, path, 1, haveFather, flag);
        }
    }

    List<Vector2> GenerateRandomPositions(Vector2 center, int count, float radius)
    {
        List<Vector2> positions = new List<Vector2>();

        for (int i = 0; i < count; i++)
        {
            // 随机角度，从0到360度
            float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad; // 转换为弧度
            // 随机半径，从0到2
            float distance = Random.Range(0f, radius);

            // 计算新位置
            Vector2 newPos = center + new Vector2(Mathf.Cos(angle) * distance, Mathf.Sin(angle) * distance);
            positions.Add(newPos);
        }

        return positions;
    }
    #endregion

    /// <summary>
    /// 生成单个单位
    /// </summary>
    /// <param name="basePos">基础坐标点</param>
    /// <param name="path">物体资源路径</param>
    /// <param name="type">单位类型</param>
    void GenerateUnit(Vector2 basePos, string path, int type, bool haveFather = false, Vector2 flag = default)
    {
        ResMgr.GetInstance().LoadAsync<GameObject>(path, (obj) => {
            obj.GetComponent<CellController>().haveFather = haveFather;

            //计算生成点距离地图边缘的水平距离
            float tmpHor = Mathf.Abs(mapEdges[0].x - basePos.x);
            float horReallyRange = tmpHor >= horizontalRange / 2 ? horizontalRange / 2: tmpHor;

            //计算生成点距离地图边缘的竖直距离
            float tmpVer = Mathf.Abs(mapEdges[1].y - Mathf.Abs(basePos.y));
            float verReallyRange = tmpVer >= verticalRange / 2 ? verticalRange / 2: tmpVer;

            Vector2 horOffset = new Vector2(Random.Range(0, horReallyRange + 1) * (Random.Range(0, 2) * 2 - 1), 
                Random.Range(0, verReallyRange + 1) * (Random.Range(0, 2) * 2 - 1));
            Vector2 pos = basePos + horOffset;
            obj.transform.position = pos;

            obj.transform.localScale = Vector3.one;
            obj.transform.rotation = Quaternion.identity;

            if (flag != default)
            {
                obj.GetComponent<CellController>().flag = flag;
            }

            if (type == 0)
            {
                nowEnemyNumber++;
                obj.GetComponent<CellController>().isEnemy = true;
                batterData.enemyList.Add(obj.transform.Find("RangeSensor").GetComponent<RangeSensor>());
            }
            else if (type == 1)
            {
                nowSoldierNumber++;
                obj.GetComponent<CellController>().isEnemy = false;
                batterData.soldierList.Add(obj.transform.Find("RangeSensor").GetComponent<RangeSensor>());
            }

            if (nowEnemyNumber == batterData.enemyNumber && nowSoldierNumber == batterData.soldierNumber)
            {
                CanStartBatter();
            }
        });
    }

    #endregion

    public void CanStartBatter()
    {
        generateComplete = true;
        StartBatter();
    }

    void StartBatter()
    {
        foreach (var item in batterData.enemyList)
        {
            item.openDetect = true;
            //testEnemy++;
        }
        foreach (var item in batterData.soldierList)
        {
            item.openDetect = true;
            //testSoldier++;
        }
        //batterData.enemyNumber=testEnemy;
        //batterData.soldierNumber=testSoldier;
    }

    private void OnDisable()
    {
        EventCenter.GetInstance().RemoveEventListener<int>("SubCount", SubMemberCount);
        EventCenter.GetInstance().RemoveEventListener<Vector2>("SubFlag", Sub2Unit);
    }

}
