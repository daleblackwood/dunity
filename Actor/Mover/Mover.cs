using UnityEngine;

public abstract class Mover : GameBehaviour
{
    public Actor actor { get; private set; }
    public bool immutable { get; protected set; }
    public float resetTime { get; private set; }
    public float time { get { return Time.time - resetTime; } }

    protected float targetTime = 0.0f;
    public Actions actions { get; protected set; }
    protected Vector3 wishMove = Vector3.zero;
    protected Quaternion wishRot = Quaternion.identity;

    protected override void OnInit()
    {
        actor = GetComponentInParent<Actor>();
    }

    protected override void OnReady()
    {
        targetTime = Time.time;
        resetTime = Time.time;
    }

    public void UpdateActions(Actions newActions)
    {
        actions = newActions;
        wishMove = Vector3.zero;
        if (actions.move.sqrMagnitude > 0.1f)
        {
            var move = actions.move;
            move = move.normalized * Mathf.Max(Mathf.Abs(move.x), Mathf.Abs(move.y));
            GetMoveCamera(move);
        }
    }

    public virtual void TakeHit(){}

    public virtual void Die(){}

    protected virtual void OnMove(){}

    public virtual void EndMover()
    {
        immutable = false;
        actor.PickMover();
    }

    protected void FixedUpdate()
    {
        OnMove();

        //F = d(mv)/dt.
        var rb = actor.GetComponent<Rigidbody>();
        var subtract = rb.velocity;
        subtract.y = 0.0f;

        var dif = (wishMove * 1.5f - subtract);
        var force = rb.mass * dif / Time.fixedDeltaTime;

        float maxForce = 5000.0f;
        force = Vector3.ClampMagnitude(force, maxForce);

        rb.AddForce(force, ForceMode.Force);

        wishMove *= 0.5f;
    }

    public Vector3 GetMoveCamera(Vector2 input)
    {
        var camT = Camera.main.transform;
        return GetMoveTransform(input, camT);
    }

    public Vector3 GetMoveTransform(Vector2 input, Transform t = null)
    {
        if (t == null)
        {
            t = actor.transform;
        }
        var right = input.x * t.right;
        var forward = Vector3.Cross(t.right, actor.transform.up);
        forward *= input.y;

        return (right + forward).normalized;
    }

    public Vector2 GetInputFromCamera(Vector3 move)
    {
        var camT = Camera.main.transform;
        move = Quaternion.Inverse(camT.rotation) * move;
        return new Vector2(move.x, move.z);
    }
}