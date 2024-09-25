using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TC2CellAttribute : CellAttribute
{
    [Header("锁血时间") , SerializeField] float ineffectiveTime;
    float startTime;
    bool openClock = false;

    protected override void Preprocessing(Damage damage)
    {
        if ((CurrentBlood - damage.damage) <= Blood * 0.5 && !openClock)
        {
            startTime = Time.time;
            openClock = true;
            dontDamage = true;
        }
        else if (openClock && (Time.time - startTime) >= ineffectiveTime && dontDamage)
        {
            dontDamage = false;
        }
    }

}
