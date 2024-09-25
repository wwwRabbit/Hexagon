using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZXL2CellController : CellController
{
    bool isFirstAttack = false;
    float firstAtkTime;
    [Header("恢复时间"), SerializeField] float recoverTime = 2f;
    [Header("第一次投掷攻击半径"), SerializeField] float atkRadius = 6f;
    bool t;
    protected override void Start()
    {
        base.Start();

        //首先重置攻击范围
        rangeSensor.atkRadius = atkRadius;
    }

    protected override bool Preprocessing()
    {
        if (!isFirstAttack)
        {
            isFirstAttack = true;
            firstAtkTime = Time.time;

            if (!haveFather)
            {
                string type = "Enemy";
                if (gameObject.tag == "Soldier")
                {
                    type = "Solider";
                    string name = "Cell_ZXL_M_" + type + "_Unit";
                    BatterManager.Instance.GenerateBelowMap(name, true);
                }
                else
                {
                    string name = "Cell_ZXL_M_" + type + "_Unit";
                    BatterManager.Instance.GenerateAboveMap(name, true);
                }
            }

            GameObject arrow = ResMgr.GetInstance().Load<GameObject>("Arrow");
            arrow.GetComponent<Arrow>().InitArrowData(data.atkValue, data.atkSpeed, this, transform.up);

            Vector2 selfPos = transform.position;
            Vector2 dir = transform.up;
            Vector2 offSet = 1f * dir;
            Vector2 center = selfPos + offSet;

            arrow.transform.position = center;
            arrow.transform.rotation = this.transform.rotation;
            

            //rangeSensor.OpenOrCloseDetect(false);

        }
        else if (Time.time - firstAtkTime >= recoverTime && isFirstAttack && !t)
        {
            //print("123");
            //rangeSensor.OpenOrCloseDetect(true);
            //改回原来的攻击范围

            //这写的比较仓促

            rangeSensor.atkRadius = data.atkRange;
            tmp = 1;
            t = true;
            return true;
        }

        if (t) return true;

        return false;
    }

    public override void ControlAttack()
    {
        Attack();
    }
}
