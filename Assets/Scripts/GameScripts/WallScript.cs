using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallScript : MonoBehaviour {
    [SerializeField] private int health;
    [SerializeField] private bool playerDependentHealth;
    [SerializeField] private GameObject player;

    void Start() {
        if (playerDependentHealth) {
            health *= GameLogic.players.Count;
        }
    }

    void OnCollisionEnter(Collision collision) {
        player.GetComponent<Prototype>().fireballs.Remove(collision.gameObject);
        Destroy(collision.gameObject);
        health--;

        if (health <= 0) { 
            transform.gameObject.SetActive(false);
        }
    }
}
