using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CamViewSwitcher : MonoBehaviour
{
    [ReadOnlyWhenPlaying] public Behaviour CurrentSelectedController;
    [ReadOnlyWhenPlaying] public CameraController cameraController;

    public static CamViewSwitcher camViewSwitcher;

    public enum StartView { FPP, TPP };
    [ReadOnlyWhenPlaying] public StartView _startView;

    public UnityEvent FPP, TPP;

    public float SwitchSmoothness = 5f;
    
    public FPPController fPPController;
    public TPPController tPPController;

    public KeyCode SwitchKey = KeyCode.RightShift;


    private void Awake()
    {
        if (camViewSwitcher == null)
            camViewSwitcher = this;
    }
 
    private void Start()
    {
        if (cameraController == null){
            cameraController = GetComponentInChildren<CameraController>();
        }

        if (_startView == StartView.FPP){
            Switch(fPPController, tPPController, FPP);
        }
        else if (_startView == StartView.TPP){
            Switch(tPPController, fPPController, TPP);
        }
    }

    private void Update()
    {
        if (Input.GetKey(SwitchKey) && Input.GetKeyDown(KeyCode.Alpha1)){
            Switch(fPPController, tPPController, FPP);
        }
        else if (Input.GetKey(SwitchKey) && Input.GetKeyDown(KeyCode.Alpha2)){
            Switch(tPPController, fPPController, TPP);
        }
    }

    public void Switch(Behaviour _script1, Behaviour _script2, UnityEvent _event)
    {
        if (_script1 == CurrentSelectedController && _script2 != CurrentSelectedController){ return; }
        CurrentSelectedController = _script1;
        _script1.enabled = true;
        _script2.enabled = false;
        _event.Invoke();
    }

    public void UISwitch()
    {
        if (CurrentSelectedController==tPPController){
            Switch(fPPController, tPPController, FPP);
        }
        else{
            Switch(tPPController, fPPController, TPP);
        }
    }
}