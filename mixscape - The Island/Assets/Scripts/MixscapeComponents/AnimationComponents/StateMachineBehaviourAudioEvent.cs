using System;
using UnityEngine;

[Serializable]
public class StateMachineBehaviourAudioEvent : StateMachineBehaviour
{
#if UNITY_EDITOR
    public byte[] valueGuid = new byte[16];
#endif

    public int eventID = 0;
}