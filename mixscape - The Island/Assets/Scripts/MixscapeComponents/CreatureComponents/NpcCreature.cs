using System;
using UnityEngine;

[RequireComponent(typeof(InteractableObject))]
public class NpcCreature : Creature
{
    public string GreetParameter = "Greeting";
    public string RespondParameter = "Responding";
    public AkEvent GreetEvent;
    public AkEvent RespondEvent;
    public float EngageTime = 5.0f;

    private bool IsEngaged { get { return _engagedTimer > 0.0f; } }

    private float _engagedTimer = -1.0f;
    private NpcVision _npcVision;

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        InteractableObject interactableObject = GetComponent<InteractableObject>();
        if(interactableObject != null)
        {
            interactableObject.CustomInteractCallback = OnInteract;
        }

        _npcVision = GetComponentInChildren<NpcVision>();

        GreetEvent.m_callbackData = ScriptableObject.CreateInstance<AkEventCallbackData>();
        GreetEvent.m_callbackData.callbackGameObj.Add(gameObject);
        GreetEvent.m_callbackData.callbackFunc.Add("OnGreetingComplete");
        GreetEvent.m_callbackData.callbackFlags.Add((int)AkCallbackType.AK_EndOfEvent);
        GreetEvent.m_callbackData.uFlags |= (int)AkCallbackType.AK_EndOfEvent;

        RespondEvent.m_callbackData = ScriptableObject.CreateInstance<AkEventCallbackData>();
        RespondEvent.m_callbackData.callbackGameObj.Add(gameObject);
        RespondEvent.m_callbackData.callbackFunc.Add("OnRespondComplete");
        RespondEvent.m_callbackData.callbackFlags.Add((int)AkCallbackType.AK_EndOfEvent);
        RespondEvent.m_callbackData.uFlags |= (int)AkCallbackType.AK_EndOfEvent;
    }

    protected override void Update()
    {
        base.Update();
        if(_engagedTimer > 0.0f && !_npcVision.CanSeePlayer())
        {
            _engagedTimer -= Time.deltaTime;
        }
    }

    public void Greet(Vector3 talkDirection)
    {
        if(!IsEngaged)
        {
            _engagedTimer = EngageTime;
            if(Animator != null && string.IsNullOrEmpty(GreetParameter) == false)
            {
                Animator.SetBool(GreetParameter, true);
            }
            GreetEvent.HandleEvent(null);
        }
    }

    public void OnGreetingComplete()
    {
        if(Animator != null && string.IsNullOrEmpty(GreetParameter) == false)
        {
            Animator.SetBool(GreetParameter, false);
        }
    }

    public void OnInteract(Vector3 talkDirection)
    {
        TurnToFace(talkDirection);

        if(IsEngaged)
        {
            Respond(talkDirection);
        }
        else
        {
            Greet(talkDirection);
        }
    }

    private void Respond(Vector3 talkDirection)
    {
        _engagedTimer = EngageTime;

        if(Animator != null && string.IsNullOrEmpty(RespondParameter) == false)
        {
            Animator.SetBool(RespondParameter, true);
        }
        RespondEvent.HandleEvent(null);
    }

    public void OnRespondComplete()
    {
        if(Animator != null && string.IsNullOrEmpty(RespondParameter) == false)
        {
            Animator.SetBool(RespondParameter, false);
        }
    }
}