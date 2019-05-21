using UnityEngine;
using System.Collections;

public class AttackActioner : BaseActioner
{
    public GameObject[] activeObjects;

    int attackCount = 0;

    protected override void OnEnable()
    {
        actionType = ActionType.Attack;
        ActivateObjects(false);
        base.OnEnable();
    }

    protected override void OnDisable()
    {
        ActivateObjects(false);
        base.OnDisable();
    }

    protected override void OnActivate()
    {
        if (actor != null && actor.targetActor != null)
        {
            if (actor.targetActor.faction != null && actor.targetActor.faction != actor.faction)
            {
                Attack();
                return;
            }

            var actionTrigger = actor.targetActor.GetComponent<ContextActionTrigger>();
            if (actor.CanTriggerContextAction(actionTrigger))
            {
                actor.TriggerContextAction(actor.targetActor);
            }
        }
        else
        {
            Attack();
        }
    }

    public virtual void Attack()
    {
        actor.SetInvincibleFor(0.2f, true);
        attackCount++;
        var trigger = attackCount % 2 == 0 ? "AttackFore" : "AttackBack";
        mover.animator.SetTrigger(trigger);
        mover.SetDownTime(coolDownTime);

        /*
        Sword sword = actor.item as Sword;
        if (sword != null)
        {
            if (sword.swordType != SwordType.Nothing)
            {
                ActivateObjects(true);
            }
        }
         */
    }

    protected override void OnCooldown()
    {
        ActivateObjects(false);
    }

    void ActivateObjects(bool active)
    {
        if (activeObjects == null)
            return;
        
        foreach (var ao in activeObjects)
        {
            ao.gameObject.SetActive(active);
        }
    }
}
