using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MusicScript : MonoBehaviour
{

    public AudioSource audioSource; 
    public AudioClip musicStart;
    public AudioClip musicLoop;
    private bool startIsPlaying = true; 
    private float introLength; 
    private float timer; 


    private void Start()
    {
        introLength = musicStart.length; 
        StartCoroutine(PlayAfterDelay(introLength));
        audioSource.PlayOneShot(musicStart);
        Debug.Log(introLength); 
    }

    IEnumerator PlayAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay); 

        audioSource.PlayOneShot(musicLoop); 
        audioSource.loop = true;

    }


}
