using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrouchController : MonoBehaviour
{
    public int l;

    public static CrouchController crouchController;

    private CharacterController controller;

    public Transform camHolder;
    public Transform ModelHolder;
    public Behaviour Stablizer, CharacterConstantYAxis;

    public bool isCrouch = false;

    public KeyCode CrouchKey = KeyCode.C;

    public float CrouchHeight = 0.5f;
    public float CrouchSpeed = 2f;

    [HideInInspector] public float StartHeight = 0.0f;
    private float CrouchAndProneDelayTimer = 0.0f;

    
    private void Awake()
    {
        if (crouchController == null)
            crouchController = this;
    }
    
    private void Start()
    {
        controller = GetComponent<CharacterController>();
        StartHeight = controller.height;
    }

    private void Update()
    {
        if (isCrouch){
            ControllerHeight(CrouchHeight, 15f);
        }
        else{
            ControllerHeight(StartHeight, 7f);
        }

        InputKeys();
        CanStandUp();

        // Stablizing Character_Y Axis While Crouching Or Proning
        if (isCrouch){
            if (Stablizer != null){
                if (Stablizer.GetComponent<GravityForcer>()){
                    Stablizer.GetComponent<GravityForcer>().enabled = true;
                }
                if (Stablizer.transform.position.y <= transform.position.y){
                    if (CharacterConstantYAxis != null){
                        CharacterConstantYAxis.enabled = true;
                    }
                }
            }
        }
        else{
            if (Stablizer != null){
                Stablizer.GetComponent<GravityForcer>().enabled = false;
            }
            if (CharacterConstantYAxis != null){
                CharacterConstantYAxis.enabled = false;
            }
            
            if (ModelHolder != null){
                ModelHolder.transform.localPosition = new Vector3(ModelHolder.localPosition.x, Mathf.Lerp(ModelHolder.localPosition.y, -1, 8f * Time.deltaTime), ModelHolder.localPosition.z);
            }
        }
    }

    private void ControllerHeight(float _height, float _smoothness)
    {
        if (this.enabled)
            controller.height = Mathf.Lerp(controller.height, _height, _smoothness * Time.deltaTime);
    }

    private void InputKeys()
    {
        if (GroundFar.groundFar.FarFromGround){
            isCrouch = false;
            return;
        }

        if (Input.GetKeyDown(CrouchKey) && Time.time > CrouchAndProneDelayTimer){
            if (!controller.isGrounded){ return; }
            if (!CanStandUp() && isCrouch){
                return;
            }

            CrouchAndProneDelayTimer = Time.time + 0.2f;
            if (isCrouch==false){
                isCrouch = true;
            }
            else{
                isCrouch = false;
            }
        }
    }

    private bool CanStandUp()
    {
        Collider[] cols = (Physics.OverlapSphere(transform.position + new Vector3(0, 1.9f, 0), 0.6f));
        l = cols.Length;
        return (cols.Length == 0) ? true : false;
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + new Vector3(0, 1.9f, 0), 0.6f);
    }
}