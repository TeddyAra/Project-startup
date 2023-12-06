using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class test : MonoBehaviour {
    [HideInInspector] public static string url = "https://alarm-queensland-floating-without.trycloudflare.com";

    void Start() { 
        DontDestroyOnLoad(this);
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.E)) {
            SceneManager.LoadScene("Alex Scene");
        }
    }
}
