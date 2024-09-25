using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class JSCellController : CellController
{

    [Header("冲刺距离"), SerializeField] private float sprintDistance = 2f;
    //[Header("冲刺速度"), SerializeField] private float sprintSpeed = 5f; // 冲刺速度
    [Header("冲刺时间"), SerializeField] private float sprintTime = 1f;
    [Header("冲刺结束后的伤害范围"), SerializeField] private float damageRange = 1f;

    // 移动速度
    [Header("推力，会给被推的物体施加一个额外速度")] public float imoveSpeed = 5f;
    // 减速系数
    [Header("其它物体被推之后，恢复原速度的倍率，越大恢复的越快")] public float ideceleration = 0.5f;

    private Transform mouth;
    private CircleCollider2D crash;
    private Rigidbody2D rb; // 添加 Rigidbody2D 引用

    bool atking = false;

    public List<CellAttribute> controlTarget = new List<CellAttribute>();

    private bool ifDashSound;

    protected override void Awake()
    {
        base.Awake();

        mouth = transform.Find("MouthPoint").GetComponent<Transform>();
        crash = transform.Find("AtkCircleCollider").GetComponent<CircleCollider2D>();
        crash.enabled = false;
        rb = GetComponent<Rigidbody2D>(); // 获取 Rigidbody2D 组件        
    }

    protected override void Start()
    {
        base.Start();
        transform.Find("AtkCircleCollider").GetComponent<Push>().moveSpeed = imoveSpeed;
        transform.Find("AtkCircleCollider").GetComponent<Push>().deceleration = ideceleration;
        target = null;
    }

    protected override void Update()
    {
        base.Update();
    }

    public override void AttackForm()
    {
        Vector3 dir = (target.transform.position - transform.position).normalized;
        Vector3 targetPos = dir * sprintDistance + transform.position;

        if (Vector3.Distance(target.transform.position, transform.position)>sprintDistance)
        {
            Debug.Log("细胞跃起");
            MusicMgr.GetInstance().PlaySound("6", false, 0.1f);//播放开始跃起的声音
            ifDashSound = true;
        }
        StartCoroutine(SprintCoroutine(targetPos));
    }

    private float timer;

    IEnumerator SprintCoroutine(Vector3 targetPos)
    {
        crash.enabled = true;
        Vector3 startPos = transform.position;
       // Vector3 direction = (targetPos - startPos).normalized;
        //float distanceToTarget = Vector3.Distance(startPos, targetPos);

        float totalTime = sprintTime; // 设定冲刺总时间
        float remainingTime = totalTime;
        float timer = 0f;

        while (remainingTime > 0)
        {
            float timeStep = Mathf.Min(Time.deltaTime, remainingTime); // 计算时间步长
            remainingTime -= timeStep;
            timer += timeStep;

            float progress = timer / totalTime;
            Vector3 newPosition = Vector3.Lerp(startPos, targetPos, progress); // 根据时间进度计算新位置
            rb.MovePosition(newPosition); // 使用 MovePosition 方法移动物体

            if(remainingTime / totalTime<0.2f && ifDashSound)
            {
                Debug.Log("细胞落地");
                MusicMgr.GetInstance().PlaySound("7", false, 0.1f);//播放落地声音
                ifDashSound=false;
            }
            yield return null;
        }

        rb.velocity = Vector2.zero; // 停止移动
        crash.enabled = false;

        if(timer<Time.time)
        {
            Debug.Log("细胞吞噬");
            MusicMgr.GetInstance().PlaySound("5", false, 0.1f);
            timer = Time.time + 0.5f;
        }

        DoDamage();
        atking = false;
    }

    protected virtual void DoDamage()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(mouth.position, damageRange, LayerMask.GetMask("Hurt"));
        float dis;
        float minDis = float.PositiveInfinity;
        Collider2D reallyTarget = null;

        foreach (Collider2D item in colliders)
        {
            if (item.tag != gameObject.tag)
            {
                dis = Vector2.Distance(item.transform.position, mouth.position);
                if (dis < minDis)
                {
                    minDis = dis;
                    reallyTarget = item;
                }
            }
        }

        if (reallyTarget != null)
        {
            if (!controlTarget.Contains(reallyTarget.gameObject.GetComponent<CellAttribute>()))
            {
                controlTarget.Add(reallyTarget.gameObject.GetComponent<CellAttribute>());
            }

            reallyTarget.gameObject.GetComponent<CellAttribute>().TakeDamage(damage, this);
        }
    }

    //玩家控制的移动方式。
    public override void ControlAttackForm()
    {
        if (!atking)
        {
            atking = true;
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;

            Vector3 dir = (mousePos - transform.position).normalized;
            Vector3 targetPos = transform.position + dir * sprintDistance;

            MusicMgr.GetInstance().PlaySound("6", false, 0.1f);//播放开始跃起的声音
            StartCoroutine(SprintCoroutine(targetPos));
        }
    }
}
