using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellState_Die : CellState
{
    public CellState_Die(PlayerInput input, CellStateMachine stateMachine, CellController cell, Animator animator = null) : base(input, stateMachine, cell, animator)
    {
        //现在想一下，这可以写成泛型。一会改一下
        stateHash = Animator.StringToHash(typeof(CellState_Die).ToString());
    }

    public override void Enter()
    {
        base.Enter();
        Debug.Log("进入死亡状态");

        if (Click2ControlCell.Instance.JudgmentControl(cell))
        {
            Click2ControlCell.Instance.EndControlCell();
        }

        //通知系统数量减少
        EventCenter.GetInstance().EventTrigger<int>("SubCount", cell.isEnemy ? 0 : 1);
        EventCenter.GetInstance().EventTrigger<Vector2>("SubFlag", cell.flag);

        cell.gameObject.SetActive(false);
        //放入缓存池

        //GameObject.Destroy(cell.gameObject, 10f);
        
    }

    public override void Exit()
    {
        base.Exit(); 
    }
}
