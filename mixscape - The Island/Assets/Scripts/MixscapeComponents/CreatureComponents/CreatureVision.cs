using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureVision : MonoBehaviour
{
    public string SeePlayerAnimTrigger;
    public bool SeePlayerTurnToFace;
    public AkEvent SeePlayerAudio;

    private Creature _creature;

    // Use this for initialization
    void Start()
    {
        _creature = GetComponentInParent<Creature>();
    }

    protected void OnTriggerEnter(Collider other)
    {
        MixscapePlayer player = other.GetComponent<MixscapePlayer>();
        if(player != null)
        {
            if(_creature != null)
            {
                if(string.IsNullOrEmpty(SeePlayerAnimTrigger) == false)
                {
                    if(_creature.Animator != null)
                    {
                        _creature.Animator.SetTrigger(SeePlayerAnimTrigger);
                    }
                }

                if(SeePlayerTurnToFace)
                {
                    _creature.TurnToFace(player.transform.position - _creature.transform.position);
                }

                if(SeePlayerAudio != null)
                {
                    SeePlayerAudio.HandleEvent(null);
                }
            }
        }
    }

    public bool IsInBounds(Collider collider)
    {
        foreach(var coll in GetComponents<Collider>())
        {
            if(coll.bounds.Intersects(collider.bounds))
            {
                return true;
            }
        }

        return false;
    }
}
