using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class CellState_Idle : CellState
{
    public CellState_Idle(PlayerInput input, CellStateMachine stateMachine, CellController cell, Animator animator = null) : base(input, stateMachine, cell, animator)
    {
        stateHash = Animator.StringToHash(typeof(CellState_Idle).ToString());
    }

    public override void Enter()
    {
        base.Enter();
        //进入待机速度就应该是0
        cell.SetVelocity(Vector3.zero);
        //Debug.Log("进入待机状态");
    }

    public override void Exit()
    {
        base.Exit();
        //Debug.Log("离开待机状态");
    }

    public override void Logic_Update()
    {
        // 血量清零，切换死亡状态
        if (cell.cellAttr.CurrentBlood <= 0)
        {
            stateMachine.SwitchState(typeof(CellState_Die));
        }

        // 玩家控制逻辑：
        // 玩家控制自己，按下按键，切换移动状态
        if (Click2ControlCell.Instance.JudgmentControl(cell) && input.Move)
        {
            stateMachine.SwitchState(typeof(CellState_Move));
        }
        // 玩家控制自己，按攻击键，切换攻击状态
        else if (Click2ControlCell.Instance.JudgmentControl(cell) && input.Attack)
        {
            cell.ControlAttack();
            stateMachine.SwitchState(typeof(CellState_Attack));
        }
        // ai控制逻辑：
        // 玩家控制的不是自己就正常调用ai的逻辑
        else if (Click2ControlCell.Instance.CurrControlSoldier != this.cell && cell.target != null)
        {
            //找到目标，切换到移动状态

            stateMachine.SwitchState(typeof(CellState_Move));
        }
    }

    public override void Physic_Update()
    {
        if (Click2ControlCell.Instance.JudgmentControl(cell))
        {
            cell.ControlRotation();
        }
    }

}
