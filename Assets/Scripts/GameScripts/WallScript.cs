using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NDream.AirConsole;

public class WallScript : MonoBehaviour {
    [SerializeField] private int health;
    [SerializeField] private bool playerDependentHealth;
    [SerializeField] private GameObject player;
    [SerializeField] private Prototype prototypeScript;

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

            JToken msg = JToken.Parse(@"{'type':'message','screen':'all'}");
            prototypeScript.SendBroadcast(msg, true);
        }
    }
}
