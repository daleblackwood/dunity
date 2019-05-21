using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateInfo
{
    public enum Status
    {
        Init,
        Running,
        CompleteDefault,
        CompleteSwitch,
        CompletePick
    }

    public string name;
    public float startTime;
    public float endTime = 0.0f;
    public float time { get { return (isComplete ? endTime : StateMachine.LocalTime ) - startTime; } }
    public bool isComplete { get { return status > Status.Running; } }
    public IEnumerator routine = null;
    public System.Func<IEnumerator> method = null;
    public Status status = Status.Init;

    public StateInfo(string name)
    {
        this.name = name;
    }
}

public class StateMachine 
{
    static public float LocalTime { get { return Time.time; } }
    static public readonly StateInfo NullState = new StateInfo("NullState");

    public IStateHost host { get; private set; }
    public StateInfo stateCurrent { get; private set; }
    public float stateChangeTime { get; private set; }
    public float stateTime { get { return LocalTime - stateChangeTime; } }
    public bool hasState { get { return stateCurrent != null; } }

    public StateInfo[] stack = new StateInfo[4];

    public System.Action onStateChange = null;
    MonoBehaviour hostMono = null;

    public StateMachine(IStateHost host)
    {
        this.host = host;
        hostMono = (MonoBehaviour)host;
    }

    public StateInfo SetState(System.Func<IEnumerator> routineMethod, string methodName = null)
    {
        // close unfinished states
        if (stateCurrent != null)
        {
            CloseState(stateCurrent, StateInfo.Status.CompleteSwitch);
        }

        for (int i=stack.Length-1; i > 0; i--)
        {
            stack[i] = stack[i-1];
        }
        stack[0] = stateCurrent;

        StateInfo stateInfo = null;
        if (routineMethod == null)
        {
            stateInfo = NullState;
        }
        else
        {
            var routineName = methodName == null ? routineMethod.Method.Name : methodName;
            stateInfo = new StateInfo(routineName);
            stateInfo.method = routineMethod;
            stateInfo.routine = routineMethod();
            stateInfo.startTime = LocalTime;
            stateInfo.status = StateInfo.Status.Init;
        }

        stateCurrent = stateInfo;
        stateChangeTime = LocalTime;

        if (hostMono != null && hostMono.gameObject.activeSelf)
        {
            hostMono.StopAllCoroutines();
            if (hostMono.enabled == false)
                return null;

            host.StartCoroutine(RunState(stateInfo));
        }
        else
        {
            return null;
        }

        return stateInfo;
    }

    public IEnumerator AddToState(string name, IEnumerator addState)
    {
        host.StartCoroutine(addState);
        return addState;
    }

    IEnumerator RunState(StateInfo stateInfo)
    {
        yield return null;

        if (onStateChange != null)
        {
            onStateChange();
        }

        yield return null;

        if (stateCurrent == null)
        {
            SetState(StateIdle);
        }

        while (stateCurrent.routine == null)
        {
            yield return null;
        }

        stateCurrent.status = StateInfo.Status.Running;
        yield return host.StartCoroutine(stateCurrent.routine);

        CloseState(stateCurrent, StateInfo.Status.CompleteDefault);
        PickState();
    }

    void CloseState(StateInfo stateInfo, StateInfo.Status status)
    {
        if (stateInfo.isComplete)
            return;
        
        stateInfo.status = status;
        stateInfo.endTime = LocalTime;
    }

    IEnumerator StateIdle()
    {
        while (true) 
        {
            yield return null;
        }
    }

    public IEnumerator PickInSeconds(float seconds, System.Action pickAction = null)
    {
        yield return new WaitForSeconds(seconds);
        PickNext(pickAction);
    }

    public IEnumerator PickWhen(System.Func<bool> condition, System.Action pickAction = null)
    {
        yield return null;
        while (condition() == false)
        {
            yield return null;
        }
        PickNext(pickAction);
    }

    public IEnumerator PickWhenNot(System.Func<bool> condition, System.Action pickAction = null)
    {
        yield return null;
        while (condition())
        {
            yield return null;
        }
        PickNext(pickAction);
    }

    void PickState()
    {
        if (IsHostActive())
        {
            host.PickState();
        }
    }

    bool IsHostActive()
    {
        return hostMono != null && hostMono.isActiveAndEnabled;
    }

    void PickNext(System.Action pickAction = null)
    {
        //host.StopAllCoroutines();
        if (pickAction == null)
        {
            pickAction = PickState;
        }
        CloseState(stateCurrent, StateInfo.Status.CompletePick);
        host.StopAllCoroutines();
        if (IsHostActive())
        {
            host.StartCoroutine(PickNextFrame(pickAction));
        }
    }

    IEnumerator PickNextFrame(System.Action pickAction)
    {
        yield return null;
        pickAction();
    }
}
