using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimationController_NEW : MonoBehaviour
{
    public static CharacterAnimationController_NEW animationController;

    [HideInInspector] public Animator anim;

    public TPPController Controller;

    [Range(0, 1)] public float SwitchSmooth = 0.075f;

    private bool GroundDetector;

    public GroundFar CalculateGroundFar;

    private bool CanStopRunAnimation = false;

    #region Switch Weapon & Combo Attack

    [Header("Is Weapon Active?")]
    public SwitchWeapon switchWeapon;
    public bool weaponActive;

    [Header("Combo attack")]
    [SerializeField] private int CountAttackClick;

    #endregion


    private void Start()
    {
        if (animationController == null)
        {
            animationController = this;
        }

        anim = GetComponent<Animator>();

        CountAttackClick = 0;
    }

    private void Update()
    {
        #region Movement (Walking/Running/Crouch/Falling)

        if (!Controller.enabled)
        {
            anim.SetBool("isGrounded", true);
            anim.SetFloat("H", 0, SwitchSmooth, Time.deltaTime);
            anim.SetFloat("V", 0, SwitchSmooth, Time.deltaTime);
            anim.SetFloat("Speed", 0);
            return;
        }
        else
        {
            if (!GroundDetector)
            {
                StartCoroutine(GroundDetectDelay(0.45f));
            }
        }

        anim.SetFloat("H", Controller.Movez, SwitchSmooth, Time.deltaTime);
        anim.SetFloat("V", Controller.Movex, SwitchSmooth, Time.deltaTime);

        anim.SetFloat("Speed", Controller.Speed);

        anim.SetBool("isCrouch", CrouchController.crouchController.isCrouch);

        anim.SetBool("Locumotion", (Controller.BodyRotateStyle == TPPController.RotateType.Locumotion) ? true : false);

        anim.SetBool("isRun", Controller.isRun);

        if (GroundDetector)
        {
            anim.SetBool("isGrounded", Controller.Grounded);
        }

        if (anim.GetCurrentAnimatorStateInfo(0).IsTag("Landing"))
        {
            Controller.FreezeMovement(false);
        }
        else
        {
            Controller.FreezeMovement(true);
        }

        anim.SetBool("Fall", CalculateGroundFar.FarFromGround ? true : false);

        if (Controller.Speed >= 0.5f)
        {
            CanStopRunAnimation = true;
        }
        if (CanStopRunAnimation && Controller.Speed < 0.5f)
        {
            //anim.Play("Run To Stop");
            CanStopRunAnimation = false;
        }

        #endregion

        #region Switch Weapon & Combo Attack System

        if (switchWeapon.weaponActive.Equals(true))
        {
            weaponActive = true;
            Debug.Log("Attack action is available");

            if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Slash))
                InputAttack();
        }
        else
        {
            switchWeapon.weaponActive.Equals(false);
            weaponActive = false;

            Debug.Log("Attack action is disable");
        }

        #endregion
    }

    #region Combo Attack Sytem -> Input and Animation events

    public void InputAttack()
    {
        CountAttackClick++;

        //Debug.Log($"Attack count = {CountAttackClick}");

        #region Play Combo Attack Animation when the button is pressed

        if (CountAttackClick == 1)
        {
            Debug.Log($"Combo Attack Count = {CountAttackClick}");
            anim.SetInteger("ComboAttackCount", 1);
        }

        #endregion
    }
    public void AnimEvent()
    {
        //Debug.Log("Animation End Check");
        CheckAttackState();
    }

    public void CheckAttackState()
    {
        Debug.Log("Checking attack state...");

        if (anim.GetCurrentAnimatorStateInfo(1).IsName("Attack1"))
        {
            Debug.Log("Current state : Combo 1");

            if (CountAttackClick > 1)
            {
                // If multiple click, play the next attack state
                anim.SetInteger("ComboAttackCount", 2);
            }
            else
            {
                // Reset attack state
                ResetAttackState();
            }
        }
        else if (anim.GetCurrentAnimatorStateInfo(1).IsName("Attack2"))
        {
            Debug.Log("Current state : Combo 2");

            if (CountAttackClick > 2)
            {
                // If multiple click, play the next attack state
                anim.SetInteger("ComboAttackCount", 3);
            }
            else
            {
                // Reset attack state
                ResetAttackState();
            }
        }
        else if (anim.GetCurrentAnimatorStateInfo(1).IsName("Attack3"))
        {
            Debug.Log("Current state : Combo 3");

            if (CountAttackClick >= 3)
                ResetAttackState();
        }
    }

    private void ResetAttackState()
    {
        CountAttackClick = 0;

        anim.SetInteger("ComboAttackCount", 0);

        Debug.Log("Current state : Combo 0");
    }

    #endregion

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