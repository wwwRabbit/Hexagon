using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ProduceEnemy : MonoBehaviour,IVirusExpandController
{
    public float infectRange;//生成细胞的范围半径

    List<Tile> ner = new List<Tile>();//用以存储检测的到的周围的格子

    public GameObject EnemyPrefab;//我们的敌人预制体

  //  bool isAddAll = false;//让方法执行一次的开关

    //2024年04月26日 17:44:05 拿到地图之后，对昨天的生成敌人进行修改
    //现在，我们要生成敌人的地方是固定死了的，不需要再去检测了。在这里，我再去弄一个列表，去专门存储这些会生成敌人的地块



    public List<Tile> EnemyPos = new List<Tile>();//这个列表把所有会生成敌人的地块存下来，这里不用像老方法那样，再检测了，这个直接拖进去就好了

    //敌人生成已经做好了，每回合调用一下这个 Produce 方法，就可以在几个预设的地方给生成这些预设好的敌人了

    //2024年04月29日 19:34:37 现在基于一种新的方法来拿到我们的地图的




    // Start is called before the first frame update
    void Start()
    {
        //infectRange = 1.5f;
        // Debug.Log(EnemyPos.Count);
       // ShowInfectColor();
       // GetHexagon();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //写这个方法，是为了让被感染的细胞显示出感染的颜色
    //用处有限，不用了
    //private void ShowInfectColor()
    //{
    //    for (int i = 0; i < EnemyPos.Count; i++)
    //    {
    //        EnemyPos[i].ChangeColor(2);
    //    }
    //}

    //用这个方法来拿到周围所有可以生成游戏物体的格子
    //public void GetHexagon()
    //{
    //    for (int i = 0; i < GameManager.instance.tiles.Length; i++)
    //    {
    //        float neiX = GameManager.instance.tiles[i].transform.position.x;//周围格子的X坐标
    //        float neiY = GameManager.instance.tiles[i].transform.position.y;//周围格子的Y坐标

    //        float Mx = this.transform.position.x;
    //        float My = this.transform.position.y;

    //        //计算坐标之间的距离；
    //        float distance = Mathf.Sqrt((neiX - Mx) * (neiX - Mx) + (neiY - My) * (neiY - My));
    //        if (distance <= infectRange)
    //        {
    //            if (GameManager.instance.tiles[i].canwalk)
    //            {
    //                ner.Add(GameManager.instance.tiles[i]);

    //            }
    //        }
    //    }
    //    Debug.Log("列表长度为：" + ner.Count);

    //    //Produce();
    //    for (int i = 0; i <+ ner.Count; i++)
    //    {            //Debug.Log(ner[i].unittag);
    //       ner[i].ChangeColor(1);
    //    }

    //}

    //拿到周围的格子，还需要在这些格子上面生成我们的游戏预制体
    public void Produce()
    {

        int max = EnemyPos.Count;
        int inf = UnityEngine.Random.Range(0, max);//取所有能拿到的物件的下标，然后取随机数;

        if (EnemyPos.Count==0)
        {
            Debug.Log("已经没有地块可以生成了");
            return;
        }

        //现在我们拿到那个随机出来的地块格子，那个格子就是这个，顺便把位置存下来
        Vector3 ProducePosition =EnemyPos[inf].transform.position;

        //在这里存一下这个格子的六边形坐标
      //  Vector2 hpos = EnemyPos[inf].HexPos;

        //搞个随机数，是来随机敌人的种类，敌人是三种，就让随机数从1到3进行随机 1是Tc,2是中性粒，3是巨噬
        int enem = UnityEngine.Random.Range(1,4 );
       
        //这里做到了生成敌人细胞，敌人的预制体和我方细胞的完全相同，仅仅是在刚刚生成的时候，给它们进行了标签的赋值
        GameObject NewEnemy = CellFactory.instance.ProduceEnemy(EnemyPos[inf], enem);
        //EnemyPos[inf].ChangeUnit();
        //下面的操作，把已经循环出来的格子从表里退出来
        EnemyPos.Remove(EnemyPos[inf]);
        max--;


    }

    public void UpdateBar(int curTurn)
    {
        throw new NotImplementedException();
    }

    //这个的存在是允许敌人进行多次生成
    public void InstantiateEnemy()
    {
       // throw new NotImplementedException();
    }

    //检验敌人是否生成
    public bool InstantiateEnd()
    {
        return true;
        //throw new NotImplementedException();
    }

    public bool GameEnd()
    {
        return true;
        //throw new NotImplementedException();
    }
}
