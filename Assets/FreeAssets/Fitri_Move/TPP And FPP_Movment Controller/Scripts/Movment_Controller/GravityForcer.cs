using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityForcer : MonoBehaviour
{
    public float Force = 5;

    RaycastHit hit;


    private void Update()
    {
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, 3f)){
            transform.position = new Vector3(transform.position.x, transform.position.y-Force * Time.deltaTime, transform.position.z);
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.down), Color.green);
        }
        else{
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.down), Color.red);
        }
    }
}