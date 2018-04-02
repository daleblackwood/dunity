using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour 
{
    static public Game main { get; private set; }

    static System.Action readyActions = null;
    static public void WhenReady(System.Action onReady)
    {
        readyActions += onReady;
    }

    static bool loadCalled = false;
    static public void MakeReady(System.Action onReady = null)
    {
        if (onReady != null)
        {
            WhenReady(onReady);
        }

        if (main == null && loadCalled == false)
        {
            SceneManager.LoadScene("Game", LoadSceneMode.Additive);   
            loadCalled = true;
        }
    }
    
    static public bool isReady { get; private set; }
    static public bool isPaused { get; private set; }
    static public bool isDevelopment { get; private set; }

    static public string[] GetSceneNames()
    {
        var list = new List<string>();

        for (int i=0, lim=SceneManager.sceneCountInBuildSettings; i<lim; i++)
        {
            var sn = SceneUtility.GetScenePathByBuildIndex(i);
            sn = sn.Replace('\\', '/');
            sn = sn.Substring(sn.LastIndexOf('/') + 1);
            sn = sn.Substring(0, sn.LastIndexOf('.'));
            list.Add(sn);
        }

        return list.ToArray();
    }

    void FireWhenReady()
    {
        if (isReady == false)
            return;
        
        if (main == null)
            return;

        if (readyActions != null)
        {
            readyActions();
            readyActions = null;
        }
    }

    public string sceneName = null;
    public GameMode gameMode { get; private set; }
    public GameStage gameStage { get; private set; }
    public string[] sceneNames = null;
    System.Action onNextLoadComplete = null;

    GameMode defaultMode = null;
    Dictionary<string, GameMode> modes = new Dictionary<string, GameMode>();

    void Awake()
    {
        main = this;
        isReady = true;

        sceneNames = GetSceneNames();

        var allModes = GetComponentsInChildren<GameMode>();
        if (allModes.Length < 1)
        {
            Debug.LogError("No game modes.");
        }
        else
        {
            foreach (var mode in allModes)
            {
                modes[mode.GetType().Name] = mode;
            }
            defaultMode = allModes[0];
        }

        if (string.IsNullOrEmpty(sceneName))
        {
            sceneName = SceneManager.GetActiveScene().name;
        }

        isDevelopment = sceneName != "Game";

        if (Camera.main == null)
        {
            var camGo = new GameObject("BlankCamera");
            var cam = camGo.AddComponent<Camera>();
            cam.backgroundColor = Color.black;
        }

        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        if (isDevelopment)
        {
            SetGameMode<DevMode>();
            LoadSceneComplete();
        }
        else
        {
            if (gameMode == null && defaultMode != null)
            {
                SetGameMode(defaultMode);
            }
        }
    }

    void Update()
    {
        if (isReady)
        {
            FireWhenReady();
        }
    }

    public void Pause()
    {
        if (isReady == false)
            return;

        if (isPaused)
            return;

        SetPaused(true);
    }

    public void Resume()
    {
        if (isPaused == false)
            return;
        
        SetPaused(false);
    }

    /*
    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            Pause();
        }
        else
        {
            Resume();
        }
    }

    void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus)
        {
            Pause();
        }
        else
        {
            Resume();
        }
    }
    */

    public void LoadScene(string nextSceneName, bool showLoad = true, System.Action onLoadComplete = null)
    {
        if (isReady == false)
        {
            Debug.LogError("Already loading.");
            return;
        }
        onNextLoadComplete = onLoadComplete;
        StartCoroutine(CoLoadScene(nextSceneName, showLoad));
    }

    IEnumerator CoLoadScene(string nextSceneName, bool showLoad = true)
    {
        isReady = false;
        var prevSceneName = sceneName;

        if (showLoad)
        {
            var loadScreen = GameMode.main.loadScreen;
            UIManager.main.SwitchTo(loadScreen);
        }

        yield return new WaitForSeconds(0.5f);

        sceneName = nextSceneName;
        SceneManager.LoadScene(sceneName);

        yield return new WaitForSeconds(0.2f);

        LoadSceneComplete();
    }

    void LoadSceneComplete()
    {
        isReady = true;
        gameStage = GameObject.FindObjectOfType<GameStage>();

        if (onNextLoadComplete != null)
        {
            onNextLoadComplete();
        }
    }

    public void SetGameMode<T>() where T : GameMode
    {
        var modeKey = typeof(T).Name;
        if (modes.ContainsKey(modeKey) == false)
        {
            throw new UnityException("Can't switch to mode " + modeKey);
        }

        SetGameMode(modes[modeKey]);
    }

    void SetGameMode(GameMode newMode)
    {
        gameMode = newMode;

        foreach (var mode in modes)
        {
            if (mode.Value == newMode)
                continue;

            if (mode.Value.gameObject.activeSelf)
            {
                mode.Value.gameObject.SetActive(false);
            }
        }

        gameMode.gameObject.SetActive(true);
        gameMode.StartMode();
    }
        
    public void ToggleSound()
    {
        bool turnOn = SoundPlayer.main.IsMuted();
        SoundPlayer.main.Mute(turnOn == false);
        PlayerPrefs.SetInt("SoundOn", turnOn ? 1 : 0);
    }

    public void ToggleMusic()
    {
        bool turnOn = MusicPlayer.main.IsMuted();
        MusicPlayer.main.Mute(turnOn == false);
        PlayerPrefs.SetInt("MusicOn", turnOn ? 1 : 0);
    }

    public void SetPaused(bool paused)
    {
        isPaused = paused;
        SetTimeScale(paused ? 0.0f : 1.0f);
    }

    public void SetTimeScale(float scale)
    {
        Time.timeScale = scale;
    }
}
