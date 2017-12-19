using System;
using System.Collections;
using UnityEngine;

public class GameTime : MonoBehaviour
{
    #region Const/Static

    const float SECONDS_PER_TURN = 2;

    public static GameTime Instance {
        get { return _Instance ?? (_Instance = FindObjectOfType<GameTime>()); }
    }
    static GameTime _Instance;

    #endregion

    public event Action<int> GameTurnEvent;

    public int CurrentTurn { get; private set; }

    void Start()
    {
        StartCoroutine(TurnIncrementor());
    }

    IEnumerator TurnIncrementor()
    {
        while (true) {
            CurrentTurn++;
            if (GameTurnEvent != null)
                GameTurnEvent(CurrentTurn);
            yield return new WaitForSeconds(SECONDS_PER_TURN);
        }
    }
}
