using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimplEnemy : unitControl
{
    //public string targetTag = "Player";//检测的目标tag

    //public float detectionRadius; // 检测半径  
    //public LayerMask detectionLayerMask; // 设置检测的层
    
    public Vector2 hex;//这个用来记录现在的六边形字典坐标

    public Dictionary<Vector2, IMapUnit> allUnits = new Dictionary<Vector2, IMapUnit>();//存所有地图的信息格子

    public List<Vector2> Message = new List<Vector2>();

   // public int infectRange;//检测半径

   // List<Tile> ner = new List<Tile>();//一个用来存储周围的格子的列表 
    //public float infectRange;//病毒的检测半径

    //获取所有的地图格子字典字典
    void GetList()
    {
        for (int i = 0; i < GameManager.instance.tiles.Length; i++)
        {
            // print(GameManager.instance.tiles[i].HexPos);
            allUnits.Add(GameManager.instance.tiles[i].HexPos, GameManager.instance.tiles[i]);
        }
        

    }

    //用新的格子字典，来拿取到我们周围的信息
    //void CheckHex()
    //{
    //   Message= MapToolBox.FindAllPointsForFight(hex, infectRange, allUnits);
    //}

    void SendMessagetoAttack()
    {

       //先暂定现在这么个情况就是，周围只有一个我方的中性粒细胞单位，这个单位的兵力是5，这个地块有一个给我方单位加血的BUFF；
        //周围的我方单位是1个，然后这个兵力的单位是5
        int Unitnumber = 1;
        int Cellnumber = 5;

        int unittag = 1;//代号1是中性粒细胞
        //地块上面的BUFF
        bool HpAdd = true;
    }



    //public void CheckTag()
    //{
    //    // 获取当前对象的位置  
    //    Vector2 center = transform.position;

    //    // 创建一个与检测半径相匹配的大小的边界框  
    //    //Collider2D[] colliders = Physics2D.OverlapBox(center, new Vector2(detectionRadius, detectionRadius), 0f, detectionLayerMask);
    //    Collider2D[] colliders = Physics2D.OverlapCircleAll(center, detectionRadius, detectionLayerMask);

    //    // 遍历检测到的碰撞体  
    //    foreach (Collider2D collider in colliders)
    //    {
    //        // 检查碰撞体是否有我们感兴趣的tag  
    //        if (collider.CompareTag(targetTag))
    //        {
    //            Debug.Log("Detected a 2D object with tag: " + targetTag);
    //            // 在这里添加你的逻辑，比如调用函数、改变状态等。  
    //        }
    //    }
    //}


    //这个方式是，检测自己周围距离地块里面，有没有含有我方单位的
    //public void CheckTiles()
    //{
    //    for (int i = 0; i < GameManager.instance.tiles.Length; i++)
    //    {
    //        float neiX = GameManager.instance.tiles[i].transform.position.x;//周围格子的X坐标
    //        float neiY = GameManager.instance.tiles[i].transform.position.y;//周围格子的Y坐标

    //        float Mx = this.transform.position.x;
    //        float My = this.transform.position.y;
    //       // Debug.Log(GameManager.instance.tiles[i].pos);

    //        //计算坐标之间的距离；
    //        float distance = Mathf.Sqrt((neiX - Mx) * (neiX - Mx) + (neiY - My) * (neiY - My));
    //        if (distance <= infectRange)
    //        {
    //            if (!GameManager.instance.tiles[i].canwalk)
    //            {
    //                if (CheckList(GameManager.instance.tiles[i],ner))
    //                {
    //                    ner.Add(GameManager.instance.tiles[i]);
    //                }                                       
    //            }
    //        }
    //    }
    //    Debug.Log("列表长度为：" + ner.Count);


    //    for (int i = 0; i < ner.Count; i++)
    //    {
    //        Debug.Log(ner[i].unittag);
    //        ner[i].ChangeColor(1);
    //       // UiManager.instance.ShowWillAttack(this.transform.position);
    //    }
    //    //int length= ner.Count;
    //    //ner[0].ShowWhereCanWalk();
    //    //Debug.Log("列表长度为：" + length); // 在控制台输出列表的长度
    //}

    //public void AttackTrigger()
    //{
    //    UiManager.instance.ShowWillAttack(this.transform.position);
    //}



    //
    /// <summary>
    /// 我在这里写一个方法来检测我们即将要往列表里面添加的元素，在这个列表里面是不是已经存在了，如果已经存在了，那么就返回一个值
    /// </summary>
    /// <param name="x">输入的元素</param>
    /// <param name="y">要检索的列表</param>
    //bool CheckList(Tile x,List<Tile> y)
    //{
    //    for (int i = 0; i < y.Count; i++)
    //    {
    //        if (x==y[i])
    //        {
    //            return false;
    //        }
    //    }
    //    return true;
    //}


}
