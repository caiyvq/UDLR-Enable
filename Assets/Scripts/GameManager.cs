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
    
    public int finishGoals = 0;//����ɵ�Ŀ������
    [HideInInspector]
    public int totalBoxes = 0;//�ܹ�����������
    public int totalGoals = 0;//�ܹ���Ŀ������

    private bool win = false;//��־ʤ�����ź�λ

    GameObject UI_quit_new;//�˳��˵�����Щ�������з�����UI��Ϊ����鷳��ɾ���󴴽��µ�UI
    public GameObject UI_mobile;//�ƶ��˵�UI

    public List<GameObject> boxes;//���Ӽ���
    public GameObject player;//��ɫ
    public SaveManager saveManager;//�浵������
    public Stack<State> historyStates = new();//��ʷ����ջ
    //��ౣ��Ĳ���
    public const int MAX_HISTORY_STEPS = 200;

    [Header("������")]
    public Animator enterNumberAnimator;
    public Animator enterImageAnimator;
    public Animator exitImageAnimator;
    public Animator victoryImageAnimator;
    public Animator victoryShadeAnimator;

    //�߽���
    [Header("��ͼ�߽�")]
    public float boundLeft;
    public float boundRight;
    public float boundUp;
    public float boundDown;
    private void Start()
    {
        Time.timeScale = 1f;//ʱ�俪ʼ����

        SetLevel0Language();//���ý̳̹ؿ�������

        ProcessQuiUI();//�����˳��˵�
        ProcessMobileUI();//�����ƶ���UI
        
        //�����볡����
        StartCoroutine(PlayEnterAnim());
        //�����ʼ״̬
        SaveState();
    }
    private void Update()
    {
        //����ؿ�δͨ��
        if (!win)
        {
            //���
            CheckWin();
        }
    }
    //���ý̳̹ص�����
    void SetLevel0Language()
    {
        //�̳̹ص�build index
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            GameObject.Find("character/Canvas/Label/Text").GetComponent<Text>().text = LanguageManager.Instance.level_0_guide_0[LanguageManager.language];
            GameObject.Find("box_heavy/Canvas/Label/Text").GetComponent<Text>().text = LanguageManager.Instance.level_0_guide_1[LanguageManager.language];
            GameObject.Find("goal/Canvas/Label/Text").GetComponent<Text>().text = LanguageManager.Instance.level_0_guide_2[LanguageManager.language];
            GameObject.Find("button_l/Canvas/Label/Text").GetComponent<Text>().text = LanguageManager.Instance.level_0_guide_3[LanguageManager.language];
            GameObject.Find("button_r/Canvas/Label/Text").GetComponent<Text>().text = LanguageManager.Instance.level_0_guide_4[LanguageManager.language];
        }
    }
    //�����˳��˵�
    private void ProcessQuiUI()
    {
        //ɾȥ������
        if (GameObject.Find("UI_quit") != null)
        {
            Destroy(GameObject.Find("UI_quit"));
        }
        //����UI_mobileԤ����ʵ��
        UI_quit_new = Instantiate(Resources.Load("UI_quit")) as GameObject;
    }
    //�����ƶ���UI
    private void ProcessMobileUI()
    {
        //ȡ��PC�˵Ĳ���UI
        if (UI_mobile != null)
        {
            UI_mobile.SetActive(false);
        }
        //��ʾ��
        print(Application.platform);
        //���ƶ��˵Ĳ���UI
        if (Application.platform == RuntimePlatform.Android)
        {
            if (UI_mobile == null)
            {
                //����UI_mobileԤ����ʵ��
                UI_mobile = Instantiate(Resources.Load("UI_mobile")) as GameObject;
            }
            else
            {
                //ֻ�贴���ؿ��ڵģ����������Լ�������UI
                if (FindAnyObjectByType<MainManager>() == null)
                {
                    Destroy(UI_mobile);
                    UI_mobile = Instantiate(Resources.Load("UI_mobile")) as GameObject;
                }
            }
            UI_mobile.SetActive(true);//����UI
            InitButtons();//��ʼ��UI��ť
        }
    }

    //Ϊ�ƶ���UI���¼�
    public void InitButtons()
    {
        if (UI_quit_new!=null)
        {
            UI_mobile.transform.Find("quit").GetComponent<UnityEngine.UI.Button>().onClick.AddListener(UI_quit_new.GetComponentInChildren<QuitMenu>().MenuOpen);//�˳���ť
        }
        UI_mobile.transform.Find("retry").GetComponent<UnityEngine.UI.Button>().onClick.AddListener(player.GetComponent<Player>().Retry);//������ť
        UI_mobile.transform.Find("undo").GetComponent<UnityEngine.UI.Button>().onClick.AddListener(player.GetComponent<Player>().Undo);//���ذ�ť
        //��������
        UI_mobile.transform.Find("Up").GetComponent<Button>().dir = 0;
        UI_mobile.transform.Find("Down").GetComponent<Button>().dir = 1;
        UI_mobile.transform.Find("Left").GetComponent<Button>().dir = 2;
        UI_mobile.transform.Find("Right").GetComponent<Button>().dir = 3;
    }

    //�߽��⣬pos���������ʵλ�ã���������ĵ�Чλ��
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
        //����һλС������ֹƫ�����
        pos.x = (float)Math.Round(pos.x, 1);
        pos.y = (float)Math.Round(pos.y, 1);
        return pos;
    }
    //�ж��Ƿ�ͨ��
    public void CheckWin()
    {
        //���ͨ������
        if (totalGoals == finishGoals && totalGoals != 0)
        {
            //������Ѽ�¼
            player.GetComponent<Player>().SaveBest();
            //�ź�λ����
            win = true;
            //��������ʤ������
            player.GetComponent<Animator>().Play("Victory");
            //��ͣ����
            EventController.RaiseOnPlayAudio(AudioType.BGM, false);
            //��ֹ�����ƶ�
            player.GetComponent<Player>().readyToMove = false;
            //����ʤ��UI����
            StartCoroutine(ShowWinAfterDelay());
            //�����˳�UI����
            StartCoroutine(PlayExitAnim(4.5f));
            //����
            saveManager.SaveLevelState(SceneManager.GetActiveScene().buildIndex - 1, true);
            //����������
            StartCoroutine(BackToMainLevel(5));
        }
    }
    //����ÿ���ƶ��������������λ��
    public void SaveState()
    {
        // ��¼��ǰ����λ��
        List<Vector2> currentBoxPositions = new();
        foreach (GameObject box in boxes)
        {
            currentBoxPositions.Add(box.GetComponent<Box>().finalPos);
        }

        // ������״̬
        State newState = new(
            player.GetComponent<Player>().finalPos,
            currentBoxPositions
        );

        // ���ջ�������Ƴ���ɵ�Ԫ��
        if (historyStates.Count >= MAX_HISTORY_STEPS)
        {
            RemoveOldestState();
        }

        historyStates.Push(newState);
    }
    //�Ƴ���ɵ�״̬
    public void RemoveOldestState()
    {
        // ��ջת��Ϊ�б�˳��ջ�� -> ջ�ף�
        List<State> statesList = new(historyStates);
        // �Ƴ��б����һ��Ԫ�أ�����ɵ�״̬��
        statesList.RemoveAt(statesList.Count - 1);
        statesList.Reverse();
        // ���ջ������ѹ��ʣ��Ԫ��
        historyStates.Clear();
        foreach (State state in statesList)
        {
            historyStates.Push(state);
        }
    }
    // ��������
    public void Undo()
    {
        if (historyStates.Count > 1)
        {
            historyStates.Pop(); // ������ǰ״̬
            State previousState = historyStates.Peek(); // ��ȡ��һ��״̬

            // �ָ����λ��
            player.transform.position = previousState.playerPosition;
            player.GetComponent<Player>().finalPos=player.transform.position;

            // �ָ�����λ��
            for (int i = 0; i < boxes.Count; i++)
            {
                boxes[i].transform.position = previousState.boxPositions[i];
                boxes[i].GetComponent<Box>().finalPos = boxes[i].transform.position;
            }
            //�ؿ���ǰ������һ
            player.GetComponent<Player>().ShowStep(-1);
        }
    }
    //����ͨ�ض���
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
    //�ӳ���Э��
    private IEnumerator ShowWinAfterDelay()
    {
        yield return new WaitForSeconds(2);
        EventController.RaiseOnPlayAudio(AudioType.Victory, true);//����ͨ����Ч
        ShowWin();//����ͨ�ض���
    }
    //�����볡����
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
            //������Ч
            EventController.RaiseOnPlayAudio(AudioType.Enter, true);
            yield return new WaitForSeconds(1);
            EventController.RaiseOnPlayAudio(AudioType.BGM, true);
        }
        yield return new WaitForSeconds(0.1f);
        //����ͼ��˳�򣬷�ֹ����UI����ס
        if (GameObject.Find("UI_enter"))
            GameObject.Find("UI_enter").GetComponent<Canvas>().sortingOrder = 0;
        FindAnyObjectByType<Player>().GetComponent<Player>().readyToMove = true;
    }
    //�����˳�����
    public IEnumerator PlayExitAnim(float time)
    {
        //����ͼ��˳�򣬵�ס����UI
        GameObject.Find("UI_exit").GetComponent<Canvas>().sortingOrder = 4;
        yield return new WaitForSeconds(time);
        if(exitImageAnimator != null)
        {
            exitImageAnimator.Play("exit_image");
        }
        EventController.RaiseOnPlayAudio(AudioType.Exit, true);
    }
    //����������
    public IEnumerator BackToMainLevel(float time)
    {
        yield return new WaitForSeconds(time);
        SceneManager.LoadScene("MainLevel");
    }
}
