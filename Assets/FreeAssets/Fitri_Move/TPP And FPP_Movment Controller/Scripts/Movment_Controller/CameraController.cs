using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController cameraController;

    [Header("Camera-Settings")]
    public Transform camHolder;
    public Camera cam;

    public float MinXAngle, MaxXAngle;

    public float RotationSmoothness;

    public float MouseSenstivity = 70;

    public bool InvertedXAxis, InvertedYAxis;

    [Header("Cursor")]
    public bool Closed = true;
    public KeyCode CloseKey = KeyCode.LeftControl;
    public Texture2D cursorTexture;
    public CursorMode cursorMode = CursorMode.Auto;
    private Vector2 hotSpot = Vector2.zero;
    
    [Header("Camera Collision (For TPP Only)")]
    public Vector2 camDistanceMinMax = new Vector2(0.5f, 5f);
    public bool CameraCollisionEnabled = true;
    [ReadOnly] public Vector3 cameraDirection;
    [ReadOnly] public float camDistance;


    [HideInInspector] public float Mousex, Mousey;
    [HideInInspector] public float XRotation, YRotation;
    [HideInInspector] public float SavedFOV;
    [HideInInspector] public Vector3 OriginalCamHolderPos;


    private void Awake()
    {
        if (cameraController == null)
            cameraController = this;
    }

    private void Start()
    {
        //cameraDirection = cam.transform.localPosition.normalized;
        camDistance = camDistanceMinMax.y;

        SavedFOV = cam.fieldOfView;
        OriginalCamHolderPos = camHolder.localPosition;
    }

    private void Update()
    {
        MouseCursor(Closed);
        ResetPosition();

        _CameraCollision(cam.transform);
    }

    private void LateUpdate()
    {
        _CameraController();
    }

    private void _CameraController()
    {
        Mousex = InputSystemYT.inputsSystem.CameraAxisDirection.y * MouseSenstivity * Time.deltaTime;
        Mousey = InputSystemYT.inputsSystem.CameraAxisDirection.x * MouseSenstivity * Time.deltaTime;

        camHolder.transform.localRotation = Quaternion.Slerp(camHolder.transform.localRotation, Quaternion.Euler(XRotation, YRotation, 0f), RotationSmoothness * Time.deltaTime);

        if (InvertedYAxis){
            YRotation -= Mousey;
        }
        else{
            YRotation += Mousey;
        }

        if (InvertedXAxis){
            XRotation += Mousex;
        }
        else{
            XRotation -= Mousex;
        }

        XRotation = Mathf.Clamp(XRotation, MinXAngle, MaxXAngle);
    }

    private void _CameraCollision(Transform cam)
    {
       Vector3 diseredCameraPos = transform.TransformPoint(cameraDirection * camDistanceMinMax.y);
       RaycastHit hit;
        if (Physics.Linecast(transform.position, diseredCameraPos, out hit)){
            camDistance = Mathf.Clamp(hit.distance, camDistanceMinMax.x, camDistanceMinMax.y);
        }
        else{
            camDistance = camDistanceMinMax.y;
        }
    }

    public void SetCameraTarget(Transform _target, Transform _target2, float _smoothness)
    {
        camHolder.transform.localPosition = Vector3.Lerp(camHolder.transform.localPosition, _target.localPosition, _smoothness * Time.deltaTime);
        
        if (CamViewSwitcher.camViewSwitcher && CamViewSwitcher.camViewSwitcher.CurrentSelectedController == TPPController.tPPController && CameraCollisionEnabled){
            camHolder.transform.GetChild(0).transform.localPosition = Vector3.Lerp(camHolder.transform.GetChild(0).transform.localPosition, cameraDirection * camDistance, _smoothness * Time.deltaTime);
        }
        else{
            camHolder.transform.GetChild(0).transform.localPosition = Vector3.Lerp(camHolder.transform.GetChild(0).transform.localPosition, _target2.localPosition, _smoothness * Time.deltaTime);
        }
    }

    void ResetPosition()
    {
        if (camHolder.localPosition == OriginalCamHolderPos || (CrouchController.crouchController.isCrouch)) return;
        camHolder.localPosition = Vector3.Lerp(camHolder.localPosition, OriginalCamHolderPos, 2.5f * Time.deltaTime);
    }

    public void ChangeFOV(float _fov, float smoothness)
    {
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, _fov, smoothness * Time.deltaTime);
    }

    private void MouseCursor(bool _closed)
    {
        if (InputSystemYT.inputsSystem.controllerType == InputSystemYT.ControllerType.Mobile || Time.timeScale < 1f){
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            return;
        }

        Cursor.visible = !_closed;
        Cursor.lockState = (!_closed) ? CursorLockMode.None : CursorLockMode.Locked;

        if (Input.GetKeyDown(CloseKey)){
            if (Closed)
                Closed = false;
            else
                Closed = true;
        }

        Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
    }
    
    public Vector3 FallMotion(float _landForce, float _landSmoothness)
    {
        Vector3 _FallMotion = Vector3.zero;
        _FallMotion.y = -_landForce * _landSmoothness * Time.deltaTime;
        return _FallMotion;
    }
}