using UnityEngine;

public class ColliderTrigger : BaseTrigger
{
    public enum ActorType
    {
        Player,
        Good,
        Bad,
        Any
    }

    public ActorType actorType;

    void Awake()
    {
        if (triggerable == null)
        {
            triggerable = GetComponent<Triggerable>();
        }
    }

    bool IsValidTrigger(Actor actor)
    {
        switch (actorType)
        {
            case ActorType.Bad:
                return actor.faction.Compare("evil");
            case ActorType.Good:
                return actor.faction.Compare("good");
            case ActorType.Player:
                return actor.player != null;
        }
        return true;
    }

    void OnTriggerEnter(Collider other)
    {
        var actor = other.GetComponent<Actor>();
        if (actor == null)
            return;

        if (IsValidTrigger(actor))
        {
            Trigger();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        OnTriggerEnter(collision.collider);
    }
}