using System.IO;
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
        }
    }

    public static void Load()
    {
        string path = Path.Combine(Application.persistentDataPath, "test.map");
        UIInGame.Instance.SetSelected(null);
        UILoadingOverlay.Instance.gameObject.SetActive(true);
        foreach (var chunk in Object.FindObjectsOfType<HexChunk>()) {
            Object.Destroy(chunk.gameObject);
        }
        foreach (var obj in Object.FindObjectsOfType<Unit>()) {
            Object.Destroy(obj.gameObject);
        }
        UIEscapeMenu.Instance.HideMenu();
        HexBoard.ActiveBoard.LoadMap(path);
    }
}


public interface ISaveable
{
    void Save(BinaryWriter writer);
    void Load(BinaryReader reader);
}