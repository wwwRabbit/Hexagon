using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 获取玩家输入相关
/// </summary>
public class PlayerInput : MonoBehaviour
{
    private static PlayerInput instance;
    public static PlayerInput Instance => instance;

    private PlayerInputActions playerInputActions;     //输入配置文件

    public Vector2 Vec => playerInputActions.Player.Move.ReadValue<Vector2>();

    public float VecY => playerInputActions.Player.Move.ReadValue<Vector2>().y;

    public Vector2 Delta => playerInputActions.Player.Rotation.ReadValue<Vector2>();

    public Vector2 mousePos => playerInputActions.Player.MousePos.ReadValue<Vector2>();

    public bool Move => Vec != Vector2.zero;    //标识是否移动
    public bool Skill => playerInputActions.Player.Skill.WasPressedThisFrame(); //标识是否使用技能
    public bool Attack => playerInputActions.Player.Attack.WasPressedThisFrame();   //标识是否攻击

    //玩家具体控制参数

    CellController cell; //记录选中对象的控制脚本

    private void Awake()
    {
        instance = this;
        playerInputActions = new PlayerInputActions();
    }

    private void Start()
    {
        
    }

    /// <summary>
    /// 开启InputSystem
    /// </summary>
    public void EnablePlayerInputs()
    {
        playerInputActions.Player.Enable();
        //Cursor.lockState = CursorLockMode.Locked;
    }

    /// <summary>
    /// 关闭InputSystem
    /// </summary>
    public void DisablePlayerInputs() 
    {
        playerInputActions.Player.Disable();
    }
}
