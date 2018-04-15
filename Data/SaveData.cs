using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO; 
using SimpleJSON;

public class SaveData : MonoBehaviour
{
    static public SaveData main { get; private set; }

    public class GameData : SaveNode
    {
        public string currentStage { 
            get { return GetString("CURRENT_STAGE"); }
            set { SetValue("CURRENT_STAGE", value); }
        }

        public string previousStage { 
            get { return GetString("PREVIOUS_STAGE"); }
            set { SetValue("PREVIOUS_STAGE", value); }
        }

        public void SetStage(string stageName)
        {
            if (stageName != currentStage)
            {
                previousStage = currentStage;
                currentStage = stageName;
            }
        }

        public StageData GetStageData(GameStage stage)
        {
            return GetChild<StageData>(stage.saveKey);
        }

        public void SetStageData(GameStage stage)
        {
            SetChild(stage.saveKey, stage.saveData);
        }
    }

    public class StageData : SaveNode
    {
        public string spawnPoint { 
            get { return GetString("LAST_DOOR"); }
            set { SetValue("LAST_DOOR", value); }
        }
    }

    public const int SAVE_SLOT_DEV = -1;

    static public string ResourcePath { get { return Application.dataPath + "/Resources"; } }
    static public string SavePath { get { return Application.persistentDataPath; } }

    static public GameData LoadGameData(int slot)
    {
        var key = EvalUtils.ParseToken("SAVE_" + slot);
        var path = SavePath + "/" + key + ".SAV";
        var str = TextRead(path);
        if (string.IsNullOrEmpty(str))
            return null;

        var json = JSON.Parse(str);
        if (json == null)
            return null;
        
        var gameData = new GameData();
        gameData.data = json.AsObject;

        return gameData;
    }

    static public void SaveGameData(int slot, GameData game)
    {
        var key = EvalUtils.ParseToken("SAVE_" + slot);
        var path = SavePath + "/" + key + ".SAV";
        var json = game.data;
        TextWrite(path, json.ToString());
    }

    static string TextRead(string path)
    {
        if (File.Exists(path) == false)
            return null;

        var str = File.ReadAllText(path);
        if (string.IsNullOrEmpty(str))
            return null;
        
        return str;
    }

    static void TextWrite(string path, string text)
    {
        File.WriteAllText(path, text);
    }

    public GameData game { get; private set; }
    public int slot { get; private set; }

    void Awake()
    {
        main = this;
        slot = SAVE_SLOT_DEV;
    }

    public void Load(int loadSlot = SAVE_SLOT_DEV)
    {
        Debug.LogFormat("Load game {0}", loadSlot);
        if (loadSlot != -1)
        {
            slot = loadSlot;
        }
        game = LoadGameData(slot);
        if (game == null)
        {
            game = new GameData();  
        }
    }

    public void Save()
    {
        Debug.LogFormat("Save game {0}", slot);
        SaveGameData(slot, game);
    }

    public string LookupValue(string key)
    {
        if (GameStage.main != null)
        {
            var stageKey = GameStage.main.saveKey;
            if (stageKey != null)
            {
                var data = game.GetString(stageKey + "." + key);
                if (string.IsNullOrEmpty(data) == false)
                {
                    return data;
                }
            }
        }
        return game.GetString(key);
    }
}


[System.Serializable]
public class SaveNode
{
    public JSONObject data = new JSONObject();

    public void SetValue(string key, int value)
    {
        key = EvalUtils.ParseToken(key);
        data[key] = new JSONNumber(value);
    }

    public void SetValue(string key, float value)
    {
        key = EvalUtils.ParseToken(key);
        data[key] = new JSONNumber(value);
    }

    public void SetValue(string key, string value)
    {
        key = EvalUtils.ParseToken(key);
        data[key] = new JSONString(value);
    }

    public void SetValue(string key, bool value)
    {
        key = EvalUtils.ParseToken(key);
        data[key] = new JSONBool(value);
    }

    public void SetChild(string key, SaveNode node)
    {
        EvalUtils.ParseToken(key);
        data[key] = node.data;
    }

    public int GetInt(string key)
    {
        var node = GetNode(key);
        if (node == null)
            return 0;
        return node.AsInt;
    }

    public float GetFloat(string key)
    {
        var node = GetNode(key);
        if (node == null)
            return 0.0f;
        return node.AsFloat;
    }

    public bool GetBool(string key)
    {
        var node = GetNode(key);
        if (node == null)
            return false;
        return node.AsBool;
    }

    public string GetString(string key)
    {
        var node = GetNode(key);
        if (node == null)
            return "";
        return node.Value;
    }

    public SaveNode GetChild(string key)
    {
        return GetChild<SaveNode>(key);
    }

    public T GetChild<T>(string key) where T : SaveNode
    {
        var node = GetNode(key);
        if (node == null)
            return null;
        
        var result = System.Activator.CreateInstance<T>();
        result.data = node.AsObject;
        return result;
    }

    public bool IsTrue(string condition)
    {
        return EvalUtils.ParseCondition(condition, SaveData.main.LookupValue);
    }

    JSONNode GetNode(string key)
    {
        var node = (JSONNode)data;
        do
        {
            var firstDot = key.IndexOf(".");
            if (firstDot >= 0)
            {
                var subkey = key.Substring(0, firstDot);
                key = key.Substring(firstDot + 1);

                var token = subkey.Trim();
                node = data[token];
                if (node == null)
                {
                    break;
                }
            }
            else
            {
                var token = EvalUtils.ParseToken(key);
                node = node[key];
                break;
            }
        }
        while (key.Length > 0);

        return node;
    }
}
