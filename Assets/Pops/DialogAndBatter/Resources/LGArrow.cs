using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LGArrow : Arrow
{
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null && fatherCell.gameObject.tag != collision.gameObject.tag && collision.gameObject.GetComponent<CellAttribute>())
        {
            Damage tmp = new Damage();
            tmp.damage = damage.damage + additionalDamage;
            tmp.atkType = damage.atkType;
            collision.gameObject.GetComponent<CellAttribute>().TakeDamage(tmp, fatherCell);
            MusicMgr.GetInstance().PlaySound("19", false, 0.1f);//Éú³ÉÐ¡Ï¸°û
            nowPentrationCount--;
            Debug.Log(target.position);
            //Vector2 vec = BatterManager.Instance.GenerateRandomPosition(target.position,0.7f);
            //BatterManager.Instance.GenerateEnmForLG(vec, "Cell_LGZ_Enemy_Unit");
        }
    }
}
