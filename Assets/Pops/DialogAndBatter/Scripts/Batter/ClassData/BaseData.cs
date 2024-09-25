using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/CellData", fileName = "BaseData")]
public class BaseData : ScriptableObject
{
    [Header("生命值")] public float Health;
    [Header("移动速度")] public float moveSpeed;
    [Header("旋转速度")] public float rotateSpeed;
    [Header("攻击力")] public float atkValue;
    //[Header("防御力")] public float defValue;
    [Header("攻击速度（巨噬细胞没用）")] public float atkSpeed;
    [Header("攻击范围")] public float atkRange;
    [Header("攻击间隔")] public float atkCD;
    [Header("是否是敌人")] public bool isEnemy;

    [Header("基础音效")]
    [Header("攻击音效")] public string attackSound;
    [Header("移动音效")] public string moveSound;

    [Header("下方音效为tc细胞独有")]
    [Header("扎入音效")] public string insertSound;
    [Header("发射音效")] public string shoutSound;

    [Header("下方音效为巨噬细胞独有")]
    [Header("起跳音效")] public string jumpSound;
    [Header("落地音效")] public string landSound;
}
