using UnityEngine;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    [SerializeField] protected Button testButton;
    [SerializeField] protected string testScene;
    [SerializeField] Transform[] dialogBlocks;

    private void Start()
    {
        if (testButton != null)
        {
            testButton.onClick.AddListener(testAwake);
        }
        void testAwake()
        {
            if(testScene!=null)
            {
                GameObject disableForConfliction = GameObject.FindGameObjectWithTag("Disable");
                Debug.Log("Finding Scene");
                awakeDialogScene(testScene);
            }
        }
    }

    /// <summary>
    /// 输入想要启用的dialogbox的名字，当前名字为TScene。
    /// </summary>
    /// <param name="sceneName"></param>
    public void awakeDialogScene(string sceneName)
    {
        foreach(Transform dialogBlock in dialogBlocks)
        {
            Debug.Log(dialogBlock.name);
            if (dialogBlock.name == sceneName)
                dialogBlock.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// 同理禁用
    /// </summary>
    /// <param name="sceneName"></param>
    public void disableDialogScene(string sceneName)
    {
        foreach (Transform dialogBlock in dialogBlocks)
        {
            Debug.Log(dialogBlock.name);
            if (dialogBlock.name == sceneName)
                dialogBlock.gameObject.SetActive(false);
        }
    }
}
