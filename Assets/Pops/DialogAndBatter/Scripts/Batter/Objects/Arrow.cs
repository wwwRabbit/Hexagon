using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    protected Damage damage;
    float flySpeed;
    protected Transform target;
    bool isAuto;

    Vector2 startDir;
    Rigidbody2D body;

    [Header("穿透次数"), SerializeField] private int penetrationCount = 1;

    protected CellController fatherCell;

    protected int nowPentrationCount;

    [Header("额外伤害，变体填，原体填0")] public float additionalDamage = 1;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        damage = new Damage();
    }

    private void OnEnable()
    {
        nowPentrationCount = penetrationCount;
    }

    private void OnDisable()
    {
        this.damage.damage = 0;
        this.flySpeed = 0;
        isAuto = false;
        startDir = Vector2.zero;
    }

    public void InitArrowData(float damage, float flySpeed, CellController fatherCell, Vector2 startDir, bool auto = false, Transform target = null)
    {
        this.damage.damage = damage;
        this.flySpeed = flySpeed;
        isAuto = auto;
        this.fatherCell = fatherCell;
        this.startDir = startDir;
        if (auto)
        {
            startDir = (target.position - transform.position).normalized;
            this.target = target;
        }
    }

    private void Update()
    {
        Vector3 v = Camera.main.WorldToScreenPoint(transform.position);

        if (nowPentrationCount < 0 || (v.x > Screen.width || v.x < 0 || v.y < 0 || v.y > Screen.height && !Click2ControlCell.Instance.JudgmentControl(fatherCell)))
        {
            End(3f);
        }

        if (nowPentrationCount >= 0 && Click2ControlCell.Instance.JudgmentControl(fatherCell))
        {
            Invoke("End2", 5f);
        }
    }

    private void FixedUpdate()
    {
        if (isAuto && nowPentrationCount == penetrationCount)
        {
            AutoTracking();
            SetRotation();
        }
        else if (!isAuto)
        {
            SetVelocity(startDir);
        }
        else if (isAuto && (target.gameObject is null || target.gameObject.activeSelf is false))
        {
            startDir = transform.up;
            isAuto = false;
        }
    }

    void End2()
    {
        End(0);
    }

    void End(float time)
    {
        gameObject.SetActive(false);
        Destroy(gameObject, time);
    }

    void AutoTracking()
    {
        //this.transform.position = Vector3.MoveTowards(this.transform.position, target.position, 1f);
        if (isAuto && (target.gameObject is null || target.gameObject.activeSelf is false))
        {
            isAuto = false;
            startDir = this.transform.up;
            SetVelocity(startDir);
        }
        else
        {
            Vector2 dir = target.position - this.transform.position;
            if (dir.magnitude < 0.3f)
            {
                isAuto = false;
            }
            SetVelocity(dir.normalized);
        }
    }

    void SetVelocity(Vector2 dir)
    {
        body.velocity = dir * flySpeed;
    }

    public void SetRotation()
    {
        //获取两点间方向
        Vector2 dir = target.transform.position - this.transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Quaternion tmp = Quaternion.Euler(0, 0, angle - 90);
        this.transform.rotation = Quaternion.Lerp(this.transform.rotation, tmp, 2f * Time.deltaTime);
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (nowPentrationCount >= 0)
        {
            if (collision != null && fatherCell.gameObject.tag != collision.gameObject.tag && collision.gameObject.GetComponent<CellAttribute>())
            {
                if (nowPentrationCount == 0)
                {
                    Damage tmp = new Damage();
                    tmp.damage = damage.damage + additionalDamage;
                    tmp.atkType = damage.atkType;
                    collision.gameObject.GetComponent<CellAttribute>().TakeDamage(tmp, fatherCell);
                    MusicMgr.GetInstance().PlaySound("3", false, 0.1f);//开始播放刺中音效
                }
                else
                {
                    collision.gameObject.GetComponent<CellAttribute>().TakeDamage(damage, fatherCell);
                    MusicMgr.GetInstance().PlaySound("3", false, 0.1f);//开始播放刺中音效
                }

                if (nowPentrationCount == penetrationCount)
                {
                    startDir = this.transform.up;
                }
                nowPentrationCount--;
            }
        }
    }
}
