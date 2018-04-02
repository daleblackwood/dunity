using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIManager : MonoBehaviour 
{
    static public UIManager main { get; private set; }

    Dictionary<string, UIScreen> screens = new Dictionary<string, UIScreen>();
    public UIScreen currentScreen { get; private set; }
    public UIScreen topScreen { get; private set; }
    public bool isGameScreen { get { return currentScreen != null && currentScreen.isGameScreen; } }
    List<UIScreen> history = new List<UIScreen>();
    Canvas canvas = null;
    RectTransform canvasRect = null;

    void Awake()
    {
        main = this;

        Index();

        canvas = GetComponent<Canvas>();
        canvasRect = canvas.GetComponent<RectTransform>();

        SwitchTo<UIScreenLoad>(true);
    }

    void Index()
    {
        screens.Clear();
        var allScreens = GetComponentsInChildren<UIScreen>(true);
        foreach (var screen in allScreens)
        {
            string key = ScreenKey(screen.GetType().Name);
            screens[key] = screen;
        }
    }

    public void Back()
    {
        Game.main.SetPaused(false);
        history.RemoveAt(history.Count - 1);
        SwitchTo(history[history.Count - 1]);
        history.RemoveAt(history.Count - 1);
    }

    public T GetScreen<T>() where T : UIScreen
    {
        string key = ScreenKey(typeof(T).Name);
        return (T)GetScreen(key);
    }

    public UIScreen GetScreen(string typeName)
    {
        string key = ScreenKey(typeName);
        if (screens.ContainsKey(typeName) == false)
            return null;

        return screens[key];
    }

    public void SwitchTo<T>(bool snap = false) where T : UIScreen
    {
        var newScreen = GetScreen<T>();
        SwitchTo(newScreen, snap);
    }

    public void SwitchTo(UIScreen newScreen, bool snap = false)
    {
        if (Time.unscaledTime < 1.0f)
        {
            snap = true;
        }

        foreach (var pair in screens)
        {
            var screen = pair.Value;
            if (screen.isPersistant == false)
            {
                bool show = screen == newScreen;
                if (screen.isShowing != show)
                {
                    screen.SetShowing(show);
                }
            }
        }

        if (newScreen != null)
        {
            history.Add(newScreen);
            currentScreen = topScreen = newScreen;

            if (newScreen.onShow != null)
            {
                newScreen.onShow();
            }
        }

        if (snap)
        {
            SnapAll();
        }
    }

    public void Overlay(UIScreen screen)
    {
        topScreen = screen;
        screen.SetShowing(true);
    }

    void SnapAll()
    {
        foreach (var pair in screens)
        {
            pair.Value.Snap();
        }
    }

    string ScreenKey(string typeName)
    {
        return typeName.ToUpper().Trim();
    }
}
