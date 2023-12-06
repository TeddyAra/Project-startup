using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class branchHit : MonoBehaviour
{
    private bool firstHit = true; 
    [SerializeField] AudioSource audioSource; 
    private void OnCollisionEnter(Collision collision)
    {
        if (firstHit)
        {
            audioSource.Play(); 
            firstHit = false;
        }
    }

}
