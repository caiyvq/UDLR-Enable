using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        FindObjectOfType<GameManager>().totalGoals++;//ÿ��Ŀ������������ڿ�ʼʱ��Ŀ������һ���õ���Ŀ����
    }
}
