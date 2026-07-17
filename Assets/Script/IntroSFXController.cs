using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroSFXController : MonoBehaviour
{
    public AudioClip popWinSound;
    public AudioClip dingSound;
    //public AudioClip popWinSound;
    public AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayPopWinSFX()
    {
        audioSource.PlayOneShot(popWinSound);
    }

    public void PlayPopDingSFX()
    {
        audioSource.PlayOneShot(dingSound);
    }
}
