using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateBodyByJoystick : MonoBehaviour
{
    public enum View { Two_D, Three_D };
    public View viewMode;

    public Joystick joystick;

    [Range(0, 100)] public float smoothness = 10f;


    private void Update()
    {
        transform.Rotate(Vector3.up * (joystick.horizontal * smoothness * Time.deltaTime));
    }
}