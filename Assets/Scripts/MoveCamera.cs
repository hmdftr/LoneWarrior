using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    public Transform cameraPositiion;

    
    // Update is called once per frame
    void Update()
    {
        transform.position = cameraPositiion.position;
    }
}
