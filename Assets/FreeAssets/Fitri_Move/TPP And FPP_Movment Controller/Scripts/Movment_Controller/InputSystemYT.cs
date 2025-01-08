using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputSystemYT : MonoBehaviour
{
    public static InputSystemYT inputsSystem;

    public enum ControllerType { Desktop, Mobile, };
    public ControllerType controllerType;
    public Dropdown controllerTypeDropdown;
    public bool AutoDetectDevice = false;

    [ReadOnlyWhenPlaying] public Canvas canvas;
    public RectTransform UIControlsPanel;
    [ReadOnlyWhenPlaying] public VirtualJoystick MovmentvirtualJoystick, CameravirtualJoystick;

    [ReadOnly] public Vector3 InputDirection = Vector3.zero;
    [ReadOnly] public Vector3 CameraAxisDirection = Vector3.zero;

    public float CameraJoystickSmoothness = 2f;

    
    private void Awake()
    {
        if (AutoDetectDevice){
            if (SystemInfo.deviceType == DeviceType.Desktop){
                controllerType = ControllerType.Desktop;
            }
            else{
                controllerType = ControllerType.Mobile;
            }
        }

        if (inputsSystem==null){
            inputsSystem = this;
        }
    }

    private void Start()
    {
        if (controllerTypeDropdown){
            List<string> optionsList = new List<string> { (ControllerType.Desktop).ToString(), (ControllerType.Mobile).ToString() };
            controllerTypeDropdown.ClearOptions();
            controllerTypeDropdown.AddOptions(optionsList);
            
            if (AutoDetectDevice){
                if (SystemInfo.deviceType == DeviceType.Desktop){
                    controllerTypeDropdown.value = 0;
                }
                else{
                    controllerTypeDropdown.value = 1;
                }
            }
            else{
                if (controllerType == ControllerType.Desktop){
                    controllerTypeDropdown.value = 0;
                }
                else{
                    controllerTypeDropdown.value = 1;
                }
            }
        }
    }

    private void Update()
    {
        if (controllerTypeDropdown.value == 0){
            controllerType = ControllerType.Desktop;
        }
        else{
            controllerType = ControllerType.Mobile;
        }


        if (UIControlsPanel != null){
            UIControlsPanel.gameObject.SetActive((controllerType==ControllerType.Mobile) ? true : false);
        }

        if (controllerType == ControllerType.Mobile){
            InputDirection = (new Vector3(0f, MovmentvirtualJoystick.InputDirection.x, MovmentvirtualJoystick.InputDirection.z)) * 1.5f;
            CameraAxisDirection = (new Vector3(CameravirtualJoystick.InputDirection.x, CameravirtualJoystick.InputDirection.z, 0f)) * CameraJoystickSmoothness;
        }
        if (controllerType == ControllerType.Desktop){
            InputDirection = new Vector3(0f, Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            CameraAxisDirection = new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"), 0f);
        }
    }

    public void JumpEvent()
    {
        if (States.states.currentControllerState == States.CurrentControllerState.ThirdPersonView){
            TPPController.tPPController.UIJump();
        }
        if (States.states.currentControllerState == States.CurrentControllerState.FirstPersonView){
            FPPController.fPPController.UIJump();
        }
    }

    public void CrouchEvent()
    {
        if (States.states.currentControllerState == States.CurrentControllerState.ThirdPersonView){
            TPPController.tPPController.UICrouch();
        }
        if (States.states.currentControllerState == States.CurrentControllerState.FirstPersonView){
            FPPController.fPPController.UICrouch();
        }
    }

    public void RunEvent(bool _isRun)
    {
        if (States.states.currentControllerState == States.CurrentControllerState.ThirdPersonView){
            TPPController.tPPController.UIRun(_isRun);
        }
        if (States.states.currentControllerState == States.CurrentControllerState.FirstPersonView){
            FPPController.fPPController.UIRun(_isRun);
        }
    }
}