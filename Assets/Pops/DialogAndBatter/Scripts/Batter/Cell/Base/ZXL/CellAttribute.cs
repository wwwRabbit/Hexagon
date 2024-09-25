using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellAttribute : MonoBehaviour
{
    //一会再改，数值应该是从数据文件里读取
    [SerializeField] float blood;
    public float Blood => blood;
    [SerializeField] float currentBlood;
    public float CurrentBlood => currentBlood;

    [SerializeField] HealthBar healthBar;

    CellController cellSelf;
    public List<CellController> cells = new List<CellController>();    //记录攻击过来的单位，一会死亡后清空他们target的引用

    protected bool dontDamage = false;
    private void Awake()
    {
        cellSelf = this.transform.parent.GetComponent<CellController>();
        currentBlood = Blood;
    }

    //对于丢失目标这个处理，想到一种更简单的，就是在检测的时候，获取目标的同时就在目标里记录一下是谁获取的他，然后在目标死亡时，统一通知刷新目标就行
    //不用在攻击的时候才处理 是谁攻击的他 
    //这样的好处是，不用再在移动态 判断 目标是否丢失，再重新寻找

    private void Start()
    {
        healthBar.SetHealth(currentBlood, blood);
    }

    public void InitAttributeData(float health)
    {
        blood = health;
        currentBlood = blood;
    }

    public void TakeDamage(Damage damage, CellController cell)
    {
        Preprocessing(damage);

        if (dontDamage) return;

        if (!cells.Contains(cell))
        {
            cells.Add(cell);
        }

        //
        //TODO:伤害类型判断
        //

        currentBlood -= damage.damage;

        healthBar.SetHealth(currentBlood, blood);

        //print(gameObject.tag + "受伤");
    }

    protected virtual void Preprocessing(Damage damage)
    {

    }

    /// <summary>
    /// 清空持有的全部对立单位的相关引用
    /// </summary>
    public void ClearCells()
    {
        cells.Clear();
    }

    /// <summary>
    /// 标识是否可以进入死亡状态
    /// </summary>
    /// <returns></returns>
    public bool Can2Die()
    {
        if (currentBlood <= 0)
        {
            //清空全部对立单位的目标引用
            foreach (CellController item in cells)
            {
                item.target = null;
            }
            //清空自己的目标引用
            cellSelf.target = null;
            ClearCells();
            return true;
        }
        return false;
    }

}
