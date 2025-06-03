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

    [Header("��Ƶ����")]
    public List<AudioData> audioDatas;

    private Dictionary<AudioType, AudioData> audioDataDic;

    [Serializable]
    public class AudioData
    {
        public AudioType type;
        public AudioClip clip;
        public AudioSource source;
    }

    //��̬ʵ��
    private static AudioManager _instance;
    //�������ʵ�
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
            _instance = this;//����Ӻ���Object��������������Ч
            DontDestroyOnLoad(this);//�л�����ʱ������ʵ��
        }
        else if (_instance != this)
        {
            Destroy(gameObject);//���ٶ����ʵ��
        }

        //��ʼ��
        InitAudioSource();
    }

    private void Update()
    {
        CheckChange();
    }

    //��������仯
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

    //��ʼ����Ƶ������
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
