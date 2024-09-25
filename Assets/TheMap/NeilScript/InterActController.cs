using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterActController : MonoBehaviour, ICardObserver
{
    public Rect monitorArea = new Rect(10, 10, 100, 100);
    public HexManager manager;
    public GameObject cameraObj;
    public ICameraControl cameraC;

    MapScreenModes curMode = MapScreenModes.idle;

    bool justIn = true;
    bool confirmUse = false;
    bool noticed = false;
    Vector2 mouseClickPos;
    GeneralCard curCard;

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red; // 设置Gizmos颜色为红色

        // 因为Rect是基于屏幕坐标的，需要转换为世界坐标
        Vector3 bottomLeft = Camera.main.ScreenToWorldPoint(new Vector3(monitorArea.x, monitorArea.y, Camera.main.nearClipPlane));
        Vector3 topLeft = Camera.main.ScreenToWorldPoint(new Vector3(monitorArea.x, monitorArea.y + monitorArea.height, Camera.main.nearClipPlane));
        Vector3 bottomRight = Camera.main.ScreenToWorldPoint(new Vector3(monitorArea.x + monitorArea.width, monitorArea.y, Camera.main.nearClipPlane));
        Vector3 topRight = Camera.main.ScreenToWorldPoint(new Vector3(monitorArea.x + monitorArea.width, monitorArea.y + monitorArea.height, Camera.main.nearClipPlane));

        // 绘制矩形
        Gizmos.DrawLine(bottomLeft, topLeft);
        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(topRight, bottomRight);
        Gizmos.DrawLine(bottomRight, bottomLeft);
    }

    #region 单例
    public static InterActController _instance;

    void Awake()
    {
        // 确保只有一个实例存在
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            // 如果已存在一个实例而且不是当前实例，则销毁当前GameObject
            if (this != _instance)
            {
                Destroy(this.gameObject);
            }
        }
    }
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        //curCard = new DoubleBuffer(manager, this);
        //curCard = new NormalMove(manager, this);
        cameraC = cameraObj.GetComponent<ICameraControl>();
    }

    // Update is called once per frame
    void Update()
    {

        UpdateMode();


    }

    public void StartMode(MapScreenModes newModes)
    {
        EndMode(curMode);
        switch (newModes)
        {
            case MapScreenModes.ChooseCard:
                Debug.Log("here");
                cameraC.FixCamera();
                confirmUse = false;
                noticed = false;
                break;
        }
        curMode = newModes;
    }

    public void EndMode(MapScreenModes lastMode)
    {
        switch (lastMode)
        {
            case MapScreenModes.ChooseCard:
                cameraC.OpenCameraMAndS();
                break;
        }
    }

    public void UpdateMode()
    {
        switch (curMode)
        {
            case MapScreenModes.idle:
                /*
                if (Input.GetMouseButtonDown(0))
                {
                    mouseClickPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                    if (monitorArea.Contains(mouseClickPos))
                    {
                        justIn = true;
                        manager.enCard(curCard);
                        StartMode(MapScreenModes.ChooseCard);
                    }
                }*/
                break;

            case MapScreenModes.ChooseCard:
                if (Input.GetMouseButton(0))
                {

                    Vector2 mousePosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

                    if (monitorArea.Contains(mousePosition))
                    {
                        //Debug.Log("鼠标在监测区域内");
                        if (!justIn)
                        {
                            manager.cancelPreTrigger();

                        }
                        justIn = true;
                    }
                    else
                    {
                        //Debug.Log("鼠标离开了监测区域");
                        if (justIn)
                        {
                            if (!manager.enterPreTrigger())
                            {
                                manager.cancelCardChoose();
                            }
                        }
                        justIn = false;
                    }

                }
                else
                {
                    if (!noticed) break;
                    if (confirmUse)
                    {
                        //StartMode();
                        HexManager._instance.useChoosedCard();
                        
                        StartMode(MapScreenModes.idle);
                    }
                    else
                    {
                        
                        StartMode(MapScreenModes.idle);
                        HexManager._instance.cancelCardChoose();
                    }
                }
                break;
        }
    }

    public void OnCardPlayed(GeneralCard card)
    {
        confirmUse = true;
    }

    public void OnCardNotice()
    {
        noticed = true;
    }

    public void OnMouseLeaveWithCard(GeneralCard cd)
    {
        justIn = true;
        manager.enCard(cd);
        StartMode(MapScreenModes.ChooseCard);
        //Debug.Log("here card");
    }

    public void RefreshCamera()
    {
        cameraObj = FindObjectOfType<Canvas>().worldCamera.gameObject;
        cameraC = cameraObj.GetComponent<ICameraControl>();
    }
}

public enum MapScreenModes
{
    idle,
    AddCellInStart,
    ChooseCard,
    CardRelease
}

public interface ICameraControl
{
    void FixCamera();
    void OpenCameraMAndS();
}
