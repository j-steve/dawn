using UnityEngine;

static public class GameDataLoader
{

    // Use this for initialization
    static public Tile[] LoadTileData()
    {
        var targetFile = Resources.Load<TextAsset>("GameData/tiles");
        var text = "{\"data\":" + targetFile.text + "}";
        return JsonUtility.FromJson<JsonDataList<Tile>>(text).data;
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
    public int impedence;
    public string imageUrl;
}
