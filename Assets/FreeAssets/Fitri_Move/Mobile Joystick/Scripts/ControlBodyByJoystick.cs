using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlBodyByJoystick : MonoBehaviour
{
    private Rigidbody rb;

    public enum MoveBy { RigidBody, Transform };
    public MoveBy moveBy;

    public enum View { Two_D, Three_D };
    public View viewMode;

    public Joystick joystick;

    [Range(0, 100)] public float speed = 5f;


    private void Update()
    {
        if (moveBy == MoveBy.Transform){
            if (viewMode == View.Two_D)
                transform.position += (new Vector3(joystick.horizontal, joystick.vertical, 0) * speed * Time.deltaTime);
            if (viewMode == View.Three_D)
                transform.position += (new Vector3(joystick.horizontal, 0f, joystick.vertical) * speed * Time.deltaTime);
        }
        else if (moveBy == MoveBy.RigidBody){
            if (rb==null){ rb = GetComponent<Rigidbody>(); }
            if (viewMode == View.Two_D)
                rb.velocity += (new Vector3(joystick.horizontal, joystick.vertical, 0) * speed * Time.deltaTime);
            if (viewMode == View.Three_D)
                rb.velocity += (new Vector3(joystick.horizontal, 0f, joystick.vertical) * speed * Time.deltaTime);
        }
    }
}