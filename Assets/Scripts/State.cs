using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//��¼�ؿ�ÿһ����Һ�����λ�����ݵ���
[System.Serializable]
public class State
{
    public Vector2 playerPosition;
    public List<Vector2> boxPositions = new();

    public State(Vector2 playerPos, List<Vector2> boxes)
    {
        playerPosition = playerPos;
        boxPositions = new List<Vector2>(boxes);
    }
}
