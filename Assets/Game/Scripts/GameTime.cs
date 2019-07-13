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

    public event Action<GameDate> GameTurnEvent;
     
    public readonly GameDate CurrentDate = new GameDate();

    void Start()
    {
        StartCoroutine(TurnIncrementor());
    }

    IEnumerator TurnIncrementor()
    {
        while (true) { 
            CurrentDate.Increment();
            if (GameTurnEvent != null)
                GameTurnEvent(CurrentDate);
            yield return new WaitForSeconds(SECONDS_PER_TURN);
        }
    }
}
