using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstantPositionLate : MonoBehaviour
{
    public bool x, y, z;

    public Transform target;


    private void LateUpdate()
    {
        if (x){
            transform.position = new Vector3(target.position.x, transform.position.y, transform.position.z);
        }
        if (y){
            transform.position = new Vector3(transform.position.x, target.position.y, transform.position.z);
        }
        if (z){
            transform.position = new Vector3(transform.position.x, transform.position.y, target.position.z);
        }
    }
}