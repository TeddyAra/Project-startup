using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NDream.AirConsole;
using Newtonsoft.Json.Linq;
using System;
using TMPro;
using UnityEngine.SceneManagement;

public class GameLogic : MonoBehaviour {
    [SerializeField] int minPlayers = 1; // Minimum amount of players needed
    [SerializeField] int maxPlayers = 8; // Maximum amount of players allowed
    [SerializeField] Canvas startingScreen; // Canvas of the starting screen
    [SerializeField] GameObject playerBlock; // UI element that contains all player profiles
    [SerializeField] TMP_Text playerCount; // Text that shows how many players are connected
    [SerializeField] TMP_Text playCode; // Text that shows the code on screen
    [SerializeField] GameObject profile; // Prefab for player profiles on the starting screen
    [SerializeField] GameObject playButton; // Button to play the game
    [SerializeField] TMP_InputField linkInput; // The link the player enters
    [SerializeField] GameObject linkText; // The text above the link input
    [SerializeField] List<Color> colours; // The colours that players can have/be
    [SerializeField] Transform cam; // The camera

    [HideInInspector] public static List<Player> players = new List<Player>(); // List of all players who are currently playing
    [HideInInspector] public static string roomCode = ""; // The code to connect with the host
    [HideInInspector] public string url;

    private bool tabIsOpened = false;

    void Awake() {
        AirConsole.instance.onReady += OnReady; // Gets called when the first device connects
        AirConsole.instance.onMessage += OnMessage; // Gets called when a message is received
        AirConsole.instance.onDisconnect += OnDisconnect; // Gets called when a player disconnects from the game
    }

    void Start() {
        // Fixes player count text and roomcode
        playerCount.text = "Players " + players.Count + "/" + maxPlayers;
        if (roomCode != "") playCode.text = "Code: " + roomCode;

        // Checks if playing is possible
        if (players.Count < minPlayers) {
            playButton.SetActive(false);
        }

        // Adds player profiles (if any)
        /*for (int i = 0; i < players.Count; i++) {
            AddPlayer(players[i].username, players[i].id, players[i], i);
        }*/
    }

    // Player has disconnected
    void OnDisconnect(int deviceID) {
        Debug.Log(deviceID + " disconnected!");

        // Player might have been connected, but was maybe not part of the game
        // If they weren't part of the game, we can ignore the disconnect
        bool playerFound = false;

        // Goes through player list from top to bottom
        for (int i = 0; i < players.Count; i++) {
            if (!playerFound) {
                // If this is the disconnected device
                Debug.Log("Player found!");
                if (deviceID == players[i].id) {
                    // Removes player data
                    Destroy(players[i].profile);
                    players.Remove(players[i]);
                    playerFound = true;

                    // Fixes text and counting
                    i--;
                    playerCount.text = "Players " + players.Count + "/" + maxPlayers;

                    // Checks if playing is possible
                    if (players.Count < minPlayers) {
                        playButton.SetActive(false);
                    }
                }
            // Player has been found, so other profiles can be moved up
            } else {
                Debug.Log(players[i].id + " moved up!");
                Vector3 newPosition = players[i].profile.GetComponent<RectTransform>().anchoredPosition;
                newPosition.y += 100;
                players[i].profile.GetComponent<RectTransform>().anchoredPosition = newPosition;
            }
        }
    }

    // Receives messages
    void OnMessage(int fromDeviceID, JToken data) {
        Debug.Log("Message received from " + fromDeviceID);

        // Creates a new JToken message to send back to the device if needed
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
                if (data["join"].ToString().Equals(player.username)) {
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

    void AddPlayer(string name, int id, Player existingPlayer = null, int num = -1) {
        // Creates a profile on UI
        GameObject newProfile = Instantiate(profile);
        DontDestroyOnLoad(newProfile);
        newProfile.transform.SetParent(playerBlock.transform);

        // Creates a player
        Player newPlayer = existingPlayer;
        if (newPlayer == null) {
            newPlayer = ScriptableObject.CreateInstance<Player>();
            newPlayer.username = name;
            newPlayer.id = id;
            newPlayer.profile = newProfile;
            newPlayer.colour = colours[players.Count];
            players.Add(newPlayer);
        }

        // Updates UI
        Vector3 newPosition = newProfile.GetComponent<RectTransform>().anchoredPosition;
        newPosition.y = 365 - players.Count * 75;
        int y = num == -1 ? players.Count : num + 1;
        newProfile.GetComponent<RectTransform>().anchoredPosition = new Vector3(-70, 375 - y * 100, 0);
        newProfile.GetComponent<TMP_Text>().text = name;
        playerCount.text = "Players " + players.Count + "/" + maxPlayers;

        // Checks if playing is possible
        if (players.Count >= minPlayers) {
            playButton.SetActive(true);
        }

        Debug.Log(name + " joined!");
    }

    // Sends a message to one device
    void SendMessage(int id, JToken data) {
        AirConsole.instance.Message(id, data);
    }

    // Sends a message to a range of devices
    void SendBroadcast(JToken msg, bool exclusive = false) {
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
        // Rotates the camera
        cam.RotateAround(Vector3.zero, Vector3.up, Time.deltaTime * 15);

        // Opens browser
        if (Input.GetKeyDown(KeyCode.Return) && !tabIsOpened) {
            url = linkInput.text;
            if (url == "") return;
            Application.OpenURL("https://www.airconsole.com/simulator/#" + url + "/?unity-editor-websocket-port=7843&unity-plugin-version=2.14");
            tabIsOpened = true;
            linkInput.gameObject.SetActive(false);
            linkText.SetActive(false);
        }

        // Player wants to start the game
        if (Input.GetButtonDown("Fire1") && players.Count >= minPlayers) {
            // Switches to wait screen
            JToken message = JToken.Parse(@"{'type':'change','screen':'wait-screen'}");
            SendBroadcast(message, true);

            // Change scene
            SceneManager.LoadScene("Prototype");
        }
    }

    void OnReady(string code) {
        // Adds code to UI
        playCode.text = "Code: " + code;
        roomCode = code;

        AirConsole.instance.browserStartMode = StartMode.Normal;
    }

    void OnDestroy() {
        if (AirConsole.instance != null) {
            AirConsole.instance.onMessage -= OnMessage;
        }
    }
}
