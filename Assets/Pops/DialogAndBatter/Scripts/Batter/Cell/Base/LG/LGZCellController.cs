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

        //// 计算每帧增加的角度，根据攻击速度和攻击范围确定
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
}
