using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

//日志：

//4-22 想了一下，这个脚本应该只负责提供功能的方法，具体的调用应该交给状态机或者玩家控制器
//  拆分了一些功能，实现基础玩家控制逻辑
//4-23 完善了基础细胞的攻击逻辑，改成了挥动攻击
//  完善了玩家控制功能

public class Damage
{
    public int atkType = 0;    //0 近战 1 远程
    public float damage = 0;
}

/// <summary>
/// 细胞不同状态涉及到的功能基类，后续如果有新类型细胞，就继承这个继续写
/// </summary>
public class CellController : MonoBehaviour
{
    public BaseData data;
    public Damage damage;

    public bool isEnemy;    //是否是敌人
    public int cap;         //所属阵营，需要跟外部界面沟通
    public bool captive;    //是否被俘虏，死了算好人阵营    

    [HideInInspector] public GameObject target = null;   //目标
    Rigidbody2D body;
    protected PlayerInput input;

    //现在的想法是把涉及到ai的东西，和基础的功能都写在这里边，然后到时候按分类调用
    [HideInInspector] public CellAttribute cellAttr;

    //ai相关参数：
    [HideInInspector] public RangeSensor rangeSensor;

    public GameObject atkBox;
    protected HitBox hit;

    protected float lastAtkTime;
    public Vector2 flag;
    public bool haveFather;

    protected float selfAtkCD;
    protected float currentAtkCD;
    protected int tmp = 0;
    protected bool isAtk;
    public bool IsAtk { get { return isAtk; } }

    [Header("起始攻击角度（只有中性粒有用）"), SerializeField] protected float startAngle = 90;
    [Header("结束攻击角度（只有中性粒有用）"), SerializeField] protected float endAngle = -90;

    //行动音效触发记录
    protected Vector3 pos;
    protected Vector3 prepos;
    protected float moveSoundDis;

    protected virtual void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        rangeSensor = this.transform.Find("RangeSensor").GetComponent<RangeSensor>();
        cellAttr = this.transform.Find("Hurt").GetComponent<CellAttribute>();

        atkBox.transform.GetChild(0).TryGetComponent(out hit);
        cellAttr.InitAttributeData(data.Health);
    }

    protected virtual void Start()
    {
        input = PlayerInput.Instance;
        damage = new Damage();
        damage.damage = data.atkValue;

        if (hit != null)
        {
            hit.damage = damage;
        }

        isEnemy = data.isEnemy;

        atkBox.SetActive(false);
        rangeSensor.atkRadius = data.atkRange;

        selfAtkCD = data.atkCD;

        pos = transform.position;
        prepos = transform.position;
        moveSoundDis = data.moveSpeed / 2f;
    }

    #region 运动&&处理和运动有关的音效
    public static void DestoryAudio(string objectName)
    {
        GameObject soundAudio = GameObject.Find("Sound Play");

        Debug.Log(soundAudio);

        if (soundAudio != null)
        {
            foreach (Transform child in soundAudio.transform)
            {
                if (child.gameObject.name == objectName)
                {
                    Destroy(child.gameObject);
                    Debug.Log($"已删除音源 {objectName}");
                    return;
                }
            }
        }
    }

    protected bool audioEnable = false;

    protected virtual void Update()
    {
        pos = transform.position;
        if (prepos != null && pos != null)
        {
            float distance = Vector3.Distance(pos, prepos);

            if (distance > moveSoundDis && data.moveSound == "2")
            {
                audioEnable = true;
                MusicMgr.GetInstance().PlaySound("2", false, 0.1f);//开始播放走动音效
                prepos = pos;
            }
        }
    }
    #endregion

    #region Ai控制模块
    /// <summary>
    /// 移动调用
    /// </summary>
    /// <param name="type">0 是玩家，1 是ai</param>
    public virtual void Move(int type = 1)
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
                SetVelocity(data.moveSpeed * dir);
            }
        }
    }

    /// <summary>
    /// 攻击调用
    /// </summary>
    public virtual void Attack()
    {
        if (!Preprocessing())
        {
            return;
        }

        if (MainTest.stopAction)
        {
            return;
        }

        if (tmp > 0)
        {
            tmp = 0;
            return;
        }

        //在冷却时间内时进入Attack携程。
        if (Time.time - lastAtkTime >= selfAtkCD && !isAtk)
        {
            AttackForm();
            lastAtkTime = Time.time;
        }
    }

    protected virtual bool Preprocessing()
    {
        return true;
    }

    /// <summary>
    /// 不同攻击方式的细胞，需要重写的函数
    /// </summary>
    public virtual void AttackForm()
    {
        try
        {
            StartCoroutine(PerformAttack());
        }
        catch
        {
            Debug.Log("这里在inactive时候触发了协程");
        }
    }

    //先前的攻击逻辑存在bug，selfAtkCD和atkSpeed没有同步，所以在协程时会出现hitbox闪烁的情况
    protected virtual IEnumerator PerformAttack()
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

        isAtk = true;

        while (elapsedTime < 1f / data.atkSpeed)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / (1f / data.atkSpeed));
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

    //void PerformAttackDetection()
    //{
    //    Vector2 selfPos = transform.position;
    //    Vector2 dir = transform.up;
    //    Vector2 offSet = 1f * dir;
    //    Vector2 center = selfPos + offSet;

    //    RaycastHit2D hit = Physics2D.Raycast(center, transform.up, 5f, LayerMask.GetMask("Hurt"));
    //    if (hit.collider != null)
    //    {            
    //        hit.collider.gameObject.GetComponent<CellAttribute>().TakeDamage(damage, this);
    //    }
    //}

    /// <summary>
    /// 技能调用
    /// </summary>
    public virtual void Skill()
    {

    }

    public virtual void Rotation()
    {
        if (target != null)
        {
            Vector2 dir = target.transform.position - this.transform.position;
            SetRotation(dir);
        }
    }

    #endregion

    #region 玩家控制模块
    public virtual void ControlMove()
    {
        //SetVelocity(input.Vec * data.moveSpeed);
        SetSmoothVelocity(input.Vec * data.moveSpeed, data.moveSpeed * 0.8f);
    }

    public virtual void ControlAttack()
    {
        if (Time.time - lastAtkTime >= selfAtkCD)
        {
            ControlAttackForm();

            lastAtkTime = Time.time;
        }
    }

    public virtual void ControlAttackForm()
    {
        AttackForm();
    }


    public virtual void ControlRotation()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(input.mousePos);
        Vector2 dir = mousePos - transform.position;
        SetRotation(dir, 2f);
    }

    public virtual void ControlSkill()
    {

    }
    #endregion

    #region 基础控制模块

    public void SetRotation(Vector2 rotationDirection, float multi = 1f)
    {
        // 如果方向为零向量，则不进行旋转
        if (rotationDirection == Vector2.zero)
        {
            return;
        }

        // 计算旋转角度
        float angle = Mathf.Atan2(rotationDirection.y, rotationDirection.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(0, 0, angle - 90);

        // 根据旋转速度插值旋转
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, data.rotateSpeed * multi * Time.deltaTime);
    }

    /// <summary>
    /// 设置x，y轴插值速度
    /// </summary>
    /// <param name="velocity"></param>
    /// <param name="smoothTIme"></param>
    public void SetSmoothVelocity(Vector3 velocity, float smoothTIme)
    {
        body.velocity = Vector3.Lerp(body.velocity, velocity, smoothTIme * Time.deltaTime);
    }


    /// <summary>
    /// 设置x，y轴速度
    /// </summary>
    /// <param name="velocity"></param>
    public void SetVelocity(Vector3 velocity)
    {
        body.velocity = velocity;
    }

    /// <summary>
    /// 设置x轴速度
    /// </summary>
    /// <param name="velocityX">速度值</param>
    public void SetVelocityX(float velocityX)
    {
        body.velocity = new Vector3(velocityX, body.velocity.y);
    }

    /// <summary>
    /// 设置y轴速度
    /// </summary>
    /// <param name="velocityY">速度值</param>
    public void SetVelocityY(float velocityY)
    {
        body.velocity = new Vector3(body.velocity.x, velocityY);
    }
    #endregion
}
