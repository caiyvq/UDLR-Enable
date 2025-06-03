using System;
using System.Collections;
using System.Collections.Generic;
//using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public bool readyToMove = false;//表示角色是否处于可操作状态
    [HideInInspector]
    public Vector2 movedir;//移动方向
    [HideInInspector]
    public Vector2 finalPos;//移动终点
    Vector3 speed = Vector3.zero;//初始速度
    [HideInInspector]
    public bool moveDone = true;//移动是否完成
    public LayerMask layerMask;//碰撞检测层

    public Text steps;//关卡当前移动步数
    public Text bestStep;//关卡最佳移动步数
    int step = 0;//关卡当前移动步数的值
    int best;//关卡最佳移动步数的值

    //每个方向能否移动
    [HideInInspector]
    public int moveUp = 0;
    [HideInInspector]
    public int moveDown = 0;
    [HideInInspector]
    public int moveLeft = 0;
    [HideInInspector]
    public int moveRight = 0;

    //当前是否在往某个方向移动两格
    [HideInInspector]
    public bool move2grid = false;
    //哪个方向能移动两格
    [HideInInspector]
    public int moveU2 = 0;
    [HideInInspector]
    public int moveD2 = 0;
    [HideInInspector]
    public int moveL2 = 0;
    [HideInInspector]
    public int moveR2 = 0;

    //边界检测
    [HideInInspector]
    public Vector3 newPos;
    // Update is called once per frame
    private void Start()
    {
        finalPos = transform.position;//初始化finalPos
        ShowBest();//显示最佳移动步数
    }
    void Update()
    {
        //重启
        if (Input.GetKeyDown(KeyCode.R))
        {
            Retry();
        }

        if (readyToMove)
        {
            //撤回
            if (Input.GetKeyDown(KeyCode.Z))
            {
                Undo();
            }
        }
        //移动
        if (Input.GetKey(KeyCode.UpArrow))
        {
            TryMove(0);
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            TryMove(1);
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            TryMove(2);
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            TryMove(3);
        }

        //得到移动信号且上一次移动已经完成
        if (movedir != Vector2.zero && moveDone)
        {
            moveDone = false;//进入下一次移动
            StartCoroutine(FinishMove());//设置协程，0.3秒后完成此次移动
            //判断能否移动
            if (CanMove(movedir))
            {
                Move(movedir);//移动
            }
            else
            {
                //播放音效
                EventController.RaiseOnPlayAudio(AudioType.Crush, true);
                //播放动画
                GetComponent<Animator>().Play("Crash");
            }
        }
        move2grid = false;//特殊移动的信号
        movedir = Vector2.zero;//移动矢量重置
        transform.position = Vector3.SmoothDamp(transform.position, finalPos, ref speed, 0.03f);//让角色从当前位置平滑移动到最终位置
        //moveDone = transform.position == (Vector3)finalPos;

    }
    //接受输入信号后判断是否进行移动
    public void TryMove(int dir)
    {
        //判断是否可操作角色以及上一次移动是否完成
        if (!(readyToMove && moveDone))
            return;
        //根据输入的移动方向分别处理
        switch (dir)
        {
            //数字代表移动方向 0:up,1:down,2:left,3:right
            case 0:
                {
                    //如果具有向上移动的能力
                    if (moveUp > 0)
                    {
                        movedir = Vector2.up;//移动矢量设为向上一个单位
                        //判断是否有特殊移动的能力
                        if (moveU2 != 0)
                        {
                            movedir *= 2;//设为两个单位
                            move2grid = true;//信号位置真
                        }
                    }
                    //不具有向上移动的能力
                    else if (moveUp == 0)
                    {
                        EventController.RaiseOnPlayAudio(AudioType.Cannot_Move, true);//播放相应音效
                        moveDone = false;
                        StartCoroutine(FinishMove());//防止连续触发音效而选择协程控制
                    }
                    break;
                }
            case 1:
                {
                    if (moveDown > 0)
                    {
                        movedir = Vector2.down;
                        if (moveD2 != 0)
                        {
                            movedir *= 2;
                            move2grid = true;
                        }
                    }
                    else if (moveDown == 0)
                    {
                        EventController.RaiseOnPlayAudio(AudioType.Cannot_Move, true);
                        moveDone = false;
                        StartCoroutine(FinishMove());
                    }
                    break;
                }
            case 2:
                {
                    if (moveLeft > 0)
                    {
                        GetComponent<SpriteRenderer>().flipX = false;
                        movedir = Vector2.left;
                        if (moveL2 != 0)
                        {
                            movedir *= 2;
                            move2grid = true;
                        }
                    }
                    else if (moveLeft == 0)
                    {
                        EventController.RaiseOnPlayAudio(AudioType.Cannot_Move, true);
                        moveDone = false;
                        StartCoroutine(FinishMove());
                    }
                    break;
                }
            case 3:
                {
                    if (moveRight > 0)
                    {
                        GetComponent<SpriteRenderer>().flipX = true;
                        movedir = Vector2.right;
                        if (moveR2 != 0)
                        {
                            movedir *= 2;
                            move2grid = true;
                        }
                    }
                    else if (moveRight == 0)
                    {
                        EventController.RaiseOnPlayAudio(AudioType.Cannot_Move, true);
                        moveDone = false;
                        StartCoroutine(FinishMove());
                    }
                    break;
                }
        }
    }
    //重新载入游戏
    public void Retry()
    {
        EventController.RaiseOnPlayAudio(AudioType.BGM, false);//播放相应音效
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);//重新载入场景
    }
    //撤回操作
    public void Undo()
    {
        FindAnyObjectByType<GameManager>().Undo();
    }
    //显示、更新已移动步数
    public void ShowStep(int add)
    {
        if (steps != null)
        {
            step += add;//更新
            steps.text = "Steps: " + step;//显示
        }
    }
    //显示最佳步数
    void ShowBest()
    {
        if (bestStep != null)
        {
            //读取数据
            GameData gameData = FindAnyObjectByType<SaveManager>().LoadData();
            best = gameData.levelList[SceneManager.GetActiveScene().buildIndex - 1].bestStep;
            //显示数据
            if (best == 999)
            {
                bestStep.text = "Best: Null";
            }
            else
            {
                bestStep.text = "Best: " + best;
            }
        }
    }
    //保存最佳步数
    public void SaveBest()
    {
        FindAnyObjectByType<SaveManager>().SaveBest(step);
    }
    //判断能否移动
    public bool CanMove(Vector2 movedir)
    {
        newPos = FindAnyObjectByType<GameManager>().CheckBound(transform.position, movedir);//边界检测后的等效位置
        //非特殊移动
        if (!move2grid)
        {
            RaycastHit2D hit = Physics2D.Raycast(newPos, movedir, 1, layerMask);//碰撞检测
            //如果没有检测到障碍物
            if (!hit)
            {
                //可以移动
                return true;
            }
            else
            {
                //如果障碍物是箱子
                if (hit.collider.gameObject.GetComponent<Box>() != null)
                {
                    //继续对箱子进行移动尝试
                    return hit.collider.GetComponent<Box>().MoveToDir(movedir);
                }
            }
        }
        //特殊移动
        else
        {
            RaycastHit2D hit = Physics2D.Raycast(newPos + (Vector3)movedir / 2 * 1.5f, movedir, 0.99f, layerMask);//碰撞检测的条件有所改变
            //如果没有检测到障碍物
            if (!hit)
            {
                //可以移动
                return true;
            }
        }
        //不能移动
        return false;
    }
    //移动
    public void Move(Vector2 movedir)
    {
        //当前步数+1
        ShowStep(1);
        //两格
        if (move2grid)
        {
            //播放音效
            EventController.RaiseOnPlayAudio(AudioType.Move_Player_2, true);
            //播放动画
            GetComponent<Animator>().Play("Move2Grids");
            //采用协程让移动变得更顺眼
            StartCoroutine(DelayMove(movedir));
        }
        //一格
        else
        {
            //播放音效
            EventController.RaiseOnPlayAudio(AudioType.Move_Player, true);
            //位置改为等效位置
            transform.position = newPos;
            //更新最终位置，开始移动
            finalPos = newPos + (Vector3)movedir;
            //记录此次操作
            FindObjectOfType<GameManager>().SaveState();
        }
    }
    //延迟移动，用来让移动和动画更加契合
    private IEnumerator DelayMove(Vector3 dir)
    {
        //动画播放0.1秒后开始移动
        yield return new WaitForSeconds(0.1f);
        transform.position = newPos;
        finalPos = newPos + dir;
        //transform.position = finalPos;
        //记录此次操作
        FindObjectOfType<GameManager>().SaveState();
    }
    //用来控制一次移动的结束的协程
    private IEnumerator FinishMove()
    {
        //0.3秒后完成当前移动
        yield return new WaitForSeconds(0.3f);
        moveDone = true;
    }
}
