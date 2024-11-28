using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PawnAnimation : MonoBehaviour
{
    public Animator pawnAnimator;

    public void TriggerFlinch()
    {
        pawnAnimator.ResetTrigger("isFlinch");
        pawnAnimator.SetTrigger("isFlinch");
    }
    public void TriggerRangeBow()
    {
        pawnAnimator.ResetTrigger("isRangeBow");
        pawnAnimator.SetTrigger("isRangeBow");
    }
    public void TriggerShortSwing()
    {
        pawnAnimator.ResetTrigger("isShortSwing");
        pawnAnimator.SetTrigger("isShortSwing");
    }
    public void TriggerWideSwing()
    {
        pawnAnimator.ResetTrigger("isWideSwing");
        pawnAnimator.SetTrigger("isWideSwing");
    }
    public void StartWalking()
    {
        pawnAnimator.SetBool("isWalk", true);
    }
        public void StopWalking()
    {
        pawnAnimator.SetBool("isWalk", false);
    }
}
