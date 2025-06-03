using System;
using System.Collections;
using System.Collections.Generic;
//using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public bool readyToMove = false;//��ʾ��ɫ�Ƿ��ڿɲ���״̬
    [HideInInspector]
    public Vector2 movedir;//�ƶ�����
    [HideInInspector]
    public Vector2 finalPos;//�ƶ��յ�
    Vector3 speed = Vector3.zero;//��ʼ�ٶ�
    [HideInInspector]
    public bool moveDone = true;//�ƶ��Ƿ����
    public LayerMask layerMask;//��ײ����

    public Text steps;//�ؿ���ǰ�ƶ�����
    public Text bestStep;//�ؿ�����ƶ�����
    int step = 0;//�ؿ���ǰ�ƶ�������ֵ
    int best;//�ؿ�����ƶ�������ֵ

    //ÿ�������ܷ��ƶ�
    [HideInInspector]
    public int moveUp = 0;
    [HideInInspector]
    public int moveDown = 0;
    [HideInInspector]
    public int moveLeft = 0;
    [HideInInspector]
    public int moveRight = 0;

    //��ǰ�Ƿ�����ĳ�������ƶ�����
    [HideInInspector]
    public bool move2grid = false;
    //�ĸ��������ƶ�����
    [HideInInspector]
    public int moveU2 = 0;
    [HideInInspector]
    public int moveD2 = 0;
    [HideInInspector]
    public int moveL2 = 0;
    [HideInInspector]
    public int moveR2 = 0;

    //�߽���
    [HideInInspector]
    public Vector3 newPos;
    // Update is called once per frame
    private void Start()
    {
        finalPos = transform.position;//��ʼ��finalPos
        ShowBest();//��ʾ����ƶ�����
    }
    void Update()
    {
        //����
        if (Input.GetKeyDown(KeyCode.R))
        {
            Retry();
        }

        if (readyToMove)
        {
            //����
            if (Input.GetKeyDown(KeyCode.Z))
            {
                Undo();
            }
        }
        //�ƶ�
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

        //�õ��ƶ��ź�����һ���ƶ��Ѿ����
        if (movedir != Vector2.zero && moveDone)
        {
            moveDone = false;//������һ���ƶ�
            StartCoroutine(FinishMove());//����Э�̣�0.3�����ɴ˴��ƶ�
            //�ж��ܷ��ƶ�
            if (CanMove(movedir))
            {
                Move(movedir);//�ƶ�
            }
            else
            {
                //������Ч
                EventController.RaiseOnPlayAudio(AudioType.Crush, true);
                //���Ŷ���
                GetComponent<Animator>().Play("Crash");
            }
        }
        move2grid = false;//�����ƶ����ź�
        movedir = Vector2.zero;//�ƶ�ʸ������
        transform.position = Vector3.SmoothDamp(transform.position, finalPos, ref speed, 0.03f);//�ý�ɫ�ӵ�ǰλ��ƽ���ƶ�������λ��
        //moveDone = transform.position == (Vector3)finalPos;

    }
    //���������źź��ж��Ƿ�����ƶ�
    public void TryMove(int dir)
    {
        //�ж��Ƿ�ɲ�����ɫ�Լ���һ���ƶ��Ƿ����
        if (!(readyToMove && moveDone))
            return;
        //����������ƶ�����ֱ���
        switch (dir)
        {
            //���ִ����ƶ����� 0:up,1:down,2:left,3:right
            case 0:
                {
                    //������������ƶ�������
                    if (moveUp > 0)
                    {
                        movedir = Vector2.up;//�ƶ�ʸ����Ϊ����һ����λ
                        //�ж��Ƿ��������ƶ�������
                        if (moveU2 != 0)
                        {
                            movedir *= 2;//��Ϊ������λ
                            move2grid = true;//�ź�λ����
                        }
                    }
                    //�����������ƶ�������
                    else if (moveUp == 0)
                    {
                        EventController.RaiseOnPlayAudio(AudioType.Cannot_Move, true);//������Ӧ��Ч
                        moveDone = false;
                        StartCoroutine(FinishMove());//��ֹ����������Ч��ѡ��Э�̿���
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
    //����������Ϸ
    public void Retry()
    {
        EventController.RaiseOnPlayAudio(AudioType.BGM, false);//������Ӧ��Ч
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);//�������볡��
    }
    //���ز���
    public void Undo()
    {
        FindAnyObjectByType<GameManager>().Undo();
    }
    //��ʾ���������ƶ�����
    public void ShowStep(int add)
    {
        if (steps != null)
        {
            step += add;//����
            steps.text = "Steps: " + step;//��ʾ
        }
    }
    //��ʾ��Ѳ���
    void ShowBest()
    {
        if (bestStep != null)
        {
            //��ȡ����
            GameData gameData = FindAnyObjectByType<SaveManager>().LoadData();
            best = gameData.levelList[SceneManager.GetActiveScene().buildIndex - 1].bestStep;
            //��ʾ����
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
    //������Ѳ���
    public void SaveBest()
    {
        FindAnyObjectByType<SaveManager>().SaveBest(step);
    }
    //�ж��ܷ��ƶ�
    public bool CanMove(Vector2 movedir)
    {
        newPos = FindAnyObjectByType<GameManager>().CheckBound(transform.position, movedir);//�߽����ĵ�Чλ��
        //�������ƶ�
        if (!move2grid)
        {
            RaycastHit2D hit = Physics2D.Raycast(newPos, movedir, 1, layerMask);//��ײ���
            //���û�м�⵽�ϰ���
            if (!hit)
            {
                //�����ƶ�
                return true;
            }
            else
            {
                //����ϰ���������
                if (hit.collider.gameObject.GetComponent<Box>() != null)
                {
                    //���������ӽ����ƶ�����
                    return hit.collider.GetComponent<Box>().MoveToDir(movedir);
                }
            }
        }
        //�����ƶ�
        else
        {
            RaycastHit2D hit = Physics2D.Raycast(newPos + (Vector3)movedir / 2 * 1.5f, movedir, 0.99f, layerMask);//��ײ�������������ı�
            //���û�м�⵽�ϰ���
            if (!hit)
            {
                //�����ƶ�
                return true;
            }
        }
        //�����ƶ�
        return false;
    }
    //�ƶ�
    public void Move(Vector2 movedir)
    {
        //��ǰ����+1
        ShowStep(1);
        //����
        if (move2grid)
        {
            //������Ч
            EventController.RaiseOnPlayAudio(AudioType.Move_Player_2, true);
            //���Ŷ���
            GetComponent<Animator>().Play("Move2Grids");
            //����Э�����ƶ���ø�˳��
            StartCoroutine(DelayMove(movedir));
        }
        //һ��
        else
        {
            //������Ч
            EventController.RaiseOnPlayAudio(AudioType.Move_Player, true);
            //λ�ø�Ϊ��Чλ��
            transform.position = newPos;
            //��������λ�ã���ʼ�ƶ�
            finalPos = newPos + (Vector3)movedir;
            //��¼�˴β���
            FindObjectOfType<GameManager>().SaveState();
        }
    }
    //�ӳ��ƶ����������ƶ��Ͷ�����������
    private IEnumerator DelayMove(Vector3 dir)
    {
        //��������0.1���ʼ�ƶ�
        yield return new WaitForSeconds(0.1f);
        transform.position = newPos;
        finalPos = newPos + dir;
        //transform.position = finalPos;
        //��¼�˴β���
        FindObjectOfType<GameManager>().SaveState();
    }
    //��������һ���ƶ��Ľ�����Э��
    private IEnumerator FinishMove()
    {
        //0.3�����ɵ�ǰ�ƶ�
        yield return new WaitForSeconds(0.3f);
        moveDone = true;
    }
}
