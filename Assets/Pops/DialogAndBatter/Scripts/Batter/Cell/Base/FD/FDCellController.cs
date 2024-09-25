using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows;

public class FDCellController : CellController
{
    //此细胞需要重写移动，攻击模式沿用中性粒（需要检测是否加速），同时死亡时生成数个细胞(暂定中性粒)

    private float currentSpeed;
    [SerializeField] protected CellAttribute cellAttribute;
    [SerializeField] protected float accelarateL;

    protected override void Awake()
    {
        base.Awake();
        currentSpeed = data.moveSpeed;
        controllSpeed = data.moveSpeed;
    }

    #region 死亡&&运动&&处理和运动有关的音效
    protected override void Update()
    {
        pos = transform.position;
        if (prepos != null && pos != null)
        {
            float distance = Vector3.Distance(pos, prepos);

            if (isAtk && audioEnable && data.moveSound == "8")//攻击时直接取消音效
            {
                Debug.Log("Move Sound remove trriger");
                DestoryAudio(data.moveSound);
                audioEnable = false;
                prepos = pos;
            }

            if (distance < data.moveSpeed * Time.deltaTime )//未移动也取消音效
            {
                Debug.Log("Move Sound remove trriger");
                DestoryAudio(data.moveSound);
                audioEnable = false;
                prepos = pos;
            }

            if (data.moveSound == "8" && !audioEnable && distance > moveSoundDis && !isAtk)
            {
                MusicMgr.GetInstance().PlaySound("8", false, 0.1f);//开始播放FD走动音效
                audioEnable = true;
                prepos = pos;
            }
        }

        if (!rangeSensor.CanAtk && target != null)
        {
            currentSpeed += accelarateL;
        }
        else
        {
            currentSpeed = data.moveSpeed;
        }

        if (cellAttribute.CurrentBlood <= 0)
        {
            MusicMgr.GetInstance().PlaySound("9", false, 0.3f);
            if (isEnemy)
                BatterManager.Instance.GenerateEnmForFD(transform.position, "Cell_ZXL_Enemy_Unit");
            else
                BatterManager.Instance.GenerateSolForFD(transform.position, "Cell_ZXL_Solider_Unit");
        }
    }

    public override void Move(int type = 1)
    {
        if (MainTest.stopAction)
        {
            return;
        }

        if (target != null && target.gameObject.activeSelf != false)
        {
            Vector2 dir = (target.transform.position - this.transform.position).normalized;
            if (!rangeSensor.CanAtk)
            {
                SetVelocity(currentSpeed * dir);
            }
        }
    }
    #endregion

    #region 重写自动攻击
    protected override IEnumerator PerformAttack()
    {
        float elapsedTime = 0;
        atkBox.SetActive(true);
        atkBox.transform.localRotation = Quaternion.identity;

        //// 计算每帧增加的角度，根据攻击速度和攻击范围确定
        //float angleIncrement = (data.atkRange > 0) ? startAngle / (Mathf.PI * data.atkRange * data.atkSpeed) : 0f;

        if (data.attackSound != null && data.attackSound == "1")
        {
            MusicMgr.GetInstance().PlaySound("1", false, 0.1f);//开始播放攻击音效 
        }

        Vector3 dir = (target.transform.position - this.transform.position).normalized;

        if (data.attackSound == "10")
        {
            MusicMgr.GetInstance().PlaySound("10", false, 0.1f);
        }

        isAtk = true;
        Vector3 origin = transform.position;

        while (elapsedTime < 1f / data.atkSpeed)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / (1f / data.atkSpeed));
            float anit = t < 0.5f ? t : 1 - t;

            //通过当前进度修改位置，实现顶一下的效果
            transform.position = origin + dir * anit;

            // 逐渐增加攻击盒的旋转角度，从 y 轴负方向（左侧）开始向右挥动
            float currentAngle = Mathf.Lerp(endAngle, startAngle, t); // 从-90度开始向右挥动，到90度结束
            Vector3 rotation = new Vector3(0f, 0f, currentAngle);
            atkBox.transform.localRotation = Quaternion.Euler(rotation);
            yield return null;
        }

        isAtk = false;

        // 将攻击盒的旋转重置为初始状态
        atkBox.transform.localRotation = Quaternion.identity;
        atkBox.SetActive(false);
    }
    #endregion

    #region 玩家操控逻辑

    private float controllSpeed;
    private Vector2 predir;
    private Vector2 controllDir;

    public override void ControlMove()
    {
        if (predir == input.Vec)
            controllSpeed += accelarateL * 2;
        else
            controllSpeed = data.moveSpeed;

        controllDir = input.Vec;
        predir = input.Vec;

        SetSmoothVelocity(input.Vec * controllSpeed, controllSpeed * 0.8f);
    }

    public override void ControlAttackForm()
    {
        if (controllDir != Vector2.zero)
        {
            StartCoroutine(controllAttack());
        }
    }

    private IEnumerator controllAttack()
    {
        float elapsedTime = 0;
        atkBox.SetActive(true);
        atkBox.transform.localRotation = Quaternion.identity;

        //// 计算每帧增加的角度，根据攻击速度和攻击范围确定
        //float angleIncrement = (data.atkRange > 0) ? startAngle / (Mathf.PI * data.atkRange * data.atkSpeed) : 0f;

        if (data.attackSound != null && data.attackSound == "1")
        {
            MusicMgr.GetInstance().PlaySound("1", false, 0.1f);//开始播放攻击音效 
        }

        Vector3 dir = new Vector3(controllDir.x, controllDir.y, 0).normalized;

        if (data.attackSound == "10")
        {
            MusicMgr.GetInstance().PlaySound("10", false, 0.1f);
        }

        isAtk = true;
        Vector3 origin = transform.position;

        while (elapsedTime < 1f / data.atkSpeed)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / (1f / data.atkSpeed));
            float anit = t < 0.5f ? t : 1 - t;
            transform.position = origin + dir * anit;

            // 逐渐增加攻击盒的旋转角度，从 y 轴负方向（左侧）开始向右挥动
            float currentAngle = Mathf.Lerp(endAngle, startAngle, t); // 从-90度开始向右挥动，到90度结束
            Vector3 rotation = new Vector3(0f, 0f, currentAngle);
            atkBox.transform.localRotation = Quaternion.Euler(rotation);
            yield return null;
        }

        isAtk = false;

        // 将攻击盒的旋转重置为初始状态
        atkBox.transform.localRotation = Quaternion.identity;
        atkBox.SetActive(false);
    }

    #endregion
}
