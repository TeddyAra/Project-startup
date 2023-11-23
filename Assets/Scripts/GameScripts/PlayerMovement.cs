using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    [SerializeField] float distance;
    [SerializeField] float speed;
    [SerializeField] GameObject directionVisual;
    [SerializeField] Transform cam;
    Transform directionVisualTransform;

    public enum Direction { 
        Forwards,
        Right,
        Backwards,
        Left
    }

    Direction direction;

    bool forwardsPossible = true;
    bool rightPossible = true;
    bool backwardsPossible = true;
    bool leftPossible = true;
    bool canMove = true;

    void Start() {
        directionVisualTransform = directionVisual.transform;
    }

    void Update() {
        if (!canMove) return;

        float horInput = Input.GetAxis("Horizontal");
        float verInput = Input.GetAxis("Vertical");

        if (verInput > 0.5f && direction != Direction.Forwards && forwardsPossible) { 
            direction = Direction.Forwards;
            ShowDirection(0f);
        }

        if (horInput > 0.5f && direction != Direction.Right && rightPossible) { 
            direction = Direction.Right;
            ShowDirection(90f);
        }

        if (verInput < -0.5f && direction != Direction.Backwards && backwardsPossible) { 
            direction = Direction.Backwards;
            ShowDirection(180f);
        }

        if (horInput < -0.5f && direction != Direction.Left && leftPossible) { 
            direction = Direction.Left;
            ShowDirection(270f);
        }

        if (Input.GetButtonDown("Fire1")) {
            StartCoroutine(Move());
        }
    }

    void ShowDirection(float angle) {
        directionVisualTransform.rotation = Quaternion.Euler(0f, angle, 0f);
    }

    IEnumerator Move() {
        canMove = false;
        directionVisual.SetActive(false);

        Vector3 goal = transform.position + directionVisualTransform.forward * distance;
        float walked = 0;

        while (walked < distance) {
            transform.Translate(directionVisualTransform.forward * speed * Time.deltaTime);
            walked += speed * Time.deltaTime;
            
            yield return null;
        }

        transform.position = goal;

        canMove = true;
        directionVisual.SetActive(true);
    }
}
