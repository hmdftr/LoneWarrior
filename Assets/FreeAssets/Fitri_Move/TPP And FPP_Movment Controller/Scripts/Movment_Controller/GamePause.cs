using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePause : MonoBehaviour
{
    public static GamePause gameActivity;

    public bool Paused = false;

    public float smooth = 7;

    public GameObject SettingsPanel;
    public GameObject Notes;


    private void Awake()
    {
        gameActivity = this;
    }

    private void Start()
    {
        Notes.SetActive(!SettingsPanel.activeSelf);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)){
            if (SettingsPanel.activeSelf){
                SettingsPanel.SetActive(false);
                Notes.SetActive(true);
            }
            else{
                SettingsPanel.SetActive(true);
                Notes.SetActive(false);
            }
        }

        Paused = SettingsPanel.activeSelf;

        AudioListener.volume = (Paused) ?  0f : 1f;

        if (Paused){
            Time.timeScale = 0.1f;
        }
        else{
            if (Time.timeScale==1) { return; }
            Time.timeScale = 1f;
        }
    }
}