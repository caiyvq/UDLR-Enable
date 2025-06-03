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
                //不能放在这里
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
    public string[] quitTexts = { "确认退出吗？" ,"Do you want to quit?"};
    public string[] deleteTexts = { "确认删除所有数据吗?（无法恢复）", "Confirm the deletion of all data? (Cannot be recovered)" };
    public string[] optionLanguageTexts = { "语言", "Language" };
    public string[] guideTexts_0 = { "上下左右键移动，但要注意只有当有箱子按下带有箭头的按钮时，角色才拥有向该箭头表示的方向移动的能力，箱子上会显示所按下的按钮的方向。\r\n认真思考，把箱子推入全部的目标位置吧。" , "Move by the arrow keys, but note that the character has the ability to move in the direction of a button only when the button is pressed by a box, and the direction of the button pressed will be displayed on the box.\r\nThink carefully and push the box into all the target positions." };
    public string[] guideTexts_1 = { "箱子有重量差异。\r\n灰色的是铁制箱子，一次只能推动一个。\r\n黄色的是木制箱子，只要排成一排，就能一起推动。" , "There is a weight difference in the box.\r\nThe gray ones are iron boxes that can only be pushed one at a time.\r\nThe yellow ones are wooden boxes, and as long as they are lined up, they can be pushed together." };
    public string[] guideTexts_2 = { "地图的边界处是循环连接的。在没有阻拦的情况下，角色可以从地图的一侧移动到相对的另一侧。\r\n当然也可以推动箱子一起这样移动。" , "The boundaries of the map are connected in a loop. Without blocking, the character can move from one side of the map to the opposite side.\r\nOf course, you can also push the boxes to move together in this way." };
    public string[] guideTexts_3 = { "绿色的按钮包含两个方向，效果相当于同时按下两个红色按钮，可以省下一个箱子。\r\n不过有时也会遇到要用红色按钮代替绿色按钮的情况。", "The green button contains two directions, which is equivalent to pressing two red buttons at the same time, saving a box.\r\nHowever, there are times when you need to use two red buttons instead of a green button." };
    public string[] guideTexts_4 = { "蓝色按钮能赋予角色在对应方向上一次移动两格的能力，能够借此穿过某些障碍。\r\n不过有得有失，角色也因此只能在该方向上一次移动两格，这会对某些情况下的移动造成不便。\r\n当然最大的问题是角色无法在这个方向上推动箱子。" , "The blue button gives the character the ability to move two grids at a time in its direction, allowing the character to pass through certain obstacles.\r\nHowever, there are trades and losses, as the character must move two blocks at a time in that direction, which can be inconvenient for movement in some situations.\r\nThe biggest problem, of course, is that the character can't push the box in this direction." };
    public string[] guideTexts_5 = { "R:重新加载\r\nZ:撤回上一步\r\nQ:退出游戏或当前关卡\r\nTab:打开选项菜单\r\nESC(PC端同时按下E,S,C):删除当前数据\r\nEnter:进入关卡(移动端为点击Start图标)" , "R:Retry\r\nZ:Undo\r\nQ:Quit\r\nTab:Options\r\nESC(Press E,S,C for PC):Delete data\r\nEnter:Enter the level(Click the Start icon on mobile)" };
    public string[] level_0_guide_0 = { "你", "You" };
    public string[] level_0_guide_1 = { "箱子", "Box" };
    public string[] level_0_guide_2 = { "目标", "Goal" };
    public string[] level_0_guide_3 = { "当带有左方向的按钮被箱子按下时，角色可以向左移动。注意角色无法按下按钮。", "When the button with the left direction is pressed by the box, the character can move to the left. Note that the character cannot press the button." };
    public string[] level_0_guide_4 = { "当带有右方向的按钮未被按下时，角色不能向右移动。" , "When the button with the right direction is not pressed, the character cannot move to the right." };

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
