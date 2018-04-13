using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIScreen : MonoBehaviour 
{
    const float OFFSET = 800.0f;

    public enum State
    {
        Off,
        Showing,
        On,
        Hiding
    }
    public State state;
    public bool isGameScreen = false;
    public bool isPersistant = false;
    public bool isShowing { get { return state >= State.Showing && state <= State.On; } }
    public float screenTime { get { return Time.unscaledTime - showTime; } }
    public System.Action onShow = null;
    protected float showTime = 0.0f;
    protected float hideTime = 0.0f;
    RectTransform rectTransform;
    protected Vector3 initPos;

    protected virtual void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        initPos = rectTransform.localPosition;
    }

    public void Show()
    {
        gameObject.SetActive(true);

        state = State.Showing;
        showTime = Time.unscaledTime;

        OnShow();

        if (SoundPlayer.main != null)
        {
            SoundPlayer.main.Play(null, "Fill");
        }

        var buttons = GetComponentsInChildren<Button>();
        foreach (var button in buttons)
        {
            if (button.navigation.mode == Navigation.Mode.None)
                continue;
            
            EventSystem.current.SetSelectedGameObject(button.gameObject);
            break;
        }

        //ShowDebug(transform, Game.IsDebug());
    }

    protected virtual void OnShow()
    {
        Vector3 pos = GetOffset(1);
        transform.localPosition = pos;
    }

    void ShowDebug(Transform t, bool show)
    {
        if (t.name.StartsWith("Debug_"))
        {
            t.gameObject.SetActive(show);
        }
            
        foreach (Transform child in t)
        {
            ShowDebug(child, show);
        }
    }

    public void Snap()
    {
        transform.localPosition = initPos;
        if (isShowing)
        {
            ShowComplete();
        }
        else
        {
            HideComplete();
        }
    }

    protected virtual void ShowComplete()
    {
        state = State.On;
    }

    public void Hide()
    {
        state = State.Hiding;
        hideTime = Time.unscaledTime;
        OnHide();
    }

    protected virtual void OnHide()
    {
    }

    protected virtual void HideComplete()
    {
        state = State.Off;
        gameObject.SetActive(false);
    }

    public void SetShowing(bool show)
    {
        if (show)
        {
            Show();
        }
        else
        {
            Hide();
        }
    }

    protected virtual void LateUpdate()
    {
        Vector3 toPos = GetOffset(0);
        if (state >= State.Hiding)
        {
            toPos = GetOffset(-1);
        }

        Vector3 dif = toPos - transform.localPosition;
        dif *= 0.5f;
        Vector3 move = Vector3.ClampMagnitude(dif, Time.unscaledDeltaTime * 4000.0f);
        transform.localPosition += move;

        bool within = dif.sqrMagnitude < 1.0f;
        if (within)
        {
            if (state == State.Showing)
            {
                ShowComplete();
            }
            else if (state == State.Hiding)
            {
                HideComplete();
            }
        }
    }

    protected virtual Vector3 GetOffset(int positions)
    {
        Vector3 offset = initPos;
        if (positions != 0)
        {
            if (Screen.width > Screen.height)
            {
                offset.y -= (float)positions * OFFSET;
            }
            else
            {
                offset.x += (float)positions * OFFSET; 
            }
        }
        return offset;
    }
}
