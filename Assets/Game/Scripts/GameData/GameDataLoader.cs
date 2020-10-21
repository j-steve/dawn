using System;
using Newtonsoft.Json;
using UnityEngine;

 static public class GameDataLoader
{
    static public T[] Load<T>(string filename)
    {
        var targetFile = Resources.Load<TextAsset>(filename);
        if (targetFile == null) { throw new ArgumentException(string.Format("Failed to load requested resource file: \"{0}\"", filename)); }
        var text = "{\"data\":" + targetFile.text + "}";
        return JsonConvert.DeserializeObject<JsonDataList<T>>(text).data;
    }

    private struct JsonDataList<T> { public T[] data; }
}