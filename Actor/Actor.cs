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

    public int maxHealth = 3;
    public int health = 3;

    public Player player { get; private set; }
    public int playerIndex { get { return player == null ? -1 : player.index; } }

    protected BaseControl control = null;
    bool movementEnabled = false;

    int index = 0;
    int nextNearCheckFrame = 0;
    Actor soughtFriend = null;
    Actor soughtFoe = null;
    Actor soughtActor = null;

    protected override void OnInit()
    {
        actorCount++;
        index = actorCount;
        mover = GetComponentInChildren<ActorMover>();
        control = GetComponent<BaseControl>();
        player = GetComponentInParent<Player>();
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
        if (isReady == false) return;
        if (movementEnabled)
        {
            if (control != null)
            {
                control.Update();
            }
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
        FXer.main.Fire(FXType.SmokePuff, transform.position + Vector3.up);
        gameObject.SetActive(false);
        BaseCam.main.Shake(0.3f);
    }

    public void EnableMovement(bool enable)
    {
        if (enable == false)
        {
            actions.Reset();
        }
        movementEnabled = enable;
    }

    public Actor SeekActor()
    {
        UpdateSeek();
        return soughtActor;
    }

    public Actor SeekFriend()
    {
        UpdateSeek();
        return soughtFriend;
    }

    public Actor SeekFoe()
    {
        UpdateSeek();
        return soughtFoe;
    }

    void UpdateSeek()
    {
        if (nextNearCheckFrame > Time.frameCount)
            return;

        float nearFriendSq = float.PositiveInfinity;
        float nearFoeSq = float.PositiveInfinity;
        float nearSq = float.PositiveInfinity;

        soughtFriend = null;
        soughtFoe = null;
        soughtActor = null;

        var actors = GameStage.main.GetActors();
        foreach (var a in actors)
        {
            if (a == null || a == this || a.isActiveAndEnabled == false)
                continue;

            var dif = a.transform.position - transform.position;
            dif.y = 0.0f;

            var dot = Vector3.Dot(transform.forward, dif.normalized);
            if (dot < -0.2f)
            {
                continue;
            }

            float dist2 = dif.sqrMagnitude;

            if (a.faction == faction && dist2 < nearFriendSq)
            {
                soughtFriend = a;
                nearFriendSq = dist2;
            }

            if (a.faction != faction && dist2 < nearFoeSq)
            {
                soughtFoe = a;
                nearFoeSq = dist2;
            }

            if (dist2 < nearSq)
            {
                soughtActor = a;
                nearSq = dist2;
            }
        }

        nextNearCheckFrame = Time.frameCount + 1;
    }
}
