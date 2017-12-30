using UnityEngine;

static public class GameDataLoader
{

    // Use this for initialization
    static public Tiles LoadTileData()
    {
        var targetFile = Resources.Load<TextAsset>("GameData/tiles");
        return JsonUtility.FromJson<Tiles>(targetFile.text);
    }
}


[System.Serializable]
public class Tiles
{
    public Tile[] tiles;
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
