using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 基类状态，继承状态接口，主要在进入时，调用动画切换函数
/// </summary>
public class CellState : IState
{
    protected CellStateMachine stateMachine;    //持有状态机的引用，后续才能通过状态机切换不同状态

    protected CellController cell;              //里边有不同的控制功能模块，不同状态直接进去调用，所以要持有引用

    protected PlayerInput input;                //读取输入相关内容

    //下方是控制动画切换的参数
    private float transitionDuration = 0.1f;    //动画切换间隔
    private float stateStartTime;               //状态开始时间
    protected int stateHash;                    //状态名对应哈希值
    protected Animator animator;                //没办法直接获取，因为没法挂在物体上
    protected float StateDuration => Time.time - stateStartTime;    //动画过渡时间
    protected bool IsAnimFinished => StateDuration >= animator.GetCurrentAnimatorStateInfo(0).length;   //动画是否播放完毕

    /// <summary>
    /// 有参构造初始化所需引用
    /// </summary>
    /// <param name="input">输入系统</param>
    /// <param name="stateMachine">细胞状态机</param>
    /// <param name="cell">细胞控制器</param>
    /// <param name="animator">动画组件</param>
    public CellState(PlayerInput input, CellStateMachine stateMachine, CellController cell ,Animator animator = null)
    {
        this.stateMachine = stateMachine;
        this.animator = animator;
        this.cell = cell;
        this.input = input;

        //在子类里去做
        //stateHash = Animator.StringToHash();
    }

    public virtual void Enter()
    {
        //没有动画：不用播放
        //如果有动画：
        //stateHash子类中会赋值，每个状态需要在动画机内对应一个文件。全部动画文件都连接AnyState就行，状态机控制切换
        if (animator != null)
        {
            animator.CrossFade(stateHash, transitionDuration);
        }
        stateStartTime = Time.time;
    }

    public virtual void Exit()
    {

    }

    public  virtual void Logic_Update()
    {

    }

    public virtual void Physic_Update()
    {

    }
}
