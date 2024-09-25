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

        //// 计算每帧增加的角度，根据攻击速度和攻击范围确定
        //float angleIncrement = (data.atkRange > 0) ? startAngle / (Mathf.PI * data.atkRange * data.atkSpeed) : 0f;


        isAtk = true;

        #region   多段变化实现大小缩放（如有缩放动画将transform重新挂载到hitbox上）
        Vector3 scale1 = new Vector3(0.5f, 0.5f, 0.5f);
        Vector3 scale2 = new Vector3(1.5f, 1.5f, 1.5f);
        Vector3 scaleEnd = Vector3.one;

        float timer = 1f / data.atkSpeed;

        while (elapsedTime < timer)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime/timer;

            Vector3 transScale = Vector3.Lerp(transform.localScale, scale1, progress); // 根据时间进度计算新位置

            gameObject.transform.localScale = transScale;
            yield return null;
        }

        elapsedTime = 0;

        if (data.attackSound != null)
        {
            MusicMgr.GetInstance().PlaySound(data.attackSound, false, 0.1f);//开始播放攻击音效 
        }

        while (elapsedTime < timer)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / timer;

            Vector3 transScale = Vector3.Lerp(transform.localScale, scale2, progress); // 根据时间进度计算新位置

            gameObject.transform.localScale = transScale;
            yield return null;
        }

        elapsedTime = 0;

        while (elapsedTime < timer)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / timer;

            Vector3 transScale = Vector3.Lerp(transform.localScale, scaleEnd, progress); // 根据时间进度计算新位置

            gameObject.transform.localScale = transScale;
            yield return null;
        }
        #endregion

        isAtk = false;

        atkBox.SetActive(false);
        gzBuff.openDetect = false;
    }

}
