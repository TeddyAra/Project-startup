using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationScript : MonoBehaviour {

    [SerializeField] Animator animator;
    private string currentState;
    public string idle = "idle";
    public string jump = "jump";
    public string walk = "walking";

    void Start() {
        ChangeAnimationState(jump);
    }

    public void ChangeAnimationState(string newState) {
        //Debug.Log(newState + " " + (newState == currentState));

        if (newState == currentState) {
            return;
        }

        animator.Play(newState);
        currentState = newState;
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
