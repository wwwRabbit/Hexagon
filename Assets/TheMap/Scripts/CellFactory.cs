using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellFactory : MonoBehaviour
{
    public GameObject CellPrefb1;//为了实验测试用的细胞1，现在就当是TC细胞了

    public GameObject CellPrefb2;//预制体2，现在是中性粒细胞

    public GameObject CellPrefb3;//预制体3，现在是巨噬细胞

    public GameObject CellPrefb4;//肥大细胞

    public GameObject CellPrefb5;//浆细胞

    public static CellFactory instance;

    private void Awake()
    {
        if (instance==null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            if (instance != this)
            {
                Destroy(gameObject);
            }
        }
        
    }

    public GameObject ProducePrefb(IMapUnit n,CellType _cell)
    {
        GameObject TheCell=CellPrefb1
            ;
        Vector3 AddCellPos = n.thisPos();
        
        if (_cell==CellType.Cell_TC)
        {
              TheCell = CellPrefb1;
            //return TheCell1;
        }
        else if (_cell==CellType.Cell_ZXL)
        {
            TheCell= CellPrefb2;
            //return TheCell2;
        }
        else if (_cell==CellType.Cell_JS)
        {
            TheCell = CellPrefb3;
            //return TheCell3;
        }
        else if (_cell==CellType.Cell_FD)
        {
            TheCell = CellPrefb4;
        }
        else if (_cell==CellType.Cell_J)
        {
            TheCell = CellPrefb5;
        }
        


        GameObject NewAddCell = Instantiate(TheCell, AddCellPos, Quaternion.identity); 

        return NewAddCell;
        

    }
    /// <summary>
    /// 用来生成敌人的方法
    /// </summary>
    /// <param name="pos"<>这里传入的是要生成的地方的格子</param>
    /// <param name="x">这个编号是代表生成的是什么细胞，因为敌人生成是随机的，所以在外界取随机数来传进去</param>
    /// <returns></returns>
    public GameObject ProduceEnemy(Tile Pos,int x)
    {
        Vector3 _pos = Pos.transform.position;
        //GameObject TheCell = CellPrefb1;
        if (x==2)
        {
            GameObject NewEnemy = Instantiate(CellPrefb1, _pos, Quaternion.identity);
            unitControl enemy = NewEnemy.GetComponent<unitControl>();//在这里拿到新生成敌人的脚本，来改变它的信息
            Pos.ChangeUnit(enemy);//这里，把敌人这个单位，给赋到格子脚本上
            enemy.MyType = CellType.Cell_TC;//这里给敌人打上标签是：敌人方的TC细胞
            enemy.FrE = cellTag.enemy;//给敌人打上敌人的识别标签
            enemy.targetColor = Color.red;
            return NewEnemy;
        }
        else if (x==3)
        {
            GameObject NewEnemy = Instantiate(CellPrefb2, _pos, Quaternion.identity);
            unitControl enemy = NewEnemy.GetComponent<unitControl>();//在这里拿到新生成敌人的脚本，来改变它的信息
            Pos.ChangeUnit(enemy);
            enemy.MyType = CellType.Cell_ZXL;//这里给生成的敌人打上标签是敌人的中性粒细胞
            enemy.FrE = cellTag.enemy;//给敌人打上敌人的识别标签
            enemy.targetColor = Color.red;

            return NewEnemy;
        }
        else
        {
            GameObject NewEnemy = Instantiate(CellPrefb3, _pos, Quaternion.identity);
            unitControl enemy = NewEnemy.GetComponent<unitControl>();//在这里拿到新生成敌人的脚本，来改变它的信息
            Pos.ChangeUnit(enemy);
            enemy.MyType = CellType.Cell_JS;//这里给生成的敌人打上标签是敌人的巨噬细胞
            enemy.FrE = cellTag.enemy;//给敌人打上敌人的识别标签
            enemy.targetColor = Color.red;
            return NewEnemy;
        }

       // return NewEnemy;
    }



}
