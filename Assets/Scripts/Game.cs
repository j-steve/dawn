using UnityEngine;

public static class Game
{
    public static bool Paused {
        get {
            return paused;
        }
        set {
            Time.timeScale = value ? 0 : 1;
            paused = value;
        }
    }

    static bool paused;
}
