using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TPPController : MonoBehaviour
{
    public static TPPController tPPController;

    private CharacterController controller;
    
    public bool isMove = false;
    public float CurrentVelocity = 0.0f;
    public float Speed = 0.0f;
    
    public Transform camHolder;
    public Transform camHolderTarget, camTarget;
    public Transform ModelHolder;

    [Header("Movement References")]
    public AudioSource MoveSoundEffect;
    public float GravityForce = -45;
    [Range(0, 50)]
    public float MoveSpeed;
    [Range(0, 20)] public float SwitchSpeedSmoothness = 4f;
    public float AirMultiplier = 5;
    private float SavedMoveSpeed = 0.0f;
    [Range(0, 50)]
    public float RunSpeed;
    public KeyCode RunKey = KeyCode.LeftShift;
    private bool isGrounded
    {
        get
        {
            return (controller.isGrounded);
        }
    }
    public bool Grounded;

    public bool isRun;

    [Header("Jumping")]
    public float JumpForce = 7f;
    public float JumpingSmoothness = 8f;
    public KeyCode JumpKey = KeyCode.Space;
    public AudioSource JumpSoundEffect, LandSoundEffect;

    [Header("Landing")]
    public bool LandEffect = true;
    public float LandEffectForce = 3.5f;
    public float LandEffectSmoothness = 5f;

    [Header("Camera References")]
    public float MaxXAngle = 35;
    public float MinXAngle = -35;

    [Space]

    public float WalkBobMultiplier = 0.25f;
    public float RunBobMultiplier = 0.4f;

    [Header("Rotate-Body")]
    public float TurnSmoothTime = 0.1f;
    float TurnSmoothVelocity;

    public enum RotateType { Locumotion, FREERotate };
    public RotateType BodyRotateStyle;
    public Dropdown BodyRotationTypeDropdown;

    [HideInInspector] public float Movex = 0.0f, Movez = 0.0f;
    private float Airx = 0.0f, Airz = 0.0f;
    private Vector3 camForward, camRight;
    private Vector3 InputDir;
    private float YDir;
    private float JumpDelayTimer = 0.0f;
    private bool Fall;
    private bool CanHeadBob = true;
    private bool CanMove = true;


    private void Awake()
    {
        if (tPPController == null){
            tPPController =  this;
        }
    }

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        SavedMoveSpeed = MoveSpeed;

        CameraController.cameraController.cameraDirection = camTarget.localPosition;
    
        if (GetComponent<CamViewSwitcher>()){
            GetComponent<CamViewSwitcher>().Switch(GetComponent<CamViewSwitcher>().tPPController, GetComponent<CamViewSwitcher>().fPPController, GetComponent<CamViewSwitcher>().TPP);
        }
        
        if (BodyRotationTypeDropdown){
            List<string> optionsList = new List<string> { (RotateType.FREERotate).ToString(), (RotateType.Locumotion).ToString() };
            BodyRotationTypeDropdown.ClearOptions();
            BodyRotationTypeDropdown.AddOptions(optionsList);
        }

        if (BodyRotateStyle == RotateType.FREERotate){
            BodyRotationTypeDropdown.value = 0;
        }
        else{
            BodyRotationTypeDropdown.value = 1;
        }
    }

    private void Update()
    {
        // Dont Update Camera Direction While UnGrounded
        if (isGrounded){
            camForward = camHolder.forward;
            camRight = camHolder.right;
        }
        
        // Input Speed
        InputDir = InputDir * MoveSpeed * Time.deltaTime;

        // Dont Allow To Move in Y Axis
        camForward = Vector3.Scale(camForward, new Vector3(1,0,1)).normalized;
        camRight = Vector3.Scale(camRight, new Vector3(1,0,1)).normalized;

        // Move By character controller
        if (CanMove){
            controller.Move(camForward * InputDir.z);
            controller.Move(camRight * InputDir.x);
        }
        controller.Move(transform.up * YDir * Time.deltaTime);

        // Calculate Character_Controller Velocity
        CurrentVelocity = controller.velocity.magnitude;

        InputKeys();
        Gravity();
        Jump();
        SpeedManager();
        RotateByInputAxis();
        _CameraController();
        
        if (CamViewSwitcher.camViewSwitcher){
            CameraController.cameraController.SetCameraTarget(camHolderTarget, camTarget, CamViewSwitcher.camViewSwitcher.SwitchSmoothness);
        }
        else{
            CameraController.cameraController.SetCameraTarget(camHolderTarget, camTarget, 3f);
        }

        if (!CrouchController.crouchController.isCrouch){
            camHolderTarget.GetComponent<Behaviour>().enabled = false;
            camHolderTarget.transform.localPosition = new Vector3(camHolderTarget.transform.localPosition.x, 0f, camHolderTarget.transform.localPosition.z);
        }
        else{
            camHolderTarget.GetComponent<Behaviour>().enabled = true;
        }
        
        isMove = ((Movex == 0 && Movez == 0) || !CanMove) ? false : true;

        if (InputSystemYT.inputsSystem.controllerType == InputSystemYT.ControllerType.Desktop){
            if (!GetComponent<HandPunchSystem>()){
                isRun = (Input.GetKey(RunKey) && isMove && (!CrouchController.crouchController.isCrouch)) ? true : false;
            }
            else{
                isRun = (Input.GetKey(RunKey) && isMove && (!CrouchController.crouchController.isCrouch) && !GetComponent<HandPunchSystem>().isPunching&&!GetComponent<HandPunchSystem>().AnimationNotFinished) ? true : false;
            }
        }
        else{
            if (!GetComponent<HandPunchSystem>()){
                if (!isMove || CrouchController.crouchController.isCrouch){
                    isRun = false;
                }
            }
            else{
                if (!isMove || CrouchController.crouchController.isCrouch || GetComponent<HandPunchSystem>().isPunching||GetComponent<HandPunchSystem>().AnimationNotFinished){
                    isRun = false;
                }
            }
        }
        
        Animator camAnim = camHolder.GetChild(0).transform.GetComponent<Animator>();
        
        camAnim.enabled = false;

        if (isGrounded){
            if (isRun || isMove){

                // HeadBob
                if (isMove || isRun || CrouchController.crouchController.isCrouch){
                    if (!CanHeadBob){
                        camAnim.SetFloat("Bob Multiplier", 0f);
                        camAnim.SetBool("isBob", false);
                    }
                    else{
                        camAnim.SetBool("isBob", true);

                        if (CrouchController.crouchController.isCrouch){
                            camAnim.SetFloat("Bob Multiplier", 0.15f);
                        }
                        if (isMove && !isRun && (!CrouchController.crouchController.isCrouch)){
                            camAnim.SetFloat("Bob Multiplier", WalkBobMultiplier);
                        }
                        else if (isMove && isRun && (!CrouchController.crouchController.isCrouch)){
                            camAnim.SetFloat("Bob Multiplier", RunBobMultiplier);
                        }
                    }
                }

                // Sound Effect
                if (isMove && !isRun){
                    if (MoveSoundEffect.isPlaying == false && MoveSoundEffect!=null){
                        if (MoveSoundEffect.enabled==true)
                            MoveSoundEffect.Play();
                    }
                    MoveSoundEffect.volume = Random.Range(0.7f, 0.7f);
                    MoveSoundEffect.pitch = Random.Range(0.35f, 0.4f);
                }
                else if (isMove && isRun){
                    if (MoveSoundEffect.isPlaying == false && MoveSoundEffect!=null){
                        if (MoveSoundEffect.enabled==true)
                            MoveSoundEffect.Play();
                    }
                    MoveSoundEffect.volume = 0.8f;
                    MoveSoundEffect.pitch = 0.8f;
                }
            }
            else{
                MoveSoundEffect.Stop();
                camAnim.SetBool("isBob", false);
                camAnim.SetFloat("Bob Multiplier", 0);
            }
        }
        else{
            MoveSoundEffect.Stop();
            camAnim.SetBool("isBob", false);
            camAnim.SetFloat("Bob Multiplier", 0);
        }
        
        //if (isGrounded){
            //if (isRun){
               // ChangeFOV(RunFov, FovSwitchSmoothness);
            //}
           // else{
                //ChangeFOV(SavedFOV, FovSwitchSmoothness);
           // }
       // }
       // else{
          //  ChangeFOV(JumpFov, 8);
       // }

        MoveSoundEffect.enabled = (CrouchController.crouchController.isCrouch) ? false : true;

        AnimationSpeedCalculate();
    }

    private void LateUpdate()
    {
    }

    private void AnimationSpeedCalculate()
    {
        if (isGrounded){
            if (isRun){
                Speed = Mathf.Lerp(Speed, 1, 5 * Time.deltaTime);
            }
            if (!isRun && isMove){
               Speed = Mathf.Lerp(Speed, 0.5f, 5 * Time.deltaTime);
            }
            if (!isMove){
                Speed = Speed = Mathf.Lerp(Speed, 0, 8 * Time.deltaTime);
            }
        }
        else{
            Speed = Speed = Mathf.Lerp(Speed, 0, 8 * Time.deltaTime);
        }
    }

    private void InputKeys()
    {
        if (isGrounded){
            Movex = InputSystemYT.inputsSystem.InputDirection.y;
            Movez = InputSystemYT.inputsSystem.InputDirection.z;

            Airx = 0;
            Airz = 0;
        }
        else{
            Airx = InputSystemYT.inputsSystem.InputDirection.y * AirMultiplier * 0.1f;
            Airz = InputSystemYT.inputsSystem.InputDirection.z * AirMultiplier * 0.1f;

            Movex += Airx;
            Movez += Airz;
        }

        // Clamping Values
        Movex = Mathf.Clamp(Movex, -1, 1);
        Movez = Mathf.Clamp(Movez, -1, 1);
        
        InputDir = new Vector3(Movex, 0, Movez);

        // Input Normalize
        InputDir.Normalize();
    }

    private bool CanStandUp()
    {
        return (Physics.Raycast(transform.position, Vector3.up, out RaycastHit hit, 3)) ? false : true;
    }

    private void SpeedManager()
    {
        if (!CrouchController.crouchController.isCrouch){
            if (isRun){
                MoveSpeed = Mathf.Lerp(MoveSpeed, RunSpeed, SwitchSpeedSmoothness * Time.deltaTime);
            }
            else if (!isRun && isMove){
                MoveSpeed = MoveSpeed = Mathf.Lerp(MoveSpeed, SavedMoveSpeed, SwitchSpeedSmoothness * Time.deltaTime);
            }
            else if (!isRun && !isMove){
                MoveSpeed = MoveSpeed = Mathf.Lerp(MoveSpeed, 0, SwitchSpeedSmoothness * Time.deltaTime);
            }
        }
        else {
            MoveSpeed = Mathf.Lerp(MoveSpeed, CrouchController.crouchController.CrouchSpeed, SwitchSpeedSmoothness * Time.deltaTime);
        }
    }

    private void Gravity()
    {
        Grounded = isGrounded;

        // Gravity Force   
        if (!isGrounded){
            Fall = true;
            YDir += GravityForce * Time.deltaTime;
        }
        if (isGrounded && Fall){
            
            if (controller.height < CrouchController.crouchController.StartHeight){
                Fall = false;
                return;
            }
    
            if (JumpSoundEffect!=null){
                JumpSoundEffect.Stop();
            }
            if (LandSoundEffect!=null){
                LandSoundEffect.Play();
            }

            if (LandEffect){
                PlayerFall(CameraController.cameraController.FallMotion(LandEffectForce, LandEffectSmoothness));
            }
        }
    }

    private void Jump()
    {
        if (GetComponent<HandPunchSystem>()){
            if (GetComponent<HandPunchSystem>().AnimationNotFinished || GetComponent<HandPunchSystem>().isPunching){
                return;
            }
        }

        if (Input.GetKeyDown(JumpKey) && isGrounded){
            if (CrouchController.crouchController.isCrouch){
                CrouchController.crouchController.isCrouch = false;
                return;
            }

            if (Time.time > JumpDelayTimer){
                JumpDelayTimer = Time.time + 1.5f;
                JumpSoundEffect.Play();
                YDir = JumpForce;
                float add = 0.0f;
                add += 1;
                controller.Move(transform.up * JumpingSmoothness * add * Time.deltaTime);

                if (isMove){
                    PushForward(7);
                }
            }
        }
    }

    public void UIJump()
    {
        if (isGrounded){
            if (CrouchController.crouchController.isCrouch){
                CrouchController.crouchController.isCrouch = false;
                return;
            }

            if (Time.time > JumpDelayTimer){
                JumpDelayTimer = Time.time + 0.7f;
                JumpSoundEffect.Play();
                YDir = JumpForce;
                float add = 0.0f;
                add += 1;
                controller.Move(transform.up * JumpingSmoothness * add * Time.deltaTime);

                if (isMove){
                    PushForward(7);
                }
            }
        }
    }

    public void UICrouch()
    {
        if (CrouchController.crouchController.isCrouch){
            CrouchController.crouchController.isCrouch = false;
        }
        else{
            CrouchController.crouchController.isCrouch = true;
        }
    }

    public void UIRun(bool _isRun)
    {
        isRun = _isRun;
    }

    void PlayerFall(Vector3 motion)
    {
        CameraController.cameraController.camHolder.localPosition += motion;
        Fall = false;
    }

    private void PushForward(float _force)
    {
        // Dont Jmup While You Crouch or Prone
        if (CrouchController.crouchController.isCrouch) return;

        YDir = JumpForce;
        
        float add = 0.0f;
        add += 1;
        controller.Move(camForward * _force * add * Time.deltaTime);
    }

    private void _CameraController()
    {
        CameraController.cameraController.MinXAngle = MinXAngle;
        CameraController.cameraController.MaxXAngle = (CrouchController.crouchController.isCrouch ? (MaxXAngle+4) : (MaxXAngle));
    }

    public void SyncLastCameraValue(FPPController fpp)
    {
        ModelHolder.transform.localRotation = Quaternion.Euler(0f, CameraController.cameraController.YRotation, 0f);
    }

    public void ForceStopHeadBob(bool _enabled)
    {
        CanHeadBob = _enabled;
    }
    
    private void RotateByInputAxis()
    {
        if (BodyRotateStyle == RotateType.Locumotion && !CrouchController.crouchController.isCrouch){
            ModelHolder.transform.rotation = Quaternion.Slerp(ModelHolder.transform.rotation, Quaternion.Euler(0f, camHolder.eulerAngles.y, 0f), 5 * Time.deltaTime);
            return;
        }

        if (isMove){
            CenterBody(TurnSmoothTime);
        }
    }

    public void CenterBody(float s)
    {
        if (ModelHolder == null) { return; }
        Vector3 dir = new Vector3(Movex, 0, Movez).normalized;
        float targetAngle = 0.0f;
        targetAngle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg + camHolder.eulerAngles.y;
        float angle = Mathf.SmoothDampAngle(ModelHolder.transform.eulerAngles.y, targetAngle, ref TurnSmoothVelocity, s);
        ModelHolder.transform.rotation = Quaternion.Euler(0f, angle, 0f);
    }

    public void FreezeMovement(bool _enabled)
    {
        CanMove = _enabled;
    }


    public void ChangeRotationStyleValue()
    {
        if (BodyRotationTypeDropdown.value == 0){
            BodyRotateStyle = RotateType.FREERotate;
        }
        else if (BodyRotationTypeDropdown.value == 1){
            BodyRotateStyle = RotateType.Locumotion;
        }
    }
}