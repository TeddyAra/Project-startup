using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationScript : MonoBehaviour {
    [SerializeField] Animator animator;
    public string idle = "idle";
    public string jump = "jump";
    public string walk = "walking";

    public void AnimateJump() {
        animator.Play(jump);
    }

    public void ChangeAnimationState(bool isWalking) {
        animator.SetBool("isWalking", isWalking);
    }

    public void Dance() {
        animator.SetBool("isDancing", true);
    }
}