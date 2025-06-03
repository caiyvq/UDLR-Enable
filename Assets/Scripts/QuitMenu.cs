using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class QuitMenu : MonoBehaviour
{
    public bool active = false;
    bool isMainlevel = true;
    public Animator shadeAnimator;
    public GameObject UI;
    void Start()
    {
        if (FindAnyObjectByType<MainManager>() == null)
        {
            isMainlevel = false;
        }
    }
    // Update is called once per frame
    void Update()
    {
        //打开界面
        if (Input.GetKeyUp(KeyCode.Q))
        {
            MenuOpen();
        }
        //处理界面逻辑
        if (active)
        {
            if(Input.GetKeyUp(KeyCode.LeftArrow))
            {
                QuitYes();
            }
            else if (Input.GetKeyUp(KeyCode.RightArrow))
            {
                MenuClose();
            }
        }
    }

    public void MenuOpen()
    {
        if (active == false && (FindAnyObjectByType<DataMenu>() == null || FindAnyObjectByType<DataMenu>().active == false) && GameObject.Find("UI_options/options_menu") == null)
        {
            //print("menu open");
            transform.Find("Text").GetComponent<Text>().text = LanguageManager.Instance.quitTexts[LanguageManager.language];
            UI.GetComponent<Canvas>().sortingOrder = 5;
            active = true;
            GetComponent<Animator>().Play("quit_show");
            EventController.RaiseOnPlayAudio(AudioType.Show_UI, true);
            if (shadeAnimator != null)
            {
                shadeAnimator.Play("Shade");
            }
            FindAnyObjectByType<Player>().readyToMove = false;
        }
    }
    public void MenuClose()
    {
        UI.GetComponent<Canvas>().sortingOrder = 2;
        EventController.RaiseOnPlayAudio(AudioType.Select, true);
        active = false;
        GetComponent<Animator>().Play("quit_close");
        if (shadeAnimator != null)
        {
            shadeAnimator.Play("Hide");
        }
        FindAnyObjectByType<Player>().readyToMove = true;
    }
    public void QuitYes()
    {
        EventController.RaiseOnPlayAudio(AudioType.Select, true);
        if (isMainlevel)
        {
            //print("quit");
            FindAnyObjectByType<SaveManager>().SavePosition();
            Application.Quit();
        }
        else
        {
            GetComponent<Animator>().Play("quit_close");
            EventController.RaiseOnPlayAudio(AudioType.BGM, false);
            //播放退出UI动画
            StartCoroutine(FindAnyObjectByType<GameManager>().PlayExitAnim(0.5f));
            //返回主界面
            StartCoroutine(FindAnyObjectByType<GameManager>().BackToMainLevel(1f));
        }
    }
}
