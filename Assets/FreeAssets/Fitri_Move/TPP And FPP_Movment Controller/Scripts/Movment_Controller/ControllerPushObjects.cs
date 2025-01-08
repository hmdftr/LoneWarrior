using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerPushObjects : MonoBehaviour
{
    public Animator anim;

    [Range(0, 50)] public float pushPower;


    private void Update()
    {
        if (GetComponent<FPPController>() || GetComponent<TPPController>()){
            if (GetComponent<FPPController>()){
                if (GetComponent<FPPController>().enabled){
                    pushPower = GetComponent<FPPController>().MoveSpeed;
                }
            }
            if (GetComponent<TPPController>()){
                if (GetComponent<TPPController>().enabled){
                    pushPower = GetComponent<TPPController>().MoveSpeed;
                }
            }
        }
        
    }

    void OnControllerColliderHit (ControllerColliderHit hit)
    {
        var body = hit.transform.GetComponent<Rigidbody>();

        if (body == null || body.isKinematic){ return; }

        var pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);

        // Apply the push
        body.velocity = pushDir * pushPower;
    }
}