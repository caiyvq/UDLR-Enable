using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Box : MonoBehaviour
{
    [HideInInspector]
    public Vector2 finalPos;//移动终点
    Vector3 speed = Vector3.zero;
    public LayerMask detectLayer;

    //边界检测
    [HideInInspector]
    public Vector3 newPos;

    void Start()
    {
        finalPos = transform.position;
        FindObjectOfType<GameManager>().totalBoxes++;
    }
    private void Update()
    {
        transform.position = Vector3.SmoothDamp(transform.position, finalPos, ref speed, 0.03f);//平滑移动
    }
    public bool MoveToDir(Vector2 dir)
    {
        newPos = FindAnyObjectByType<GameManager>().CheckBound(transform.position,dir);//边界检测
        RaycastHit2D hit = Physics2D.Raycast(newPos + (Vector3)dir * 0.36f, dir, 1f, detectLayer);//碰撞检测
        //没有障碍物
        if (!hit)
        {
            transform.position = newPos;
            finalPos = newPos + (Vector3)dir;
            EventController.RaiseOnPlayAudio(AudioType.Move_Box, true);
            return true;
        }
        //下面的代码让角色可以推多个箱子,重箱子只能单独推
        else
        {
            //本对象是重箱
            if (this.CompareTag("heavy"))
            {
                return false;
            }
            //碰撞物是箱子
            if (hit.collider.gameObject.GetComponent<Box>() != null)
            {
                //碰撞物是轻箱
                if (hit.collider.GetComponent<Box>().CompareTag("light") && hit.collider.GetComponent<Box>().MoveToDir(dir))
                {
                    transform.position = newPos;
                    finalPos = newPos + (Vector3)dir;
                    EventController.RaiseOnPlayAudio(AudioType.Move_Box, true);
                    return true;
                }
            }
        }
        return false;
    }
    //箱子进入按钮时需要进行的操作
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //collision可能为goal, button, level
        if (collision.CompareTag("Goal"))
        {
            EventController.RaiseOnPlayAudio(AudioType.Box_Into_Goal, true);
            GetComponent<Animator>().Play("Box_Final");
            FindObjectOfType<GameManager>().finishGoals++;//完成的目标加一
            FindAnyObjectByType<Player>().GetComponent<Animator>().Play("happy");
        }
        else
        {
            //不是level或goal，即button
            if (!collision.CompareTag("Level"))
            {
                EventController.RaiseOnPlayAudio(AudioType.Button_Press, true);
            }
            if (collision.CompareTag("Down"))
            {
                FindObjectOfType<Player>().moveDown++;//角色增加对应方向的移动能力，由于可能同时按下多个同一方向的按键，所以采用int类型
            }
            if (collision.CompareTag("Up"))
            {
                FindObjectOfType<Player>().moveUp++;
            }
            if (collision.CompareTag("Left"))
            {
                FindObjectOfType<Player>().moveLeft++;
            }
            if (collision.CompareTag("Right"))
            {
                FindObjectOfType<Player>().moveRight++;
            }
            if (collision.CompareTag("DL"))
            {
                FindObjectOfType<Player>().moveDown++;
                FindObjectOfType<Player>().moveLeft++;
            }
            if (collision.CompareTag("DR"))
            {
                FindObjectOfType<Player>().moveDown++;
                FindObjectOfType<Player>().moveRight++;
            }
            if (collision.CompareTag("UL"))
            {
                FindObjectOfType<Player>().moveUp++;
                FindObjectOfType<Player>().moveLeft++;
            }
            if (collision.CompareTag("UR"))
            {
                FindObjectOfType<Player>().moveUp++;
                FindObjectOfType<Player>().moveRight++;
            }
            if (collision.CompareTag("U2"))
            {
                FindObjectOfType<Player>().moveUp++;
                FindObjectOfType<Player>().moveU2++;
            }
            if (collision.CompareTag("D2"))
            {
                FindObjectOfType<Player>().moveDown++;
                FindObjectOfType<Player>().moveD2++;
            }
            if (collision.CompareTag("L2"))
            {
                FindObjectOfType<Player>().moveLeft++;
                FindObjectOfType<Player>().moveL2++;
            }
            if (collision.CompareTag("R2"))
            {
                FindObjectOfType<Player>().moveRight++;
                FindObjectOfType<Player>().moveR2++;
            }
        }

    }
    //箱子在按钮上时需要进行的操作
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Goal"))
        {
            GetComponent<Animator>().Play("Box_Final");
        }
        else
        {
            //如果按到按钮
            if (collision.GetComponent<Animator>() != null)
            {
                collision.GetComponent<Animator>().Play("Pressed");
            }
            if (collision.CompareTag("Down"))
            {
                GetComponent<Animator>().Play("Box_Button_Down");
            }
            if (collision.CompareTag("Up"))
            {
                GetComponent<Animator>().Play("Box_Button_Up");
            }
            if (collision.CompareTag("Left"))
            {
                GetComponent<Animator>().Play("Box_Button_Left");
            }
            if (collision.CompareTag("Right"))
            {
                GetComponent<Animator>().Play("Box_Button_Right");
            }
            if (collision.CompareTag("DL"))
            {
                GetComponent<Animator>().Play("Box_Button_dl");
            }
            if (collision.CompareTag("DR"))
            {
                GetComponent<Animator>().Play("Box_Button_dr");
            }
            if (collision.CompareTag("UL"))
            {
                GetComponent<Animator>().Play("Box_Button_ul");
            }
            if (collision.CompareTag("UR"))
            {
                GetComponent<Animator>().Play("Box_Button_ur");
            }
            if (collision.CompareTag("U2"))
            {
                GetComponent<Animator>().Play("Box_Button_u2");
            }
            if (collision.CompareTag("D2"))
            {
                GetComponent<Animator>().Play("Box_Button_d2");
            }
            if (collision.CompareTag("L2"))
            {
                GetComponent<Animator>().Play("Box_Button_l2");
            }
            if (collision.CompareTag("R2"))
            {
                GetComponent<Animator>().Play("Box_Button_r2");
            }
        }
    }
    //箱子移出按钮需要进行的操作
    private void OnTriggerExit2D(Collider2D collision)
    {
        GetComponent<Animator>().Play("Box_Idle");
        if (collision.CompareTag("Goal"))
        {
            FindObjectOfType<GameManager>().finishGoals--;//完成的目标减一
        }
        else
        {
            //如果按到按钮
            if (collision.GetComponent<Animator>() != null)
            {
                collision.GetComponent<Animator>().Play("Idle");
            }
            if (collision.CompareTag("Down"))
            {
                FindObjectOfType<Player>().moveDown--;
            }
            if (collision.CompareTag("Up"))
            {
                FindObjectOfType<Player>().moveUp--;
            }
            if (collision.CompareTag("Left"))
            {
                FindObjectOfType<Player>().moveLeft--;
            }
            if (collision.CompareTag("Right"))
            {
                FindObjectOfType<Player>().moveRight--;
            }
            if (collision.CompareTag("DL"))
            {
                FindObjectOfType<Player>().moveDown--;
                FindObjectOfType<Player>().moveLeft--;
            }
            if (collision.CompareTag("DR"))
            {
                FindObjectOfType<Player>().moveDown--;
                FindObjectOfType<Player>().moveRight--;
            }
            if (collision.CompareTag("UL"))
            {
                FindObjectOfType<Player>().moveUp--;
                FindObjectOfType<Player>().moveLeft--;
            }
            if (collision.CompareTag("UR"))
            {
                FindObjectOfType<Player>().moveUp--;
                FindObjectOfType<Player>().moveRight--;
            }
            if (collision.CompareTag("U2"))
            {
                FindObjectOfType<Player>().moveUp--;
                FindObjectOfType<Player>().moveU2--;
            }
            if (collision.CompareTag("D2"))
            {
                FindObjectOfType<Player>().moveDown--;
                FindObjectOfType<Player>().moveD2--;
            }
            if (collision.CompareTag("L2"))
            {
                FindObjectOfType<Player>().moveLeft--;
                FindObjectOfType<Player>().moveL2--;
            }
            if (collision.CompareTag("R2"))
            {
                FindObjectOfType<Player>().moveRight--;
                FindObjectOfType<Player>().moveR2--;
            }
        }
    }
}
