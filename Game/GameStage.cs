using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Plavatar;

public class GameStage : GameBehaviour, IStateHost
{
    static public GameStage main { get; private set; }

    static public bool hasData { get { return main != null && main.saveData != null; } }

    static public readonly Actor[] NO_ACTORS = new Actor[0];

    [HideInInspector] public string saveKey = null;
    [HideInInspector] public SaveData.StageData saveData = null;

    public bool isPlaying { get; protected set; }
    public bool isMovementEnabled = false;

    protected StateMachine state;

    Dictionary<Transform, Vector3> initPositions = new Dictionary<Transform, Vector3>();
    Dictionary<Transform, Quaternion> initRotations = new Dictionary<Transform, Quaternion>();
    protected List<Player> players = new List<Player>();
    protected List<Actor> actors = new List<Actor>();
    protected Entity[] allEntities = null;

    Dictionary<string, Actor[]> cacheFriends = new Dictionary<string, Actor[]>();
    Dictionary<string, Actor[]> cacheFoes = new Dictionary<string, Actor[]>();
    Actor[] cacheActors = null;

    bool isDirtyCache = false;

    protected override void Awake()
    {
        main = this;
        state = new StateMachine(this);
        base.Awake();
    }

    protected override void OnInit()
    {
        main = this;
        Game.MakeReady();
        base.OnInit();
    }

    protected override void OnEnable()
    {
        main = this;
        base.OnEnable();
    }

    public virtual void Spawn()
    {
    }

    protected override void OnReady()
    {
        if (SaveData.main.game == null)
        {
            SaveData.main.Load();
            SaveData.main.game.SetStage(saveKey);
        }
        saveData = SaveData.main.game.GetStageData(this);
        if (saveData == null)
        {
            saveData = new SaveData.StageData();
        }

        Spawn();

        ReindexActors();
        EnableMovement(true);

        base.OnReady();

        var gbs = GameObject.FindObjectsOfType<GameBehaviour>();
        foreach (var gb in gbs)
        {
            gb.Reset();
        }

        StartStage();
    }

    public virtual void StartStage()
    {
        var playScreen = GameMode.main.playScreen;
        UIManager.main.SwitchTo(playScreen);
    }

    public virtual void RestartStage()
    {
        StartStage();
    }

    public void Register(Actor actor)
    {
        if (actors.Contains(actor))
            return;

        actors.Add(actor);
        isDirtyCache = true;
    }

    protected virtual void LateUpdate()
    {
        for (int i=actors.Count-1; i>=0; i--)
        {
            if (actors[i] == null)
            {
                isDirtyCache = true;
            }
        }

        if (isDirtyCache)
        {
            UpdateCache();
        }
    }

    void ReindexActors()
    {
        var allActors = GameObject.FindObjectsOfType<Actor>();
        foreach (var actor in allActors)
        {
            if (actor == null || actor.isActiveAndEnabled == false)
                continue;
            
            Register(actor);
        }
        UpdateCache();
    }

    void UpdateCache()
    {
        cacheActors = NO_ACTORS;
        cacheFriends.Clear();
        cacheFoes.Clear();

        allEntities = GameObject.FindObjectsOfType<Entity>();

        var listActors = new List<Actor>();
        var listFriends = new Dictionary<string, List<Actor>>();
        var listFoes = new Dictionary<string, List<Actor>>();

        // clean and build actor list
        for (int i=actors.Count-1; i>=0; i--)
        {
            var actor = actors[i];

            if (actor == null)
            {
                actors.RemoveAt(i);
                continue;
            }

            listActors.Add(actor);
            
            if (actor.faction == null)
                continue;

            var factionKey = actor.faction.GetKey();
            if (listFriends.ContainsKey(factionKey) == false)
            {
                listFriends[factionKey] = new List<Actor>();
            }

            if (listFoes.ContainsKey(factionKey) == false)
            {
                listFoes[factionKey] = new List<Actor>();
            }

            listFriends[factionKey].Add(actor);
        }

        cacheActors = listActors.ToArray();

        // list foes
        foreach (var actor in actors)
        {
            if (actor.faction == null)
                continue;
            
            var factionKey = actor.faction.GetKey();
           
            foreach (var pair in listFoes)
            {
                var foeKey = pair.Key;
                if (factionKey == foeKey)
                    continue;

                pair.Value.Add(actor);
            }
        }

        // build faction arrays
        foreach (var pair in listFriends)
        {
            cacheFriends[pair.Key] = pair.Value.ToArray();
        }
        foreach (var pair in listFoes)
        {
            cacheFoes[pair.Key] = pair.Value.ToArray();
        }

        isDirtyCache = false;
    }

    public virtual void PickState()
    {
        state.SetState("Null", null);
    }

    protected void SetPlaying(bool enable)
    {
        if (enable)
        {
            EnablePlayers(true);
            isPlaying = true;
        }
        else
        {
            isPlaying = false;
            EnablePlayers(false);
        }
        EnableMovement(enable);
    }

    public void OnSave()
    {
        if (allEntities != null)
        {
            foreach (var entity in allEntities)
            {
                if (entity != null && entity.isActiveAndEnabled)
                {
                    entity.OnSave();
                }
            }
        }
    }

    public void EnablePlayers(bool enable)
    {
        foreach (var player in players)
        {
            player.enabled = false;
            if (enable)
            {
                player.enabled = enable;
            }
        }
    }

    protected void ResetPositions()
    {
        foreach (var pair in initPositions)
        {
            pair.Key.position = pair.Value;
        }
        foreach (var pair in initRotations)
        {
            pair.Key.rotation = pair.Value;
        }
    }

    protected void SetInitPosition(Transform t)
    {
        initPositions[t] = t.position;
        initRotations[t] = t.rotation;
    }

    public void EnableMovement(bool enable)
    {
        foreach (var actor in actors)
        {
            actor.EnableMovement(enable);
        }
        isMovementEnabled = enable;
    }

    public bool IsActionPressed()
    {
        foreach (var player in players)
        {
            if (player.IsActionPressed())
            {
                return true;
            }
        }
        return false;
    }

    public Player[] GetPlayers()
    {
        return players.ToArray();
    }

    public Player GetPlayer(int index)
    {
        if (index < 0 || index >= players.Count)
            return null;

        return players[index];
    }

    public Actor[] GetActors()
    {
        if (cacheActors == null || cacheActors.Length < 1)
            return NO_ACTORS;
        
        return cacheActors;
    }

    public Actor[] GetFriends(Faction faction)
    {
        var factionKey = faction.GetKey();
        if (cacheFriends.ContainsKey(factionKey) == false)
            return NO_ACTORS;
        
        var cache = cacheFriends[factionKey];
        if (cache == null || cache.Length < 1)
            return NO_ACTORS;

        return cache;
    }

    public Actor[] GetFoes(Faction faction)
    {
        var factionKey = faction.GetKey();
        if (cacheFoes.ContainsKey(factionKey) == false)
            return NO_ACTORS;

        var cache = cacheFoes[factionKey];
        if (cache == null || cache.Length < 1)
            return NO_ACTORS;

        return cache;
    }
}
