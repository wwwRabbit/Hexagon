using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
//using UnityEngine.UIElements;
using UnityEngine.UI;

public class CargDrag : CardBasic,IBeginDragHandler,IEndDragHandler,IDragHandler,IPointerEnterHandler,IPointerExitHandler,IPointerClickHandler, ICardUi, IPointerDownHandler
{
    private RectTransform rectTrans;//拿到图片组件
    private CanvasGroup canvasGroup;//控制渲染顺序的组件
    private Vector2 startpos;//起始位置
     private Vector3 offect;//也是用来存储位置的一个中介

    private Vector2 originalSize;//图片的原始大小1

    public Vector2 enlargedSize;//放大之后的大小


    [Header("临时卡燃烧相关")]
    public float upSpeed = 2f;  // 控制上升速度
    public float fadeSpeed = 0.1f;  // 控制淡出速度
    public bool ifBurned = false;
    bool ifDestroyed = false;
    public float delay = 3f;  // 延迟时间，以秒为单位

    GeneralCard selfFunctionCard;
    cardState curState = cardState.zero;
    bool revealInfoEnabled = false; //是否会展示卡牌信息
    bool dragEnabled = false;
    bool changeTransparency = false;

    [Header("悬停相关")]
    [SerializeField] private Canvas uiCanvas; // UI Canvas
    [SerializeField] private Sprite hoverSprite; // 鼠标悬停时要显示的图片
    [SerializeField] private Vector2 offset = new Vector2(0, 50); // 图片与鼠标的偏移

    private Camera uiCamera;
    private RectTransform hoverImageRect;
    private Image originalImage; // 原始组件的Image
    private GameObject hoverImageObj; // 鼠标悬停时生成的新Image对象
    private bool isMouseInside = false;
    private Color originalImageColor; // 保存原始的Image颜色

    public GameObject useTime1;
    public GameObject useTime2;
    public GameObject useTime3;
    public int useTime = 0;

    

    private void Awake()
    {
        originalSize = GetComponent<RectTransform>().sizeDelta;//拿到物体一开始的大小

        enlargedSize = originalSize * 1.30f;

        rectTrans = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        //startpos = rectTrans.anchoredPosition;

        Vector3 screenPosition = Camera.main.WorldToScreenPoint(gameObject.transform.position);

        Vector3 mScreenPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPosition.z);

        Vector3 offect = gameObject.transform.position - Camera.main.ScreenToWorldPoint(mScreenPosition);


    }

    private void Start()
    {
        uiCanvas = GameObject.Find("主要幕布").GetComponent<Canvas>();
        // 获取Canvas及其关联的Camera
        if (uiCanvas == null || uiCanvas.renderMode != RenderMode.ScreenSpaceCamera)
        {
            Debug.LogError("请确保Canvas设置为Screen Space - Camera.");
            return;
        }
        uiCamera = uiCanvas.worldCamera;

        // 获取当前组件的Image
        originalImage = GetComponent<Image>();
        if (originalImage == null)
        {
            Debug.LogError("请确保组件上有一个Image.");
            return;
        }
        hoverSprite = originalImage.sprite;
        originalImageColor = originalImage.color;
    }

    internal override void Update()
    {
        if (ifBurned)
        {
            
        }
        else
        {
            base.Update();
            
        }

        UpdateStage();

        if (changeTransparency)
        {
            float distance = Vector3.Distance(rectTransform.position, target.position);
            canvasGroup.alpha = Mathf.Clamp(1-distance / 10, 0, 1);

        }
    }

    #region 状态机相关
    public void StartStage(cardState newState)
    {
        EndStage(curState);
        switch (newState)
        {
            case cardState.Initializing:
                //完成
                SetMovementEnabled(true);
                SetRevealInfoEnabled(false);
                SetDragEnabled(false);
                SetTSChange(false);
                break;

            case cardState.Normal:
                //完成
                SetMovementEnabled(true);
                SetRevealInfoEnabled(true);
                SetDragEnabled(true);
                SetTSChange(false);
                break;

            case cardState.Draged:
                //完成
                SetMovementEnabled(false);//这里只是不让自动移动
                SetRevealInfoEnabled(false);
                SetDragEnabled(true);
                SetTSChange(true);
                DownReveal();

                CSAScript._instance.SetOtherCardsWhenOneIsDragged(this);
                break;

            case cardState.Burned:

                SetMovementEnabled(false);//这里只是不让自动移动
                SetRevealInfoEnabled(false);
                SetDragEnabled(false);
                SetTSChange(false);
                break;

            case cardState.NormalWhileOtherAreDraged:
                //完成
                SetMovementEnabled(true);
                SetRevealInfoEnabled(false);
                SetDragEnabled(false);
                SetTSChange(false);
                break;

            case cardState.CancelUse:
                //完成
                SetMovementEnabled(true);//这里只是不让自动移动
                SetRevealInfoEnabled(false);
                SetDragEnabled(false);
                SetTSChange(true);
                CSAScript._instance.ReSetOtherCardsAfterOneIsDragged(this);
                break;

            case cardState.Used:
                try
                {
                    SetMovementEnabled(false);//这里只是不让自动移动
                    SetRevealInfoEnabled(false);
                    SetDragEnabled(false);
                    SetTSChange(false);
                    //CSAScript._instance.ReSetOtherCardsAfterOneIsDragged(this);
                }
                catch (UnityEngine.MissingReferenceException)
                {
                    Debug.Log("有问题，但是被抓住了");
                }
                break;
        }
        curState = newState;
    }

    public void EndStage(cardState oldState)
    {
        switch (oldState)
        {
            case cardState.Initializing:
                break;

            case cardState.Used:
                try
                {
                    Destroy(gameObject);
                }
                catch (UnityEngine.MissingReferenceException)
                {
                    Debug.Log("有问题，但是被抓住了");
                }
                
                break;
        }
    }

    public void UpdateStage()
    {
        switch (curState)
        {
            case cardState.Initializing:
                if (!isMoving)
                {
                    StartStage(cardState.Normal);
                }
                break;

            case cardState.Used:
                gameObject.SetActive(false);
                StartStage(cardState.zero);
                break;

            case cardState.Burned:
                // 每帧向上移动
                rectTrans.anchoredPosition += Vector2.up * upSpeed * Time.deltaTime;
                upSpeed *= 0.993f;

                // 每帧减少透明度
                canvasGroup.alpha -= fadeSpeed * Time.deltaTime;

                if (canvasGroup.alpha <= 0.3)
                {
                    ifDestroyed = true;
                }

                // 当透明度小于等于0时，销毁对象
                if (canvasGroup.alpha <= 0)
                {
                    //ifDestroyed = true;
                    StartCoroutine(DestroyObjectAfterDelay());
                }
                break;

            case cardState.CancelUse:
                if (!isMoving)
                {
                    StartStage(cardState.Normal);
                }
                break;
        }
    }
    #endregion

    public void SetRevealInfoEnabled(bool ifEnabled)
    {
        revealInfoEnabled = ifEnabled;
        if (!ifEnabled )
            rectTrans.sizeDelta = originalSize;
    }

    public void SetDragEnabled(bool ifEnabled)
    {
        dragEnabled = ifEnabled;
    }

    public void SetTSChange(bool ifEnabled)
    {
        changeTransparency = ifEnabled;
    }


    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!dragEnabled) return;
        canvasGroup.blocksRaycasts = false;
        Debug.Log("here");
        if (selfFunctionCard != null)
        {
            Debug.Log("here");
            InterActController._instance.OnMouseLeaveWithCard(selfFunctionCard);
        }
        StartStage(cardState.Draged);
        
        //canvasGroup.alpha = 0.35f;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!dragEnabled) return;
        if(curState == cardState.Draged)
            rectTrans.anchoredPosition += eventData.delta;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!dragEnabled) return;
        //if (curState == cardState.Draged)
        canvasGroup.blocksRaycasts = true;
        //canvasGroup.alpha = 1f;
       // rectTrans.anchoredPosition = startpos;
        //transform.position = offect;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!revealInfoEnabled) return;
        OnReveal();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!revealInfoEnabled) return;
        DownReveal();

    }

    public void DestoryThis()
    {

       // Destroy(this);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //Destroy(gameObject);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        /*
        //Debug.Log("here");
        if (curState == cardState.Normal)
        {
            if (selfFunctionCard != null)
            {
                Debug.Log("here");
                InterActController._instance.OnMouseLeaveWithCard(selfFunctionCard);
            }
        }
        //这里是关于触发功能的*/
    }

    public void OnReveal()
    {
        if (isMouseInside) return; // 防止重复触发
        isMouseInside = true;

        // 隐藏原始组件的Image（通过将Alpha设置为0）
        if (originalImage != null)
        {
            Color tempColor = originalImage.color;
            tempColor.a = 0;
            originalImage.color = tempColor;
        }

        // 创建一个新的Image对象作为鼠标悬停显示的图片
        hoverImageObj = new GameObject("HoverImage", typeof(Image));
        hoverImageObj.transform.SetParent(uiCanvas.transform, false);

        // 设置Image组件
        Image hoverImageComponent = hoverImageObj.GetComponent<Image>();
        hoverImageComponent.sprite = hoverSprite;
        hoverImageComponent.SetNativeSize(); // 设置为精灵的原始大小

        hoverImageComponent.raycastTarget = false; // 防止遮挡鼠标事件

        // 获取并存储RectTransform
        hoverImageRect = hoverImageObj.GetComponent<RectTransform>();
        hoverImageRect.localScale = new Vector3(1, 1, 1);
        UpdateHoverImagePosition();
    }

    public void DownReveal()
    {
        if (!isMouseInside) return; // 防止重复触发
        isMouseInside = false;

        // 删除新创建的Image对象
        if (hoverImageObj != null)
        {
            Destroy(hoverImageObj);
            hoverImageObj = null;
        }

        // 恢复原始组件的Image
        if (originalImage != null)
        {
            originalImage.color = originalImageColor;
        }

    }

    private void UpdateHoverImagePosition()
    {
        // 获取原始UI元素的本地坐标
        Vector3 originalLocalPosition = originalImage.rectTransform.localPosition;

        // 获取原始UI元素的大小
        float originalHeight = originalImage.rectTransform.rect.height;

        // 确保悬停图片的pivot与原始图片一致
        hoverImageRect.pivot = originalImage.rectTransform.pivot;

        // 设置悬停图片的位置，使其位于原始UI组件的正上方
        hoverImageRect.localPosition = new Vector3(originalLocalPosition.x, originalLocalPosition.y + originalHeight / 2 + offset.y, originalLocalPosition.z);
    }

    #region ICardUi相关
    public bool ifMoveEnd()
    {
        return IsMovementComplete();
    }
    public bool ifTempCard()
    {
        return selfFunctionCard.ifCardTemp();
    }

    public GameObject getUiObj()
    {
        return gameObject;
    }

    public void getBurned()
    {
        ifBurned = true;
        StartStage(cardState.Burned);
    }

    public bool ifBurnEnd()
    {
        return ifDestroyed;
    }

    public void enCard(GeneralCard card)
    {
        //Debug.Log("填了卡牌！");
        selfFunctionCard = card;
    }

    public GeneralCard getCard()
    {
        return selfFunctionCard;
    }

    public void useOneTime()
    {
        //Debug.Log("还可以用");
        if (!useTime1.activeInHierarchy) {
            useTime1.SetActive(true);
            useTime = 1;
        }
        else if (!useTime2.activeInHierarchy)
        {
            useTime2.SetActive(true);
            useTime =2;
        }
        else if (!useTime3.activeInHierarchy)
        {
            useTime3.SetActive(true);
            useTime = 3;
        }
    }

    #endregion

    IEnumerator DestroyObjectAfterDelay()
    {
        // 等待指定的延迟时间
        yield return new WaitForEndOfFrame();

        // 销毁GameObject
        Destroy(gameObject);
    }
}

public enum cardState
{
    zero,
    Initializing,
    Normal,
    Burned,
    Draged,
    NormalWhileOtherAreDraged,
    CancelUse,
    Used
}
