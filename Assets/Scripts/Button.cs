using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Button : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public int dir;//��ʾ����ť����
    bool isPressed = false;//��ʾ�Ƿ���
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
        ChangeState();//��ť�ɼ���
    }
    //��ť����
    public void OnPointerDown(PointerEventData eventData)
    {
        isPressed = true;
    }
    //��ť�ɿ�
    public void OnPointerUp(PointerEventData eventData)
    {
        isPressed = false;
    }
    //�����ƶ������ı䰴ť�ɼ���
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
