using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeEnvironmentEvent : MonoBehaviour
{
#if UNITY_EDITOR
    public byte[] valueGuid = new byte[16];
#endif

    public int eventID = 0;
}
