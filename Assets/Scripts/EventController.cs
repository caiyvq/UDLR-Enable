using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventController
{
    public static UnityAction<AudioType,bool> OnPlayAudio;

    public static void RaiseOnPlayAudio(AudioType type, bool play)
    {
        OnPlayAudio?.Invoke(type, play);
    }
}
