using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using DawnX.UI;
using UnityEngine;

public static class Game
{
    public static bool Paused {
        get { return _paused; }
        set {
            Time.timeScale = value ? 0 : 1;
            _paused = value;
        }
    }
    static bool _paused;

    public static void Save()
    {
        string path = Path.Combine(Application.persistentDataPath, "test.map");
        using (var writer = new BinaryWriter(File.Open(path, FileMode.Create))) {
            HexBoard.ActiveBoard.SaveMap(writer);
            //var serializer = new BinaryFormatter();
            //serializer.Serialize(filestream, HexBoard.ActiveBoard.mapSize);
            ////serializer.Serialize(filestream, HexBoard.ActiveBoard.mapSize.Area * HexConstants.CELLS_PER_CHUNK_ROW);
            //foreach (var cell in HexBoard.ActiveBoard.hexCells.Values) {
            //    serializer.Serialize(filestream, cell);
            //}
        }
    }

    public static void Load()
    {
        string path = Path.Combine(Application.persistentDataPath, "test.map");
        //using (var reader = new BinaryReader(File.Open(path, FileMode.Open))) {
        UIInGame.ActiveInGameUI.SetSelected(null);
        DawnX.UI.UILoadingOverlay.ActiveLoadingOverlay.gameObject.SetActive(true);
        foreach (var chunk in Object.FindObjectsOfType<HexChunk>()) {
            Object.Destroy(chunk.gameObject);
        }
        foreach (var obj in Object.FindObjectsOfType<HexCell>()) {
            Object.Destroy(obj.gameObject);
        }
        foreach (var obj in Object.FindObjectsOfType<Unit>()) {
            Object.Destroy(obj.gameObject);
        }
        UIEscapeMenu.Instance.HideMenu();
        HexBoard.ActiveBoard.LoadMap(path);
        //var deserializer = new BinaryFormatter();
        //var mapSize = (RectangleInt)deserializer.Deserialize(filestream);
        //Debug.LogFormat("Deserialized! {0}x{1}", mapSize.Height, mapSize.Width);
        //var hexCellCount = (int)deserializer.Deserialize(filestream);
        //for (var i = 0; i < hexCellCount; i++) {
        //    var cell = (HexCell)deserializer.Deserialize(filestream);
        //}
        //}
        //using (var reader = new BinaryReader(File.Open(path, FileMode.Open))) {
        //    int header = reader.ReadInt32();
        //    if (header != 1)
        //        throw new Exception("Incompatible file type.");

        //}
    }
}


public interface ISaveable
{
    void Save(BinaryWriter writer);
    void Load(BinaryReader reader);
}