using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandPunchSystem : MonoBehaviour
{
    public Animator Tpp_anim, Fpp_anim;

    public Camera cam;

    //public float PunchDelay = 0.2f;
    public float PunchRange = 2f;

    public Transform leftHand, rightHand;

    private Collider[] left_cols, right_cols;

    [ReadOnly] public bool isPunching = false;
    [ReadOnly] public bool AnimationNotFinished = false;

    [Header("Punch Button.... if you want left Mouse Click write (Fire1) and for right write (Fire2)")]
    public string MouseButton = "Fire1";

    [Header("Sound Effect")]
    public AudioSource PunchSound;

    ///////////////////////////////
    private float DelayTimer = 0.0f;
    private bool canDamage = false;
    private float PunchTimer = 0.0f;

    
    private void Awake()
    {
    }

    private void Start()
    {
        if (States.states.currentControllerState == States.CurrentControllerState.ThirdPersonView){
            leftHand = Tpp_anim.GetBoneTransform(HumanBodyBones.LeftHand);
            rightHand = Tpp_anim.GetBoneTransform(HumanBodyBones.RightHand);

            if (Tpp_anim==null || leftHand==null || rightHand==null){
                this.enabled = false;
            }
        }
    }

    private void Update()
    {
        if (!States.states.isGrounded){
            return;
        }

        canDamage = ((isPunching && Time.time > PunchTimer) ? true : false);

        if (InputSystemYT.inputsSystem.controllerType == InputSystemYT.ControllerType.Desktop)
        {
            if (States.states.currentControllerState == States.CurrentControllerState.ThirdPersonView){
                TPP_Punch();
                isPunchBool(Input.GetButton(MouseButton));

                if (canDamage){
                    PunchTimer = Time.time + (Tpp_anim.GetFloat("PunchMultiplier")-0.2f);
                }
            }
            else if (States.states.currentControllerState == States.CurrentControllerState.FirstPersonView){
                if (Input.GetButton(MouseButton)){
                    FPP_Punch();
                }
                else{
                    AnimationNotFinished = (false);
                    Fpp_anim.SetBool("punch", false);
                }

                if (canDamage){
                    PunchTimer = Time.time + (Fpp_anim.GetFloat("ShakeMultiplier")-0.2f);
                }
            }

            if (Input.GetButton(MouseButton) || isPunching || AnimationNotFinished){
                if (!PunchSound.isPlaying){
                    PunchSound.Play();
                }
            }
        }
        else{
            if (States.states.currentControllerState == States.CurrentControllerState.ThirdPersonView){
                if (isPunching || AnimationNotFinished){
                    TPP_Punch();
                    if (!PunchSound.isPlaying){
                        PunchSound.Play();
                    }
                }
                else{
                    AnimationNotFinished = (false);
                    Tpp_anim.SetBool("punch", false);
                }

                if (canDamage){
                    PunchTimer = Time.time + (Tpp_anim.GetFloat("PunchMultiplier")-0.2f);
                }
            }
            else if (States.states.currentControllerState == States.CurrentControllerState.FirstPersonView){
                if (isPunching || AnimationNotFinished){
                    FPP_Punch();
                }
                else{
                    AnimationNotFinished = (false);
                    Fpp_anim.SetBool("punch", false);
                }

                if (canDamage){
                    PunchTimer = Time.time + (Fpp_anim.GetFloat("ShakeMultiplier")-0.2f);
                }
            }
        }
    }

    public void TPP_Punch()
    {
        if (States.states.currentControllerState == States.CurrentControllerState.ThirdPersonView){
            if (leftHand==null||rightHand==null){
                leftHand = Tpp_anim.GetBoneTransform(HumanBodyBones.LeftHand);
                rightHand = Tpp_anim.GetBoneTransform(HumanBodyBones.RightHand);
            }

            left_cols = Physics.OverlapSphere(leftHand.position, PunchRange);
            right_cols = Physics.OverlapSphere(rightHand.position, PunchRange);


                if (TPPController.tPPController.BodyRotateStyle == TPPController.RotateType.FREERotate&&isPunching){
                    TPPController.tPPController.CenterBody(0.03f);
                }

                if (isPunching&&canDamage){
                    foreach(Collider Lcol in left_cols){
                        if (Lcol.transform.GetComponent<TakeDamageEvent>() && Lcol.transform != transform.root){
                            Lcol.transform.GetComponent<TakeDamageEvent>()._event.Invoke();
                        }
                    }
                    foreach(Collider Rcol in right_cols){
                        if (Rcol.transform.GetComponent<TakeDamageEvent>() && Rcol.transform != transform.root){
                            Rcol.transform.GetComponent<TakeDamageEvent>()._event.Invoke();
                        }
                    }
                }

            AnimationNotFinished = (Tpp_anim.GetCurrentAnimatorStateInfo(1).IsName("Punch") ? true : false);
            Tpp_anim.SetBool("punch", isPunching);
        }
    }

    public void FPP_Punch()
    {
        if (States.states.currentControllerState == States.CurrentControllerState.FirstPersonView){
            bool canPunch = false;
            if (!Fpp_anim.GetCurrentAnimatorStateInfo(0).IsName("Shake") && Time.time > DelayTimer){
                DelayTimer = Time.time + 1.2f;
                canPunch = true;
                if (Physics.Raycast(cam.transform.position, cam.transform.TransformDirection(Vector3.forward), out RaycastHit hit, 1.8f)){
                    if (hit.transform.GetComponent<TakeDamageEvent>() && canDamage && hit.transform != transform.root){
                        hit.transform.GetComponent<TakeDamageEvent>()._event.Invoke();
                    }
                }
                if (!PunchSound.isPlaying){
                    PunchSound.Play();
                }
            }
            else{
                canPunch = false;
            }

            AnimationNotFinished = (Fpp_anim.GetCurrentAnimatorStateInfo(0).IsName("Shake") ? true : false);
            Fpp_anim.SetBool("punch", canPunch);
        }
    }

    public void isPunchBool(bool state)
    {
        isPunching = state;
        Fpp_anim.SetBool("punch", state);
    }

    private void OnDrawGizmosSelected()
    {
        if (States.states){
            if (States.states.currentControllerState == States.CurrentControllerState.ThirdPersonView){
                if (!leftHand||!rightHand) { return; }
                    Gizmos.color = Color.red;
                    Gizmos.DrawWireSphere(leftHand.position, PunchRange);
                    Gizmos.DrawWireSphere(rightHand.position, PunchRange);
            }
        }
    }
}