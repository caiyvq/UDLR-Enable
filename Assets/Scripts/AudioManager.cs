using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum AudioType
{
    BGM,
    BGM_Level,
    Move_Player,
    Move_Player_2,
    Cannot_Move,
    Move_Box,
    Crush,
    Button_Press,
    Victory,
    Box_Into_Goal,
    Confirm_Level,
    Exit,
    Enter,
    Show_UI,
    Select
}
public class AudioManager : MonoBehaviour
{
    [HideInInspector]
    public float vMusic = 0;
    [HideInInspector]
    public float vSound = 0;
    private float preVMusic = 0;
    private float preVSound = 0;

    [Header("音频数据")]
    public List<AudioData> audioDatas;

    private Dictionary<AudioType, AudioData> audioDataDic;

    [Serializable]
    public class AudioData
    {
        public AudioType type;
        public AudioClip clip;
        public AudioSource source;
    }

    //静态实例
    private static AudioManager _instance;
    //公共访问点
    public static AudioManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindAnyObjectByType<AudioManager>();
                DontDestroyOnLoad( _instance);
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if(_instance == null)
        {
            _instance = this;//必须从含有Object的主界面进入才有效
            DontDestroyOnLoad(this);//切换场景时不销毁实例
        }
        else if (_instance != this)
        {
            Destroy(gameObject);//销毁多余的实例
        }

        //初始化
        InitAudioSource();
    }

    private void Update()
    {
        CheckChange();
    }

    //监测音量变化
    private void CheckChange()
    {
        if (preVMusic != vMusic || preVSound != vSound)
        {
            foreach (AudioData data in audioDatas)
            {
                if (data.type == AudioType.BGM || data.type == AudioType.BGM_Level)
                {
                    data.source.volume = vMusic;
                }
                else
                {
                    data.source.volume = vSound;
                }
            }
        }
    }

    //初始化音频管理器
    private void InitAudioSource()
    {
        print("Init audio");
        audioDataDic = new Dictionary<AudioType, AudioData>();
        var audioSources = GetComponents<AudioSource>();

        for(int i = 0; i < audioDatas.Count; i++)
        {
            if (audioDatas[i].source == null)
            {
                if (i < audioSources.Length)
                {
                    audioDatas[i].source = audioSources[i];
                }
                else
                {
                    var newAudioSource = this.AddComponent<AudioSource>();
                    audioDatas[i].source = newAudioSource;
                }
            }
            audioDatas[i].source.clip = audioDatas[i].clip;
            audioDatas[i].source.volume = 0;
            audioDataDic[audioDatas[i].type] = audioDatas[i];
        }
    }

    private void PlayAudio(AudioType type, bool play)
    {
        if(audioDataDic.ContainsKey(type))
        {
            if (play)
            {
                audioDataDic[type].source.Play();
            }
            else
            {
                audioDataDic[type].source.Pause();
            }
            print(type + " " + play);
        }
    }

    private void OnEnable()
    {
        EventController.OnPlayAudio += PlayAudio;
    }
    private void OnDisable()
    {
        EventController.OnPlayAudio -= PlayAudio;
    }
}
