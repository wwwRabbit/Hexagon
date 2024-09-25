using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IState
{
    /// <summary>
    /// 进入状态
    /// </summary>
    void Enter();

    /// <summary>
    /// 离开状态
    /// </summary>
    void Exit();

    /// <summary>
    /// 帧更新
    /// </summary>
    void Logic_Update();

    /// <summary>
    /// 物理帧更新
    /// </summary>
    void Physic_Update();
}
