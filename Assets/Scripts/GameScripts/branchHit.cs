using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class branchHit : MonoBehaviour {
    private bool firstHit = true; 
    [SerializeField] AudioSource audioSource;
    //[SerializeField] LayerMask ground;
    [HideInInspector] public GameObject particle;

    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.layer != 8) 
            return;

        if (firstHit) {
            Debug.Log("Hit!");
            audioSource.Play(); 
            firstHit = false;
            Destroy(particle);
        }
    }
}
