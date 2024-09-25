using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 状态机，就负责状态切换，添加状态，移除状态，帧更新
/// </summary>
public class StateMachine : MonoBehaviour 
{
    IState currentState;

    protected Dictionary<System.Type, IState> stateDic;

    private void Update()
    {
        currentState.Logic_Update();
    }

    public void FixedUpdate()
    {
        currentState.Physic_Update();
    }

    protected void SwitchOn(IState newState)
    {
        currentState = newState;
        currentState.Enter();
    }

    /// <summary>
    /// 切换状态
    /// </summary>
    /// <param name="newState">状态类</param>
    public void SwitchState(IState newState)
    {
        currentState.Exit();
        SwitchOn(newState);
    }

    /// <summary>
    /// 切换状态
    /// </summary>
    /// <param name="newStateType">状态类型</param>
    public void SwitchState(System.Type newStateType)
    {
        SwitchState(stateDic[newStateType]);
    }

    /// <summary>
    /// 添加状态
    /// </summary>
    /// <param name="state"></param>
    public void AddState(IState state)
    {
        if (currentState is null) currentState = state;
        stateDic.Add(state.GetType(), state);
    }

    /// <summary>
    /// 移除状态
    /// </summary>
    /// <param name="state"></param>
    public void RemoveState(IState state)
    {
        if (stateDic.ContainsKey(state.GetType()))
        {
            stateDic.Remove(state.GetType());
        }
    }

}
