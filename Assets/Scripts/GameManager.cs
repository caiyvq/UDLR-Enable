using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
//using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using UnityEngine.UI;
using UnityEditor;

public class GameManager : MonoBehaviour
{
    
    public int finishGoals = 0;//已完成的目标数量
    [HideInInspector]
    public int totalBoxes = 0;//总共的箱子数量
    public int totalGoals = 0;//总共的目标数量

    private bool win = false;//标志胜利的信号位

    GameObject UI_quit_new;//退出菜单，有些场景里有废弃的UI，为免除麻烦，删除后创建新的UI
    public GameObject UI_mobile;//移动端的UI

    public List<GameObject> boxes;//箱子集合
    public GameObject player;//角色
    public SaveManager saveManager;//存档管理器
    public Stack<State> historyStates = new();//历史撤回栈
    //最多保存的步数
    public const int MAX_HISTORY_STEPS = 200;

    [Header("动画器")]
    public Animator enterNumberAnimator;
    public Animator enterImageAnimator;
    public Animator exitImageAnimator;
    public Animator victoryImageAnimator;
    public Animator victoryShadeAnimator;

    //边界检测
    [Header("地图边界")]
    public float boundLeft;
    public float boundRight;
    public float boundUp;
    public float boundDown;
    private void Start()
    {
        Time.timeScale = 1f;//时间开始流动

        SetLevel0Language();//设置教程关卡的语言

        ProcessQuiUI();//处理退出菜单
        ProcessMobileUI();//处理移动端UI
        
        //播放入场动画
        StartCoroutine(PlayEnterAnim());
        //保存初始状态
        SaveState();
    }
    private void Update()
    {
        //如果关卡未通关
        if (!win)
        {
            //检查
            CheckWin();
        }
    }
    //设置教程关的语言
    void SetLevel0Language()
    {
        //教程关的build index
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            GameObject.Find("character/Canvas/Label/Text").GetComponent<Text>().text = LanguageManager.Instance.level_0_guide_0[LanguageManager.language];
            GameObject.Find("box_heavy/Canvas/Label/Text").GetComponent<Text>().text = LanguageManager.Instance.level_0_guide_1[LanguageManager.language];
            GameObject.Find("goal/Canvas/Label/Text").GetComponent<Text>().text = LanguageManager.Instance.level_0_guide_2[LanguageManager.language];
            GameObject.Find("button_l/Canvas/Label/Text").GetComponent<Text>().text = LanguageManager.Instance.level_0_guide_3[LanguageManager.language];
            GameObject.Find("button_r/Canvas/Label/Text").GetComponent<Text>().text = LanguageManager.Instance.level_0_guide_4[LanguageManager.language];
        }
    }
    //处理退出菜单
    private void ProcessQuiUI()
    {
        //删去废弃的
        if (GameObject.Find("UI_quit") != null)
        {
            Destroy(GameObject.Find("UI_quit"));
        }
        //创建UI_mobile预制体实例
        UI_quit_new = Instantiate(Resources.Load("UI_quit")) as GameObject;
    }
    //处理移动端UI
    private void ProcessMobileUI()
    {
        //取消PC端的操作UI
        if (UI_mobile != null)
        {
            UI_mobile.SetActive(false);
        }
        //提示用
        print(Application.platform);
        //打开移动端的操作UI
        if (Application.platform == RuntimePlatform.Android)
        {
            if (UI_mobile == null)
            {
                //创建UI_mobile预制体实例
                UI_mobile = Instantiate(Resources.Load("UI_mobile")) as GameObject;
            }
            else
            {
                //只需创建关卡内的，主界面有自己单独的UI
                if (FindAnyObjectByType<MainManager>() == null)
                {
                    Destroy(UI_mobile);
                    UI_mobile = Instantiate(Resources.Load("UI_mobile")) as GameObject;
                }
            }
            UI_mobile.SetActive(true);//激活UI
            InitButtons();//初始化UI按钮
        }
    }

    //为移动端UI绑定事件
    public void InitButtons()
    {
        if (UI_quit_new!=null)
        {
            UI_mobile.transform.Find("quit").GetComponent<UnityEngine.UI.Button>().onClick.AddListener(UI_quit_new.GetComponentInChildren<QuitMenu>().MenuOpen);//退出按钮
        }
        UI_mobile.transform.Find("retry").GetComponent<UnityEngine.UI.Button>().onClick.AddListener(player.GetComponent<Player>().Retry);//重来按钮
        UI_mobile.transform.Find("undo").GetComponent<UnityEngine.UI.Button>().onClick.AddListener(player.GetComponent<Player>().Undo);//撤回按钮
        //上下左右
        UI_mobile.transform.Find("Up").GetComponent<Button>().dir = 0;
        UI_mobile.transform.Find("Down").GetComponent<Button>().dir = 1;
        UI_mobile.transform.Find("Left").GetComponent<Button>().dir = 2;
        UI_mobile.transform.Find("Right").GetComponent<Button>().dir = 3;
    }

    //边界检测，pos是物体的真实位置，返回物体的等效位置
    public Vector3 CheckBound(Vector3 pos, Vector3 dir)
    {
        if (pos.x + dir.x > boundRight)
        {
            pos.x = boundLeft - (boundRight - pos.x) - 1;
        }
        if (pos.x + dir.x < boundLeft)
        {
            pos.x = boundRight + (pos.x - boundLeft) + 1;
        }
        if (pos.y + dir.y > boundUp)
        {
            pos.y = boundDown - (boundUp - pos.y) - 1;
        }
        if (pos.y + dir.y < boundDown)
        {
            pos.y = boundUp + (pos.y - boundDown) + 1;
        }
        //保留一位小数，防止偏差积累
        pos.x = (float)Math.Round(pos.x, 1);
        pos.y = (float)Math.Round(pos.y, 1);
        return pos;
    }
    //判断是否通关
    public void CheckWin()
    {
        //检查通关条件
        if (totalGoals == finishGoals && totalGoals != 0)
        {
            //保存最佳记录
            player.GetComponent<Player>().SaveBest();
            //信号位置真
            win = true;
            //播放人物胜利动画
            player.GetComponent<Animator>().Play("Victory");
            //暂停音乐
            EventController.RaiseOnPlayAudio(AudioType.BGM, false);
            //禁止人物移动
            player.GetComponent<Player>().readyToMove = false;
            //播放胜利UI动画
            StartCoroutine(ShowWinAfterDelay());
            //播放退出UI动画
            StartCoroutine(PlayExitAnim(4.5f));
            //保存
            saveManager.SaveLevelState(SceneManager.GetActiveScene().buildIndex - 1, true);
            //返回主界面
            StartCoroutine(BackToMainLevel(5));
        }
    }
    //保存每次移动后的人物与箱子位置
    public void SaveState()
    {
        // 记录当前箱子位置
        List<Vector2> currentBoxPositions = new();
        foreach (GameObject box in boxes)
        {
            currentBoxPositions.Add(box.GetComponent<Box>().finalPos);
        }

        // 创建新状态
        State newState = new(
            player.GetComponent<Player>().finalPos,
            currentBoxPositions
        );

        // 如果栈已满，移除最旧的元素
        if (historyStates.Count >= MAX_HISTORY_STEPS)
        {
            RemoveOldestState();
        }

        historyStates.Push(newState);
    }
    //移除最旧的状态
    public void RemoveOldestState()
    {
        // 将栈转换为列表（顺序：栈顶 -> 栈底）
        List<State> statesList = new(historyStates);
        // 移除列表最后一个元素（即最旧的状态）
        statesList.RemoveAt(statesList.Count - 1);
        statesList.Reverse();
        // 清空栈并重新压入剩余元素
        historyStates.Clear();
        foreach (State state in statesList)
        {
            historyStates.Push(state);
        }
    }
    // 撤销操作
    public void Undo()
    {
        if (historyStates.Count > 1)
        {
            historyStates.Pop(); // 弹出当前状态
            State previousState = historyStates.Peek(); // 获取上一个状态

            // 恢复玩家位置
            player.transform.position = previousState.playerPosition;
            player.GetComponent<Player>().finalPos=player.transform.position;

            // 恢复箱子位置
            for (int i = 0; i < boxes.Count; i++)
            {
                boxes[i].transform.position = previousState.boxPositions[i];
                boxes[i].GetComponent<Box>().finalPos = boxes[i].transform.position;
            }
            //关卡当前步数减一
            player.GetComponent<Player>().ShowStep(-1);
        }
    }
    //播放通关动画
    private void ShowWin()
    {
        if(victoryShadeAnimator != null)
        {
            victoryShadeAnimator.Play("Shade");
        }
        if(victoryImageAnimator != null)
        {
            victoryImageAnimator.Play("VictoryImage");
        }
    }
    //延迟用协程
    private IEnumerator ShowWinAfterDelay()
    {
        yield return new WaitForSeconds(2);
        EventController.RaiseOnPlayAudio(AudioType.Victory, true);//播放通关音效
        ShowWin();//播放通关动画
    }
    //播放入场动画
    private IEnumerator PlayEnterAnim()
    {
        if (enterNumberAnimator != null)
        {
            enterNumberAnimator.Play("number");
            yield return new WaitForSeconds(2);
        }
        if (enterImageAnimator != null)
        {
            enterImageAnimator.Play("enter_image");
            //播放音效
            EventController.RaiseOnPlayAudio(AudioType.Enter, true);
            yield return new WaitForSeconds(1);
            EventController.RaiseOnPlayAudio(AudioType.BGM, true);
        }
        yield return new WaitForSeconds(0.1f);
        //调整图层顺序，防止操作UI被挡住
        if (GameObject.Find("UI_enter"))
            GameObject.Find("UI_enter").GetComponent<Canvas>().sortingOrder = 0;
        FindAnyObjectByType<Player>().GetComponent<Player>().readyToMove = true;
    }
    //播放退出动画
    public IEnumerator PlayExitAnim(float time)
    {
        //调整图层顺序，挡住操作UI
        GameObject.Find("UI_exit").GetComponent<Canvas>().sortingOrder = 4;
        yield return new WaitForSeconds(time);
        if(exitImageAnimator != null)
        {
            exitImageAnimator.Play("exit_image");
        }
        EventController.RaiseOnPlayAudio(AudioType.Exit, true);
    }
    //返回主界面
    public IEnumerator BackToMainLevel(float time)
    {
        yield return new WaitForSeconds(time);
        SceneManager.LoadScene("MainLevel");
    }
}
