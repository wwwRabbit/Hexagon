using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartButton : MonoBehaviour
{
    public GameObject child;

    private void OnMouseEnter()
    {
        Debug.Log("11111");
      //  child.SetActive(true);
    }

    public void ShowChild()
    {
        child.SetActive(true);
    }

    public void test()
    {
        Debug.Log("22222222");
    }

    //往这个地方输入场景几就是场景几
    public void changescene()
    {
        //Debug.Log("11133333333333");
        // ChangeSceneManager.Instance.SwitchScene(1);
        // SceneManager.LoadScene(1);
        SceneManager.LoadScene(2);

        //Debug.Log("22222333333");

    }

    //退出游戏的代码
    public void exit()
    {
        //这个是从编辑器中退出游戏模式
        //UnityEditor.EditorApplication.isPlaying = false;

        //这个是从游戏中退出
        Application.Quit(); // 在构建的应用程序中退出游戏  
    }
}
