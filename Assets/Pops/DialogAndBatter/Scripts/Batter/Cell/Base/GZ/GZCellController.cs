using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GZCellController : CellController
{
    // Start is called before the first frame update
    [SerializeField] protected GZBuff gzBuff;

    protected override IEnumerator PerformAttack()
    {
        float elapsedTime = 0;
        atkBox.SetActive(true);
        gzBuff.openDetect = true;
        atkBox.transform.localRotation = Quaternion.identity;

        //// ����ÿ֡���ӵĽǶȣ����ݹ����ٶȺ͹�����Χȷ��
        //float angleIncrement = (data.atkRange > 0) ? startAngle / (Mathf.PI * data.atkRange * data.atkSpeed) : 0f;


        isAtk = true;

        #region   ��α仯ʵ�ִ�С���ţ��������Ŷ�����transform���¹��ص�hitbox�ϣ�
        Vector3 scale1 = new Vector3(0.5f, 0.5f, 0.5f);
        Vector3 scale2 = new Vector3(1.5f, 1.5f, 1.5f);
        Vector3 scaleEnd = Vector3.one;

        float timer = 1f / data.atkSpeed;

        while (elapsedTime < timer)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime/timer;

            Vector3 transScale = Vector3.Lerp(transform.localScale, scale1, progress); // ����ʱ����ȼ�����λ��

            gameObject.transform.localScale = transScale;
            yield return null;
        }

        elapsedTime = 0;

        if (data.attackSound != null)
        {
            MusicMgr.GetInstance().PlaySound(data.attackSound, false, 0.1f);//��ʼ���Ź�����Ч 
        }

        while (elapsedTime < timer)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / timer;

            Vector3 transScale = Vector3.Lerp(transform.localScale, scale2, progress); // ����ʱ����ȼ�����λ��

            gameObject.transform.localScale = transScale;
            yield return null;
        }

        elapsedTime = 0;

        while (elapsedTime < timer)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / timer;

            Vector3 transScale = Vector3.Lerp(transform.localScale, scaleEnd, progress); // ����ʱ����ȼ�����λ��

            gameObject.transform.localScale = transScale;
            yield return null;
        }
        #endregion

        isAtk = false;

        atkBox.SetActive(false);
        gzBuff.openDetect = false;
    }

}
