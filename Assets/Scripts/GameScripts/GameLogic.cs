using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NDream.AirConsole;
using Newtonsoft.Json.Linq;
using System;

public class GameLogic : MonoBehaviour {
    struct Player {
        public int id;
        public Vector2 position;
        public Transform visual;

        public Player(int id, int x, int y, Transform visual) {
            this.id = id;
            position = new Vector2(x, y);
            this.visual = visual;
        }
    }

    [SerializeField] int speed;
    [SerializeField] GameObject playerPrefab;
    [SerializeField] AirConsole airConsole;
    [SerializeField] UnityEngine.Object newSite;

    List<Player> players = new List<Player>();

    void Awake() {
        AirConsole.instance.onMessage += OnMessage;
    }

    void OnMessage(int fromDeviceID, JToken data) {
        // Checks if player exists
        bool doesPlayerExist = false;
        Player currentPlayer = new Player();

        foreach (Player player in players) {
            if (fromDeviceID == player.id) {
                doesPlayerExist = true;
                currentPlayer = player;
                break;
            }
        }

        // Creates a player if one doesn't exist
        if (!doesPlayerExist) {
            GameObject player = Instantiate(playerPrefab);
            currentPlayer = new Player(fromDeviceID, 0, 0, player.transform);
            players.Add(currentPlayer);
        }

        // Reads the input
        Vector2 prevPos = new Vector2(currentPlayer.visual.position.x, currentPlayer.visual.position.y);

        Debug.Log("message from " + fromDeviceID + ", data: " + data);
        if (data["action"] != null) {
            if (data["action"].ToString().Equals("up"))
                currentPlayer.visual.Translate(0, 0, speed);
            if (data["action"].ToString().Equals("right"))
                currentPlayer.visual.Translate(speed, 0, 0);
            if (data["action"].ToString().Equals("down"))
                currentPlayer.visual.Translate(0, 0, -speed);
            if (data["action"].ToString().Equals("left"))
                currentPlayer.visual.Translate(-speed, 0, 0);
        }

        // Checks position
        if (currentPlayer.visual.position.x < -2 && prevPos.x >= -2) {
            airConsole.Message(fromDeviceID, "1");
        }

        if (currentPlayer.visual.position.x >= -2 &&
            currentPlayer.visual.position.x <= 2 &&
            (prevPos.x < -2 || prevPos.x > 2)) {
            airConsole.Message(fromDeviceID, "2");
        }

        if (currentPlayer.visual.position.x > 2 && prevPos.x <= 2)
        {
            airConsole.Message(fromDeviceID, "1");
        }
    }

    void OnDestroy() {
        if (AirConsole.instance != null) {
            AirConsole.instance.onMessage -= OnMessage;
        }
    }
}
