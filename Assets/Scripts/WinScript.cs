using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinScript : MonoBehaviour
{
    public GameObject youWinText;
    public float delay;
    // Start is called before the first frame update
    void Start()
    {
        youWinText.SetActive(false);
    }

    void OnTriggerEnter (Collider other)
    {
        if (other.gameObject.name == "Player")
        {
            youWinText.SetActive(true);
            StartCoroutine (Countdown ());
        }
    }

    IEnumerator Countdown ()
    {
        yield return new WaitForSeconds (delay);
        SceneManager.LoadScene (1);
    }
    
}
