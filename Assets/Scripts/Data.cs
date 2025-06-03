using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//关卡数据类
[System.Serializable]
public class LevelData
{
    public int state;//状态
    public int index;//编号
    public int bestStep = 999;//最佳步数
}
//游戏数据类
[System.Serializable]
public class GameData
{
    public List<LevelData> levelList = new();//全关卡的数据
    public List<Vector2> boxList = new();//箱子的位置数据
    public Vector2 playerPosition;//角色的位置数据
    public List<State> historyStates = new();//历史撤回栈数据
    public float vMusic = 0.5f;//音量数据
    public float vSound = 0.5f;
    public bool hasFinished = false;//游戏通关状态数据
    public List<bool> guideStates = new();//操作说明是否出现过的数据
    public int language = 0;//语言数据，0:中文;1:English
}