using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�ؿ�������
[System.Serializable]
public class LevelData
{
    public int state;//״̬
    public int index;//���
    public int bestStep = 999;//��Ѳ���
}
//��Ϸ������
[System.Serializable]
public class GameData
{
    public List<LevelData> levelList = new();//ȫ�ؿ�������
    public List<Vector2> boxList = new();//���ӵ�λ������
    public Vector2 playerPosition;//��ɫ��λ������
    public List<State> historyStates = new();//��ʷ����ջ����
    public float vMusic = 0.5f;//��������
    public float vSound = 0.5f;
    public bool hasFinished = false;//��Ϸͨ��״̬����
    public List<bool> guideStates = new();//����˵���Ƿ���ֹ�������
    public int language = 0;//�������ݣ�0:����;1:English
}