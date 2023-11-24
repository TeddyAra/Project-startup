using NDream.AirConsole;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Prototype : MonoBehaviour {
    // References
    [Header("References")]
    [SerializeField] Transform cam;
    [SerializeField] private CharacterController controller;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private LayerMask defaultMask;
    [SerializeField] private GameObject canvas1;
    [SerializeField] private TMP_Text text1;
    [SerializeField] private GameObject canvas2;
    [SerializeField] private TMP_Text text2_1;
    [SerializeField] private GameObject text2_2;

    // Settings
    [Header("Settings")]
    [SerializeField] private float speed = 12f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float groundDistance = 0.4f;
    [SerializeField] private float jumpHeight = 1f;
    [SerializeField] private float distanceCheck = 0.1f;
    [SerializeField] private string fallTileTag = "Fall";
    [SerializeField] private string winTag = "Win";
    [SerializeField] private string deathTag = "Death";
    [SerializeField] private bool camIgnoreX = true;

    // Variables
    private Vector3 velocity;
    private bool isGrounded;
    private bool isDead;
    private bool hasWon;
    private Vector3 move;
    private Vector3 fallPosition;
    private Vector3 fallVelocity;
    private bool gameStarted;
    private int numberOfPlayers;
    private int checkPlayerPause;

    void Awake() {
        AirConsole.instance.onMessage += OnMessage;
    }

    void OnMessage(int id, JToken data) {
        Debug.Log(id + " sent " + data);
    }

    void SendMessage(int id, JToken data) {
        AirConsole.instance.Message(id, data);
    }

    void SendBroadcast(string msg) {
        string data = msg;

        foreach (int id in AirConsole.instance.GetControllerDeviceIds()) {
            Debug.Log("Sending " + data + " to " + id);
            AirConsole.instance.Message(id, data);
        }
    }

    void Update() {
        // Starts the game
        if (!gameStarted) {
            checkPlayerPause += 1;
            if (AirConsole.instance.IsAirConsoleUnityPluginReady() && checkPlayerPause > 30) {
                checkPlayerPause = 0;
                numberOfPlayers = AirConsole.instance.GetControllerDeviceIds().Count;
                text2_1.text = "Players: " + numberOfPlayers + "/3";

                if (numberOfPlayers == 3) { 
                    text2_2.SetActive(true);
                }
            }

            if (Input.GetButtonDown("Fire1") || Input.GetKeyDown(KeyCode.E)) {
                if (numberOfPlayers == 3) {
                    gameStarted = true;
                    canvas2.SetActive(false);

                    List<string> screens = new List<string> {
                        "one", "two", "three"
                    };
                    foreach (int id in AirConsole.instance.GetControllerDeviceIds()) {
                        int index = UnityEngine.Random.Range(0, screens.Count);
                        SendMessage(id, screens[index]);
                        screens.RemoveAt(index);
                    }
                }
            }
            return;
        }

        // Calculates gravity and jump force
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0) {
            velocity.y = -2f;
        }

        if (Input.GetButtonDown("Fire1") && isGrounded && !isDead) {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;

        // Moves player to the center of the fallen tile
        if (isDead) {
            fallVelocity = (fallPosition - transform.position).normalized * Time.deltaTime * 4;
            fallVelocity.y = 0f;
            velocity += fallVelocity;

            velocity.x *= 1f - Time.deltaTime;
            velocity.z *= 1f - Time.deltaTime;
        }

        // Applies gravity
        controller.Move(velocity * Time.deltaTime);

        // No input if dead
        if (isDead || hasWon) return;

        // Gets input
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // Applies input        
        move = transform.right * x + transform.forward * z;
        controller.Move(move * speed * Time.deltaTime);

        // Checks if player is on a falling tile
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, distanceCheck, groundMask)) {
            if (hit.transform.CompareTag(fallTileTag)) {
                fallPosition = hit.transform.position;
                hit.transform.gameObject.AddComponent<Rigidbody>();
                hit.transform.gameObject.layer = defaultMask;
                isDead = true;
            }
        }
    }

    // Checks if player should die or win
    void OnTriggerEnter(Collider other) {
        if (other.CompareTag(deathTag)) {
            canvas1.SetActive(true);
        } else if (other.CompareTag(winTag)) {
            hasWon = true;
            text1.SetText("You won!");
            canvas1.SetActive(true);
        }
    }

    void OnDestroy() {
        if (AirConsole.instance != null) {
            AirConsole.instance.onMessage -= OnMessage;
        }
    }
}