using UnityEngine;
using System.Collections;

public class AttackActioner : BaseActioner
{
    public GameObject[] activeObjects;

    int attackCount = 0;

    protected override void OnEnable()
    {
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
        if (actor != null && actor.target != null)
        {
            if (actor.target.faction == actor.faction)
            {
                return;
            }
        }
        Attack();
    }

    void Attack()
    {
        attackCount++;
        var trigger = attackCount % 2 == 0 ? "AttackFore" : "AttackBack";
        mover.animator.SetTrigger(trigger);
        mover.SetDownTime(coolDownTime);
        ActivateObjects(true);
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
