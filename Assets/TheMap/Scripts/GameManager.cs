using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public Tile Judian;

    public Tile[] tiles;
    public static GameManager instance;

    public unitControl selectUnit;
    public unitControl theone;

    //[Header("各个主管的好感度")]

    [SerializeField] private int Depart1;
    [SerializeField] private int Depart2;
    [SerializeField] private int Depart3;
    [SerializeField] private int Depart4;

    //修改各个主管好感度的方法
    /// <summary>
    /// 
    /// </summary>
    /// <param name="change">这个表示要修改几号主管</param>
    /// <param name="num">这个表示要修改多好好感度</param>
    public void ChangeFavor(int change,int num)
    {
        switch (change)
        {
            case 1:
                Depart1 = Depart1 + num;break;
            case 2:
                Depart2 = Depart2 + num;break;
            case 3:
                Depart3 = Depart3 + num; break;
            case 4:
                Depart4 = Depart4 + num; break;

        }


    }



    //List<Tile> openSet = new List<Tile>(); // 待探索的格子集合

    private void Awake()
    {
        if (instance==null)
        {
            instance = this;
        }
        else
        {
            if (instance!=this)
            {
                Destroy(gameObject);
            }
        }

    }

    private void Update()
    {
        if (selectUnit==null)
        {
            selectUnit = theone;
        }

        NextScene();
    }

    void NextScene()
    {
        
        if (Judian.preUnit().num > 0)
        {
            Debug.Log("1111111");
        }


       if ( Judian.preUnit().num<=0)
        {
            ChangeSceneManager.Instance.SwitchScene(4);
        }
    }



}
