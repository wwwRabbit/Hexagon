using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ShooterCellController : CellController
{
    public override void AttackForm()
    {        
        if (target is null || target.activeSelf is false)
        {
            return;
        }

        GameObject arrow = ResMgr.GetInstance().Load<GameObject>("Arrow");
        arrow.GetComponent<Arrow>().InitArrowData(data.atkValue, data.atkSpeed, this, Vector2.zero, true, target.transform);

        MusicMgr.GetInstance().PlaySound("4", false,0.1f);//开始播放发射音效

        Vector2 selfPos = transform.position;
        Vector2 dir = transform.up;
        Vector2 offSet = 1f * dir;
        Vector2 center = selfPos + offSet;

        arrow.transform.position = center;
        arrow.transform.rotation = this.transform.rotation;
    }

    private float controllSpeedTimer=0f;
    public override void ControlAttack()
    {
        if(Time.time>controllSpeedTimer)
        {
            GameObject arrow = ResMgr.GetInstance().Load<GameObject>("Arrow");
            arrow.GetComponent<Arrow>().InitArrowData(data.atkValue, data.atkSpeed, this, transform.up);

            MusicMgr.GetInstance().PlaySound("4", false, 0.1f);//开始播放发射音效

            controllSpeedTimer = Time.time + selfAtkCD;

            Vector2 selfPos = transform.position;
            Vector2 dir = transform.up;
            Vector2 offSet = 1f * dir;
            Vector2 center = selfPos + offSet;

            arrow.transform.position = center;
            arrow.transform.rotation = this.transform.rotation;
        }
    }
}
