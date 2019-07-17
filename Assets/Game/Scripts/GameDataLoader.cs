﻿using Newtonsoft.Json;
using UnityEngine;

 static public class GameDataLoader
{
    static public T[] Load<T>(string filename)
    {
        var targetFile = Resources.Load<TextAsset>("GameData/" + filename);
        var text = "{\"data\":" + targetFile.text + "}";
       // return JsonUtility.FromJson<JsonDataList<T>>(text).data;
        return JsonConvert.DeserializeObject<JsonDataList<T>>(text).data;
    }

    struct JsonDataList<T> { public T[] data; }
}

[System.Serializable]
public class Tile
{
    public string parent;
    public string biome;
    public string tile;
    public string resources;
    public float impedence;
    public string imageUrl;
}
