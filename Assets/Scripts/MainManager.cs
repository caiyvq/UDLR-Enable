using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainManager : MonoBehaviour
{
    public Animator enterImageAnimator;
    public Animator victoryShadeAnimator;
    public Animator victoryImageAnimator;

    [Header("物体管理")]
    public List<GameObject> boxs;
    public List<GameObject> levels;
    public List<GameObject> guides;
    [HideInInspector]
    public List<bool> guideStates;

    public SaveManager saveManager;

    [Header("菜单")]
    public GameObject optionsMenu;
    public Slider vMusicSlider;
    public Slider vSoundSlider;

    public Sprite hint_mobile;
    public GameObject Hint_Image;

    bool win = false;

    private void Awake()
    {
        LoadData();
        if (Application.platform == RuntimePlatform.Android)
        {
            Hint_Image.GetComponent<Image>().sprite = hint_mobile;
            Hint_Image.GetComponent<RectTransform>().sizeDelta = new Vector2(450, Hint_Image.GetComponent<RectTransform>().rect.height);
        }
    }
    void Start()
    {
        //print("play BGM");
        StartCoroutine(PlayBGM());
        LanguageManager.Instance.Start();
    }
    private void Update()
    {
        if (!win)
        { 
            CheckWin();
        }
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            PressTab();
        }
    }
    IEnumerator PlayBGM()
    {
        if (enterImageAnimator != null)
        {
            //播放音效
            EventController.RaiseOnPlayAudio(AudioType.Enter, true);
            enterImageAnimator.Play("enter_image");
        }

        yield return new WaitForSeconds(0.5f);
        GameObject.Find("UI_enter").GetComponent<Canvas>().sortingOrder = 0;
        EventController.RaiseOnPlayAudio(AudioType.BGM, true);
    }
    public void PressTab()
    {
        if (optionsMenu.activeSelf == false && FindAnyObjectByType<DataMenu>().active == false && FindAnyObjectByType<QuitMenu>().active == false)
        {
            LanguageManager.Instance.languageSet = GameObject.Find("UI_options").transform.Find("options_menu/Language").GetComponent<Dropdown>();
            LanguageManager.Instance.languageSet.value = LanguageManager.language;
            optionsMenu.transform.Find("Language/Text").GetComponent<Text>().text = LanguageManager.Instance.optionLanguageTexts[LanguageManager.language];

            EventController.RaiseOnPlayAudio(AudioType.Show_UI,true);
            FindAnyObjectByType<Player>().readyToMove = false;
            optionsMenu.SetActive(true);
            optionsMenu.GetComponent<Animator>().Play("quit_show");
            vMusicSlider.Select();
        }
        else if (optionsMenu.activeSelf == true)
        {
            EventController.RaiseOnPlayAudio (AudioType.Select,true);
            FindAnyObjectByType<Player>().readyToMove = true;
            optionsMenu.GetComponent<Animator>().Play("quit_close");
            StartCoroutine(DisableOption());
            saveManager.SaveVolume(vMusicSlider.value, vSoundSlider.value);
        }
    }
    IEnumerator DisableOption()
    {
        yield return new WaitForSeconds(0.5f);
        optionsMenu.SetActive(false);
    }
    public void PressEnter()
    {
        foreach(var level in levels)
        {
            level.GetComponent<Level>().EnterLevel();
        }
    }
    public void CheckWin()
    {
        if (levels[^1].GetComponent<Level>().state == 2)
        {
            win = true;
            saveManager.SaveWin(win);
            //show win
            if (victoryShadeAnimator != null)
            {
                victoryShadeAnimator.Play("Shade_short");
            }
            if (victoryImageAnimator != null)
            {
                victoryImageAnimator.Play("VictoryImage");
            }
        }
    }
    //读取存档加载主界面
    public void LoadData()
    {
        GameData gameData=saveManager.LoadData();
        //检查存档文件是否存在
        if (gameData != null)
        {
            //加载角色信息
            FindAnyObjectByType<Player>().transform.position = gameData.playerPosition;
            FindAnyObjectByType<Player>().GetComponent<Player>().finalPos = gameData.playerPosition;

            //检查存档文件是否完整正确
            if (levels.Count != gameData.levelList.Count || boxs.Count != gameData.boxList.Count || gameData.historyStates == null)
            {
                print("Load Data Error");
                DeleteData();
                return;
            }

            //加载关卡信息
            for (int i = 0; i < levels.Count; i++)
            {
                levels[i].GetComponent<Level>().state = gameData.levelList[i].state;
            }

            //加载箱子信息
            for (int i = 0; i < boxs.Count; i++)
            {
                boxs[i].transform.position = gameData.boxList[i];
                boxs[i].GetComponent<Box>().finalPos = gameData.boxList[i];
            }

            //加载撤回栈信息，gameData保存的栈信息是从栈顶到栈底
            gameData.historyStates.Reverse();
            //此处去掉最后一个元素的原因：进入场景时，GameManager会自动保存初始位置，和栈顶重复了
            gameData.historyStates.RemoveAt(gameData.historyStates.Count - 1);
            foreach (State state in gameData.historyStates)
            {
                FindAnyObjectByType<GameManager>().historyStates.Push(state);
            }

            //加载音量
            FindAnyObjectByType<AudioManager>().vMusic = gameData.vMusic;
            FindAnyObjectByType<AudioManager>().vSound = gameData.vSound;
            vMusicSlider.value = gameData.vMusic;
            vSoundSlider.value = gameData.vSound;

            win = gameData.hasFinished;

            guideStates = gameData.guideStates;
        }
    }
    public void DeleteData()
    {
        //show UI
        //调用SaveManager.DeleteData
        saveManager.DeleteData();
        //Destroy(LanguageManager.Instance);
        //重新加载
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void SetVolume()
    {
        vMusicSlider.value = (float)Math.Round(vMusicSlider.value, 1);
        vSoundSlider.value= (float)Math.Round( vSoundSlider.value, 1);
        FindAnyObjectByType<AudioManager>().vMusic = vMusicSlider.value;
        FindAnyObjectByType<AudioManager>().vSound = vSoundSlider.value;
    }

    public void GuideClose(int index)
    {
        guideStates[index] = false;
        guides[index].SetActive(false);
        saveManager.SavePosition();
        if (index == 0)
        {
            guides[5].SetActive(true);
            guides[5].GetComponentInChildren<UnityEngine.UI.Button>().Select();
            LanguageManager.Instance.languageSet = guides[5].transform.Find("Language").GetComponent<Dropdown>();
            LanguageManager.Instance.languageSet.value = LanguageManager.language;
            guides[5].transform.Find("Language/Text").GetComponent<Text>().text = LanguageManager.Instance.optionLanguageTexts[LanguageManager.language];
        }
        else
            FindAnyObjectByType<Player>().readyToMove = true;
    }
    public void SetLanguage()
    {
        LanguageManager.Instance.SetLanguage();
    }
}
