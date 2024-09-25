using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Push2 : Push
{
    [HideInInspector] public JSCell2Controller cell;

    protected override void DoDamage(Collider2D collider)
    {
        // collider.gameObject.GetComponent<CellAttribute>().TakeDamage(cell.damage, cell);
        if (!cell.controlTarget.Contains(collider.GetComponentInChildren<CellAttribute>()))
        {
            cell.controlTarget.Add(collider.GetComponentInChildren<CellAttribute>());
        }
        collider.GetComponentInChildren<CellAttribute>().TakeDamage(cell.damage, cell);
        cell.ReduceAtkCD();
    }
}
