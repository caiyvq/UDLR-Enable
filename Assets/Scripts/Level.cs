using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/*
关卡设计
00 教程关
01 红按钮1（简单）
02 红按钮2（简单）
03 红按钮3（简单）
04 红按钮 轻箱（简单）
05 红按钮 循环1（简单）
06 红按钮 循环2（简单）
07 红按钮 轻箱 循环（中等）
08 红按钮 绿按钮 轻箱（中等）
09 红按钮 绿按钮 循环（困难）
10 红按钮 蓝按钮1（简单）
11 红按钮 蓝按钮2（困难）
12 红按钮 蓝按钮 循环1（中等）
13 红按钮 蓝按钮 循环2（中等）
14 红按钮 绿按钮 蓝按钮 循环（困难）
15 最终关 全部要素（中等）
 */
public class Level : MonoBehaviour
{
    //关卡状态
    private const int LOCKED = 0;
    private const int UNLOCK = 1;
    private const int FINISH = 2;
    //角色是否位于关卡上
    private bool active = false;
    public int state;//当前关卡状态
    public Sprite[] sprites;//关卡贴图
    //提示按键的UI: press enter
    public GameObject hintUI;
    //含参委托，简化代码
    delegate void FuncOneParam(string name);

    MainManager mainManager;
    Player player;

    private void Start()
    {
        gameObject.GetComponent<SpriteRenderer>().sprite = sprites[state];
        mainManager = FindAnyObjectByType<MainManager>();
        player=FindAnyObjectByType<Player>();
    }
    private void Update()
    {
        //按下进入关卡按键
        if (Input.GetKeyDown(KeyCode.Return))
        {
            EnterLevel();
        }
    }
    //进入关卡
    public void EnterLevel()
    {
        //如果该关卡已解锁且角色处于上面
        if(active && state != LOCKED && player.readyToMove)
        {
            hintUI.SetActive(false);
            GameObject.Find("UI_exit").GetComponent<Canvas>().sortingOrder = 4;
            //播放音效
            EventController.RaiseOnPlayAudio(AudioType.BGM, false);
            //保存
            FindAnyObjectByType<SaveManager>().SavePosition();
            //播放动画和音效
            FindAnyObjectByType<Player>().GetComponent<Animator>().Play("Enter");
            EventController.RaiseOnPlayAudio(AudioType.Confirm_Level, true);
            StartCoroutine(DelayDo(1, FindAnyObjectByType<GameManager>().exitImageAnimator.Play, "exit_image"));
            EventController.RaiseOnPlayAudio(AudioType.Exit, true);
            StartCoroutine(DelayDo(2, SceneManager.LoadScene, gameObject.name));//延迟加载场景
        }
    }
    //延迟执行的方法，带一个参数
    private IEnumerator DelayDo(float time,FuncOneParam f,string name)
    {
        yield return new WaitForSeconds(time);
        f(name);
    }
    //private IEnumerator DelayLoadScene()
    //{
    //    yield return new WaitForSeconds(2);

    //    SceneManager.LoadScene(gameObject.name);
    //}
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //关卡已解锁
        if (collision.GetComponent<Player>()!=null && state!=LOCKED)
        {
            hintUI.SetActive(true);//显示进入关卡的提示
            active = true;
            //根据当前关卡决定是否出现操作说明
            if (gameObject.name == "Level_0" && mainManager.guideStates[0] == true)
            {
                StartCoroutine(ShowGuide(0));
            }
            if (gameObject.name == "Level_3" && mainManager.guideStates[1] == true && state==FINISH)
            {
                StartCoroutine(ShowGuide(1));
            }
            if (gameObject.name == "Level_4" && mainManager.guideStates[2] == true && state == FINISH)
            {
                StartCoroutine(ShowGuide(2));
            }
            if (gameObject.name == "Level_7" && mainManager.guideStates[3] == true && state == FINISH)
            {
                StartCoroutine(ShowGuide(3));
            }
            if (gameObject.name == "Level_9" && mainManager.guideStates[4] == true && state == FINISH)
            {
                StartCoroutine(ShowGuide(4));
            }
        }
    }
    //出现操作说明
    public IEnumerator ShowGuide(int index)
    {
        yield return new WaitForSeconds(0.1f);
        FindAnyObjectByType<Player>().readyToMove = false;

        yield return new WaitForSeconds(0.2f);

        mainManager.guides[index].SetActive(true);
        mainManager.guides[index].GetComponentInChildren<UnityEngine.UI.Button>().Select(); 

        LanguageManager.Instance.languageSet = mainManager.guides[index].transform.Find("Language").GetComponent<Dropdown>();
        LanguageManager.Instance.languageSet.value = LanguageManager.language;
        mainManager.guides[index].transform.Find("Language/Text").GetComponent<Text>().text = LanguageManager.Instance.optionLanguageTexts[LanguageManager.language];
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>() != null)
        {
            hintUI.SetActive(false);
            active = false;
        }
    }
}
