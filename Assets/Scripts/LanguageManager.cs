using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LanguageManager:MonoBehaviour
{
    static LanguageManager instance;
    public static LanguageManager Instance
    {
        get
        {
            if(instance == null)
            {
                instance = FindAnyObjectByType<LanguageManager>();
                if(instance == null)
                {
                    GameObject go = new("LanguageManager");
                    instance = go.AddComponent<LanguageManager>();
                    DontDestroyOnLoad(instance.gameObject);
                    isDontDestroy=true;
                }
                //���ܷ�������
                //DontDestroyOnLoad(instance.gameObject);
                if (!isDontDestroy)
                {
                    DontDestroyOnLoad(instance.gameObject);
                    isDontDestroy = true;
                }
            }
            return instance;
        }
    }
    static bool isDontDestroy = false;

    SaveManager saveManager;
    public Dropdown languageSet;

    //0:chinese;1:english
    public static int language = 0;
    public string[] quitTexts = { "ȷ���˳���" ,"Do you want to quit?"};
    public string[] deleteTexts = { "ȷ��ɾ������������?���޷��ָ���", "Confirm the deletion of all data? (Cannot be recovered)" };
    public string[] optionLanguageTexts = { "����", "Language" };
    public string[] guideTexts_0 = { "�������Ҽ��ƶ�����Ҫע��ֻ�е������Ӱ��´��м�ͷ�İ�ťʱ����ɫ��ӵ����ü�ͷ��ʾ�ķ����ƶ��������������ϻ���ʾ�����µİ�ť�ķ���\r\n����˼��������������ȫ����Ŀ��λ�ðɡ�" , "Move by the arrow keys, but note that the character has the ability to move in the direction of a button only when the button is pressed by a box, and the direction of the button pressed will be displayed on the box.\r\nThink carefully and push the box into all the target positions." };
    public string[] guideTexts_1 = { "�������������졣\r\n��ɫ�����������ӣ�һ��ֻ���ƶ�һ����\r\n��ɫ����ľ�����ӣ�ֻҪ�ų�һ�ţ�����һ���ƶ���" , "There is a weight difference in the box.\r\nThe gray ones are iron boxes that can only be pushed one at a time.\r\nThe yellow ones are wooden boxes, and as long as they are lined up, they can be pushed together." };
    public string[] guideTexts_2 = { "��ͼ�ı߽紦��ѭ�����ӵġ���û������������£���ɫ���Դӵ�ͼ��һ���ƶ�����Ե���һ�ࡣ\r\n��ȻҲ�����ƶ�����һ�������ƶ���" , "The boundaries of the map are connected in a loop. Without blocking, the character can move from one side of the map to the opposite side.\r\nOf course, you can also push the boxes to move together in this way." };
    public string[] guideTexts_3 = { "��ɫ�İ�ť������������Ч���൱��ͬʱ����������ɫ��ť������ʡ��һ�����ӡ�\r\n������ʱҲ������Ҫ�ú�ɫ��ť������ɫ��ť�������", "The green button contains two directions, which is equivalent to pressing two red buttons at the same time, saving a box.\r\nHowever, there are times when you need to use two red buttons instead of a green button." };
    public string[] guideTexts_4 = { "��ɫ��ť�ܸ����ɫ�ڶ�Ӧ������һ���ƶ�������������ܹ���˴���ĳЩ�ϰ���\r\n�����е���ʧ����ɫҲ���ֻ���ڸ÷�����һ���ƶ���������ĳЩ����µ��ƶ���ɲ��㡣\r\n��Ȼ���������ǽ�ɫ�޷�������������ƶ����ӡ�" , "The blue button gives the character the ability to move two grids at a time in its direction, allowing the character to pass through certain obstacles.\r\nHowever, there are trades and losses, as the character must move two blocks at a time in that direction, which can be inconvenient for movement in some situations.\r\nThe biggest problem, of course, is that the character can't push the box in this direction." };
    public string[] guideTexts_5 = { "R:���¼���\r\nZ:������һ��\r\nQ:�˳���Ϸ��ǰ�ؿ�\r\nTab:��ѡ��˵�\r\nESC(PC��ͬʱ����E,S,C):ɾ����ǰ����\r\nEnter:����ؿ�(�ƶ���Ϊ���Startͼ��)" , "R:Retry\r\nZ:Undo\r\nQ:Quit\r\nTab:Options\r\nESC(Press E,S,C for PC):Delete data\r\nEnter:Enter the level(Click the Start icon on mobile)" };
    public string[] level_0_guide_0 = { "��", "You" };
    public string[] level_0_guide_1 = { "����", "Box" };
    public string[] level_0_guide_2 = { "Ŀ��", "Goal" };
    public string[] level_0_guide_3 = { "����������İ�ť�����Ӱ���ʱ����ɫ���������ƶ���ע���ɫ�޷����°�ť��", "When the button with the left direction is pressed by the box, the character can move to the left. Note that the character cannot press the button." };
    public string[] level_0_guide_4 = { "�������ҷ���İ�ťδ������ʱ����ɫ���������ƶ���" , "When the button with the right direction is not pressed, the character cannot move to the right." };

    public void Start()
    {
        //Init();
        saveManager = FindAnyObjectByType<SaveManager>();
        GameData gameData = saveManager.LoadData();
        language = gameData.language;
        if (FindAnyObjectByType<MainManager>() != null)
        {
            languageSet = GameObject.Find("UI_options").transform.Find("options_menu/Language").GetComponent<Dropdown>();
            languageSet.value = language;
            SetLanguage();
        }
           
    }
    public void SetLanguage()
    {
        //print("change language");
        language = languageSet.value;
        languageSet.transform.Find("Text").GetComponent<Text>().text = optionLanguageTexts[language];
        GameObject.Find("Guidenesses").transform.Find("Guide_0/Text").GetComponent<Text>().text = guideTexts_0[language];
        GameObject.Find("Guidenesses").transform.Find("Guide_1/Text").GetComponent<Text>().text = guideTexts_1[language];
        GameObject.Find("Guidenesses").transform.Find("Guide_2/Text").GetComponent<Text>().text = guideTexts_2[language];
        GameObject.Find("Guidenesses").transform.Find("Guide_3/Text").GetComponent<Text>().text = guideTexts_3[language];
        GameObject.Find("Guidenesses").transform.Find("Guide_4/Text").GetComponent<Text>().text = guideTexts_4[language];
        GameObject.Find("Guidenesses").transform.Find("Guide_5/Text").GetComponent<Text>().text = guideTexts_5[language];
        saveManager.SaveLanguage(language);
    }
}
