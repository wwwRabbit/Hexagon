using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class CellState_Attack : CellState
{
    public CellState_Attack(PlayerInput input, CellStateMachine stateMachine, CellController cell, Animator animator = null) : base(input, stateMachine, cell, animator)
    {
        stateHash = Animator.StringToHash(typeof(CellState_Attack).ToString());
    }

    public override void Enter()
    {
        base.Enter();
        Debug.Log("进入攻击状态");
    }

    public override void Exit() 
    {
        base.Exit();
        Debug.Log("离开攻击状态");
    }

    public override void Logic_Update()
    {
        
        //血量清零，切换死亡状态
        if (cell.cellAttr.Can2Die())
        {            
            stateMachine.SwitchState(typeof(CellState_Die));
        }

        //玩家控制自己，执行相关逻辑
        if (Click2ControlCell.Instance.JudgmentControl(cell) && input.Attack)
        {
            cell.ControlAttack();
        }
        //玩家控制自己，移动，切换移动状态
        else if (Click2ControlCell.Instance.JudgmentControl(cell) && input.Move)
        {
            stateMachine.SwitchState(typeof(CellState_Idle));
        }
        //玩家没有控制自己，执行ai攻击逻辑
        else if (Click2ControlCell.Instance.CurrControlSoldier != cell)
        {
            cell.Attack();

            //目标被摧毁，这个敌方的逻辑还能改
            //目标为空，或目标失活，表示当前阶段的攻击以完成目标，切换至下一状态
            //全场已无目标，进入待机状态
            //有目标，进入移动追击状态
            if (cell.target is null || cell.target.activeSelf is false)
            {
                //清空自己持有的全部对立单位的目标引用
                cell.cellAttr.ClearCells();
                //检测模块重新寻找新目标
                cell.rangeSensor.ClearTarget();
                stateMachine.SwitchState(typeof(CellState_Idle));
            }
            else if (cell.rangeSensor.TargetLeaveRange())
            {
                stateMachine.SwitchState(typeof(CellState_Move));
            }
        }

    }

    public override void Physic_Update()
    {
        if (Click2ControlCell.Instance.JudgmentControl(cell))
        {
            cell.ControlRotation();
        }
        else
        {
            cell.Rotation();
        }
    }
}
