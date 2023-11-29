using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Player")]
public class Player : ScriptableObject {
    public string username; // Name displayed on host screen
    public int id; // Device id
    public GameObject profile; // The position of the profile
    public int clickCounter = 0; // Click counter for the wall destroy minigame

    /*public Player(string username, int id, GameObject profile) {
        this.username = username;
        this.id = id;
        this.profile = profile;
    }*/
}
