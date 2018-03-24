using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ActorMover : GameBehaviour 
{
    static public int GetCastMask()
    {
        return ~(
            1 << LayerMask.NameToLayer("Water") &
            1 << LayerMask.NameToLayer("ActorIgnore")
        );
    }

    private class CollideWater
    {
        public const float OFF_TIME = 0.1f;

        public Collider collider;
        public float offTime = OFF_TIME;
        public Vector3 lastPos = Vector3.zero;

        public CollideWater(Collider collider)
        {
            this.collider = collider;
            lastPos = collider.transform.position;
        }
    }

    static readonly Actions NO_ACTIONS = new Actions();

    public Actions actions;
    public float speed = 12.0f;
    public float jump = 1.0f;
    public bool canPush = true;
    public bool canSwim = false;
    public bool canJump = false;
    [Range(0.0f, 1.0f)] public float power = 0.75f;

    public bool onGround { get; private set; }
    public bool onWall { get; protected set; }
    public bool inWater { get; protected set; }
    public bool pushing { get; private set; }
    public bool knocking { get { return knock.sqrMagnitude > 0.1f; } }
    public Actor player { get; private set; }

    protected Rigidbody body;
    protected new CapsuleCollider collider;
    public ActorAnimator animator { get; protected set; }
    protected RaycastHit hitWall = new RaycastHit();
    protected RaycastHit hitGround = new RaycastHit();
    protected RaycastHit hitDiag = new RaycastHit();
    public float groundY { get; private set; }

    protected float timeLastGround = 0.0f;
    public float timeSinceGround { get { return Time.time - timeLastGround; } }

    protected float timeLastJump = 0.0f;
    public float timeSinceJump { get { return Time.time - timeLastJump; } }

    protected float timeLastMove = 0.0f;
    public float timeSinceMove { get { return Time.time - timeLastMove; } }

    protected float timeLastWater = 0.0f;
    public float timeSinceWater { get { return Time.time - timeLastWater; } }

    public Vector3 velocity { get { return body == null ? Vector3.zero : body.velocity; } }

    public bool holdJump { get; private set; }
    public bool jumping { get; private set; }

    public bool holdAction { get; private set; }

    float downTime = 0.0f;

    BaseActioner[] actioners = null;

    Quaternion targetRotation = Quaternion.identity;
    int layerCast = 0;
    int layerOneWay = 0;
    int layerWater = 0;
    Vector3 wallOrigin = Vector3.zero;
    bool wasOnGround = false;
    bool bounced = false;

    protected float knockPower = 1.0f;
    Vector3 knock = Vector3.zero;

    List<Collider> oneWays = new List<Collider>();
    List<CollideWater> waters = new List<CollideWater>();

    protected override void OnInit()
    {
        body = GetComponent<Rigidbody>();
        collider = GetComponent<CapsuleCollider>();
        animator = GetComponentInChildren<ActorAnimator>();
        layerCast = GetCastMask();
        layerOneWay = LayerMask.NameToLayer("OneWay");
        layerWater = LayerMask.NameToLayer("Water");
        player = GetComponentInParent<Actor>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        actioners = GetComponents<BaseActioner>();
        CancelMomentum();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        CancelMomentum();
    }

    public override void Reset()
    {
        CancelMomentum();
        base.Reset();
    }

    protected void FixedUpdate()
    {
        if (isReady == false) return;

        UpdateCollision();
        OnMove();
    }

    void UpdateCollision()
    {
        var middle = transform.position + body.velocity * 0.01f;
        var forward = body.velocity.sqrMagnitude > 0.001f ? body.velocity : transform.forward;
        forward.y = 0.0f;
        forward.Normalize();

        // ground
        onGround = false;
        var radius = collider.radius;
        var origin = middle;
        origin.y += radius;
        if (Cast(out hitGround, origin, Vector3.down, radius * 1.7f))
        {
            onGround = hitGround.normal.y > 0.3f;
            groundY = hitGround.point.y;
        }

        // one way platforms
        if (oneWays.Count > 0)
        {
            oneWays.Sort((a, b) => {
                return a.bounds.max.y > b.bounds.max.y ? -1 : 1;
            });
            for (int i = oneWays.Count-1; i >= 0; i--)
            {
                var oneWay = oneWays[i];
                var placed = PlaceOnOneWay(oneWay);
                if (placed == false)
                {
                    oneWays.RemoveAt(i);
                    continue;
                }

                onGround = true;
                groundY = oneWay.bounds.max.y;
                break;
            }
        }

        UpdateWaters();

        animator.SetBool("Grounded", onGround || timeSinceGround < 0.1f);

        if (onGround)
        {
            timeLastGround = Time.time;
            bounced = false;
        }
        else
        {
            float minY = collider.bounds.min.y;
            if (minY < groundY)
            {
                groundY = minY;
            }
        }

        // walls
        onWall = false;
        pushing = false;

        radius = collider.radius * 1.2f;
        var dir = forward;
        wallOrigin = collider.bounds.center;
        wallOrigin.y = collider.bounds.min.y + radius + collider.bounds.size.y * 0.15f;

        if (SphericalCast(out hitWall, wallOrigin, transform.forward, collider.radius * 1.6f))
        {
            onWall = hitWall.rigidbody == null || onGround == false;
            if (onWall)
            {
                onWall = Vector3.Dot(transform.forward, hitWall.normal) < -0.2f;
            }
            if (onWall)
            {
                onWall = Mathf.Abs(hitWall.normal.y) < 0.3f;
                if (onWall == false && onGround == false)
                {
                    body.AddForce(Vector3.up * 2.0f, ForceMode.VelocityChange);
                }
            }
            if (onWall)
            {
                float pushDist = radius * 1.1f;
                var dif = hitWall.point - wallOrigin;
                if (dif.sqrMagnitude < pushDist * pushDist)
                {
                    dif = hitWall.normal;
                    dif.y = 0.0f;
                    transform.position += dif.normalized * pushDist * 0.8f;
                }   
            }   
            else
            {
                if (canPush)
                {
                    pushing = onGround;
                }
            }

            //DebugTools.DrawSphere(wallOrigin, onWall ? Color.red : Color.white, radius);
        }

        if (canJump)
        {
            float diagRadius = collider.height * 0.9f;
            if (Cast(out hitDiag, collider.bounds.center, transform.forward - Vector3.up * 0.9f, diagRadius))
            {
                if (hitDiag.normal.y > 0.8f && actions.move.sqrMagnitude > 0.1f)
                {
                    if (body.velocity.y < 5.0f)
                    {
                        body.AddForce(Vector3.up * 25.0f, ForceMode.Impulse);
                    }
                }
            }
        }

        if (canPush) 
        {
            animator.SetBool("Pushing", pushing);
        }

        if (body.velocity.sqrMagnitude > 0.1f)
        {
            timeLastMove = Time.time;
        }
    }

    void UpdateWaters()
    {
        // water collisions
        inWater = false;
        if (waters.Count > 0)
        {
            var waterOrigin = transform.position;
            for (int i=waters.Count-1; i >= 0; i--)
            {
                var water = waters[i];

                var lastPos = water.lastPos;
                water.lastPos = water.collider.transform.position;
                var waterMove = water.lastPos - lastPos;
                waterMove *= 0.8f;
                waterMove.y = 0.0f;

                if (waterMove.sqrMagnitude > 0.01f)
                {
                    transform.position += waterMove;
                }

                bool remove = false;
                if (water.collider.bounds.Contains(waterOrigin) == false)
                {
                    remove = true;
                }

                if (remove)
                {
                    water.offTime -= Time.deltaTime;
                    if (water.offTime < 0.0f)
                    {
                        waters.RemoveAt(i);
                    }
                    continue;
                }

                water.offTime = 0.0f;
            }

            var origin = collider.bounds.center;
            origin.y = collider.bounds.max.y;
            var dist = origin.y - collider.bounds.min.y + 0.5f;
            RaycastHit hitInfo;
            if (Physics.Raycast(origin, Vector3.down, out hitInfo, dist, 1 << layerWater, QueryTriggerInteraction.Collide))
            {
                inWater = true;
                timeLastWater = Time.time;
                if (canSwim)
                {
                    if (hitInfo.distance < dist * 0.5f)
                    {
                        body.AddForce(Physics.gravity * -2.0f, ForceMode.Impulse);
                    }
                }
            }
            else
            {
                inWater = false;
            }
        }

        animator.SetBool("InWater", inWater || timeSinceWater < 0.1f);
    }

    bool PlaceOnOneWay(Collider oneWay)
    {
        if (oneWay == null)
            return false;

        var bounds = collider.bounds;
        var max = bounds.max;
        max.y = Mathf.Lerp(max.y, bounds.center.y, 0.5f);
        bounds.max = max;
        if (oneWay.bounds.Intersects(bounds) == false)
        {
            return false;
        }

        var pos = transform.position;
        if (collider.bounds.min.y - 0.02f < oneWay.bounds.max.y)
        {
            pos.y = oneWay.bounds.max.y - 0.01f;
            pos.y += transform.position.y - collider.bounds.min.y;
            transform.position = pos;
        }

        var vel = body.velocity;
        if (vel.y <= 0.0f)
        {
            vel.y = 0.0f;
            body.velocity = vel;
        }

        return true;
    }

    protected bool Cast(out RaycastHit hitInfo, Vector3 origin, Vector3 dir, float dist)
    {
        dist = dist > 0.0f ? dist : collider.radius;
        dir.Normalize();
        var isHit = Physics.Raycast(origin, dir, out hitInfo, dist, layerCast, QueryTriggerInteraction.Ignore);
        Debug.DrawLine(origin, isHit ? hitInfo.point : origin + dir * dist, isHit ? Color.red : Color.white);
        return isHit;
    }

    bool SphericalCast(out RaycastHit hitInfo, Vector3 origin, Vector3 defaultDir, float radius)
    {
        var colliders = Physics.OverlapSphere(origin, radius, layerCast, QueryTriggerInteraction.Ignore);
        foreach (var c in colliders)
        {
            if (c.transform.IsChildOf(transform))
                continue;

            var dir = defaultDir;

            var meshC = c as MeshCollider;
            if (meshC == null || meshC.convex)
            {
                var p = c.ClosestPoint(origin);
                var dif = p - origin;
                dir = dif.normalized;
            }

            Physics.Raycast(origin, dir, out hitInfo, radius, layerCast, QueryTriggerInteraction.Ignore);
            Debug.DrawLine(origin, hitInfo.point);
            return true;
        }
        hitInfo = new RaycastHit();
        return false;
    }

    public void SetDownTime(float downTime)
    {
        this.downTime = downTime;
    }

    public virtual void UpdateActions(Actions newActions)
    {
        if (downTime > 0.0f)
        {
            downTime -= Time.deltaTime;
            actions = NO_ACTIONS;
            return;
        }
        actions = newActions;

        if (actioners != null)
        {
            foreach (var actioner in actioners)
            {
                actioner.UpdateAction(actions);
            }
        }
    }

    protected virtual bool IsHoldingJump()
    {
        return actions.primary;
    }

    protected virtual void OnJumpPress()
    {   
        Jump();
    }

    protected virtual void OnJumpRelease() 
    {
        JumpStop();
    }

    protected virtual bool IsHoldingAction()
    {
        return actions.secondary;
    }

    protected virtual void OnActionPress()
    {   
        Jump();
    }

    protected virtual void OnActionRelease() 
    {
        JumpStop();
    }

    protected void CancelMomentum()
    {
        body.velocity = Vector3.zero;
        knock = Vector3.zero;
        jumping = false;
    }

    protected virtual Vector3 GetMoveVector()
    {
        var camT = Camera.main.transform;
        var right = camT.transform.right;
        var forward = Vector3.Cross(right, Vector3.up);

        var move = actions.move;
        var targetVelocity = move.y * forward;
        targetVelocity += move.x * right;
        targetVelocity = Vector3.ClampMagnitude(targetVelocity, 1.0f);

        return targetVelocity;
    }

    protected virtual void OnMove()
    {
        var speedLimited = speed;
        if (inWater)
        {
            speedLimited *= 0.4f;
        }

        var targetVelocity = GetMoveVector() * speedLimited;

        if (targetVelocity.sqrMagnitude > 0.01f)
        {
            targetRotation = Quaternion.LookRotation(targetVelocity.normalized, Vector3.up);
        }

        if (onWall)
        {
            var hitNorm = hitWall.normal;
            hitNorm.y = 0.0f;
            hitNorm.Normalize();
            var hitRight = Vector3.Cross(hitNorm, Vector3.up);
            targetVelocity += hitNorm * targetVelocity.magnitude;
            targetVelocity *= 0.5f;
            body.velocity += hitNorm * (body.velocity.magnitude * 0.9f);
        }

        var moveForce = targetVelocity;
        float forceM = 4.0f * power;
        if (knocking)
        {
            moveForce = knock * 100.0f;
            knock *= MathUtils.FixedDeltaEase(0.9f);

            var knockDir = -knock.normalized;
            knockDir.y = 0.0f;
            targetRotation = Quaternion.LookRotation(knockDir, Vector3.up);
            transform.rotation = targetRotation;
        }
        else
        {
            var removePower = 1.0f;
            var removeVelocity = body.velocity;

            /*
            if (onGround == false)
            {
                if (targetVelocity.sqrMagnitude < 0.1f)
                {
                    removePower = 0.0f;
                }
            }
            */

            removeVelocity.y = 0.0f;
            var removeMag = Mathf.Min(speed, removeVelocity.magnitude) * removePower;
            removeVelocity = removeVelocity.normalized * removeMag;
            moveForce -= removeVelocity;
        }

        // down force
        UpdateFall();

        wasOnGround = onGround;

        var addForce = forceM * moveForce;
        body.AddForce(addForce, ForceMode.Impulse);
    }

    protected virtual void UpdateFall()
    {
        if (onGround == false)
        {
            float downForce = 1.0f;
            if (body.velocity.y < 0.0f || (jumping == false && bounced == false))
            {
                downForce = body.velocity.y < 0.0f ? 3.0f : 25.0f;
            }
            body.AddForce(Vector3.down * downForce, ForceMode.Impulse);
        }
        else
        {
            var vel = body.velocity;
            if (vel.y > 0.0f)
            {
                vel.y = 0.0f;
                body.velocity = vel;
            }
        }
        body.useGravity = onGround == false;
    }

    public bool Jump(float power = 1.0f)
    {
        if (CanJump == false)
            return false;
        
        float jumpForce = jump * 100.0f * power;
        body.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        timeLastJump = Time.time;
        jumping = true;
        onGround = false;

        return true;
    }

    public void JumpStop()
    {
        jumping = false;
    }

    public void Bounce(Vector3 force)
    { 
        body.AddForce(force, ForceMode.Impulse);
        timeLastJump = Time.time;
        bounced = true;
        timeLastGround = 0.0f;
        onGround = false;
    }

    public void Knock(Vector3 knockDir)
    {
        knock = knockDir.normalized * knockPower;
        body.AddForce(knock * 100.0f);
    }

    public bool CanJump
    {
        get
        {
            if ((Time.time - timeLastJump) < 0.2f)
                return false;

            if ((Time.time - timeLastGround) > 0.2f)
                return false;

            return true;
        }
    }

    void LateUpdate()
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, MathUtils.DeltaEase(0.2f)); 
        var vel = body.velocity;
        vel.y = 0.0f;
        float animSpeed = vel.magnitude * 0.18f;
        animator.SetFloat("Speed", animSpeed);
    }

    public Vector2 GetInputFromCamera(Vector3 move)
    {
        move.y = 0.0f;
        move.Normalize();

        var camT = Camera.main.transform;
        move = Quaternion.Inverse(camT.rotation) * move;

        var input = new Vector2(move.x, move.z);
        input.Normalize();
        return input;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == layerOneWay)
        {
            PlaceOnOneWay(other);
            if (oneWays.Contains(other) == false)
            {
                oneWays.Add(other);
            }
            return;
        }

        if (other.gameObject.layer == layerWater)
        {
            foreach (var water in waters)
            {
                if (water.collider == other)
                    return;
            }

            waters.Add(new CollideWater(other));
            return;
        }
    }

    /*void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == layerOneWay)
        {
            if (oneWays.Contains(other))
            {
                oneWays.Remove(other);
            }
        }
    }*/
}
