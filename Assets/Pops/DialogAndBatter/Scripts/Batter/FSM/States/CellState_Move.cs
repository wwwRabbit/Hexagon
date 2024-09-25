using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellState_Move : CellState
{
    public CellState_Move(PlayerInput input, CellStateMachine stateMachine, CellController cell, Animator animator = null) : base(input, stateMachine, cell, animator)
    {
        stateHash = Animator.StringToHash(typeof(CellState_Move).ToString());
    }

    public override void Enter()
    {
        base.Enter();
        Debug.Log("进入移动状态");
    }

    public override void Exit()
    {
        base.Exit();
        Debug.Log("离开移动状态");
        //速度改为0
        cell.SetVelocity(Vector3.zero);
    }

    public override void Logic_Update()
    {
        // 血量清零，切换死亡状态
        if (cell.cellAttr.CurrentBlood <= 0)
        {
            stateMachine.SwitchState(typeof(CellState_Die));
        }

        // 玩家控制自己，没有按移动键，切换待机状态
        if (Click2ControlCell.Instance.JudgmentControl(cell) && !input.Move)
        {
            stateMachine.SwitchState(typeof(CellState_Idle));
        }
        // 玩家控制自己，按攻击键并且没移动，切换攻击状态
        else if (Click2ControlCell.Instance.JudgmentControl(cell) && input.Attack && !input.Move)
        {
            cell.ControlAttack();
            stateMachine.SwitchState(typeof(CellState_Attack));
        }
        //玩家控制自己，按攻击键并且移动。其实应该再引入一个移动攻击状态的
        else if (Click2ControlCell.Instance.JudgmentControl(cell) && input.Attack && input.Move)
        {
            cell.ControlAttack();
        }
        // 玩家没有控制自己，可以攻击，切换攻击状态
        else if (cell.rangeSensor.CanAtk && Click2ControlCell.Instance.CurrControlSoldier != cell)
        {
            stateMachine.SwitchState(typeof(CellState_Attack));
        }

        // 如果移动中，目标死亡，就重新检测，检测不到就保持待机。因为目标持有的对立列表是在被攻击时才刷新，这会还在移动没有记录
        if (Click2ControlCell.Instance.CurrControlSoldier != cell)
        {
            if (cell.target is null || cell.target != null && cell.target.activeSelf is false)
            {
                cell.rangeSensor.ClearTarget();
                stateMachine.SwitchState(typeof(CellState_Idle));
            }
        }

    }

    public override void Physic_Update()
    {
        if (Click2ControlCell.Instance.JudgmentControl(cell))
        {
            cell.ControlMove();
            cell.ControlRotation();
        }
        else
        {
            cell.Move();
            cell.Rotation();
        }
    }
}
