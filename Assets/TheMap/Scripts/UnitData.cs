using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class NeedData
{
  //  public UnitData badData = new();

    public List<UnitData> badDatas = new();

    public List<UnitData> goodDatas = new();
}


//细胞种类的标签写在这里了
public enum CellType
{
    Cell_ZXL = 0,
    Cell_TC = 1,
    Cell_JS = 2,
    Cell_ZXL_M = 3,
    Cell_TC_M = 4,
    Cell_JS_M = 5,
    Cell_FD = 6,
    Cell_FD_M = 7,
    Cell_J = 8,
    Cell_J_M = 9,
    Cell_Null = 10
}

public enum UnitType
{
    Solider_Unit = 0,
    Enemy_Unit = 1
}

public class UnitData
{
    public Vector2 flag;

    //细胞的数量
    public int cellNumber;
    //
    //TODO:BUFF
    //
    //格子类型
    public UnitType unitType;
    //细胞种类
    public CellType cellType;

    public List<UnitBuff> sideBuffs;
    public UnitBuff mainBuff;
}
