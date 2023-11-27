using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NDream.AirConsole;
using Newtonsoft.Json.Linq;
using System;

public class GameLogic : MonoBehaviour {
    [SerializeField] int maxPlayers = 8;

    struct Player {
        public string name; // Name displayed on host screen
        public int id; // Device id

        public Player(string name, int id) {
            this.name = name;
            this.id = id;
        }
    }

    List<Player> players = new List<Player>();

    void Awake() {
        AirConsole.instance.onMessage += OnMessage;
    }

    // Receives messages
    void OnMessage(int fromDeviceID, JToken data) {
        Debug.Log("Message received from " + fromDeviceID);

        // A player wants to join the game
        if (data["join"] != null) {
            Debug.Log("Player wants to join!");

            // There are too many players
            if (players.Count >= maxPlayers) {
                SendMessage(fromDeviceID, "max");
                Debug.Log("Too many players");
                return;
            }

            // There is already someone with that name
            foreach (Player player in players) {
                if (data["join"].ToString().Equals(player.name)) {
                    SendMessage(fromDeviceID, "name");
                    Debug.Log("Already someone with that name");
                    return;
                }
            }

            // Player can join the game
            SendMessage(fromDeviceID, "joined");
            AddPlayer(data["join"].ToString(), fromDeviceID);
            return;
        }
    }

    void AddPlayer(string name, int id) {
        Player newPlayer = new Player(name, id);
        players.Add(newPlayer);

        Debug.Log(newPlayer.name + " joined!");
    }

    // Sends a message to one device
    void SendMessage(int id, JToken data) {
        AirConsole.instance.Message(id, data);
    }

    // Sends a message to a range of devices
    void SendBroadcast(string msg, bool exclusive = false) {
        // Only sends broadcast to devices currently playing
        if (exclusive) {
            foreach (Player player in players) {
                Debug.Log("Sending " + msg + " to " + player.id);
                AirConsole.instance.Message(player.id, msg);
            }
        // Sends broadcast to all connected devices
        } else {
            foreach (int id in AirConsole.instance.GetControllerDeviceIds()) {
                Debug.Log("Sending " + msg + " to " + id);
                AirConsole.instance.Message(id, msg);
            }
        }
    }


    void OnDestroy() {
        if (AirConsole.instance != null) {
            AirConsole.instance.onMessage -= OnMessage;
        }
    }
}
