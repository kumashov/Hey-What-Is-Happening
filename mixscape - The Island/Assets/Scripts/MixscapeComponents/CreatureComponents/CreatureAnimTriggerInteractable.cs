using UnityEngine;

public class CreatureAnimTriggerInteractable : AnimTriggerInteractable
{
    public bool TurnToFace = true;
    private Creature _creature;

    protected override void Start()
    {
        base.Start();
        _creature = GetComponent<Creature>();
    }

    public override void Interact(Vector3 interactDirection = new Vector3())
    {
        base.Interact(interactDirection);

        if(TurnToFace)
        {
            _creature.TurnToFace(interactDirection);
        }
    }
}