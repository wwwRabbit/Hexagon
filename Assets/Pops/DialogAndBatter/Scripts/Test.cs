using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : CellController
{
    private int count;
    public override void Attack()
    {
        if (!Preprocessing())
        {
            return;
        }

        if (MainTest.stopAction)
        {
            return;
        }

        if (tmp > 0)
        {
            tmp = 0;
            return;
        }

        //����ȴʱ����ʱ����AttackЯ�̡�
        if (!isAtk)
        {
            AttackForm();
            lastAtkTime = Time.time;
        }
    }
}
