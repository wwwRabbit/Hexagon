using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JSCell2Controller : JSCellController
{
    [Header("最低攻击间隔"), SerializeField] float minAtkCD;
    [Header("间隔减少时间"), SerializeField] float reduceCD;
    bool targetChange;

    protected override void Awake()
    {
        base.Awake();

        transform.Find("AtkCircleCollider").GetComponent<Push2>().cell = this;
    }

    protected override void Update()
    {
        base.Update();
        if ((target is null || target.activeSelf is false) && targetChange)
        {
            targetChange = false;
            selfAtkCD -= reduceCD;
            Mathf.Clamp(selfAtkCD, minAtkCD, data.atkCD);
        }
        else if (target != null && targetChange is false)
        {
            targetChange = true;
        }

        //ReduceAtkCD();
        
    }

    protected override void DoDamage()
    {
        base.DoDamage();
        ReduceAtkCD();
    }

    public void ReduceAtkCD()
    {
        List<CellAttribute> itemsToRemove = new List<CellAttribute>();

        foreach (CellAttribute item in controlTarget)
        {
            if (item != null && item.CurrentBlood <= 0)
            {
                selfAtkCD -= reduceCD;
                selfAtkCD = Mathf.Clamp(selfAtkCD, minAtkCD, data.atkCD);
                print(selfAtkCD);
                itemsToRemove.Add(item);
            }
        }

        foreach (CellAttribute itemToRemove in itemsToRemove)
        {
            controlTarget.Remove(itemToRemove);
        }
    }

}
