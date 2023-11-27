using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NDream.AirConsole;
using Newtonsoft.Json.Linq;
using System;
using TMPro;

public class GameLogic : MonoBehaviour {
    [SerializeField] int maxPlayers = 8;
    [SerializeField] Canvas startingScreen;
    [SerializeField] GameObject playerBlock;
    [SerializeField] GameObject profile;

    struct Player {
        public string name; // Name displayed on host screen
        public int id; // Device id

        public Player(string name, int id) {
            this.name = name;
            this.id = id;
        }
    }

    List<Player> players = new List<Player>();

    private bool gameStarted = false;
    private bool showingCode = false;
    private int checkPlayerPause = 0;

    void Awake() {
        AirConsole.instance.onMessage += OnMessage;
    }

    // Receives messages
    void OnMessage(int fromDeviceID, JToken data) {
        Debug.Log("Message received from " + fromDeviceID);

        // Creates a new JToken message to send back to the device
        JToken message;

        // A player wants to join the game
        if (data["join"] != null) {
            Debug.Log("Player wants to join!");

            // There are too many players
            if (players.Count >= maxPlayers) {
                message = JToken.Parse(@"{'type':'message','message':'too-many'}");
                SendMessage(fromDeviceID, message);
                Debug.Log("Too many players");
                return;
            }

            // Name empty
            if (data["join"].ToString().Length < 1) {
                message = JToken.Parse(@"{'type':'message','message':'not-valid'}");
                SendMessage(fromDeviceID, message);
                Debug.Log("Name empty");
                return;
            }

            // There is already someone with that name
            foreach (Player player in players) {
                if (data["join"].ToString().Equals(player.name)) {
                    message = JToken.Parse(@"{'type':'message','message':'taken'}");
                    SendMessage(fromDeviceID, message);
                    Debug.Log("Already someone with that name");
                    return;
                }
            }

            // Player can join the game
            message = JToken.Parse(@"{'type':'change','screen':'join-screen'}");
            SendMessage(fromDeviceID, message);

            AddPlayer(data["join"].ToString(), fromDeviceID);
            return;
        }
    }

    void AddPlayer(string name, int id) {
        // Creates a profile on UI
        GameObject newProfile = Instantiate(profile);
        newProfile.transform.parent = playerBlock.transform;
        Vector3 newPosition = newProfile.GetComponent<RectTransform>().anchoredPosition;
        newPosition.y = 365 - players.Count * 75;
        newProfile.GetComponent<RectTransform>().anchoredPosition = new Vector3(25, 350 - players.Count * 100, 0);
        newProfile.GetComponent<TMP_Text>().text = name;

        // Creates a player struct
        Player newPlayer = new Player(name, id);
        players.Add(newPlayer);

        Debug.Log(name + " joined!");
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

    void Update() { 
        /*if (!gameStarted) {
            checkPlayerPause += 1;
            if (AirConsole.instance.IsAirConsoleUnityPluginReady() && checkPlayerPause > 30) {
                if (!showingCode) {
                    // Show code here!
                    showingCode = true;
                }
                checkPlayerPause = 0;
            }
        }*/
    }


    void OnDestroy() {
        if (AirConsole.instance != null) {
            AirConsole.instance.onMessage -= OnMessage;
        }
    }
}
