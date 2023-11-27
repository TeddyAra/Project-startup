using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour {
    [SerializeField] private Transform player;
    [SerializeField] private float distance = 2.5f;

    void Start() {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update() {
        transform.position = new Vector3(transform.position.x, transform.position.y, player.position.z - distance);
    }
}
