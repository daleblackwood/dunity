using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO; 
using SimpleJSON;

public class SaveData : MonoBehaviour
{
    static public SaveData main { get; private set; }

    [Serializable]
    public class GameData
    {
        public string lastStage = null;
        public SerializableDictionary<string, StageData> stages = new SerializableDictionary<string, StageData>();

        public StageData GetStage(GameStage stage)
        {
            var key = ParseSaveKey(stage.saveKey);
            if (stages.ContainsKey(key) == false)
            {
                return LoadStageDefaultData(stage);
            }

            return stages[key];
        }

        public void SetStage(GameStage stage)
        {
            var key = ParseSaveKey(stage.saveKey);
            stages[key] = stage.saveData;
        }

        public JSONObject Serialise()
        {
            var json = new JSONObject();
            json["lastStage"] = new JSONString(lastStage);

            var stageNodes = json["stages"] = new JSONObject();
            foreach (var pair in stages)
            {
                stageNodes[pair.Key] = pair.Value.Serialise();
            }
            return json;
        }

        public void Deserialise(JSONObject json)
        {
            stages.Clear();
            lastStage = json["lastStage"].Value;

            var stageNodes = json["stages"].AsObject;
            if (stageNodes != null)
            {
                foreach (KeyValuePair<string, JSONNode> node in stageNodes)
                {
                    var stage = new StageData();
                    stage.Deserialise(node.Value.AsObject);
                }
            }
        }
    }

    [Serializable]
    public class StageData
    {
        public SerializableDictionary<string, EntitySaveState> entities = new SerializableDictionary<string, EntitySaveState>();

        public EntitySaveState GetEntity(Entity entity)
        {
            var key = ParseSaveKey(entity.saveKey);
            if (entities.ContainsKey(key) == false)
                return EntitySaveState.None;

            return entities[key];
        }

        public void SetEntity(Entity entity)
        {
            var key = ParseSaveKey(entity.saveKey);
            entities[key] = entity.saveState;
        }

        public JSONObject Serialise()
        {
            var json = new JSONObject();
            var entityNodes = json["entities"] = new JSONObject();
            foreach (var pair in entities)
            {
                entityNodes[pair.Key] = (int)pair.Value;
            }
            return json;
        }

        public void Deserialise(JSONObject json)
        {
            entities.Clear();
            var entityNodes = json["entities"].AsObject;
            foreach (KeyValuePair<string, JSONNode> node in entityNodes)
            {
                entities[node.Key] = (EntitySaveState)(node.Value.AsInt);
            }
        }
    }

    public enum EntitySaveState
    {
        None,
        Triggered,
        Removed
    }

    static public string ResourcePath { get { return Application.dataPath + "/Resources"; } }
    static public string SavePath { get { return Application.persistentDataPath; } }

    static public GameData LoadGameData(int slot)
    {
        var key = ParseSaveKey("SAVE_" + slot);
        var path = SavePath + "/" + key + ".SAV";
        var str = TextRead(path);
        if (string.IsNullOrEmpty(str))
            return null;

        var json = JSON.Parse(str);
        if (json == null)
            return null;
        
        var gameData = new GameData();
        gameData.Deserialise(json.AsObject);

        return gameData;
        //return DataRead<GameData>(path);
    }

    static public void SaveGameData(int slot, GameData game)
    {
        var key = ParseSaveKey("SAVE_" + slot);
        var path = SavePath + "/" + key + ".SAV";
        var json = game.Serialise();
        TextWrite(path, json.ToString());
        //DataWrite(path, game);
    }

    static public StageData LoadStageDefaultData(GameStage stage)
    {
        var path = GetStageDefaultDataPath(stage);
        var str = TextRead(path);
        if (str == null)
            return null;

        var json = JSON.Parse(str);
        if (json == null)
            return null;
        
        var stageData = new StageData();
        stageData.Deserialise(json.AsObject);
        return stageData;
        //return DataRead<StageData>(path);
    }

    static public string GetStageDefaultDataPath(GameStage stage)
    {
        return ResourcePath + "/" + stage.saveKey + ".STAGE";
    }

    /*
    static public T DataRead<T>(string path)
    {
        var json = StringRead(path);
        if (string.IsNullOrEmpty(json))
            return default(T);
        return JsonUtility.FromJson<T>(json);
    }

    static public void DataWrite<T>(string path, T data)
    {
        //var serializer = new XmlSerializer(typeof(T));
        //serializer.Serialize(writer, data);
        var json = JsonUtility.ToJson(data);
        File.WriteAllText(path, json);
    }
    */

    static public string TextRead(string path)
    {
        if (File.Exists(path) == false)
            return null;

        var str = File.ReadAllText(path);
        if (string.IsNullOrEmpty(str))
            return null;
        
        return str;
    }

    static public void TextWrite(string path, string text)
    {
        File.WriteAllText(path, text);
    }

    static public string ParseSaveKey(string name)
    {
        if (string.IsNullOrEmpty(name))
            return null;
        
        name = name.ToUpper().Trim();
        var key = "";
        for (var i=0; i<name.Length; i++)
        {
            var c = name[i];
            if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z'))
            {
                key += c;
            }
            else
            {
                key += "_";
            }
        }
        return key;
    }

    public GameData game { get; private set; }
    public int slot { get; private set; }

    public void Load(int loadSlot)
    {
        slot = loadSlot;
        game = LoadGameData(slot);
    }

    public void Save()
    {
        SaveGameData(slot, game);
    }
}
