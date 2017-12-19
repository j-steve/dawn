using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AITurnStartedEventArgs : EventArgs
{
    public readonly int Turn;
    public readonly IList<Func<bool>> coroutines = new List<Func<bool>>();
    public AITurnStartedEventArgs(int turn) { Turn = turn; }
}

public class GameTime : MonoBehaviour
{
    #region Const/Static

    const float SECONDS_PER_TURN = 5;

    public static GameTime Instance {
        get { return _Instance ?? (_Instance = FindObjectOfType<GameTime>()); }
    }
    static GameTime _Instance;

    #endregion
    public event Action<AITurnStartedEventArgs> AITurnStartedEvent;

    public event Action<int> AITurnCompletedEvent;

    public int CurrentTurn { get; private set; }

    void Start()
    {
        StartCoroutine(TurnIncrementor());
    }

    IEnumerator TurnIncrementor()
    {
        while (true) {
            CurrentTurn++;
            if (AITurnStartedEvent != null) {
                var eventArgs = new AITurnStartedEventArgs(CurrentTurn);
                AITurnStartedEvent(eventArgs);
                foreach (var coroutine in eventArgs.coroutines) {
                    yield return new WaitUntil(coroutine);
                }
                // TODO: yield return wait for all coroutines to complete.
            }
            if (AITurnCompletedEvent != null)
                AITurnCompletedEvent(CurrentTurn);
            yield return new WaitForSeconds(SECONDS_PER_TURN);
        }
    }
}
