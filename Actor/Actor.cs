using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : Entity 
{
    static int actorCount = 0;

    public Faction faction = null;

    public Actions actions = new Actions();

    public ActorMover mover { get; private set; }
    public ActorAnimator animator { get; private set; }
    public ActorHead head { get; private set; }
    public AttackActioner attackActioner { get; private set; }
    public ItemHolder itemHolder { get; private set; }

    public Triggerable triggerOnKill;

    public int maxHealth = 3;
    public int health = 3;
    public bool isAlive { get { return health > 0; } }
    public float distanceTargetMax = 10.0f;
    public float distanceContextMax = 5.0f;
    [Range(0.0f, 1.0f)] public float peripheralPower = 0.5f;

    public float invincibleTime = 0.0f;
    public bool isInvincible { get { return invincibleTime > 0.0f; } }
    public bool invincibleMuted = false;

    float actionDownTime = 0.0f;
    float showHealthTime = 0.0f;

    public Player player { get; private set; }
    public int playerIndex { get { return player == null ? -1 : player.index; } }

    protected BaseControl control = null;
    bool enableMovement = true;

    int index = 0;
    int nextNearCheckFrame = 0;
    public Actor targetFriend { get; private set; }
    public Actor targetFoe { get; private set; }
    public Actor targetActor { get; private set; }
    public Actor targetContext { get; private set; }
    public ContextActionTrigger contextTrigger { get; private set; }
    ContextActionTrigger usedTrigger = null;

    public bool waterAllergic = false;

    public System.Action onReset = null;
    public System.Action onKill = null;

    public bool isActionPressed
    {
        get
        {
            if (actions == null)
                return false;

            return actions.primary || actions.secondary;
        }
    }

    public bool isAttacking
    {
        get
        {
            if (attackActioner == null)
                return false;

            return attackActioner.activated;
        }
    }

    public bool isTalking
    {
        get
        {
            //if (UIScreenMessage.main == null)
                return false;

            //return UIScreenMessage.main.speech.isActiveAndEnabled;
        }
    }

    public bool isMovementEnabled 
    { 
        get 
        {
            if (isTalking)
                return false;
            
            return enableMovement && actionDownTime <= 0.0f; 
        }
    }

    public bool isDownTime
    {
        get
        {
            return actionDownTime > 0.0f;
        }
    }

    public float healthPC
    {
        get
        {
            return (float)health / (float)maxHealth;
        }
    }

    public Item item
    {
        get
        {
            if (itemHolder == null)
                return null;

            return itemHolder.item;
        }
    }

    protected override void OnInit()
    {
        actorCount++;
        index = actorCount;
        mover = GetComponentInChildren<ActorMover>(true);
        control = GetComponent<BaseControl>();
        player = GetComponentInParent<Player>();
        head = GetComponentInChildren<ActorHead>(true);
        attackActioner = GetComponentInChildren<AttackActioner>();
        itemHolder = GetComponentInChildren<ItemHolder>();
        contextTrigger = GetComponent<ContextActionTrigger>();
        EnableMovement(false);
        base.OnInit();
    }

    protected override void OnEnable()
    {
        EnableMovement(true);
        base.OnEnable();
    }

    protected override void OnReady()
    {
        GameStage.main.Register(this);
        base.OnReady();
    }

    protected virtual void Start()
    {
        EnableMovement(false);
        EnableMovement(true);
    }

    public override void Reset()
    {
        health = maxHealth;
        base.Reset();
        if (mover != null)
        {
            mover.enabled = true;
            mover.Reset();
        }
        if (onReset != null)
        {
            onReset();
        }
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        if (control != null)
        {
            control.Reset();
        }
        EnableMovement(false);
    }

    void Update()
    {
        if (isReady == false) 
            return;

        if (isInvincible)
        {
            invincibleTime -= Time.deltaTime;
        }

        if (actionDownTime > 0.0f)
        {
            actionDownTime -= Time.deltaTime;
        }

        if (showHealthTime > 0.0f)
        {
            DisplayHealth();
            showHealthTime -= Time.deltaTime;
        }

        if (control != null)
        {
            control.UpdateControl();
        }

        if (mover != null)
        {
            if (isMovementEnabled == false)
            {
                actions.move = Vector2.zero;
            }
            mover.UpdateActions(actions);
            if (mover.inWater && waterAllergic)
            {
                Kill();
            }
        }
    }

    public T SetControl<T>(bool removeOthers = false) where T : BaseControl
    {
        var control = Utils.Require<T>(gameObject);
        return SetControl(control, removeOthers) as T;
    }

    public BaseControl SetControl(BaseControl control, bool removeOthers = false)
    {
        this.control = control;

        var controls = GetComponents<BaseControl>();
        foreach (var c in controls)
        {
            if (c == control)
                continue;

            c.enabled = false;
            if (removeOthers)
            {
                GameObject.Destroy(c);
            }
        }

        control.enabled = true;
        return control;
    }

    public override void TakeHit(EntityAttack attack = null) 
    {
        if (isInvincible)
            return;

        if (attack == null)
        {
            attack = new EntityAttack();
        }
        
        Actor otherActor = attack.entity == null ? null : (Actor)attack.entity;
        if (otherActor == null || faction != otherActor.faction)
        {
            SetActionDownTime(0.2f);

            Vector3 knock = attack.knock.sqrMagnitude > 0.01f ? attack.knock : Vector3.up * 0.2f + transform.forward;
            mover.Knock(knock);

            if (health <= 1)
            {
                Kill(attack);
            }
            else
            {
                SetInvincibleFor(1.0f);
                health -= attack.damage;
                ShowHealth();
            }
        }
    }


    public void ShowHealth()
    {
        if (player == null)
        {
            showHealthTime = 3.0f;
        }
    }

    void DisplayHealth()
    {
        /*
        var meterer = UIProgressMeterer.main;
        if (meterer == null)
            return;

        meterer.ShowProgress(transform, faction, healthPC, Vector3.down * 1.0f);
         */
    }

    public void SetInvincibleFor(float time, bool muted = false)
    {
        if (time > invincibleTime)
        {
            invincibleTime = Mathf.Max(time);
            invincibleMuted = muted;
        }
    }

    public void SetActionDownTime(float time)
    {
        actionDownTime = Mathf.Max(actionDownTime, time);
        if (mover != null)
        {
            mover.SetDownTime(actionDownTime);
        }
    }

    public bool CanTriggerContextAction(ContextActionTrigger trigger)
    {
        return trigger != null && trigger != usedTrigger;
    }

    public void TriggerContextAction(Actor actor)
    {
        if (CanTriggerContextAction(actor.contextTrigger))
        {
            usedTrigger = actor.contextTrigger;
            actor.contextTrigger.Trigger();
        }
    }

    public virtual void Kill(EntityAttack hit = null)
    {
        if (health <= 0)
            return;

        if (mover != null)
        {
            mover.enabled = false;
        }
        
        health = 0;
        EnableMovement(false);
        //BaseCam.main.Shake(0.3f);

        StartCoroutine(CoKill());
    }

    IEnumerator CoKill()
    {
        EnableMovement(false);
        SetActionDownTime(1.0f);
        yield return new WaitForSeconds(faction == Factions.main.GetFaction(1) ? 0.4f : 0.2f);

        FXer.main.Fire(FXType.Explosion, transform.position + Vector3.up, 0.5f);
        BaseCam.main.Shake(0.3f);
        gameObject.SetActive(false);

        if (onKill != null)
        {
            onKill();
        }
        if (triggerOnKill != null)
        {
            triggerOnKill.FireTrigger();
        }
    }

    public void EnableMovement(bool enable)
    {
        if (enable == false)
        {
            actions.Reset();
        }
        if (mover != null)
        {
            //mover.enabled = enable;
        }
        enableMovement = enable;
    }

    public Actor SeekActor()
    {
        UpdateSeek();
        return targetActor;
    }

    public Actor SeekFriend()
    {
        UpdateSeek();
        return targetFriend;
    }

    public Actor SeekFoe()
    {
        UpdateSeek();
        return targetFoe;
    }

    public Actor SeekContextActor()
    {
        UpdateSeek();
        if (targetContext != null && targetContext.contextTrigger == usedTrigger)
        {
            return null;
        }
        return targetContext;
    }

    void UpdateSeek()
    {
        if (this == null || isActiveAndEnabled == false)
            return;

        if (nextNearCheckFrame > Time.frameCount)
            return;

        float maxSq = distanceTargetMax * distanceTargetMax;

        float nearFriendSq = maxSq;
        float nearFoeSq = maxSq;
        float nearSq = maxSq;

        targetFriend = null;
        targetFoe = null;
        targetActor = null;

        var actors = GameStage.main.GetActors();
        foreach (var a in actors)
        {
            if (a == null || a == this || a.isActiveAndEnabled == false)
                continue;

            var dif = a.transform.position - transform.position;
            dif.y = 0.0f;

            var dot = Vector3.Dot(transform.forward, dif.normalized);
            if (dot < -Mathf.Clamp01(peripheralPower))
            {
                continue;
            }

            float dist2 = dif.sqrMagnitude;

            if (a.faction == faction && dist2 < nearFriendSq)
            {
                targetFriend = a;
                nearFriendSq = dist2;
            }

            if (a.faction != faction && a.faction != null && dist2 < nearFoeSq)
            {
                targetFoe = a;
                nearFoeSq = dist2;
            }

            if (dist2 < nearSq)
            {
                targetActor = a;
                nearSq = dist2;
            }
        }

        if (targetContext != null)
        {
            var dif = targetContext.transform.position - transform.position;
            var offDist = distanceContextMax + 1.0f;
            if (dif.sqrMagnitude > offDist * offDist)
            {
                targetContext = null;
            }
            else
            {
                var dot = Vector3.Dot(transform.forward, dif.normalized);
                if (dot < -Mathf.Lerp(-1, Mathf.Clamp01(peripheralPower), 0.5f))
                {
                    targetContext = null;
                }
            }
        }

        if (targetContext == null && targetActor != null && nearSq < distanceContextMax * distanceContextMax)
        {
            if (targetActor.contextTrigger != null && targetActor.contextTrigger.triggerable != null)
            {
                targetContext = targetActor;
            }
        }

        if (targetContext == null || targetContext.contextTrigger != usedTrigger)
        {
            usedTrigger = null;
        }

        nextNearCheckFrame = Time.frameCount + 1;
    }

    public void SetTarget(Actor target)
    {
        targetActor = target;
        targetFoe = target.faction == faction ? null : target;
        targetFriend = target.faction == faction ? target : null;
        targetContext = null;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == Layers.Kills)
        {
            FXer.main.Fire(FXType.Explosion, transform.position, 2.0f);
            BaseCam.main.Jolt(0.3f);
            Kill();
        }
        if (other.gameObject.layer == Layers.Hurts)
        {
            BaseCam.main.Jolt(0.3f);
            TakeHit();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == Layers.Hurts || collision.gameObject.layer == Layers.Kills)
        {
            OnTriggerEnter(collision.collider);
        }
    }

    public bool IsNearTarget(float distance)
    {
        return IsNearActor(targetActor, distance);
    }

    public bool IsNearFoe(float distance)
    {
        return IsNearActor(targetFoe, distance);
    }

    public bool IsNearFriend(float distance)
    {
        return IsNearActor(targetFriend, distance);
    }

    public bool IsNearActor(Actor actor, float distance)
    {
        if (actor == null)
            return false;
        
        return IsNearTransform(actor.transform, distance);
    }

    public bool IsNearTransform(Transform other, float distance)
    {
        if (other == null)
            return false;
        
        float dist2 = (transform.position - other.position).sqrMagnitude;
        return dist2 < distance * distance;
    }
}
