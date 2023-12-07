using NDream.AirConsole;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Prototype : MonoBehaviour {
    // References
    [Header("References")]
    [SerializeField] Transform cam;
    [SerializeField] Transform fireballPoint;
    [SerializeField] private CharacterController controller;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private LayerMask defaultMask;
    [SerializeField] private GameObject fireball;
    [SerializeField] private GameObject playerModel;

    // Settings
    [Header("Settings")]
    [SerializeField] public bool usingAirConsole = true;
    [SerializeField] private float speed = 12f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float groundDistance = 0.4f;
    [SerializeField] private float jumpHeight = 1f;
    [SerializeField] private float distanceCheck = 0.1f;
    [SerializeField] private int clickNumToShoot = 10;
    [SerializeField] private string fallTileTag = "Fall";
    [SerializeField] private string winTag = "Win";
    [SerializeField] private string deathTag = "Death";
    [SerializeField] private string wallTag = "Wall";
    [SerializeField] private string tileTag = "Tiles";
    [SerializeField] private string coloursTag = "Colours";
    [SerializeField] private string buttonTag = "Button";
    [SerializeField] private string checkpointTag = "Checkpoint";
    [SerializeField] private float fireballLifeTime = 5;
    [SerializeField] private bool camIgnoreX = true;
    [SerializeField] private float camRotSpeed = 15;

    // Variables
    private Vector3 velocity;
    private bool isGrounded;
    private bool prevIsGrounded; 
    private bool isDead;
    private Vector3 move;
    private Vector3 fallPosition;
    private Vector3 fallVelocity;
    private Dictionary<int, int> spamClicks;
    [HideInInspector] public Dictionary<GameObject, float> fireballs;
    [HideInInspector] public bool colourMinigame;
    [HideInInspector] public GameObject possibleButton;
    private bool previousButton;
    [HideInInspector] public bool standingOnWrongButton;
    private Vector3 checkpoint;
    private AnimationScript animator;
    public AudioSource audioSource; 
    public AudioClip step1; 
    public AudioClip step2; 
    public AudioClip step3; 
    public AudioClip step4; 
    private float stepTimer;
    [SerializeField] float stepTime; 
    public AudioClip jump; 
    public AudioClip jumpLand; 
    public AudioClip fireBall; 
    public AudioClip waterFall;
    private bool isFinished;

    void Awake() {
        if (usingAirConsole) AirConsole.instance.onMessage += OnMessage;
        animator = playerModel.GetComponent<AnimationScript>();
    }

    void OnMessage(int id, JToken data) {
        Debug.Log(id + " sent " + data);

        // If a button has been pressed
        if (data["button"] != null) {
            // If the spam button has been pressed
            if (data["button"].ToString().Equals("spam")) {
                //Debug.Log("Spam clicked");
                spamClicks[id]++;
                if (spamClicks[id] >= clickNumToShoot) {
                    audioSource.PlayOneShot(fireBall); 
                    spamClicks[id] = 0;
                    //Debug.Log("Fireball!");
                    
                    GameObject newFireball = Instantiate(fireball, fireballPoint.position + Vector3.right * UnityEngine.Random.Range(-3f, 3f), Quaternion.identity);
                    newFireball.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 20);

                    Color colour = Color.white;
                    foreach (Player player in GameLogic.players) {
                        if (player.id == id) {
                            colour = player.colour;
                            break;
                        }
                    }
                    newFireball.GetComponent<Renderer>().material.color = colour;

                    fireballs.Add(newFireball, 0);
                }
            }
        }
    }

    void SendMessage(int id, JToken data) {
        if (!usingAirConsole) return;

        Debug.Log(data);
        AirConsole.instance.Message(id, data);
    }

    // Sends a message to a range of devices
    public void SendBroadcast(JToken msg, bool exclusive = false) {
        if (!usingAirConsole) return;

        Debug.Log("Broadcast sent");

        // Only sends broadcast to devices currently playing
        if (exclusive) {
            foreach (Player player in GameLogic.players) {
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

    void Start() {
        // Creates dictionaries for wall break game
        spamClicks = new Dictionary<int, int>();
        fireballs = new Dictionary<GameObject, float>();

        // Adds each player to the dictionary
        foreach (Player player in GameLogic.players) {
            spamClicks.Add(player.id, 0);
        }

        // Spawn position
        checkpoint = transform.position + Vector3.up * 2f;
    }

    void Update() {
        // Resets the scene
        if (Input.GetKeyDown(KeyCode.R)) {
            // Sends players back to start screen
            JToken message = JToken.Parse(@"{'type':'change','screen':'start-screen'}");
            SendBroadcast(message, true);

            // Hides all messages
            message = JToken.Parse(@"{'type':'message','screen':'all'}");
            SendBroadcast(message, true);

            // Clears player list and loads new scene
            GameLogic.players.Clear();
            SceneManager.LoadScene("Alex Scene");
        }

        // Animates dance
        if (isFinished) {
            animator.Dance();
            return;
        }

        prevIsGrounded = isGrounded;
        if (move.magnitude > 0) {
            animator.ChangeAnimationState(true);
        } else {
            animator.ChangeAnimationState(false);
        }

        // Continues timer for each fireball
        for (int i = 0; i < fireballs.Count; i++) { 
            GameObject currentFireball = fireballs.ElementAt(i).Key;
            fireballs[currentFireball] += Time.deltaTime;

            if (fireballs[currentFireball] > fireballLifeTime) {
                Destroy(currentFireball);
                fireballs.Remove(currentFireball);
            }
        }

        // Calculates gravity and jump force
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (isGrounded != prevIsGrounded && isGrounded)
            audioSource.PlayOneShot(jumpLand); 
            

        if (isGrounded && velocity.y < 0) {
            velocity.y = -2f;
        }

        if (Input.GetButtonDown("Fire1") && isGrounded && !isDead) {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            animator.AnimateJump(); 
            audioSource.PlayOneShot(jump); 
            
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
        if (isDead) return;

        // Gets input
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // Applies input        
        move = transform.right * x + transform.forward * z;
        controller.Move(move * speed * Time.deltaTime);

        // Rotates player to move direction
        if (move.magnitude > 0) {
            playerModel.transform.LookAt(transform.position + move);
            int stepType = UnityEngine.Random.Range(0, 3);

            stepTimer += Time.deltaTime; 

            if (stepTimer > stepTime && isGrounded)
            {
                stepTimer = 0;
                switch (stepType) {
                  case 0:
                    audioSource.PlayOneShot(step1); 
                    break;
                  case 1:
                    audioSource.PlayOneShot(step2);
                    break;
                  case 2:
                    audioSource.PlayOneShot(step3);
                    break;
                  case 3:
                    audioSource.PlayOneShot(step4);
                    break;
                }
            }
        

        }

        // Checks if player is on a falling tile
        previousButton = possibleButton;

        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, distanceCheck, groundMask)) {
            if (!standingOnWrongButton) {
                if (hit.transform.CompareTag(buttonTag)) {
                    possibleButton = hit.transform.gameObject;
                } else {
                    possibleButton = null;
                }
            }

            if (hit.transform.gameObject != previousButton) {
                standingOnWrongButton = false;
            }

            if (hit.transform.CompareTag(fallTileTag)) {
                fallPosition = hit.transform.position;
                Rigidbody rb = hit.transform.gameObject.AddComponent<Rigidbody>();
                rb.excludeLayers = groundMask; 
                hit.transform.gameObject.layer = defaultMask;
                isDead = true;
            }
        }
    }

    // Checks if screens should change
    void OnTriggerEnter(Collider other) {
        JToken message;

        // Host died
        if (other.transform.CompareTag(deathTag)) {
            audioSource.PlayOneShot(waterFall);
            message = JToken.Parse(@"{'type':'change','screen':'wait-screen'}");
            SendBroadcast(message, true);

            message = JToken.Parse(@"{'type':'message','screen':'all'}");
            SendBroadcast(message, true);

            //SceneManager.LoadScene("Prototype"); 

            KnockBack(); 
            fallPosition = Vector3.zero; 
            fallVelocity = Vector3.zero;    
            isDead = false; 

            velocity = Vector3.zero; 
            
        // For prototype, reset the scene. For final version, put a death screen here
        // Players won minigame
        } else if (other.transform.CompareTag(winTag)) {
            message = JToken.Parse(@"{'type':'change','screen':'wait-screen'}");
            SendBroadcast(message, true);

            message = JToken.Parse(@"{'type':'message','screen':'all'}");
            SendBroadcast(message, true);

            colourMinigame = false;

            // Finish line
            if (other.GetComponent<Variation>().variation == 1) {
                isFinished = true;
                StartCoroutine(MoveCamera());
                cam.GetComponent<CameraScript>().canMove = true;
                GetComponent<BranchesScript>().disabled = true;
            }

            Debug.Log("Minigame won");
        // Players play wall minigame
        } else if (other.transform.CompareTag(wallTag)) {
            message = JToken.Parse(@"{'type':'change','screen':'wall-screen'}");
            SendBroadcast(message, true);
            Debug.Log("Minigame wall");
        // Players play tiles minigame
        } else if (other.transform.CompareTag(tileTag)) {
            message = JToken.Parse(@"{'type':'change','screen':'tiles-screen'}");
            SendBroadcast(message, true);
            Debug.Log("Minigame tiles");

            // Changes image for each player
            int tileVariation = other.GetComponent<Variation>().variation;

            switch (tileVariation) {
                case 0:
                    Debug.Log(tileVariation);
                    for (int i = 0; i < GameLogic.players.Count; i++) {
                        string msg = "";
                        switch (i) {
                            case 0:
                                msg = @"{'type':'message','message':'one'}";
                                break;
                            case 1:
                                msg = @"{'type':'message','message':'two'}";
                                break;
                            case 2:
                                msg = @"{'type':'message','message':'three'}";
                                break;
                        }
                        message = JToken.Parse(msg);
                        SendMessage(GameLogic.players[i].id, message);
                    }
                    break;
                case 1:
                    Debug.Log(tileVariation);
                    for (int i = 0; i < GameLogic.players.Count; i++) {
                        string msg = "";
                        switch (i) {
                            case 0:
                                msg = @"{'type':'message','message':'four'}";
                                break;
                            case 1:
                                msg = @"{'type':'message','message':'five'}";
                                break;
                            case 2:
                                msg = @"{'type':'message','message':'six'}";
                                break;
                        }
                        message = JToken.Parse(msg);
                        SendMessage(GameLogic.players[i].id, message);
                    }
                    break;
                case 2:
                    Debug.Log(tileVariation);
                    for (int i = 0; i < GameLogic.players.Count; i++) {
                        string msg = "";
                        switch (i) {
                            case 0:
                                msg = @"{'type':'message','message':'seven'}";
                                break;
                            case 1:
                                msg = @"{'type':'message','message':'eight'}";
                                break;
                            case 2:
                                msg = @"{'type':'message','message':'nine'}";
                                break;
                        }
                        message = JToken.Parse(msg);
                        SendMessage(GameLogic.players[i].id, message);
                    }
                    break;
                case 3:
                    Debug.Log(tileVariation);
                    for (int i = 0; i < GameLogic.players.Count; i++) {
                        string msg = "";
                        switch (i) {
                            case 0:
                                msg = @"{'type':'message','message':'ten'}";
                                break;
                            case 1:
                                msg = @"{'type':'message','message':'eleven'}";
                                break;
                            case 2:
                                msg = @"{'type':'message','message':'twelve'}";
                                break;
                        }
                        message = JToken.Parse(msg);
                        SendMessage(GameLogic.players[i].id, message);
                    }
                    break;
            }
        } else if (other.transform.CompareTag(coloursTag)) {
            colourMinigame = true;

            message = JToken.Parse(@"{'type':'change','screen':'colours-screen'}");
            SendBroadcast(message, true);
            Debug.Log("Minigame colours");

            // Changes colours for each player
            int colourVatiation = other.GetComponent<Variation>().variation;

            switch (colourVatiation) {
                case 0:
                    Debug.Log(colourVatiation);
                    for (int i = 0; i < GameLogic.players.Count; i++) {
                        string msg = "";
                        switch (i) {
                            case 0:
                                msg = @"{'type':'message','message':'colours-one'}";
                                break;
                            case 1:
                                msg = @"{'type':'message','message':'colours-two'}";
                                break;
                            case 2:
                                msg = @"{'type':'message','message':'colours-three'}";
                                break;
                        }
                        message = JToken.Parse(msg);
                        SendMessage(GameLogic.players[i].id, message);
                    }
                    break;
                case 1:
                    Debug.Log(colourVatiation);
                    for (int i = 0; i < GameLogic.players.Count; i++) {
                        string msg = "";
                        switch (i) {
                            case 0:
                                msg = @"{'type':'message','message':'colours-four'}";
                                break;
                            case 1:
                                msg = @"{'type':'message','message':'colours-five'}";
                                break;
                            case 2:
                                msg = @"{'type':'message','message':'colours-six'}";
                                break;
                        }
                        message = JToken.Parse(msg);
                        SendMessage(GameLogic.players[i].id, message);
                    }
                    break;
            }
        } else if (other.transform.CompareTag(checkpointTag)) {
            checkpoint = other.transform.position;
        }
    }

    void OnCollisionEnter(Collision collision) {
        Debug.Log("Collision");
        if (collision.gameObject.CompareTag(deathTag) && collision.transform.position.y > 1) {
            Debug.Log("Knocking back");
            KnockBack();
        }
    }

    public void KnockBack() {
        controller.enabled = false;
        transform.position = checkpoint;
        controller.enabled = true;
        Debug.Log($"Knocking back to {checkpoint}!");
    }

    void OnDestroy() {
        if (AirConsole.instance != null) {
            if (usingAirConsole) AirConsole.instance.onMessage -= OnMessage;
        }
    }

    IEnumerator MoveCamera() {
        while (cam.position.y > 2.5f) {
            Vector3 camPosition = cam.position;
            camPosition.y = 0;
            Vector3 playerPosition = transform.position;
            playerPosition.y = 0;

            Vector3 change = (transform.position + Vector3.up * 2 - cam.position).normalized * Time.deltaTime;
            if ((camPosition - playerPosition).magnitude < 1.5f) {
                change.x = 0;
                change.z = 0;
            }

            cam.position += change;
            cam.RotateAround(transform.position, Vector3.up, Time.deltaTime * camRotSpeed);
            yield return null;
        }

        while (true) {
            cam.RotateAround(transform.position, Vector3.up, Time.deltaTime * camRotSpeed);
            yield return null;
        }
    }
}