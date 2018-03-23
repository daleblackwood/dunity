using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBehaviour : MonoBehaviour 
{
    public bool hasInited { get; private set; }
    public bool isReady { get; private set; }

    protected Vector3 initPosition = Vector3.zero;
    protected Quaternion initRotation = Quaternion.identity;
    protected Vector3 initScale = Vector3.one;

    bool firstReadied = false;

    protected virtual void Awake() 
    {
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
        CallInit();
        CallFirstReady();
        isReady = true;
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
