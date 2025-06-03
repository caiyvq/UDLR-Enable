using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/*
�ؿ����
00 �̳̹�
01 �찴ť1���򵥣�
02 �찴ť2���򵥣�
03 �찴ť3���򵥣�
04 �찴ť ���䣨�򵥣�
05 �찴ť ѭ��1���򵥣�
06 �찴ť ѭ��2���򵥣�
07 �찴ť ���� ѭ�����еȣ�
08 �찴ť �̰�ť ���䣨�еȣ�
09 �찴ť �̰�ť ѭ�������ѣ�
10 �찴ť ����ť1���򵥣�
11 �찴ť ����ť2�����ѣ�
12 �찴ť ����ť ѭ��1���еȣ�
13 �찴ť ����ť ѭ��2���еȣ�
14 �찴ť �̰�ť ����ť ѭ�������ѣ�
15 ���չ� ȫ��Ҫ�أ��еȣ�
 */
public class Level : MonoBehaviour
{
    //�ؿ�״̬
    private const int LOCKED = 0;
    private const int UNLOCK = 1;
    private const int FINISH = 2;
    //��ɫ�Ƿ�λ�ڹؿ���
    private bool active = false;
    public int state;//��ǰ�ؿ�״̬
    public Sprite[] sprites;//�ؿ���ͼ
    //��ʾ������UI: press enter
    public GameObject hintUI;
    //����ί�У��򻯴���
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
        //���½���ؿ�����
        if (Input.GetKeyDown(KeyCode.Return))
        {
            EnterLevel();
        }
    }
    //����ؿ�
    public void EnterLevel()
    {
        //����ùؿ��ѽ����ҽ�ɫ��������
        if(active && state != LOCKED && player.readyToMove)
        {
            hintUI.SetActive(false);
            GameObject.Find("UI_exit").GetComponent<Canvas>().sortingOrder = 4;
            //������Ч
            EventController.RaiseOnPlayAudio(AudioType.BGM, false);
            //����
            FindAnyObjectByType<SaveManager>().SavePosition();
            //���Ŷ�������Ч
            FindAnyObjectByType<Player>().GetComponent<Animator>().Play("Enter");
            EventController.RaiseOnPlayAudio(AudioType.Confirm_Level, true);
            StartCoroutine(DelayDo(1, FindAnyObjectByType<GameManager>().exitImageAnimator.Play, "exit_image"));
            EventController.RaiseOnPlayAudio(AudioType.Exit, true);
            StartCoroutine(DelayDo(2, SceneManager.LoadScene, gameObject.name));//�ӳټ��س���
        }
    }
    //�ӳ�ִ�еķ�������һ������
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
        //�ؿ��ѽ���
        if (collision.GetComponent<Player>()!=null && state!=LOCKED)
        {
            hintUI.SetActive(true);//��ʾ����ؿ�����ʾ
            active = true;
            //���ݵ�ǰ�ؿ������Ƿ���ֲ���˵��
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
    //���ֲ���˵��
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
