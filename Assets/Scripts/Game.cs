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
}
