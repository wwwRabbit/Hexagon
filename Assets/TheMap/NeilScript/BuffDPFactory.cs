using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BuffDPFactory:MonoBehaviour 
{
    public static BuffDPFactory _instance;

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

    public Sprite zx_throw;
    public Sprite tc_HealthLock;
    public Sprite all_HealthUp;

    public Sprite buffDPFactory(UnitBuff buff)
    {
        switch (buff)
        {
            case buff_HealthUp:
                return all_HealthUp;

            case buff_TCLookHealth:
                return tc_HealthLock;

            case buff_ZXThrow:
                return zx_throw;
        }
        return zx_throw;
    }
}

public class GeneralBuff: UnitBuff
{
    internal bool ifTrans = false;//用于表示是否为变体类buff
    internal buffTagForEffectType type = buffTagForEffectType.enmey; //这个用于表示buff的作用对象
    public bool ifTransform()
    {
        return ifTrans;
    }

    public buffTagForEffectType returnEffectCellType()
    {
        return type;
    }

    public virtual string returnMessage() { return "general"; }

    public virtual CellType returnTransFormTag(CellType inputC)
    {
        return CellType.Cell_Null;
    }
}

public class buff_HealthUp : GeneralBuff
{
    public buff_HealthUp()
    {
        ifTrans = false;
        type = buffTagForEffectType.both; //默认为对所有进行影响
    }

    public buff_HealthUp(buffTagForEffectType tp)
    {
        ifTrans = false;
        type = tp;//自定义作用对象
    }

    public override string returnMessage() { return "HealthUp"; }
}

public class buff_NumUp : GeneralBuff
{
    public buff_NumUp()
    {
        ifTrans = false;
        type = buffTagForEffectType.both; //默认为对所有进行影响
    }

    public buff_NumUp(buffTagForEffectType tp)
    {
        ifTrans = false;
        type = tp;//自定义作用对象
    }

    public override string returnMessage() { return "NumUp"; }
}

public class buff_TCLookHealth : GeneralBuff
{
    public buff_TCLookHealth(buffTagForEffectType tp) //这个也是，自定义作用对象，可以是我方也可以是敌方，也可以both
    {
        ifTrans = true;
        type = tp;
    }

    public override string returnMessage() { return "TcLockHealth"; }

    public override CellType returnTransFormTag(CellType inputC)
    {
        if (inputC == CellType.Cell_TC)
            return CellType.Cell_TC_M;
        else
            return CellType.Cell_Null;
    }
}

public class buff_TCPenetrate : GeneralBuff //穿透能力增强
{
    public buff_TCPenetrate(buffTagForEffectType tp) //自定义作用对象，可以是我方也可以是敌方,也可以both
    {
        ifTrans = true;
        type = tp;
    }
    public override string returnMessage() { return "TCPenetrate"; }

    public override CellType returnTransFormTag(CellType inputC)
    {
        if (inputC == CellType.Cell_TC)
            return CellType.Cell_TC_M;
        else
            return CellType.Cell_Null;
    }
}

public class buff_ZXThrow : GeneralBuff //中性粒变远程投掷
{
    public buff_ZXThrow(buffTagForEffectType tp)
    {
        ifTrans = true;
        type = tp;
    }

    public override string returnMessage() { return "ZXLThrow"; }

    public override CellType returnTransFormTag(CellType inputC)
    {
        if (inputC == CellType.Cell_ZXL)
            return CellType.Cell_ZXL_M;
        else
            return CellType.Cell_Null;
    }
}

public class buff_ZXCopy : GeneralBuff //中性粒第一次攻击复制一下自己
{
    public buff_ZXCopy(buffTagForEffectType tp)
    {
        ifTrans = true;
        type = tp;
    }

    public override string returnMessage() { return "ZXCopy"; }

    public override CellType returnTransFormTag(CellType inputC)
    {
        if (inputC == CellType.Cell_ZXL)
            return CellType.Cell_ZXL_M;
        else
            return CellType.Cell_Null;
    }
}

public class buff_EnAddRecruitable : GeneralBuff //敌人获得可以招募的援军
{
    public buff_EnAddRecruitable()
    {
        ifTrans = false;
        type = buffTagForEffectType.enmey;
    }

    public override string returnMessage() { return "EnAddRecruitable"; }
}

public class buff_FDRushToEnd : GeneralBuff //肥大细胞选择最远的敌人攻击
{
    public buff_FDRushToEnd(buffTagForEffectType tp)
    {
        ifTrans = true;
        type = tp;
    }

    public override string returnMessage() { return "FDRushToEnd"; }

    public override CellType returnTransFormTag(CellType inputC)
    {
        if (inputC == CellType.Cell_FD)
            return CellType.Cell_FD_M;
        else
            return CellType.Cell_Null;
    }
}

public class buff_JSAoe : GeneralBuff //巨噬细胞共会对推开的单位造成伤害
{
    public buff_JSAoe(buffTagForEffectType tp)
    {
        ifTrans = true;
        type = tp;
    }

    public override string returnMessage() { return "JSAoe"; }

    public override CellType returnTransFormTag(CellType inputC)
    {
        if (inputC == CellType.Cell_JS)
            return CellType.Cell_JS_M;
        else
            return CellType.Cell_Null;
    }
}

public class buff_JSKillerBook: GeneralBuff //巨噬细胞击败敌人后攻速增加,杀人书
{
    public buff_JSKillerBook(buffTagForEffectType tp)
    {
        ifTrans = true;
        type = tp;
    }
    public override string returnMessage() { return "JSKillerBook"; }

    public override CellType returnTransFormTag(CellType inputC)
    {
        if (inputC == CellType.Cell_JS)
            return CellType.Cell_JS_M;
        else
            return CellType.Cell_Null;
    }
}

public class buff_EnAddEnemy : GeneralBuff //敌人获得可以援军
{
    public buff_EnAddEnemy()
    {
        ifTrans = false;
        type = buffTagForEffectType.enmey;
    }

    public override string returnMessage() { return "EnAddEnemy"; }
}

public class buff_AtkSpeedUp : GeneralBuff
{
    public buff_AtkSpeedUp()
    {
        ifTrans = false;
        type = buffTagForEffectType.both; //默认为对所有进行影响
    }

    public buff_AtkSpeedUp(buffTagForEffectType tp)
    {
        ifTrans = false;
        type = tp;//自定义作用对象
    }

    public override string returnMessage() { return "AtkSpeedUp"; }
}

public class buff_JXBBecomeHealer : GeneralBuff //浆细胞变成治疗
{
    public buff_JXBBecomeHealer(buffTagForEffectType tp)
    {
        ifTrans = true;
        type = tp;
    }

    public override string returnMessage() { return "JXBBecomeHealer"; }

    public override CellType returnTransFormTag(CellType inputC)
    {
        if (inputC == CellType.Cell_J)
            return CellType.Cell_J_M;
        else
            return CellType.Cell_Null;
    }
}