using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundFar : MonoBehaviour
{
    public static GroundFar groundFar;

    public bool FarFromGround = false;

    public float detectRange = 7f;
    
    RaycastHit hit;


    void Awake()
    {
        if (groundFar == null)
            groundFar = this;
    }

    void Update()
    {
        FarFromGround = (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, detectRange)) ? false : true;
    }
}