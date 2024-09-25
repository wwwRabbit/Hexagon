using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;




public class ChangeSceneManager : MonoBehaviour
{
    private static ChangeSceneManager instance;
    public static ChangeSceneManager Instance => instance;

    private NeedData needData;//这个是存储战斗方面信息的地方

    private List<IMapUnit> OtherData;//这个是存储非战斗区域的格子单位的信息
    private Dictionary<Vector2,cellUnit> OtherDataPreUnit;

    private List<IMapUnit> AttackData;//这个是存储战斗区域格子信息的

    public bool x = false;

    private void Awake()
    {
        if (instance != null)
        {
            // 如果已存在一个实例而且不是当前实例，则销毁当前GameObject
            if (this != instance)
            {
                Destroy(this.gameObject);
            }
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        needData = new NeedData();
      
    }

    //获取needdata的值
    public NeedData GetMapUnitDatas()
    {
        return needData;
    }

    //这个是用来更改needData的值的
    public void SetMapUnitDatas(List<UnitData> goodDatas, List<UnitData> badDatas)
    {
        x = true;
        needData.goodDatas = goodDatas;
        //needData.badData = badData;
        needData.badDatas = badDatas;
       // HexManager._instance.allMapUnits[badData.flag].ReData(goodDatas, badData);
    }

    //这个是获取所有其他非战斗格子信息的值的
    public List<IMapUnit> GetOtherTileData()
    {
        return OtherData;
    }

    //这个是存储所有非战斗信息的值的
    public void SetOtherTileData(List<IMapUnit> set)
    {
        OtherData = new();
        OtherData = set;

        //Debug.Log(OtherData.Count);
        OtherDataPreUnit = new Dictionary<Vector2, cellUnit>();
        foreach (IMapUnit mpu in set)
        {

            if (mpu.preUnit() == null) OtherDataPreUnit.Add(mpu.returnPos(),null);
            else
                OtherDataPreUnit.Add(mpu.returnPos(), new CellUnitInfoHolder(mpu.preUnit()));

        }
    }

    public Dictionary<Vector2, cellUnit> getPreUnits() { return OtherDataPreUnit; }

    //这个是获取到战斗格子的信息的值的
    public List<IMapUnit> GetAttackTileData()
    {
        return AttackData;
    }

    //这个是存储涉及战斗的格子的信息的
    public void SetAttackTileData(List<IMapUnit> set)
    {
        AttackData = new();
        AttackData = set;
    }



    public void SwitchScene(int index)
    {
        SceneManager.LoadScene(index);
    }

    //我在这里再写个方法用来更新战斗之后所有单位的值的
    public void GengXinAllTileData()
    {
        //先判断这个OtherData是不是空的
        if (OtherData!=null)
        {
            //IMapUnit
        }

    }



}
