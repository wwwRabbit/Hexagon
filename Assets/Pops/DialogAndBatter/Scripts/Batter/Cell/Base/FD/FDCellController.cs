using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows;

public class FDCellController : CellController
{
    //��ϸ����Ҫ��д�ƶ�������ģʽ��������������Ҫ����Ƿ���٣���ͬʱ����ʱ��������ϸ��(�ݶ�������)

    private float currentSpeed;
    [SerializeField] protected CellAttribute cellAttribute;
    [SerializeField] protected float accelarateL;

    protected override void Awake()
    {
        base.Awake();
        currentSpeed = data.moveSpeed;
        controllSpeed = data.moveSpeed;
    }

    #region ����&&�˶�&&������˶��йص���Ч
    protected override void Update()
    {
        pos = transform.position;
        if (prepos != null && pos != null)
        {
            float distance = Vector3.Distance(pos, prepos);

            if (isAtk && audioEnable && data.moveSound == "8")//����ʱֱ��ȡ����Ч
            {
                Debug.Log("Move Sound remove trriger");
                DestoryAudio(data.moveSound);
                audioEnable = false;
                prepos = pos;
            }

            if (distance < data.moveSpeed * Time.deltaTime )//δ�ƶ�Ҳȡ����Ч
            {
                Debug.Log("Move Sound remove trriger");
                DestoryAudio(data.moveSound);
                audioEnable = false;
                prepos = pos;
            }

            if (data.moveSound == "8" && !audioEnable && distance > moveSoundDis && !isAtk)
            {
                MusicMgr.GetInstance().PlaySound("8", false, 0.1f);//��ʼ����FD�߶���Ч
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

    #region ��д�Զ�����
    protected override IEnumerator PerformAttack()
    {
        float elapsedTime = 0;
        atkBox.SetActive(true);
        atkBox.transform.localRotation = Quaternion.identity;

        //// ����ÿ֡���ӵĽǶȣ����ݹ����ٶȺ͹�����Χȷ��
        //float angleIncrement = (data.atkRange > 0) ? startAngle / (Mathf.PI * data.atkRange * data.atkSpeed) : 0f;

        if (data.attackSound != null && data.attackSound == "1")
        {
            MusicMgr.GetInstance().PlaySound("1", false, 0.1f);//��ʼ���Ź�����Ч 
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

            //ͨ����ǰ�����޸�λ�ã�ʵ�ֶ�һ�µ�Ч��
            transform.position = origin + dir * anit;

            // �����ӹ����е���ת�Ƕȣ��� y �Ḻ������ࣩ��ʼ���һӶ�
            float currentAngle = Mathf.Lerp(endAngle, startAngle, t); // ��-90�ȿ�ʼ���һӶ�����90�Ƚ���
            Vector3 rotation = new Vector3(0f, 0f, currentAngle);
            atkBox.transform.localRotation = Quaternion.Euler(rotation);
            yield return null;
        }

        isAtk = false;

        // �������е���ת����Ϊ��ʼ״̬
        atkBox.transform.localRotation = Quaternion.identity;
        atkBox.SetActive(false);
    }
    #endregion

    #region ��Ҳٿ��߼�

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

        //// ����ÿ֡���ӵĽǶȣ����ݹ����ٶȺ͹�����Χȷ��
        //float angleIncrement = (data.atkRange > 0) ? startAngle / (Mathf.PI * data.atkRange * data.atkSpeed) : 0f;

        if (data.attackSound != null && data.attackSound == "1")
        {
            MusicMgr.GetInstance().PlaySound("1", false, 0.1f);//��ʼ���Ź�����Ч 
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

            // �����ӹ����е���ת�Ƕȣ��� y �Ḻ������ࣩ��ʼ���һӶ�
            float currentAngle = Mathf.Lerp(endAngle, startAngle, t); // ��-90�ȿ�ʼ���һӶ�����90�Ƚ���
            Vector3 rotation = new Vector3(0f, 0f, currentAngle);
            atkBox.transform.localRotation = Quaternion.Euler(rotation);
            yield return null;
        }

        isAtk = false;

        // �������е���ת����Ϊ��ʼ״̬
        atkBox.transform.localRotation = Quaternion.identity;
        atkBox.SetActive(false);
    }

    #endregion
}
