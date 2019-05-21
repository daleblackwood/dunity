using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBehaviour : MonoBehaviour 
{
    public bool hasInited { get; private set; }
    public bool isReady { get; private set; }

    public Vector3 initPosition { get; protected set; }
    public Quaternion initRotation { get; protected set; }
    public Vector3 initScale { get; protected set; }

    public float timeSinceReady { get { return Time.time - timeLastReady; } }
    float timeLastReady = 0.0f;

    bool firstReadied = false;

    protected virtual void Awake() 
    {
        initPosition = Vector3.zero;
        initRotation = Quaternion.identity;
        initScale = Vector3.one;
        CallInit();   
    }

    protected virtual void OnEnable()
    {
        isReady = false;
        CallInit();
        Game.WhenReady(CallReady);
    }

    void CallInit()
    {
        if (hasInited)
            return;
        
        PreInit();
        OnInit();
        hasInited = true;
    }

    void CallFirstReady()
    {
        if (firstReadied)
            return;

        initPosition = transform.position;
        initScale = transform.localScale;
        initRotation = transform.localRotation;

        OnFirstReady();
        firstReadied = true;
    }

    void CallReady()
    {
        if (gameObject == null)
            return;
        
        CallInit();

        timeLastReady = Time.time;
        isReady = true;

        CallFirstReady();
        OnReady();
    }

    public virtual void ResetPosition()
    {
        transform.position = initPosition;
        transform.localRotation = initRotation;
    }

    protected virtual void PreInit() {}

    protected virtual void OnInit(){}

    protected virtual void OnFirstReady(){}

    protected virtual void OnReady(){}

    public virtual void Reset() {}

    protected virtual void OnDisable()
    {
        isReady = false;
    }
}
