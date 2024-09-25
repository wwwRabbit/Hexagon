using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBox : MonoBehaviour
{
    CellController cell;

    public Damage damage;

    private void Awake()
    {
        cell = transform.parent.parent.GetComponent<CellController>();
    }

    private void OnEnable()
    {
        //print("攻击框激活");
    }

    //private void OnDisable()
    //{
    //    damage = null;
    //    cell = null;
    //}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //print("触发检测调用");
        //Debug.Log($"层级{collision.gameObject.layer}, need {LayerMask.NameToLayer("Hurt")}, tag1 {gameObject.tag}, tag2 {collision.gameObject.tag}");
        if (collision.gameObject.layer == LayerMask.NameToLayer("Hurt") && collision.gameObject.tag != gameObject.tag)
        {
            print($"{cell.transform.gameObject.name} 对{collision.transform.parent.name}，造成伤害 {damage.damage}");
            collision.gameObject.GetComponent<CellAttribute>().TakeDamage(damage, cell);
        }
    }
}
