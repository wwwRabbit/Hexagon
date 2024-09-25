using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 细胞状态机，获取所需组件，初始化并添加不同状态，最后启动初始状态
/// </summary>
public class CellStateMachine : StateMachine
{
    CellState[] states; //存储所有的状态

    Animator animator;

    CellController cell;

    PlayerInput input;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        cell = GetComponent<CellController>();
        stateDic = new Dictionary<System.Type, IState>();
    }

    private void Start()
    {
        input = PlayerInput.Instance;

        CellState_Idle idle = new CellState_Idle(input, this, cell);
        AddState(idle);
        CellState_Move move = new CellState_Move(input, this, cell);
        AddState(move);
        CellState_Attack atk = new CellState_Attack(input, this, cell);
        AddState(atk);
        CellState_Die die = new CellState_Die(input, this, cell);
        AddState(die);

        //最开始启动待机状态
        SwitchOn(stateDic[typeof(CellState_Idle)]);
    }
}
