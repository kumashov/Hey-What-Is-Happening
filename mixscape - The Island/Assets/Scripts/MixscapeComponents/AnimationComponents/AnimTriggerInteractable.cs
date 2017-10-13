using UnityEngine;

public class AnimTriggerInteractable : InteractableObject
{
    public string Trigger = "shout";

    private Animator _animator;

    // Use this for initialization
    protected virtual void Start()
    {
        _animator = GetComponent<Animator>();
    }

    public override void Interact(Vector3 interactDirection = new Vector3())
    {
        base.Interact(interactDirection);

        if(_animator != null)
        {
            _animator.SetTrigger(Trigger);
        }
    }
}