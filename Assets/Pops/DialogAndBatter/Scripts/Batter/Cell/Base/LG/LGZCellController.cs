using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LGZCellController : CellController
{
    protected override IEnumerator PerformAttack()
    {
        float elapsedTime = 0;
        atkBox.SetActive(true);
        atkBox.transform.localRotation = Quaternion.identity;

        //// ����ÿ֡���ӵĽǶȣ����ݹ����ٶȺ͹�����Χȷ��
        //float angleIncrement = (data.atkRange > 0) ? startAngle / (Mathf.PI * data.atkRange * data.atkSpeed) : 0f;

        Vector3 dir = (target.transform.position - this.transform.position).normalized;
        MusicMgr.GetInstance().PlaySound("20", false, 0.1f);

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
}
