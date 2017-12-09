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
        animator = GetComponent<Animator>();
        StartCoroutine(StartIdleAnimation());
        //Debug.LogFormat("Animator: {0}, animation: {1}", animator.name, animation.name);
        //animation.PlayQueued("Idle02", QueueMode.CompleteOthers);
        //animation.PlayQueued("Idle01", QueueMode.CompleteOthers);
        //animation.PlayQueued("Idle", QueueMode.CompleteOthers);
        //animation.wrapMode = WrapMode.Loop;
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
                //animator.ResetTrigger(triggerMove);
            }
            else {
                animator.SetTrigger(triggerMoving);
                isMoving = true;
                //animator.ResetTrigger(triggerStop);
            }
            //animator.SetBool(moveHash, !animator.GetBool(moveHash));
        }
    }
}