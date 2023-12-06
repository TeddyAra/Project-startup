using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationScript : MonoBehaviour {

    [SerializeField] Animator animator;
    private string currentState;
    public string idle = "idle";
    public string jump = "jump";
    public string walk = "walking";

    bool previousBool = false;
    bool isJumping = false;

    void Start() {

    }

    void Update() {
        if (previousBool != isJumping && isJumping) {
            isJumping = false;
            animator.SetBool("isJumping", false);
        }

        previousBool = isJumping;
    }

    public void AnimateJump() {
        animator.SetBool("isJumping", true);
        isJumping = true;
    }

    public void ChangeAnimationState(bool isWalking) {
        animator.SetBool("isWalking", isWalking);
    }

    public bool isAnimationPlaying(string stateName) {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName(stateName) &&
            animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f) {
            return true;
        } else {
            return false;
        }
    }
}