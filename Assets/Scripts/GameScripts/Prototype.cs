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
    [SerializeField] private GameObject canvas;
    [SerializeField] private TMP_Text text;

    // Settings
    [Header("Settings")]
    [SerializeField] private float speed = 12f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float groundDistance = 0.4f;
    [SerializeField] private float jumpHeight = 1f;
    [SerializeField] private float distanceCheck = 0.1f;
    [SerializeField] private string deathTileTag = "Death";
    [SerializeField] private string winTag = "Win";
    [SerializeField] private bool camIgnoreX = true;

    // Variables
    private Vector3 velocity;
    private bool isGrounded;
    private bool isDead;

    void Update() {
        // No input if dead
        if (isDead) return;

        // Gets input
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // Applies input        
        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * speed * Time.deltaTime);

        if (camIgnoreX)
            cam.position = new Vector3(0f, cam.position.y, cam.position.z);

        // Calculates gravity and jump force
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0) {
            velocity.y = -2f;
        }

        if (Input.GetButtonDown("Fire1") && isGrounded) {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;

        // Applies gravity
        controller.Move(velocity * Time.deltaTime);

        // Checks if player is on a death tile
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, distanceCheck, groundMask)) {
            if (hit.transform.CompareTag(deathTileTag)) {
                isDead = true;
                canvas.SetActive(true);
            } else if (hit.transform.CompareTag(winTag)) {
                isDead = true;
                canvas.SetActive(true);
                text.SetText("You won!");
            }
        }
    }
}