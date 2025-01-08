using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class States : MonoBehaviour
{
    public static States states;

    public enum CurrentControllerState { ThirdPersonView, FirstPersonView };
    public CurrentControllerState currentControllerState;

    [HideInInspector] public bool isGrounded
    {
        get{
            return GetComponent<CharacterController>().isGrounded;
        }
    }


    private void Awake()
    {
        states = this;

        if (CamViewSwitcher.camViewSwitcher){
            if (CamViewSwitcher.camViewSwitcher.CurrentSelectedController==TPPController.tPPController){
                currentControllerState = CurrentControllerState.ThirdPersonView;
            }
            else if (CamViewSwitcher.camViewSwitcher.CurrentSelectedController==FPPController.fPPController){
                currentControllerState = CurrentControllerState.FirstPersonView;
            }
        }
        else{
            if (this.GetComponent<TPPController>()){
                currentControllerState = CurrentControllerState.ThirdPersonView;
            }
            if (this.GetComponent<FPPController>()){
                currentControllerState = CurrentControllerState.FirstPersonView;
            }
        }
    }

    private void Start()
    {
    }

    private void Update()
    {
        if (CamViewSwitcher.camViewSwitcher){
            if (CamViewSwitcher.camViewSwitcher.CurrentSelectedController==TPPController.tPPController){
                currentControllerState = CurrentControllerState.ThirdPersonView;
            }
            else if (CamViewSwitcher.camViewSwitcher.CurrentSelectedController==FPPController.fPPController){
                currentControllerState = CurrentControllerState.FirstPersonView;
            }
        }
        else{
            if (this.GetComponent<TPPController>()){
                currentControllerState = CurrentControllerState.ThirdPersonView;
            }
            if (this.GetComponent<FPPController>()){
                currentControllerState = CurrentControllerState.FirstPersonView;
            }
        }
    }
}