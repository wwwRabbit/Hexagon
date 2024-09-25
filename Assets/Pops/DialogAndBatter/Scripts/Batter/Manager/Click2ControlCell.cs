using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

/// <summary>
/// 负责玩家点击控制士兵
/// </summary>
public class Click2ControlCell : MonoBehaviour
{
    public static Click2ControlCell Instance { get; private set; }

    private bool playerControl;
    public bool PlayerControl => playerControl;                        //记录是否控制某个士兵

    private CellController currControlSoldier;
    public CellController CurrControlSoldier => currControlSoldier;    //记录当前控制的士兵脚本

    [SerializeField] LayerMask layer;

    public CinemachineVirtualCamera virtualCamera;
    public Transform baseTarget;
    float baseFov;
    [SerializeField] float newFov;
    [SerializeField] float transitionDuration = 0.8f;
    Coroutine currCoroutine;
    private void Awake()
    {
        Instance = this;
        baseFov = virtualCamera.m_Lens.OrthographicSize;
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && !playerControl)
        {
            print("点击了界面");
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero, 1000f, LayerMask.GetMask("Soldier"));

            if (hit.collider != null)
            {
                PlayerInput.Instance.EnablePlayerInputs();
                currControlSoldier = hit.collider.gameObject.GetComponent<CellController>();
                playerControl = true;
                currControlSoldier.rangeSensor.OpenOrCloseDetect(false);
                currControlSoldier.cellAttr.ClearCells();

                FollowControlTarget();
                print("获取士兵成功");
            }

        }

        if (playerControl)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                EndControlCell();
            }
        }
    }

    public void EndControlCell()
    {
        currControlSoldier.rangeSensor.OpenOrCloseDetect(true);

        currControlSoldier = null;
        playerControl = false;

        UnfollowControlTarget();

        PlayerInput.Instance.DisablePlayerInputs();
    }

    public bool JudgmentControl(CellController cell)
    {
        if (currControlSoldier == cell && playerControl && !cell.isEnemy)
        {
            return true;
        }
        return false;
    }

    void FollowControlTarget()
    {
        if (currCoroutine != null)
        {
            StopCoroutine(currCoroutine);
        }
        currCoroutine = StartCoroutine(SmoothTransition(currControlSoldier is null ? baseTarget : currControlSoldier.gameObject.transform, newFov));
    }

    void UnfollowControlTarget()
    {
        if (currCoroutine != null)
        {
            StopCoroutine(currCoroutine);
        }
        currCoroutine = StartCoroutine(SmoothTransition(baseTarget, baseFov));
    }

    IEnumerator SmoothTransition(Transform newTarget, float newFov)
    {
        // 如果目标或FOV没有变化，则退出协程
        if (newTarget == currControlSoldier && Mathf.Approximately(newFov, baseFov))
            yield break;
        
        virtualCamera.Follow = newTarget;

        float elapsedTime = 0f;
        float startOrthographicSize = virtualCamera.m_Lens.OrthographicSize;

        while (elapsedTime < transitionDuration)
        {
            // 通过插值函数逐渐改变FOV值
            virtualCamera.m_Lens.OrthographicSize = Mathf.Lerp(startOrthographicSize, newFov, elapsedTime / transitionDuration);

            // 更新经过的时间
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        // 最终设置跟随目标和FOV值
        virtualCamera.m_Lens.OrthographicSize = newFov;
    }
}
