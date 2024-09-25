using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JCellController : CellController
{
    private bool autoReady = true;

    #region 蓄力射击
    [SerializeField] protected int shootNum;
    [SerializeField] protected float preShootTime;

    IEnumerator Shoot()
    {
        if (!switchToHand)
        {
            MusicMgr.GetInstance().PlaySound("14", false, 0.1f);//开始播放发射蓄力音效
            yield return new WaitForSeconds(preShootTime);
            for (int i = 0; i < shootNum; i++)
            {
                GameObject arrow = ResMgr.GetInstance().Load<GameObject>("Arrow");//操作必须在代码中实现
                arrow.GetComponent<Arrow>().InitArrowData(data.atkValue, data.atkSpeed, this, Vector2.zero, true, target.transform);

                Vector2 selfPos = transform.position;
                Vector2 dir = transform.up;
                Vector2 offSet = 1f * dir;
                Vector2 center = selfPos + offSet;

                arrow.transform.position = center;
                arrow.transform.rotation = this.transform.rotation;
                MusicMgr.GetInstance().PlaySound("15", false, 0.1f);//开始播放发射音效
                if (i != shootNum)
                    yield return new WaitForSeconds(0.2f);
            }
            autoReady = true;
        }
        else
        {
            attackCD = true;
            MusicMgr.GetInstance().PlaySound("14", false, 0.1f);//开始播放发射蓄力音效
            yield return new WaitForSeconds(preShootTime);
            for (int i = 0; i < shootNum; i++)
            {
                GameObject arrow = ResMgr.GetInstance().Load<GameObject>("Arrow");//操作必须在代码中实现
                arrow.GetComponent<Arrow>().InitArrowData(data.atkValue, data.atkSpeed, this,transform.up);
                Vector2 selfPos = transform.position;
                Vector2 dir = transform.up;
                Vector2 offSet = 1f * dir;
                Vector2 center = selfPos + offSet;
                arrow.transform.position = center;
                arrow.transform.rotation = this.transform.rotation;
                MusicMgr.GetInstance().PlaySound("15", false, 0.1f);//开始播放发射音效
                if (i != shootNum)
                    yield return new WaitForSeconds(0.2f);
            }
            attackCD = false;
            switchToHand = false;
        }
    }

    public override void AttackForm()
    {
        if (target is null || target.activeSelf is false || autoReady is false)
        {
            return;
        }
        autoReady = false;
        StartCoroutine(Shoot());
    }
    #endregion 

    private bool attackCD = false;
    private bool switchToHand = false;

    public override void ControlAttack()
    {
        if(!attackCD)
        {
            switchToHand = true;
            StartCoroutine(Shoot());
        }
    }
}
