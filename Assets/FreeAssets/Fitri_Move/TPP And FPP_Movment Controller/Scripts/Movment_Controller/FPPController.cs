using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPPController : MonoBehaviour
{
    public static FPPController fPPController;

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
    public float AirMultiplier = 5;
    private float SavedMoveSpeed = 0.0f;
    [Range(0, 50)]
    public float RunSpeed;
    public KeyCode RunKey = KeyCode.LeftShift;
    private bool isGrounded
    {
        get
        {
            return controller.isGrounded;
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
    public float MaxXAngle = 90;
    public float MinXAngle = -90;

    [Space]

    public float WalkBobMultiplier = 0.25f;
    public float RunBobMultiplier = 0.4f;

    [Space]
    
    public float FovSwitchSmoothness = 7f;
    public float RunFov = 70;
    public float JumpFov = 75;

    [HideInInspector] public float Movex = 0.0f, Movez = 0.0f;
    private float Airx = 0.0f, Airz = 0.0f;
    private Vector3 camForward, camRight;
    private Vector3 InputDir;
    private float YDir;
    private float JumpDelayTimer = 0.0f;
    private bool Fall;
    private bool CanHeadBob = true;
    [HideInInspector] public float Mousex, Mousey;

    
    private void Awake()
    {
        if (fPPController == null){
            fPPController =  this;
        }
    }

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        SavedMoveSpeed = MoveSpeed;

        if (CamViewSwitcher.camViewSwitcher){
            GetComponent<CamViewSwitcher>().Switch(GetComponent<CamViewSwitcher>().fPPController, GetComponent<CamViewSwitcher>().tPPController, GetComponent<CamViewSwitcher>().FPP);
        }
    }

    private void Update()
    {
        // Dont Update Camera Direction While UnGrounded
        if (isGrounded){
            camForward = camHolder.forward;
            camRight = camHolder.right;
        }

        // Input Normalize
        InputDir.Normalize();
        
        // Input Speed
        InputDir = InputDir * MoveSpeed * Time.deltaTime;

        // Dont Allow To Move in Y Axis
        camForward = Vector3.Scale(camForward, new Vector3(1,0,1)).normalized;
        camRight = Vector3.Scale(camRight, new Vector3(1,0,1)).normalized;

        // Move By character controller
        controller.Move(camForward * InputDir.z);
        controller.Move(camRight * InputDir.x);
        controller.Move(transform.up * YDir * Time.deltaTime);

        // Calculate Character_Controller Velocity
        CurrentVelocity = controller.velocity.magnitude;

        InputKeys();
        Gravity();
        Jump();
        SpeedManager();
        _CameraController();
        CenterBodyInCamDir();

        if (CamViewSwitcher.camViewSwitcher){
            CameraController.cameraController.SetCameraTarget(camHolderTarget, camTarget, CamViewSwitcher.camViewSwitcher.SwitchSmoothness);
        }
        else{
            CameraController.cameraController.SetCameraTarget(camHolderTarget, camTarget, 3f);
        }
        
        isMove = (Movex == 0 && Movez == 0) ? false : true;

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

        camAnim.enabled = true;
        
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
        
        if (isGrounded){
            if (isRun){
                CameraController.cameraController.ChangeFOV(RunFov, FovSwitchSmoothness);
            }
            else{
                CameraController.cameraController.ChangeFOV(CameraController.cameraController.SavedFOV, FovSwitchSmoothness);
            }
        }
        else{
            CameraController.cameraController.ChangeFOV(JumpFov, 8);
        }

        MoveSoundEffect.enabled = (CrouchController.crouchController.isCrouch) ? false : true;

        AnimationSpeedCalculate();
    }

    private void AnimationSpeedCalculate()
    {
        if (isGrounded){
            if (isRun){
                Speed = Mathf.Lerp(Speed, 1, 8 * Time.deltaTime);
            }
            if (!isRun && isMove){
               Speed = Mathf.Lerp(Speed, 0.5f, 8 * Time.deltaTime);
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

        InputDir = new Vector3(Movex, 0, Movez);
    }

    private bool CanStandUp()
    {
        return (Physics.Raycast(transform.position, Vector3.up, out RaycastHit hit, 4.5f)) ? false : true;
    }

    private void SpeedManager()
    {
        if (!CrouchController.crouchController.isCrouch){
            if (isRun){
                MoveSpeed = RunSpeed;
            }
            else if (!isRun && isMove){
                MoveSpeed = SavedMoveSpeed;
            }
        }
        else {
            MoveSpeed = CrouchController.crouchController.CrouchSpeed;
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

        if ((Input.GetKeyDown(JumpKey)) && isGrounded){
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
        CameraController.cameraController.MaxXAngle = MaxXAngle;
    }

    public void ForceStopHeadBob(bool _enabled)
    {
        CanHeadBob = _enabled;
    }

    public void CenterBodyInCamDir()
    {
        if (ModelHolder == null) { return; }
        ModelHolder.transform.rotation = Quaternion.Euler(0f, camHolder.eulerAngles.y, 0f);
    }
}