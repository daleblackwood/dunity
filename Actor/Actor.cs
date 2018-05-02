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

    public Player player { get; private set; }
    public int playerIndex { get { return player == null ? -1 : player.index; } }

    protected BaseControl control = null;
    bool enableMovement = true;
    public bool isMovementEnabled { get { return enableMovement && actionDownTime <= 0.0f;  } }

    int index = 0;
    int nextNearCheckFrame = 0;
    public Actor targetFriend { get; private set; }
    public Actor targetFoe { get; private set; }
    public Actor targetActor { get; private set; }
    public Actor targetContext { get; private set; }

    public bool waterAllergic = false;

    public System.Action onReset = null;
    public System.Action onKill = null;

    protected override void OnInit()
    {
        actorCount++;
        index = actorCount;
        mover = GetComponentInChildren<ActorMover>();
        control = GetComponent<BaseControl>();
        player = GetComponentInParent<Player>();
        head = GetComponentInChildren<ActorHead>();
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
        mover.enabled = true;
        mover.Reset();
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

        if (control != null)
        {
            control.UpdateControl();
        }
        if (isMovementEnabled)
        {
            if (mover != null)
            {
                mover.UpdateActions(actions);
            }
        }

        if (isInvincible)
        {
            invincibleTime -= Time.deltaTime;
        }

        if (actionDownTime > 0.0f)
        {
            actionDownTime -= Time.deltaTime;
        }

        if (mover.inWater && waterAllergic)
        {
            Kill();
        }
    }

    public void SetControl(BaseControl control)
    {
        this.control = control;
    }

    public override void TakeHit(EntityHit hit = null) 
    {
        if (isInvincible)
            return;

        if (hit == null)
        {
            hit = new EntityHit();
        }
        
        Actor otherActor = hit.entity == null ? null : (Actor)hit.entity;
        if (otherActor == null || faction != otherActor.faction)
        {
            SetActionDownTime(0.2f);

            Vector3 knock = hit.knock.sqrMagnitude > 0.01f ? hit.knock : Vector3.up * 0.2f + transform.forward;
            mover.Knock(knock);

            if (health <= 1)
            {
                Kill(hit);
            }
            else
            {
                SetInvincibleFor(1.0f);
                health -= hit.damage;
            }
        }
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

    public virtual void Kill(EntityHit hit = null)
    {
        if (health <= 0)
            return;

        mover.enabled = false;
        
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

        FXer.main.Fire(FXType.EnemyExplode, transform.position + Vector3.up, 0.5f);
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
        return targetContext;
    }

    void UpdateSeek()
    {
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

            if (a.faction != faction && dist2 < nearFoeSq)
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
            targetContext = targetActor;
        }

        nextNearCheckFrame = Time.frameCount + 1;
    }
}
