using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
public class Unit : MonoBehaviour
{
    static readonly int triggerMoving = Animator.StringToHash("Moving");
    static readonly int triggerIdle = Animator.StringToHash("Idle");

    private Animator animator;

    private bool isMoving;

    private void Awake()
    {
        animator = this.GetRequiredComponent<Animator>();
        StartCoroutine(StartIdleAnimation());
    }


    /// <summary>
    /// Wait a short random interval before starting the idle animation loop,
    /// to prevent all units from having the exact same idle animation schedule.
    /// </summary>
    IEnumerator StartIdleAnimation()
    {
        var secs = Random.Range(0f, 2f);
        yield return new WaitForSeconds(secs);
        animator.SetTrigger(triggerIdle);

        var animationn = TriggerMoveAaimation();
        yield return animationn.Current;
        while (animationn.MoveNext()) {
            yield return animationn.Current;
        }
    }

    IEnumerator TriggerMoveAaimation()
    {
        while (true) {
            var secs = Random.Range(0f, 10f);
            yield return new WaitForSeconds(secs);
            Debug.LogFormat("Done waiing {0} secs", secs);
            if (isMoving) {
                animator.SetTrigger(triggerIdle);
                isMoving = false;
            }
            else {
                animator.SetTrigger(triggerMoving);
                isMoving = true;
            }
        }
    }
}