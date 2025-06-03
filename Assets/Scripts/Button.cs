using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Button : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public int dir;//表示本按钮方向
    bool isPressed = false;//表示是否按下
    public Player player;

    private void Awake()
    {
        player = FindAnyObjectByType<Player>();
    }
    private void Update()
    {
        if (isPressed)
        {
            player.TryMove(dir);
        }
        ChangeState();//按钮可见度
    }
    //按钮按下
    public void OnPointerDown(PointerEventData eventData)
    {
        isPressed = true;
    }
    //按钮松开
    public void OnPointerUp(PointerEventData eventData)
    {
        isPressed = false;
    }
    //根据移动能力改变按钮可见度
    private void ChangeState()
    {
        switch(dir)
        {
            case 0:
                {
                    Color color = GetComponent<Image>().color;
                    if (player.moveUp == 0)
                    {
                        GetComponent<Image>().color = new Color(color.r, color.g, color.b, 0.3f);
                    }
                    else
                    {
                        GetComponent<Image>().color = new Color(color.r, color.g, color.b, 0.8f);
                    }
                    break;
                }
            case 1:
                {
                    Color color = GetComponent<Image>().color;
                    if (player.moveDown == 0)
                    {
                        GetComponent<Image>().color = new Color(color.r, color.g, color.b, 0.3f);
                    }
                    else
                    {
                        GetComponent<Image>().color = new Color(color.r, color.g, color.b, 0.8f);
                    }
                    break;
                }
            case 2:
                {
                    Color color = GetComponent<Image>().color;
                    if (player.moveLeft == 0)
                    {
                        GetComponent<Image>().color = new Color(color.r, color.g, color.b, 0.3f);
                    }
                    else
                    {
                        GetComponent<Image>().color = new Color(color.r, color.g, color.b, 0.8f);
                    }
                    break;
                }
            case 3:
                {
                    Color color = GetComponent<Image>().color;
                    if (player.moveRight == 0)
                    {
                        GetComponent<Image>().color = new Color(color.r, color.g, color.b, 0.3f);
                    }
                    else
                    {
                        GetComponent<Image>().color = new Color(color.r, color.g, color.b, 0.8f);
                    }
                    break;
                }
        }
    }
}
