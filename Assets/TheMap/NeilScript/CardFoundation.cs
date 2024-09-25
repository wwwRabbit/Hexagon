using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.VFX;

public class CardFoundation : MonoBehaviour
{

    public static GeneralCard CardSpecificCopy(GeneralCard toCopy)
    {
        switch (toCopy)
        {
            case NormalMove_Level01_01:
                return new NormalMove_Level01_01(toCopy as NormalMove_Level01_01);

            case NormalMove_Level01_02:
                return new NormalMove_Level01_02(toCopy as NormalMove_Level01_02);

            case NormalAdd_ZXL:
                return new NormalAdd_ZXL(toCopy as NormalAdd_ZXL);

            case NormalAdd_TC:
                return new NormalAdd_TC(toCopy as NormalAdd_TC);

            case NormalAdd_JS:
                return new NormalAdd_JS(toCopy as NormalAdd_JS);

            case DoubleBuffer:
                return new DoubleBuffer(toCopy as DoubleBuffer);

            case ExtraVitaminBooster:
                return new ExtraVitaminBooster(toCopy as ExtraVitaminBooster);

            case NormalMove_Level02_01:
                return new NormalMove_Level02_01(toCopy as NormalMove_Level02_01);

            case NormalMove_Level02_02:
                return new NormalMove_Level02_02(toCopy as NormalMove_Level02_02);

            case NormalAdd_FD:
                return new NormalAdd_FD(toCopy as NormalAdd_FD);

            case ImmuneCheckLocker:
                return new ImmuneCheckLocker(toCopy as ImmuneCheckLocker);

            case FKAllergySignal:
                return new FKAllergySignal(toCopy as FKAllergySignal);

            case TargetTransport:
                return new TargetTransport(toCopy as TargetTransport);

            case OxidativeBurst:
                return new OxidativeBurst(toCopy as OxidativeBurst);

            case BloodActivator:
                return new BloodActivator(toCopy as BloodActivator);

            case NormalAdd_JXB:
                return new NormalAdd_JXB(toCopy as NormalAdd_JXB);

            case TCGeneEditing:
                return new TCGeneEditing(toCopy as TCGeneEditing);

            case ZXLActivator:
                return new ZXLActivator(toCopy as ZXLActivator);

            case BCellForcedSplit:
                return new BCellForcedSplit(toCopy as BCellForcedSplit);

            case AntiBodyModifier:
                return new AntiBodyModifier(toCopy as AntiBodyModifier);

            case CellBindingCapsule:
                return new CellBindingCapsule(toCopy as CellBindingCapsule);

            default:
                return null;
        }
    }
}

//卡牌的使用逻辑
/*
 * 卡牌离开区域 -> CheckFee = true -> 触发TriggerMainFunction并且消耗Fee
 *      卡牌返回区域 ->触发CanelTrigger() 并且返回fee
 *      鼠标松开 -> 没有问题继续执行
 *      
 *      外部执行结束 ->触发CorrespondingEffect()
 */

//默认非零时，用一次，费用 = 0；
public class GeneralCard
{
    internal HexManager manager;
    internal ICardObserver observer;

    public GameObject relatedObj;

    internal bool ifTempCard = false;
    internal int canBeUsed = 1;
    internal int cost = 0;

    public GeneralCard(HexManager m, ICardObserver ob)
    {
        manager = m;
        observer = ob;
    }

    // 深拷贝构造函数
    public GeneralCard(GeneralCard other)
    {
        manager = other.manager; // 这里保持引用
        observer = other.observer; // 这里保持引用

        relatedObj = other.relatedObj; // 这里通常保持引用，因为GameObject不应该被复制

        ifTempCard = other.ifTempCard;
        canBeUsed = other.canBeUsed;
        cost = other.cost;

        // 如果还有其他的对象属性需要深拷贝，你需要在这里处理
    }

    public virtual bool CheckFee(int curFee)
    {
        return true;
    }

    public virtual int UseFee(int curFee)
    {
        Debug.Log(Mathf.Clamp(curFee - cost, 0, 4));
        return Mathf.Clamp(curFee-cost,0,4);
    }

    public virtual void UseConfirmed() //决定使用了
    {
        Debug.Log("Confirm了");
        
        observer.OnCardPlayed(this);
        
    }

    public virtual void UseNotice() { observer.OnCardNotice(); }

    public virtual int ReturnFee(int curFee)
    {
        return Mathf.Clamp(curFee + cost, 0, 4);
    }

    public virtual void TriggerMainFunction()
    {
        //
    }

    public virtual void CancelTrigger()
    {
        Debug.Log("Cancel Effect");
        manager.StartMode(FunctionMode.zero);
    }

    public virtual void CorrespondingEffect(HexManager.SelectionRangeMaintainer rM)
    {

    }

    public bool ifCardTemp() { return ifTempCard; }

    ICardUi selfUI;
    public void enUi(ICardUi cui)
    {
        selfUI = cui;
    }

    public ICardUi returnCUI() { return selfUI; }

    public virtual bool ifConfirmHG() { return true; }
}



public class NormalMove : GeneralCard
{
    internal List<Vector2> moveShape = new List<Vector2>();
    internal int range;
    public NormalMove(HexManager m, ICardObserver ob, List<Vector2> mShape, int r = 2) : base(m, ob)
    {
        moveShape = mShape;
        range = r;
    }

    public NormalMove(HexManager m, ICardObserver ob) : base(m, ob)
    {
        moveShape.Add(new Vector2(0, 0));
        moveShape.Add(new Vector2(1, -1));
    }

    public override void TriggerMainFunction()
    {
        base.TriggerMainFunction();
        manager.TriggerMovement(range, moveShape);
    }

}

public class NormalGenerate: GeneralCard
{
    internal cellUnit cellToCreate;
    internal int cellNum;
    public NormalGenerate(HexManager m, ICardObserver ob, cellUnit cell, int cellnum):base(m,ob)
    {
        cellToCreate = cell;
        cellNum = cellnum;
    }

    public override void TriggerMainFunction()
    {
        base.TriggerMainFunction();
        manager.TriggerAddCell(cellToCreate,cellNum);
    }
}

public interface ICardObserver //用在Interaction controller上，用于聆听卡牌的使用
{
    void OnCardPlayed(GeneralCard card);
    void OnCardNotice(); //告诉状态更新了
}
#region 第一关相关卡牌

//第一张零时移动牌.移动2格，范围2
public class NormalMove_Level01_01 : NormalMove
{
    public NormalMove_Level01_01(HexManager m, ICardObserver ob) : base(m, ob, new List<Vector2>(), 2)
    {
        moveShape.Add(new Vector2(0,0));
        moveShape.Add(new Vector2(1, -1));
        ifTempCard = true;
    }

    public NormalMove_Level01_01(NormalMove_Level01_01 otherC) : base(otherC.manager, otherC.observer, new List<Vector2>(), 2)
    {
        moveShape.Add(new Vector2(0, 0));
        moveShape.Add(new Vector2(1, -1));
        ifTempCard = true;
    }
}

//第二张零时移动牌。移动一格，范围3
public class NormalMove_Level01_02 : NormalMove
{
    public NormalMove_Level01_02(HexManager m, ICardObserver ob) : base(m, ob, new List<Vector2>(), 3)
    {
        moveShape.Add(new Vector2(0, 0));
        ifTempCard = true;
    }
    public NormalMove_Level01_02(NormalMove_Level01_02 otherC) : base(otherC.manager, otherC.observer, new List<Vector2>(), 3)
    {
        moveShape.Add(new Vector2(0, 0));
        ifTempCard = true;
    }
}

//零时生成的3张细胞卡
public class NormalAdd_ZXL : NormalGenerate
{
    public NormalAdd_ZXL(HexManager m, ICardObserver ob, int n):base(m,ob, new cellUnit_ZXL(), n)
    {
        cost = 0;
        ifTempCard = true;
    }

    public NormalAdd_ZXL(NormalAdd_ZXL otherC): base(otherC.manager, otherC.observer, new cellUnit_ZXL(), otherC.cellNum)
    {
        cost = 0;
        ifTempCard = true;
    }
}

public class NormalAdd_TC : NormalGenerate
{
    public NormalAdd_TC(HexManager m, ICardObserver ob, int n) : base(m, ob, new cellUnit_TC(), n)
    {
        cost = 0;
        ifTempCard = true;
    }

    public NormalAdd_TC(NormalAdd_TC otherC) : base(otherC.manager, otherC.observer, new cellUnit_TC(), otherC.cellNum)
    {
        cost = 0;
        ifTempCard = true;
    }
}

public class NormalAdd_JS : NormalGenerate
{
    public NormalAdd_JS(HexManager m, ICardObserver ob, int n) : base(m, ob, new cellUnit_JS(), n)
    {
        cost = 0;
        ifTempCard = true;
    }

    public NormalAdd_JS(NormalAdd_JS otherC) : base(otherC.manager, otherC.observer, new cellUnit_JS(), otherC.cellNum)
    {
        cost = 0;
        ifTempCard = true;
    }
}



public class DoubleBuffer : GeneralCard
{
    List<Vector2> totalRange = new List<Vector2>();
    List<Vector2> sideEffectRange = new List<Vector2>();
    UnitBuff buffMain = new buff_ZXThrow(buffTagForEffectType.friendly);
    UnitBuff buffSide =  new buff_TCLookHealth(buffTagForEffectType.both);

    public DoubleBuffer(HexManager m, ICardObserver ob) : base(m, ob)
    {
        totalRange.Add(new Vector2(0, 0));
        totalRange.Add(new Vector2( -1,1));
        totalRange.Add(new Vector2(1, 0));
        sideEffectRange.Add(new Vector2(-1, 1));
        canBeUsed = 3;
        cost = 1;
    }

    public DoubleBuffer(DoubleBuffer otherC) : base(otherC.manager, otherC.observer)
    {
        totalRange.Add(new Vector2(0, 0));
        totalRange.Add(new Vector2(-1, 1));
        totalRange.Add(new Vector2(1, 0));
        sideEffectRange.Add(new Vector2(-1, 1));
        canBeUsed = 3;
        cost = 1;
    }



    public override void TriggerMainFunction()
    {
        Debug.Log("Do Main Effect");
        manager.TriggerAddEffect(buffMain, totalRange);
        GameObject.Find("主要幕布").GetComponent<MapSceneUiController>().ModifyHGAppearance(true, Moniter.gusui);
    }

    public override void CancelTrigger()
    {
        base.CancelTrigger();
        GameObject.Find("主要幕布").GetComponent<MapSceneUiController>().ModifyHGAppearance(false);
    }

    public override void CorrespondingEffect(HexManager.SelectionRangeMaintainer rM)
    {
        Debug.Log("Do Side Effect");

        foreach (Vector2 v in sideEffectRange)
        {
            if (manager.allMapUnits.ContainsKey(rM.curPivot + v))
            {
                manager.SwitchEffect(manager.allMapUnits[rM.curPivot + v], buffMain, buffSide);
            }
        }
    }

    public override bool ifConfirmHG()
    {
        return true;
    }

}

public class ExtraVitaminBooster : GeneralCard
{
    List<Vector2> totalRange = new List<Vector2>();
    List<Vector2> sideEffectRange = new List<Vector2>();
    UnitBuff buffMain = new buff_HealthUp(buffTagForEffectType.friendly);
    UnitBuff buffSide = new buff_EnAddRecruitable();

    public ExtraVitaminBooster(HexManager m, ICardObserver ob) : base(m, ob)
    {
        totalRange.Add(new Vector2(0, 0));
        totalRange.Add(new Vector2(1, -1));
        totalRange.Add(new Vector2(-1, 1));
        sideEffectRange.Add(new Vector2(0, 0));
        canBeUsed = 2;
        cost = 1;
    }

    public ExtraVitaminBooster(ExtraVitaminBooster otherC) : base(otherC.manager, otherC.observer)
    {
        totalRange.Add(new Vector2(0, 0));
        totalRange.Add(new Vector2(1, -1));
        totalRange.Add(new Vector2(-1, 1));
        sideEffectRange.Add(new Vector2(0, 0));
        canBeUsed = 2;
        cost = 1;
    }

    public override void TriggerMainFunction()
    {
        Debug.Log("Do Main Effect");
        manager.TriggerAddEffect(buffMain, totalRange);
    }

    public override void CorrespondingEffect(HexManager.SelectionRangeMaintainer rM)
    {
        Debug.Log("Do Side Effect");

        foreach (Vector2 v in sideEffectRange)
        {
            if (manager.allMapUnits.ContainsKey(rM.curPivot + v))
            {
                manager.SwitchEffect(manager.allMapUnits[rM.curPivot + v], buffMain, buffSide);
            }
        }
    }


}

#endregion

#region 第二关相关卡牌
//第一张零时移动牌.
public class NormalMove_Level02_01 : NormalMove
{

    //未完成
    public NormalMove_Level02_01(HexManager m, ICardObserver ob) : base(m, ob, new List<Vector2>(), 3)
    {
        moveShape.Add(new Vector2(0, 0));
        moveShape.Add(new Vector2(1, -1));
        ifTempCard = true;
    }

    public NormalMove_Level02_01(NormalMove_Level02_01 otherC) : base(otherC.manager, otherC.observer, new List<Vector2>(), 3)
    {
        moveShape.Add(new Vector2(0, 0));
        moveShape.Add(new Vector2(1, -1));
        ifTempCard = true;
    }
}

//第二张零时移动牌，
public class NormalMove_Level02_02 : NormalMove
{

    //未完成
    public NormalMove_Level02_02(HexManager m, ICardObserver ob) : base(m, ob, new List<Vector2>(), 2)
    {
        moveShape.Add(new Vector2(0, 0));
        moveShape.Add(new Vector2(0, -1));
        ifTempCard = true;
    }
    public NormalMove_Level02_02(NormalMove_Level02_02 otherC) : base(otherC.manager, otherC.observer, new List<Vector2>(), 2)
    {
        moveShape.Add(new Vector2(0, 0));
        moveShape.Add(new Vector2(0, -1));
        ifTempCard = true;
    }
}

//零时生成的1张细胞卡
public class NormalAdd_FD : NormalGenerate
{
    public NormalAdd_FD(HexManager m, ICardObserver ob, int n) : base(m, ob, new cellUnit_FD(), n)
    {
        cost = 0;
        ifTempCard = true;
    }

    public NormalAdd_FD(NormalAdd_FD otherC) : base(otherC.manager, otherC.observer, new cellUnit_FD(), otherC.cellNum)
    {
        cost = 0;
        ifTempCard = true;
    }
}


public class ImmuneCheckLocker : GeneralCard //免疫检查抑制剂
{
    List<Vector2> totalRange = new List<Vector2>();
    List<Vector2> sideEffectRange = new List<Vector2>();
    List<Vector2> addRange = new List<Vector2>();
    UnitBuff buffMain = new buff_TCPenetrate(buffTagForEffectType.friendly);
    UnitBuff buffSide = new buff_HealthUp(buffTagForEffectType.enmey);
    cellUnit_TC toAdd = new cellUnit_TC();

    public ImmuneCheckLocker(HexManager m, ICardObserver ob) : base(m, ob)
    {
        totalRange.Add(new Vector2(0, 0));
        totalRange.Add(new Vector2(0, -1));
        totalRange.Add(new Vector2(1, 0));
        totalRange.Add(new Vector2(-1, 1));
        totalRange.Add(new Vector2(0, 1));
        sideEffectRange.Add(new Vector2(0, 0));
        sideEffectRange.Add(new Vector2(0, -1));
        addRange.Add(new Vector2(-1, 1));
        addRange.Add(new Vector2(1, 0));
        addRange.Add(new Vector2(0, 1));
        canBeUsed = 2;
        cost = 2;
    }

    public ImmuneCheckLocker(ImmuneCheckLocker otherC) : base(otherC.manager, otherC.observer)
    {
        totalRange.Add(new Vector2(0, 0));
        totalRange.Add(new Vector2(0, -1));
        totalRange.Add(new Vector2(1, 0));
        totalRange.Add(new Vector2(-1, 1));
        totalRange.Add(new Vector2(0, 1));
        sideEffectRange.Add(new Vector2(0, 0));
        sideEffectRange.Add(new Vector2(0, -1));
        addRange.Add(new Vector2(-1, 1));
        addRange.Add(new Vector2(1, 0));
        addRange.Add(new Vector2(0, 1));
        canBeUsed = 2;
        cost = 2;
    }



    public override void TriggerMainFunction()
    {
        Debug.Log("Do Main Effect");
        manager.TriggerAddEffect(buffMain, totalRange);
    }

    public override void CorrespondingEffect(HexManager.SelectionRangeMaintainer rM)
    {
        Debug.Log("Do Side Effect");

        foreach (Vector2 v in sideEffectRange)
        {
            if (manager.allMapUnits.ContainsKey(rM.curPivot + v))
            {
                manager.SwitchEffect(manager.allMapUnits[rM.curPivot + v], buffMain, buffSide);
            }
        }

        foreach (Vector2 v in addRange)
        {
            if (manager.allMapUnits.ContainsKey(rM.curPivot + v) && manager.allMapUnits[rM.curPivot + v].returnData().cellType == CellType.Cell_TC)
            {
                manager.AddCurCell(manager.allMapUnits[rM.curPivot+v], toAdd,5);
            }
        }
    }
}

public class FKAllergySignal : NormalGenerate //仿真过敏信号
{
    public FKAllergySignal(HexManager m, ICardObserver ob, int n) : base(m, ob, new cellUnit_FD(), n)
    {
        cost = 1;
        canBeUsed = 2;
    }

    public FKAllergySignal(FKAllergySignal otherC) : base(otherC.manager, otherC.observer, new cellUnit_FD(), otherC.cellNum)
    {
        cost = 1;
        canBeUsed = 2;
    }
}

public class TargetTransport : GeneralCard
{
    List<Vector2> totalRange = new List<Vector2>();
    List<Vector2> sideEffectRange = new List<Vector2>();
    UnitBuff buffMain = new buff_FDRushToEnd(buffTagForEffectType.friendly);
    UnitBuff buffSide = new buff_ZXThrow(buffTagForEffectType.both);

    public TargetTransport(HexManager m, ICardObserver ob) : base(m, ob)
    {
        totalRange.Add(new Vector2(0, 0));
        totalRange.Add(new Vector2(-1, 1));
        totalRange.Add(new Vector2(0, 1));
        totalRange.Add(new Vector2(-1, 0));
        totalRange.Add(new Vector2(1, 0));
        sideEffectRange.Add(new Vector2(-1, 0));
        sideEffectRange.Add(new Vector2(1, 0));
        canBeUsed = 2;
        cost = 1;
    }

    public TargetTransport(TargetTransport otherC) : base(otherC.manager, otherC.observer)
    {
        totalRange.Add(new Vector2(0, 0));
        totalRange.Add(new Vector2(-1, 1));
        totalRange.Add(new Vector2(0, 1));
        totalRange.Add(new Vector2(-1, 0));
        totalRange.Add(new Vector2(1, 0));
        sideEffectRange.Add(new Vector2(-1, 0));
        sideEffectRange.Add(new Vector2(1, 0));
        canBeUsed = 2;
        cost = 1;
    }

    public override void TriggerMainFunction()
    {
        Debug.Log("Do Main Effect");
        manager.TriggerAddEffect(buffMain, totalRange);
    }

    public override void CorrespondingEffect(HexManager.SelectionRangeMaintainer rM)
    {
        Debug.Log("Do Side Effect");

        foreach (Vector2 v in sideEffectRange)
        {
            if (manager.allMapUnits.ContainsKey(rM.curPivot + v))
            {
                manager.SwitchEffect(manager.allMapUnits[rM.curPivot + v], buffMain, buffSide);
            }
        }
    }


}

public class OxidativeBurst : GeneralCard //氧化爆发
{
    List<Vector2> totalRange = new List<Vector2>();
    List<Vector2> sideEffectRange = new List<Vector2>();
    UnitBuff buffMain = new buff_JSAoe(buffTagForEffectType.friendly);
    UnitBuff buffSide = new buff_EnAddRecruitable();

    public OxidativeBurst(HexManager m, ICardObserver ob) : base(m, ob)
    {
        totalRange.Add(new Vector2(0, 0));
        totalRange.Add(new Vector2(-1, 1));
        totalRange.Add(new Vector2(0, 1));
        totalRange.Add(new Vector2(0, -1));
        sideEffectRange.Add(new Vector2(0, -1));
        canBeUsed = 3;
        cost = 2;
    }

    public OxidativeBurst(OxidativeBurst otherC) : base(otherC.manager, otherC.observer)
    {
        totalRange.Add(new Vector2(0, 0));
        totalRange.Add(new Vector2(-1, 1));
        totalRange.Add(new Vector2(0, 1));
        totalRange.Add(new Vector2(0, -1));
        sideEffectRange.Add(new Vector2(0, -1));
        canBeUsed = 3;
        cost = 2;
    }

    public override void TriggerMainFunction()
    {
        Debug.Log("Do Main Effect");
        manager.TriggerAddEffect(buffMain, totalRange);
    }

    public override void CorrespondingEffect(HexManager.SelectionRangeMaintainer rM)
    {
        Debug.Log("Do Side Effect");

        foreach (Vector2 v in sideEffectRange)
        {
            if (manager.allMapUnits.ContainsKey(rM.curPivot + v))
            {
                manager.SwitchEffect(manager.allMapUnits[rM.curPivot + v], buffMain, buffSide);
            }
        }
    }


}

public class BloodActivator : NormalMove //活血药汤
{
    public BloodActivator(HexManager m, ICardObserver ob) : base(m, ob, new List<Vector2>(), 3)
    {
        moveShape.Add(new Vector2(0, 0));
        moveShape.Add(new Vector2(0, -1));
        moveShape.Add(new Vector2(-1, 1));
        cost = 1;
        canBeUsed = 4;
    }
    public BloodActivator(BloodActivator otherC) : base(otherC.manager, otherC.observer, new List<Vector2>(), 3)
    {
        moveShape.Add(new Vector2(0, 0));
        moveShape.Add(new Vector2(0, -1));
        moveShape.Add(new Vector2(-1, 1));
        cost = 1;
        canBeUsed = 4;
    }


}

#endregion

#region 第三关相关卡牌
public class NormalAdd_JXB : NormalGenerate
{
    public NormalAdd_JXB(HexManager m, ICardObserver ob, int n) : base(m, ob, new cellUnit_JXB(), n)
    {
        cost = 0;
        ifTempCard = true;
    }

    public NormalAdd_JXB(NormalAdd_JXB otherC) : base(otherC.manager, otherC.observer, new cellUnit_JXB(), otherC.cellNum)
    {
        cost = 0;
        ifTempCard = true;
    }
}

public class TCGeneEditing : GeneralCard //tc生存基因编辑
{
    List<Vector2> totalRange = new List<Vector2>();
    List<Vector2> sideEffectRange = new List<Vector2>();
    List<Vector2> addRange = new List<Vector2>();
    UnitBuff buffMain = new buff_TCLookHealth(buffTagForEffectType.friendly);
    UnitBuff buffSide = new buff_JSAoe(buffTagForEffectType.both);
    cellUnit_TC toAdd = new cellUnit_TC();

    //要加上生成

    public TCGeneEditing(HexManager m, ICardObserver ob) : base(m, ob)
    {
        totalRange.Add(new Vector2(0, 0));
        totalRange.Add(new Vector2(0, -1));
        totalRange.Add(new Vector2(1, -1));
        totalRange.Add(new Vector2(1, 0));
        totalRange.Add(new Vector2(0, 1));
        totalRange.Add(new Vector2(1, -2));
        sideEffectRange.Add(new Vector2(1, -2));
        sideEffectRange.Add(new Vector2(0, 1));
        addRange.Add(new Vector2(0, 0));
        addRange.Add(new Vector2(0, -1));
        addRange.Add(new Vector2(1, -1));
        addRange.Add(new Vector2(1, 0));
        canBeUsed = 2;
        cost = 2;
    }

    public TCGeneEditing(TCGeneEditing otherC) : base(otherC.manager, otherC.observer)
    {
        totalRange.Add(new Vector2(0, 0));
        totalRange.Add(new Vector2(0, -1));
        totalRange.Add(new Vector2(1, -1));
        totalRange.Add(new Vector2(1, 0));
        totalRange.Add(new Vector2(0, 1));
        totalRange.Add(new Vector2(1, -2));
        sideEffectRange.Add(new Vector2(1, -2));
        sideEffectRange.Add(new Vector2(0, 1));
        addRange.Add(new Vector2(0, 0));
        addRange.Add(new Vector2(0, -1));
        addRange.Add(new Vector2(1, -1));
        addRange.Add(new Vector2(1, 0));
        canBeUsed = 2;
        cost = 2;
    }

    public override void TriggerMainFunction()
    {
        Debug.Log("Do Main Effect");
        manager.TriggerAddEffect(buffMain, totalRange);
    }

    public override void CorrespondingEffect(HexManager.SelectionRangeMaintainer rM)
    {
        Debug.Log("Do Side Effect");

        foreach (Vector2 v in sideEffectRange)
        {
            if (manager.allMapUnits.ContainsKey(rM.curPivot + v))
            {
                manager.SwitchEffect(manager.allMapUnits[rM.curPivot + v], buffMain, buffSide);
            }
        }

        foreach (Vector2 v in addRange)
        {
            if (manager.allMapUnits.ContainsKey(rM.curPivot + v) && manager.allMapUnits[rM.curPivot + v].returnData().cellType == CellType.Cell_TC)
            {
                manager.AddCurCell(manager.allMapUnits[rM.curPivot + v], toAdd,5);
            }
        }
    }


}

public class ZXLActivator : GeneralCard //中性粒活性片
{
    List<Vector2> totalRange = new List<Vector2>();
    List<Vector2> sideEffectRange = new List<Vector2>();
    UnitBuff buffMain = new buff_ZXCopy(buffTagForEffectType.friendly);
    UnitBuff buffSide = new buff_AtkSpeedUp(buffTagForEffectType.both);

    public ZXLActivator(HexManager m, ICardObserver ob) : base(m, ob)
    {
        totalRange.Add(new Vector2(0, 0));
        totalRange.Add(new Vector2(1, -1));
        totalRange.Add(new Vector2(0, 1));
        totalRange.Add(new Vector2(1, 1));
        sideEffectRange.Add(new Vector2(0, 0));
        canBeUsed = 2;
        cost = 1;
    }

    public ZXLActivator(ZXLActivator otherC) : base(otherC.manager, otherC.observer)
    {
        totalRange.Add(new Vector2(0, 0));
        totalRange.Add(new Vector2(1, -1));
        totalRange.Add(new Vector2(0, 1));
        totalRange.Add(new Vector2(1, 1));
        sideEffectRange.Add(new Vector2(0, 0));
        canBeUsed = 2;
        cost = 1;
    }

    public override void TriggerMainFunction()
    {
        Debug.Log("Do Main Effect");
        manager.TriggerAddEffect(buffMain, totalRange);
    }

    public override void CorrespondingEffect(HexManager.SelectionRangeMaintainer rM)
    {
        Debug.Log("Do Side Effect");

        foreach (Vector2 v in sideEffectRange)
        {
            if (manager.allMapUnits.ContainsKey(rM.curPivot + v))
            {
                manager.SwitchEffect(manager.allMapUnits[rM.curPivot + v], buffMain, buffSide);
            }
        }
    }


}

public class BCellForcedSplit : NormalGenerate //强制B分化，产生浆细胞
{
    public BCellForcedSplit(HexManager m, ICardObserver ob, int n) : base(m, ob, new cellUnit_JXB(), n)
    {
        cost = 1;
        canBeUsed = 2;
    }

    public BCellForcedSplit(BCellForcedSplit otherC) : base(otherC.manager, otherC.observer, new cellUnit_JXB(), otherC.cellNum)
    {
        cost = 1;
        canBeUsed = 2;
    }
}

public class AntiBodyModifier : GeneralCard //抗体改良器
{
    List<Vector2> totalRange = new List<Vector2>();
    List<Vector2> sideEffectRange = new List<Vector2>();
    UnitBuff buffMain = new buff_JXBBecomeHealer(buffTagForEffectType.friendly);
    UnitBuff buffSide = new buff_TCPenetrate(buffTagForEffectType.both);

    public AntiBodyModifier(HexManager m, ICardObserver ob) : base(m, ob)
    {
        totalRange.Add(new Vector2(0, 0));
        totalRange.Add(new Vector2(0, -1));
        totalRange.Add(new Vector2(0, 1));
        sideEffectRange.Add(new Vector2(0, 0));
        canBeUsed = 2;
        cost = 1;
    }

    public AntiBodyModifier(AntiBodyModifier otherC) : base(otherC.manager, otherC.observer)
    {
        totalRange.Add(new Vector2(0, 0));
        totalRange.Add(new Vector2(0, -1));
        totalRange.Add(new Vector2(0, 1));
        sideEffectRange.Add(new Vector2(0, 0));
        canBeUsed = 2;
        cost = 1;
    }

    public override void TriggerMainFunction()
    {
        Debug.Log("Do Main Effect");
        manager.TriggerAddEffect(buffMain, totalRange);
    }

    public override void CorrespondingEffect(HexManager.SelectionRangeMaintainer rM)
    {
        Debug.Log("Do Side Effect");

        foreach (Vector2 v in sideEffectRange)
        {
            if (manager.allMapUnits.ContainsKey(rM.curPivot + v))
            {
                manager.SwitchEffect(manager.allMapUnits[rM.curPivot + v], buffMain, buffSide);
            }
        }
    }


}

public class CellBindingCapsule : GeneralCard //抗体改良器
{
    List<Vector2> totalRange = new List<Vector2>();
    List<Vector2> sideEffectRange = new List<Vector2>();
    UnitBuff buffMain = new buff_JSKillerBook(buffTagForEffectType.friendly);
    UnitBuff buffSide = new buff_EnAddRecruitable();

    public CellBindingCapsule(HexManager m, ICardObserver ob) : base(m, ob)
    {
        totalRange.Add(new Vector2(0, 0));
        totalRange.Add(new Vector2(0, -1));
        totalRange.Add(new Vector2(-1, 0));
        totalRange.Add(new Vector2(-1, 1));
        totalRange.Add(new Vector2(0, 1));
        sideEffectRange.Add(new Vector2(0, 0));
        sideEffectRange.Add(new Vector2(-1, 0));
        canBeUsed = 2;
        cost = 1;
    }

    public CellBindingCapsule(CellBindingCapsule otherC) : base(otherC.manager, otherC.observer)
    {
        totalRange.Add(new Vector2(0, 0));
        totalRange.Add(new Vector2(0, -1));
        totalRange.Add(new Vector2(-1, 0));
        totalRange.Add(new Vector2(-1, 1));
        totalRange.Add(new Vector2(0, 1));
        sideEffectRange.Add(new Vector2(0, 0));
        sideEffectRange.Add(new Vector2(-1, 0));
        canBeUsed = 2;
        cost = 1;
    }

    public override void TriggerMainFunction()
    {
        Debug.Log("Do Main Effect");
        manager.TriggerAddEffect(buffMain, totalRange);
    }

    public override void CorrespondingEffect(HexManager.SelectionRangeMaintainer rM)
    {
        Debug.Log("Do Side Effect");

        foreach (Vector2 v in sideEffectRange)
        {
            if (manager.allMapUnits.ContainsKey(rM.curPivot + v))
            {
                manager.SwitchEffect(manager.allMapUnits[rM.curPivot + v], buffMain, buffSide);
            }
        }
    }
}
#endregion
