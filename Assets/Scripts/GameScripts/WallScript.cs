using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NDream.AirConsole;

public class WallScript : MonoBehaviour {
    [SerializeField] private int health;
    [SerializeField] private bool playerDependentHealth;
    [SerializeField] private GameObject player;
    
    private Prototype prototypeScript;

    void Start() {
        if (playerDependentHealth) {
            health *= GameLogic.players.Count;
        }

        prototypeScript = player.GetComponent<Prototype>();
    }

    void OnCollisionEnter(Collision collision) {
        player.GetComponent<Prototype>().fireballs.Remove(collision.gameObject);
        Destroy(collision.gameObject);
        health--;

        if (health <= 0) {
            Debug.Log("Wall destroyed!");
            JToken message = JToken.Parse(@"{'type':'change','screen':'wait-screen'}");
            prototypeScript.SendBroadcast(message, true);

            transform.gameObject.SetActive(false);
        }
    }
}
