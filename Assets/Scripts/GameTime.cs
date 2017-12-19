using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AITurnStartedEventArgs : EventArgs
{
    public readonly int Turn;
    public readonly IList<Func<IEnumerator>> coroutines = new List<Func<IEnumerator>>();
    public AITurnStartedEventArgs(int turn) { Turn = turn; }
}

public class GameTime : MonoBehaviour
{
    #region Const/Static

    const float SECONDS_PER_TURN = 2;

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
                yield return StartCoroutine(WaitForAll(eventArgs.coroutines));
            }
            Debug.Log("AITurnCompletedEvent");
            if (AITurnCompletedEvent != null)
                AITurnCompletedEvent(CurrentTurn);
            yield return new WaitForSeconds(SECONDS_PER_TURN);
        }
    }

    IEnumerator WaitForAll(IList<Func<IEnumerator>> coroutines)
    {
        for (var i = coroutines.Count - 1; i >= 0; i--) {
            StartCoroutine(WaitForOne(coroutines, i));
        }
        yield return new WaitUntil(() => coroutines.Count == 0);
    }

    IEnumerator WaitForOne(IList<Func<IEnumerator>> coroutines, int i)
    {
        var coroutine = coroutines[i];
        yield return coroutine();
        coroutines.Remove(coroutine);
    }
}
