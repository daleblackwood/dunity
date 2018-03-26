using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Saver : MonoBehaviour 
{
    static public Saver main { get; private set; }

    static void SaveLocal(string reason = "No Reason")
    {
        Debug.Log("Saved: " + reason);
        PlayerPrefs.Save();
    }

    static public void SaveGameNode(int slot, SaveGame outGameScore)
    {
        outGameScore.slot = slot;
        string key = "Game" + slot.ToString();
        SaveGameNode(key, outGameScore);
    }

    static void SaveGameNode(string key, SaveGame outGameScore)
    {
        string jsonStr = JsonUtility.ToJson(outGameScore, false);
        PlayerPrefs.SetString(key, jsonStr);
        SaveLocal("GameNode " + key);
    }

    static public SaveGame LoadGameNode(int slot)
    {
        string key = "Game" + slot.ToString();
        return LoadGameNode(key);
    }

    static SaveGame LoadGameNode(string key)
    {
        string gameJson = PlayerPrefs.GetString(key, null);
        var gameNode = JsonUtility.FromJson<SaveGame>(gameJson);
        return gameNode != null ? gameNode : new SaveGame();
    }

    static int ScaleInt(float scale, int value)
    {
        return Mathf.RoundToInt((float)value * scale);
    }

    [System.Serializable] public class IntMap : SerializableDictionary<string, int> {}
    [System.Serializable] public class StringMap : SerializableDictionary<string, string> {}

    [System.Serializable]
    public class SaveGame
    {
        public int slot = 0;
        public int score = 0;
        public bool isNew = true;
        public ItemCounter itemCounter = new ItemCounter();
        public IntMap values = new IntMap();
        public IntMap markers = new IntMap();
        public StringMap transforms = new StringMap();
        public string spawnIsland = "DAWN";
        public string saveName = "New Game";
        public int iteration { get; private set; }

        public SaveGame()
        {
            int startHearts = 3;
            if (itemCounter.GetCount(ItemType.Heart) < startHearts)
            {
                itemCounter.SetCount(ItemType.Heart, startHearts);
            }
        }

        public void SetMarker(string key, bool value = true)
        {
            key = ParseKey(key);
            if (value ==  false)
            {
                UnsetMarker(key);
                return;
            }
            markers[key] = value ? 1 : 0;
            iteration++;
        }

        public void UnsetMarker(string key)
        {
            key = ParseKey(key);
            if (markers.ContainsKey(key))
            {
                markers.Remove(key);
            }
            iteration++;
        }

        public void UnsetMarkers()
        {
            markers.Clear();
            iteration++;
        }

        public void SetCollected(Entity entity, bool value = true)
        {
            SetValue(entity.GetKey(), value);
        }

        public bool IsCollected(Entity entity)
        {
            return GetBool(entity.GetKey());
        }

        public void SetValue(string key, bool value)
        {
            key = ParseKey(key);
            SetValue(key, value ? 1 : 0);
        }

        public void SetValue(string key, int value)
        {
            key = ParseKey(key);
            values[key] = value;
            iteration++;
        }

        public bool GetBool(string key)
        {
            key = ParseKey(key);
            return GetInt(key) > 0;
        }

        public int GetInt(string key)
        {
            key = ParseKey(key);
            if (values.ContainsKey(key) == false)
                return 0;

            return values[key];
        }

        public void SaveTransform(Entity entity)
        {
            string key = ParseKey(entity.GetKey());

            var t = entity.transform;
            var pos = t.position;
            var angle = t.localEulerAngles.y;

            var parts = new string[]
            {
                pos.x.ToString("0.0"),
                pos.y.ToString("0.0"),
                pos.z.ToString("0.0"),
                Mathf.RoundToInt(angle).ToString()
            };

            var tstr = string.Join(",", parts);
            transforms[key] = tstr;
        }

        public void LoadTransform(Entity entity)
        {
            string key = ParseKey(entity.GetKey());

            if (transforms.ContainsKey(key) == false)
                return;

            var tstr = transforms[key];
            var parts = tstr.Split(',');

            var pos = Vector3.zero;

            float.TryParse(parts[0], out pos.x);
            float.TryParse(parts[1], out pos.y);
            float.TryParse(parts[2], out pos.z);

            pos.y += 1.0f;

            var angle = 0.0f;
            float.TryParse(parts[3], out angle);

            var t = entity.transform;
            t.SetParent(null, false);
            t.position = pos;
            t.localEulerAngles = new Vector3(0, angle, 0);
        }

        string ParseKey(string key)
        {
            return key.Trim().ToUpper();
        }

        public SaveGame Clone()
        {
            return JsonUtility.FromJson<SaveGame>(JsonUtility.ToJson(this));
        }
    }

    public SaveGame saveGame;

    void Awake()
    {
        main = this;
    }

    public void Reset()
    {
        saveGame = new SaveGame();
    }

    public void ResetAll()
    {
        Reset();
        saveGame = new SaveGame();
    }

    public void Load(int slot = -1)
    {
        if (slot < 0)
        {
            if (saveGame.slot >= 0)
            {
                slot = saveGame.slot;
            }
        }

        saveGame = LoadGameNode(slot);
        saveGame.slot = slot;
    }

    public void ClearSlot(int slot)
    {
        SaveGameNode(slot, new SaveGame());
        Load(slot);
    }

    public void Save()
    {
        if (Game.isReady)
        {
            StartSave();
        }
        else
        {
            SaveNow();
        }
    }

    public void SaveNow()
    {
        saveGame.isNew = false;
        SaveGameNode(saveGame.slot, saveGame);
    }

    public void StartSave()
    {
        StartCoroutine(CoSave());
    }

    IEnumerator CoSave()
    {
        //Game.Instance.ui.ShowLoader(true);

        yield return new WaitForSecondsRealtime(1.0f);

        SaveNow();

        yield return new WaitForSecondsRealtime(0.2f);

        //Game.Instance.ui.ShowLoader(false);
    }
}
