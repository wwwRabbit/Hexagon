using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LGCellController : CellController
{
    public override void AttackForm()
    {
        if (target is null || target.activeSelf is false)
        {
            return;
        }

        GameObject arrow = ResMgr.GetInstance().Load<GameObject>("LGArrow");
        arrow.GetComponent<Arrow>().InitArrowData(data.atkValue, data.atkSpeed, this, Vector2.zero, true, target.transform);

        MusicMgr.GetInstance().PlaySound("17", false, 0.1f);//开始播放发射音效

        Vector2 selfPos = transform.position;
        Vector2 dir = transform.up;
        Vector2 offSet = 1f * dir;
        Vector2 center = selfPos + offSet;

        arrow.transform.position = center;
        arrow.transform.rotation = this.transform.rotation;
    }

}
