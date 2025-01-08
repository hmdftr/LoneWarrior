using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class doorController : MonoBehaviour
{
    public bool resetable;
    public GameObject door;
    public bool startOpen;

    public AudioSource audioPlayer;

    bool firstTrigger = false;
    bool open = true;

    Animator doorAnim;
    // Start is called before the first frame update
    void Start()
    {
        doorAnim = door.GetComponent<Animator>();

        if(!startOpen)
        {
            open = false;
            doorAnim.SetTrigger("doorTrigger");
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name == "Player" && !firstTrigger)
        {
            if (!resetable) firstTrigger = true;
            doorAnim.SetTrigger("doorTrigger");
            open = !open;
            audioPlayer.Play();

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
