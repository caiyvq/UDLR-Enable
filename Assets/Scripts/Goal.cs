using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        FindObjectOfType<GameManager>().totalGoals++;//每个目标对象生命周期开始时总目标数加一，得到总目标数
    }
}
