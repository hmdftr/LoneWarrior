using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimationController : MonoBehaviour
{
    public static CharacterAnimationController animationController;

    [HideInInspector] public Animator anim;

    public TPPController Controller;

    [Range(0,1)] public float SwitchSmooth = 0.075f;

    private bool GroundDetector;

    public GroundFar CalculateGroundFar;

    private bool CanStopRunAnimation = false;


    private void Start()
    {
        if (animationController == null){
            animationController = this;
        }

        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (!Controller.enabled){
            anim.SetBool("isGrounded", true);
            anim.SetFloat("H", 0, SwitchSmooth, Time.deltaTime);
            anim.SetFloat("V", 0, SwitchSmooth, Time.deltaTime);
            anim.SetFloat("Speed", 0);
            return;
        }
        else{
            if (!GroundDetector){
                StartCoroutine(GroundDetectDelay(0.45f));
            }
        }

        anim.SetFloat("H", Controller.Movez, SwitchSmooth, Time.deltaTime);
        anim.SetFloat("V", Controller.Movex, SwitchSmooth, Time.deltaTime);

        anim.SetFloat("Speed", Controller.Speed);

        anim.SetBool("isCrouch", CrouchController.crouchController.isCrouch);

        anim.SetBool("Locumotion", (Controller.BodyRotateStyle == TPPController.RotateType.Locumotion) ? true : false);
        
        anim.SetBool("isRun", Controller.isRun);

        if (GroundDetector){
            anim.SetBool("isGrounded", Controller.Grounded);
        }

        if (anim.GetCurrentAnimatorStateInfo(0).IsTag("Landing")){
            Controller.FreezeMovement(false);
        }
        else{
            Controller.FreezeMovement(true);
        }

        anim.SetBool("Fall", CalculateGroundFar.FarFromGround ? true : false);

        if (Controller.Speed >= 0.5f){
            CanStopRunAnimation = true;
        }
        if (CanStopRunAnimation && Controller.Speed < 0.5f){
            //anim.Play("Run To Stop");
            CanStopRunAnimation = false;
        }
    }

    private IEnumerator GroundDetectDelay(float _time)
    {
        GroundDetector = false;
        yield return new WaitForSeconds(_time);
        GroundDetector = true;
    }

    public void SwitchSide()
    {
        //if (anim.GetCurrentAnimatorStateInfo(1).IsName("Left Turn")){
          //  return;
       // }
        anim.Play("Left Turn");
    }
}