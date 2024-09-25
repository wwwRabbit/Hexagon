using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cellUnitImplements : MonoBehaviour
{
}


public class cellUnitAsTag : cellUnit //这些全是友方，在卡牌生成里用
{
    public virtual cellTag returnTag()
    {
        return cellTag.friendly;
    }

    public bool compareUnit(cellUnit other)
    {
        return false;
    }

    public int returnNum()
    {
        return 0;
    }
    public int changeNum(int modifier)
    {
        return 0;
    }
    public int setNum(int toNum)
    {
        return 0;
    }

    //看清楚，这里这个前面是大写的C，定义在UnitData脚本里
    public virtual CellType returnSpcCelltag()
    {
        return CellType.Cell_ZXL;
    }
}

public class cellUnit_ZXL : cellUnitAsTag
{
    public override CellType returnSpcCelltag()
    {
        return CellType.Cell_ZXL;
    }
}

public class cellUnit_JS : cellUnitAsTag
{
    public override CellType returnSpcCelltag()
    {
        return CellType.Cell_JS;
    }
}

public class cellUnit_TC : cellUnitAsTag
{
    public override CellType returnSpcCelltag()
    {
        return CellType.Cell_TC;
    }
}

public class cellUnit_FD : cellUnitAsTag
{
    public override CellType returnSpcCelltag()
    {
        return CellType.Cell_FD;
    }
}

public class cellUnit_JXB : cellUnitAsTag
{
    public override CellType returnSpcCelltag()
    {
        return CellType.Cell_J;
    }
}

