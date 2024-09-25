using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cellbuff : MonoBehaviour, UnitBuff
{
    public bool ifTransform()
    {
        return true;
    }

    public buffTagForEffectType returnEffectCellType()
    {
        return buffTagForEffectType.enmey;
    }

    public string returnMessage(){ return "error"; }

    public CellType returnTransFormTag(CellType inputC) { return CellType.Cell_Null; }
}
