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

    public int maxHealth = 3;
    public int health = 3;
    public bool isAlive { get { return health > 0; } }
    public float distanceTargetMax = 10.0f;
    public float distanceContextMax = 5.0f;
    [Range(0.0f, 1.0f)] public float peripheralPower = 0.5f;

    public Player player { get; private set; }
    public int playerIndex { get { return player == null ? -1 : player.index; } }

    protected BaseControl control = null;
    public bool isMovementEnabled { get; private set; }

    int index = 0;
    int nextNearCheckFrame = 0;
    public Actor targetFriend { get; private set; }
    public Actor targetFoe { get; private set; }
    public Actor targetActor { get; private set; }
    public Actor targetContext { get; private set; }

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
    }

    public void SetControl(BaseControl control)
    {
        this.control = control;
    }

    public override void TakeHit(Entity other, int damage = 1) 
    {
        var otherActor = (Actor)other;
        if (otherActor == null)
            return;
        
        if (faction != otherActor.faction)
        {
            health -= damage;
            if (health <= 0)
            {
                health = 0;
                Kill();
            }
        }
    }

    public virtual void Kill() 
    {
        health = 0;
        FXer.main.Fire(FXType.SmokePuff, transform.position + Vector3.up, 0.5f);
        gameObject.SetActive(false);
        BaseCam.main.Shake(0.3f);

        if (onKill != null)
        {
            onKill();
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
            mover.enabled = enable;
        }
        isMovementEnabled = enable;
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
