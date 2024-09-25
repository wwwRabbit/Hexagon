using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShooterCellControl2 : CellController
{
    public override void AttackForm()
    {
        if (target is null || target.activeSelf is false)
        {
            return;
        }

        GameObject arrow = ResMgr.GetInstance().Load<GameObject>("Arrow1");
        arrow.GetComponent<Arrow>().InitArrowData(data.atkValue, data.atkSpeed, this, Vector2.zero, true, target.transform);

        Vector2 selfPos = transform.position;
        Vector2 dir = transform.up;
        Vector2 offSet = 1f * dir;
        Vector2 center = selfPos + offSet;

        arrow.transform.position = center;
        arrow.transform.rotation = this.transform.rotation;
    }

    public override void ControlAttackForm()
    {
        GameObject arrow = ResMgr.GetInstance().Load<GameObject>("Arrow1");
        arrow.GetComponent<Arrow>().InitArrowData(data.atkValue, data.atkSpeed, this, transform.up);

        Vector2 selfPos = transform.position;
        Vector2 dir = transform.up;
        Vector2 offSet = 1f * dir;
        Vector2 center = selfPos + offSet;

        arrow.transform.position = center;
        arrow.transform.rotation = this.transform.rotation;
    }
}
