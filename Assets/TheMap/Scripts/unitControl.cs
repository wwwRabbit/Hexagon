using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class unitControl : MonoBehaviour,cellUnit
{
    //public bool Judian=false;//检测是不是据点的开关
    public bool boss=false;
    
    //public Color startColor; // 初始颜色  
    public Color targetColor; // 目标颜色  
    public float duration = 5.0f; // 变化持续时间（秒）  
    private bool isColorChanging = false; // 标记是否正在改变颜色  
    private Color startColor; // 起始颜色  



    [SerializeField] [Range(1, 8)] private int moveRange;//移动距离
    [SerializeField] private float moveSpeed;

    private Rigidbody2D rb;

    private Vector3 initialScale; // 初始尺寸
    private float scaleFactor = 1.5f; // 缩放因子，即每次点击后的缩放比例@

    [Header("这个是区分这个细胞是哪一种细胞的")]
    public CellType MyType;//这个是来判断自己是什么样的细胞
    [Header("这个是区分这个细胞是敌方还是我方的")]
    public cellTag FrE;//这个标签是用来区分是我放还是地方的 
   // public Vector2 hex;//用来记录自己所在的格子的六边形坐标

    //public TheUnitTag MyUnit;//这个标签是用来判断自己是何种细胞，是TC，还是中性粒，还是巨噬

    public int num = 5;//单位上面的细胞数量

    [Header("TextFollow to show number")]
    public GameObject textPrefab;
    GameObject textObj;
    TextFollow textFollow;


    // Start is called before the first frame update
    void Start()
    {

        //Judian = false;
        startColor = GetComponent<Renderer>().material.color;
        LogColor();

        moveRange = 1;
        moveSpeed = 1.0f;
        initialScale = transform.localScale; // 记录初始尺寸
        rb = GetComponent<Rigidbody2D>(); // 获取物体的Rigidbody2D组件

        //生成一个跟随的文字组件
        Canvas canvas = FindObjectOfType<Canvas>();
        textObj = Instantiate(textPrefab,canvas.gameObject.transform);
        textFollow = textObj.GetComponent<TextFollow>();
        textFollow.gameCamera = Camera.main;
        textFollow.target = transform;
        //changeNum(5);
        setNum(num);
    }

    //这个方法，是给细胞一个出场的时候有渐变色的效果
    void LogColor()
    {
        if (targetColor != Color.clear && targetColor != Color.black) // 你可以设置其他默认颜色  
        {
            isColorChanging = true;
            StartCoroutine(LerpColor());
        }

    }

    IEnumerator LerpColor()
    {
        float lerpT = 0.0f;

        while (lerpT < 1f && isColorChanging)
        {
            // 线性插值颜色  
            Color color = Color.Lerp(startColor, targetColor, lerpT);
            GetComponent<Renderer>().material.color = color;

            // 等待一小段时间  
            lerpT += Time.deltaTime / duration;
            yield return null;
        }

        // 完成后设置最终颜色（以防浮点误差）  
        if (isColorChanging)
        {
            GetComponent<Renderer>().material.color = targetColor;
        }
    }



    // Update is called once per frame
    void Update()
    {

    }

    private void OnDestroy()
    {
        Destroy(textObj);
    }

    //public void Moveto(Vector3 _this,Vector3 _trans)
    //{
    //    StartCoroutine(MoveCo1(_this, _trans));
    //    transform.localScale = initialScale;
    //    ResetTiles();
    //}
    //IEnumerator MoveCo1(Vector3 _this,Vector3 _trans)
    //{

    //    while (_this != _trans)
    //    {
    //        transform.position = Vector3.MoveTowards(_this, new Vector3(_trans.x, _trans.y, transform.position.z), moveSpeed * Time.deltaTime);
    //        yield return new WaitForSeconds(0);
    //    }

    //}



    public void Move(Vector3 _trans)
    {
        StartCoroutine(MoveToTarget(_trans));
        Debug.Log("我为什么不动》");
        //StartCoroutine(MoveCo(_trans));
        //transform.localScale = initialScale;
        //ResetTiles();
    }


    IEnumerator MoveToTarget(Vector3 targetPosition)
    {
        Vector3 currentPosition = transform.position;

        while (Vector2.Distance(currentPosition, targetPosition) > 0.01f) // 当物体距离目标还有一定距离时继续移动  
        {
            // 计算朝向目标的方向  
            Vector2 direction = (targetPosition - currentPosition).normalized;
            // 计算移动量  
            Vector2 moveAmount = direction * moveSpeed * Time.deltaTime;
            // 移动物体  
            transform.Translate(moveAmount);
            //Debug.Log("这里动了");
            // 更新当前位置  
            currentPosition = transform.position;
            //Debug.Log("时间："+ Time.deltaTime+ "速度："+ moveSpeed + " 移动方向: "+ direction + "移动距离："+ moveAmount+" 当前位置："+currentPosition);
            // 等待下一帧  
            yield return null;
        }
    }

    IEnumerator MoveCo(Vector3 _trans)
    {

        while (transform.position != _trans)
        {
            Debug.Log("ismoving");
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(_trans.x, _trans.y, _trans.z), moveSpeed * Time.deltaTime);
            yield return new WaitForSeconds(0);
        }

    }

    private void ResetTiles()
    {
        for (int i = 0; i < GameManager.instance.tiles.Length; i++)
        {
            GameManager.instance.tiles[i].ChangeColor(0);
        }
    }

    public void OnMouseDown()
    {
        //Debug.Log("点下去了");
        //GameManager.instance.selectUnit = this;
        //ScaleObject(); // 缩放游戏物体
    }

    private void OnMouseEnter()
    {
        //Debug.Log("进去了");
        //ScaleObject(); // 缩放游戏物体

    }

    //用以缩小游戏物体
    void ScaleObject()
    {
        // 计算缩放后的尺寸
        Vector3 newScale = initialScale * scaleFactor;
        // 缩放游戏物体
        transform.localScale = newScale;
    }

   public void ShowWalkableTiles()
    {
        for (int i = 0; i < GameManager.instance.tiles.Length; i++)
        {
            float neiX = GameManager.instance.tiles[i].transform.position.x;//周围格子的X坐标
            float neiY = GameManager.instance.tiles[i].transform.position.y;//周围格子的Y坐标
                                                                            //自己的坐标，方便计算，拿出来
                                                                            //float Mx = this.transform.position.x;
                                                                            //float My = this.transform.position.y;

            float Mx = GameManager.instance.selectUnit.transform.position.x;
            float My = GameManager.instance.selectUnit.transform.position.y;

            //计算坐标之间的距离；
            float distance = Mathf.Sqrt((neiX - Mx) * (neiX - Mx) + (neiY - My) * (neiY - My));
            if (distance <= moveRange + 0.5f)
            {
                if (GameManager.instance.tiles[i].canwalk)
                {
                    //GameManager.instance.tiles[i].HighLightTiles();
                    GameManager.instance.tiles[i].ShowWhereCanWalk();
                }
            }


        }
    }

    public cellTag returnTag()
    {
        return FrE;
    }

    public bool compareUnit(cellUnit other)
    {
        throw new System.NotImplementedException();
    }

    public int returnNum()
    {
        return num;
    }

    public int changeNum(int modifier)
    {
        num = Mathf.Max(num+modifier,0);
        textFollow.UpdateUnitCount(num);
        return num;
    }

    public int setNum(int toNum)
    {
        num = Mathf.Max(toNum, 0);
        if(textFollow!=null)textFollow.UpdateUnitCount(num);
        return num;
    }

    //这里来返回自己的具体细胞类型，是怎么样子的，三种，TC，中性粒，巨噬
    public CellType returnSpcCelltag()
    {
        return MyType;
        //throw new System.NotImplementedException();
    }
}
