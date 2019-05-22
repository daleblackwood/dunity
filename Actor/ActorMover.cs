using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ActorMover : MonoBehaviour 
{
    private class WaterContact
    {
        public const float OFF_TIME = 0.1f;

        public Collider collider;
        public float offTime = OFF_TIME;
        public Vector3 lastPos = Vector3.zero;

        public WaterContact(Collider collider)
        {
            this.collider = collider;
            lastPos = collider.transform.position;
        }
    }

    static readonly Actions NO_ACTIONS = new Actions();

    public Actions actions;
    public float jump = 1.0f;
    public bool canPush = false;
    public bool canSwim = false;
    public bool canJump = false;
    [Range(0.0f, 1.0f)] public float power = 0.75f;
    public bool sphereCasting = false;
    [Range(0.5f, 2.0f)] public float wallDetectMulti = 1.0f;

    public Actor actor { get; private set; }
    public ActorAnimator animator { get; private set; }
    public Rigidbody body { get; private set; }
    public new CapsuleCollider collider { get; private set; }

    public float speed = 0.0f;
    public float initSpeed { get; protected set; }

    public bool onGround { get; private set; }
    public bool onWall { get; protected set; }
    public bool inWater { get; protected set; }
    public bool pushing { get; private set; }
    public bool knocking { get { return knock.sqrMagnitude > 0.1f; } }
    public bool isSlippery { get; private set; }
    public Vector3 lastGroundPos { get; private set; }

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

    protected float timeLastNonWater = 0.0f;
    public float timeInWater { get { return Time.time - timeLastNonWater; } }

    public Vector3 velocity { get { return body == null ? Vector3.zero : body.velocity; } }
    public Vector3 targetVelocity { get; protected set; }
    public Vector3 animateVelocity { get; protected set; }

    public bool holdJump { get; private set; }
    public bool jumping { get; private set; }

    public bool holdAction { get; private set; }

    float downTime = 0.0f;

    BaseActioner[] actioners = null;

    Quaternion targetRotation = Quaternion.identity;
    Vector3 wallOrigin = Vector3.zero;
    float wallAvoidDist = 1.0f;
    bool wasOnGround = false;
    bool bounced = false;

    protected float knockPower = 1.0f;
    Vector3 knock = Vector3.zero;

    Collider contactingOneWay = null;
    List<WaterContact> contactingWaters = new List<WaterContact>();
    public Vector3 waterMove = Vector3.zero;

    // FIXME: Remove debug float
    public float debugFloat = 0.0f;

    protected virtual void Awake()
    {
        body = GetComponent<Rigidbody>();
        body.constraints = RigidbodyConstraints.FreezeRotation;

        initSpeed = speed;
        animator = GetComponentInChildren<ActorAnimator>();

        actor = GetComponentInParent<Actor>();
        collider = GetComponent<CapsuleCollider>();

        initSpeed = speed;
    }

    protected virtual void OnEnable()
    {
        actioners = GetComponents<BaseActioner>();
        CancelMomentum();
    }

    protected virtual void OnDisable()
    {
        CancelMomentum();
        if (animator != null)
        {
            animator.Reset();
        }
    }

    public void Reset()
    {
        CancelMomentum();
        speed = initSpeed;
        timeLastNonWater = Time.time;
    }

    protected void FixedUpdate()
    {
        UpdateWaterCollision();
        UpdateCollision();

        UpdateWaterMove();
        OnMove();
    }

    void UpdateCollision()
    {
        var forward = actions.move.magnitude * transform.forward * 0.5f;
        forward.y = 0.0f;

        // ground
        onGround = false;
        var radius = collider.radius;
        var origin = collider.bounds.center;
        //origin += forward * 0.5f;
        var dist = collider.height * 0.5f + 0.1f;
        //var dir = Vector3.down + forward;
        //dist *= dir.magnitude;
        if (timeSinceJump > 0.05f)
        {
            isSlippery = false;
            if (Cast(out hitGround, origin, Vector3.down, dist))
            {
                onGround = hitGround.normal.y > 0.3f;
                groundY = hitGround.point.y;
                
                if (onGround)
                {
                    isSlippery = hitGround.transform.gameObject.CompareTag("Slip");
                }
            }

            // one way platform
            if (contactingOneWay != null)
            {
                var placed = PlaceOnOneWay(contactingOneWay);
                if (placed == false)
                {
                    contactingOneWay = null;
                }
                else
                {
                    onGround = true;
                    groundY = contactingOneWay.bounds.max.y;
                }
            }
        }

        if (animator != null)
        {
            animator.SetBool("Grounded", onGround || timeSinceGround < 0.1f);
        }

        if (onGround)
        {
            timeLastGround = Time.time;
            lastGroundPos = transform.position;
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
        wallAvoidDist = collider.radius * 2.3f * wallDetectMulti;
        wallOrigin = collider.bounds.center;
        wallOrigin.y = collider.bounds.min.y;
        wallOrigin.y += collider.radius * 1.8f;
        wallOrigin -= transform.forward * collider.radius;

        if (Cast(out hitWall, wallOrigin, transform.forward, wallAvoidDist))
        {
            // rigidbodys can't be pushed in mid-air
            bool isWall = hitWall.rigidbody == null || onGround == false;
            if (isWall)
            {
                // slopes aren't walls
                onWall = hitWall.normal.y < 0.5f;
            }
            else
            {
                // rigidbodys can be pushed on ground
                if (canPush)
                {
                    pushing = onGround;
                }
            }
        }

        // elevation help
        if (canJump && inWater == false && body.velocity.sqrMagnitude > 0.01f)
        {
            float diagRadius = collider.height * 0.66f;
            if (Physics.Raycast(collider.bounds.center, transform.forward - Vector3.up * 0.9f, out hitDiag, diagRadius, Layers.CastMask, QueryTriggerInteraction.Ignore))
            {
                if (hitDiag.normal.y > 0.8f && actions.move.sqrMagnitude > 0.1f)
                {
                    if (body.velocity.y < 5.0f)
                    {
                        body.AddForce(Physics.gravity * -0.15f * body.mass, ForceMode.Impulse);
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

    void UpdateWaterCollision()
    {
        // water collisions
        inWater = false;

        {
        /*
        for (int i=contactingWaters.Count-1; i >= 0; i--)
        {
            var water = contactingWaters[i];
            if (water == null)
            {
                contactingWaters.RemoveAt(i);
                continue;
            }

            if (water.collider.bounds.Contains(transform.position) == false) 
            {
                contactingWaters.RemoveAt(i);
                continue;
            }
            */

            float waterIn = 0.1f;

            var origin = collider.bounds.center;
            origin.y = collider.bounds.max.y;
            origin.y += waterIn + 0.7f;
            var dist = origin.y - collider.bounds.min.y;
            RaycastHit hitInfo;
            if (Physics.Raycast(origin, Vector3.down, out hitInfo, dist, ~(1 << Layers.Actor), QueryTriggerInteraction.Collide))
            {
                inWater = hitInfo.collider.gameObject.layer == Layers.Water;
                if (inWater)
                {
                    timeLastWater = Time.time;
                    if (canSwim)
                    {
                        if (hitInfo.distance < dist * 0.5f - waterIn)
                        {
                            body.AddForce(Physics.gravity * -10.0f, ForceMode.Acceleration);
                        }
                    }
                }
            }
        }

        if (timeSinceWater > 0.1f)
        {
            contactingWaters.Clear();
            timeLastNonWater = Time.time;
        }
    }

    void UpdateWaterMove()
    {
        waterMove = Vector3.zero;

        foreach (var contactingWater in contactingWaters)
        {   
            var lastPos = contactingWater.lastPos;
            contactingWater.lastPos = contactingWater.collider.transform.position;
            var wm = contactingWater.lastPos - lastPos;
            if (wm.sqrMagnitude > waterMove.sqrMagnitude)
            {
                waterMove = wm;
            }

            //if (waterMove.sqrMagnitude > 0.01f)
            {
                /*
                var subtractVel = waterMove;
                subtractVel.y = 0.0f;
                subtractVel.Normalize();
                var targetVelocity = MathUtils.VectorSubtractDirection(body.velocity, subtractVel);
                targetVelocity += waterMove / Time.fixedDeltaTime;

                var currentDirVel = MathUtils.VectorOnLine(body.velocity, waterMove);
                var targetDirVel = waterMove / Time.fixedDeltaTime * 1.5f;
                var force = targetDirVel - currentDirVel;
                force.y = 0.0f;

                body.AddForce(force, ForceMode.Impulse);
                */
            }
        }

        /*
        if (waterMove.sqrMagnitude > 0.01f)
        {
            var waterVelocity = waterMove;
            waterVelocity.y = 0.1f;
            //waterVelocity /= Time.fixedUnscaledDeltaTime;

            var waterCross = Vector3.Cross(waterVelocity.normalized, Vector3.up);
            //var bodyVelocity = MathUtils.VectorSubtractDirection(body.velocity, waterVelocity.normalized);

            var subtractVelocity = MathUtils.VectorOnLine(body.velocity, waterCross);
            //newVelocity += waterMove;

            var force = body.mass * (waterVelocity - subtractVelocity);
            //body.velocity = newVelocity;

            body.AddForce(force, ForceMode.Impulse);
        }
        */

        if (canSwim)
        {
            animator.SetBool("InWater", inWater || timeSinceWater < 0.1f);
        }
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
        var radius = collider.radius;
        dist = dist > 0.0f ? dist : radius;

        if (sphereCasting)
        {
            dir.Normalize();
            var isHit = Physics.SphereCast(origin, collider.radius, dir, out hitInfo, dist - collider.radius, Layers.CastMask, QueryTriggerInteraction.Ignore);
            #if UNITY_EDITOR
            var color = isHit ? Color.red : Color.white;
            Debug.DrawLine(origin, isHit ? hitInfo.point : origin + dir * dist, isHit ? Color.red : Color.white);
            DebugTools.DrawSphere(origin, color, radius);
            DebugTools.DrawSphere(isHit ? hitInfo.point : origin + dir * dist, color, radius);
            #endif
            return isHit;
        }
        else
        {
            var isHit = Physics.SphereCast(origin, collider.radius, dir, out hitInfo, dist - collider.radius, Layers.CastMask, QueryTriggerInteraction.Ignore);
            #if UNITY_EDITOR
            Debug.DrawLine(origin, isHit ? hitInfo.point : origin + dir * dist, isHit ? Color.red : Color.white);
            #endif
            return isHit;
        }
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
                if (actioner.isActiveAndEnabled)
                {
                    actioner.UpdateAction(actions);
                }
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
        var vel = move.y * forward;
        vel += move.x * right;
        vel = Vector3.ClampMagnitude(vel, 1.0f);

        return vel;
    }

    protected virtual void OnMove()
    {
        var speedLimited = speed;
        if (inWater)
        {
            speedLimited *= 0.4f;
        }

        targetVelocity = GetMoveVector() * speedLimited;

        if (targetVelocity.sqrMagnitude > 0.01f)
        {
            targetRotation = Quaternion.LookRotation(targetVelocity.normalized, Vector3.up);
        }

        if (onWall)
        {
            // cancel or redirect forward momentum when on a wall
            var wallDir = hitWall.normal;
            wallDir.y = 0.0f;
            wallDir.Normalize();

            var targetDir = targetVelocity.normalized;
            var targetWallDot = Vector3.Dot(targetDir, -wallDir);

            if (targetWallDot > 0.75f)
            {
                // remove all forward
                targetVelocity = targetVelocity.y * Vector3.up;
                body.velocity = body.velocity.y * Vector3.up;
            }
            else if (targetWallDot > 0.0f)
            {
                // redirect motion to wall cross
                var crossA = Vector3.Cross(wallDir, Vector3.up);
                var crossB = -crossA;
                var dotA = Vector3.Dot(targetDir, crossA);
                var dotB = Vector3.Dot(targetDir, crossB);
                var newDir = dotA > dotB ? crossA : crossB;
                var slowDown = 0.7f;
                targetVelocity = targetVelocity.magnitude * newDir * slowDown;
                body.velocity = body.velocity.magnitude * newDir * slowDown;
            }
        }
        else if (onGround)
        {
            // no wall, need help getting up slopes
            if (hitGround.normal.y > 0.8f)
            {
                float helpPower = Mathf.Clamp01((1.0f - hitGround.normal.y) * 3.0f);
                debugFloat = helpPower;
                body.AddForce(helpPower * Physics.gravity * -3.0f, ForceMode.Acceleration);
            }
        }

        if (waterMove.sqrMagnitude > 0.01f)
        {
            var waterDir = waterMove.normalized;
            targetVelocity = MathUtils.VectorSubtractDirection(targetVelocity, waterDir, false);
            targetVelocity += waterDir;
        }

        animateVelocity = targetVelocity;
        if (isSlippery)
        {
            var bodyVel = Vector3.ClampMagnitude(body.velocity, speed);
            targetVelocity = Vector3.Lerp(bodyVel, targetVelocity, MathUtils.DeltaEase(0.1f));
        }

        var moveForce = targetVelocity;
        float forceM = power * 20.0f + 5.0f;
        if (knocking)
        {
            moveForce = knock * 1.0f;
            knock *= MathUtils.FixedDeltaEase(0.5f);
        }
        else
        {
            var removePower = 1.0f;
            var removeVelocity = body.velocity;

            body.angularVelocity = Vector3.zero;

            removeVelocity.y = 0.0f;
            var removeMag = Mathf.Min(speed, removeVelocity.magnitude) * removePower;
            removeVelocity = removeVelocity.normalized * removeMag;

            moveForce -= removeVelocity;
        }

        // down force
        UpdateGravity();

        if (wasOnGround != onGround)
        {
            wasOnGround = onGround;
        }

        var addForce = forceM * moveForce;
        body.AddForce(addForce, ForceMode.Acceleration);
    }

    protected virtual void UpdateGravity()
    {
        if (onGround == false)
        {
            float downForce = 10.0f;
            if (body.velocity.y < 0.0f || (jumping == false && bounced == false))
            {
                downForce = body.velocity.y < 0.0f ? 30.0f : 100.0f;
            }
            body.AddForce(Vector3.down * downForce, ForceMode.Acceleration);
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
        body.position += Vector3.up * 0.1f;
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

    public void Knock(Vector3 knockForce)
    {
        knock = knockForce * knockPower * 2.0f;
        Vector3 impulse = knock * body.mass * 10.0f;
        body.constraints = RigidbodyConstraints.None;
        body.AddForce(impulse, ForceMode.Impulse);

        body.AddRelativeTorque(Vector3.right * body.mass * 100.0f, ForceMode.Impulse);

        var knockDir = -knock.normalized;
        knockDir.y = 0.0f;
        targetRotation = Quaternion.LookRotation(knockDir, Vector3.up);
        transform.rotation = targetRotation;
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
        if (timeSinceMove < 0.1f)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, MathUtils.DeltaEase(0.2f)); 
        }
        var vel = body.velocity.sqrMagnitude > 0.1f ? animateVelocity : Vector3.zero;
        vel.y = 0.0f;
        float animSpeed = vel.magnitude * 0.13f;
        if (animator != null)
        {
            animator.SetFloat("Speed", animSpeed);
        }
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
        if (other.gameObject.layer == Layers.OneWay)
        {
            PlaceOnOneWay(other);
            contactingOneWay = other;
            return;
        }

        if (other.gameObject.layer == Layers.Water)
        {
            foreach (var water in contactingWaters)
            {
                if (water.collider == other)
                    return;
            }
            contactingWaters.Add(new WaterContact(other));
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == Layers.Water)
        {
            for (int i = contactingWaters.Count - 1; i >= 0; i--)
            {
                if (contactingWaters[i].collider == other)
                {
                    contactingWaters.RemoveAt(i);
                }
            }
        }
    }
}
