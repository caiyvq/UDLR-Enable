using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class SaveManager:MonoBehaviour
{
    [HideInInspector]
    public string savePath;//保存路径
    private void Awake()
    {
        savePath = Application.persistentDataPath + "/game_save.json";//设置保存路径
    }
    private void Update()
    {
        //用来处理游戏进行中存档被删除的情况
        if (!File.Exists(savePath))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
    //加载数据
    public GameData LoadData()
    {
        //检测是否存在文件
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);//读取Json文件
            return JsonUtility.FromJson<GameData>(json);//转化为Gamedata类型
        }
        //创建新存档文件
        else
        {
            CreateData();
            return LoadData();
        }
    }
    //创建初次保存数据
    public void CreateData()
    {
        //创建存档文件
        File.Create(savePath).Dispose();

        //创建存档数据
        GameData newData = new()
        {
            //角色位置
            playerPosition = FindAnyObjectByType<Player>().transform.position
        };

        //关卡信息
        for (int i = 0; i < FindAnyObjectByType<MainManager>().levels.Count; i++)
        {
            LevelData levelData = new()
            {
                state = 0,
                index = i
            };
            newData.levelList.Add(levelData);
        }
        //第0关默认开启
        newData.levelList[0].state = 1;

        //箱子位置
        foreach (GameObject box in FindAnyObjectByType<MainManager>().boxs)
        {
            Vector2 pos = box.transform.position;
            newData.boxList.Add(pos);
        }

        //撤回栈信息
        State initState = new(FindAnyObjectByType<Player>().transform.position, newData.boxList);
        newData.historyStates.Add(initState);
        //操作说明
        for(int i = 0;i < FindAnyObjectByType<MainManager>().guides.Count; i++)
        {
            newData.guideStates.Add(true);
        }
        //保存数据
        SaveData(newData);
    }
    //保存数据
    public void SaveData(GameData gameData)
    {
        string json = JsonUtility.ToJson(gameData, false);//转化为Json数据
        File.WriteAllText(savePath, json);//保存为Json文件
    }
    //保存音量
    public void SaveVolume(float vMusic, float vSound)
    {
        GameData gameData = LoadData();
        gameData.vMusic = vMusic;//背景音
        gameData.vSound = vSound;//效果音
        SaveData(gameData);
    }
    //保存关卡状态
    public void SaveLevelState(int index,bool pass)
    {
        GameData gameData = LoadData();
        //如果当前关卡通过
        if (pass)
        {
            gameData.levelList[index].state = 2;//状态改为已通关
            //当本关不是最后一关且下一关未解锁时解锁下一关
            if (index + 1 < gameData.levelList.Count && gameData.levelList[index + 1].state == 0)
            {
                gameData.levelList[index + 1].state = 1;
            }
        }
        SaveData(gameData);
    }
    //保存主界面角色和箱子位置信息
    public void SavePosition()
    {
        //加载存档数据
        GameData gameData=LoadData();
        //保存玩家信息
        gameData.playerPosition = FindAnyObjectByType<Player>().transform.position;
        //保存箱子信息
        for(int i = 0;i< FindAnyObjectByType<MainManager>().boxs.Count; i++)
        {
            gameData.boxList[i]= FindAnyObjectByType<MainManager>().boxs[i].transform.position;
        }
        //保存撤回栈信息
        List<State> statesList = new(FindAnyObjectByType<GameManager>().historyStates);
        gameData.historyStates = statesList;
        //保存教学是否展示过
        gameData.guideStates = FindAnyObjectByType<MainManager>().guideStates;
        SaveData(gameData);
    }
    //保存关卡最佳步数
    public void SaveBest(int step)
    {
        //加载存档数据
        GameData gameData = LoadData();
        int level = SceneManager.GetActiveScene().buildIndex - 1;
        gameData.levelList[level].bestStep = Math.Min(step, gameData.levelList[level].bestStep);
        SaveData(gameData);
    }
    //保存全游戏通关的信号
    public void SaveWin(bool win)
    {
        //加载存档数据
        GameData gameData = LoadData();
        gameData.hasFinished = win;
        SaveData(gameData);
    }
    //保存语言信息
    public void SaveLanguage(int language)
    {
        GameData gameData = LoadData();
        gameData.language = language;
        SaveData(gameData);
    }
    //删除数据
    public void DeleteData()
    {
        File.Delete(savePath);
    }
}
