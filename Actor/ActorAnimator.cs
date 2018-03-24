using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorAnimator : GameBehaviour 
{
    Animator animator = null;
    Dictionary<string, bool> animProps = new Dictionary<string, bool>();
    List<string> revertInts = new List<string>();
    List<string> revertBools = new List<string>();
    public bool uninteruptable { get; private set; }
    bool hasClips = false;

    protected override void OnInit()
    {
        animator = GetComponentInChildren<Animator>();
        if (animator != null)
        {
            foreach (AnimatorControllerParameter param in animator.parameters)
            {
                string key = param.name;
                animProps[key] = true;
            }
            hasClips = animator.GetCurrentAnimatorClipInfoCount(0) > 0;
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        Reset();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        Reset();
    }

    public override void Reset()
    {
        SetFloat("Speed", 0.0f);
        base.Reset();
    }

    void LateUpdate()
    {
        if (revertInts.Count > 0)
        {
            foreach (var prop in revertInts)
            {
                animator.SetInteger(prop, 0);
            }
            revertInts.Clear();
        }

        if (revertBools.Count > 0)
        {
            foreach (var prop in revertBools)
            {
                animator.SetBool(prop, false);
            }
            revertBools.Clear();
        }

        if (IsActive())
        {
            var state = animator.GetCurrentAnimatorStateInfo(0);
            if (state.normalizedTime >= 0.99f)
            {
                uninteruptable = false;
            }
        }
    }

    public void SetFloat(string key, float value)
    {
        if (CanSet(key) == false)
            return;

        if (animator.GetFloat(key) != value)
        {
            animator.SetFloat(key, value);
        }
    }

    public void SetBool(string key, bool value)
    {
        if (CanSet(key) == false)
            return;

        if (animator.GetBool(key) != value)
        {
            animator.SetBool(key, value);
        }
    }

    public void SetTrigger(string key, bool uninteruptable = false)
    {
        if (CanSet(key) == false)
            return;

        animator.SetTrigger(key);
        this.uninteruptable = uninteruptable;
    }

    public void TriggerBool(string key)
    {
        if (CanSet(key) == false)
            return;

        animator.SetBool(key, true);
        revertBools.Add(key);
    }

    public bool CanSet(string key)
    {
        if (uninteruptable)
            return false;

        if (IsActive() == false)
            return false;

        if (animProps.ContainsKey(key))
            return true;

        //Debug.LogWarning("Can't set animation property " + key + " on " + name);

        return false;
    }

    public bool IsActive()
    {
        return animator != null && animator.isActiveAndEnabled && hasClips;
    }
}
