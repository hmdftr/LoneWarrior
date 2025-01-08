using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwitchWeapon : MonoBehaviour
{
    [Header("Switch weapon")]
    public float mouseScrollValue;
    public GameObject playerWeapon;
    public GameObject weaponImage;
    public bool weaponActive;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        #region Switch weapon and Combo attack

        if (Input.GetAxis("Mouse ScrollWheel") > 0f && mouseScrollValue < 1)
            mouseScrollValue++;

        else if (Input.GetAxis("Mouse ScrollWheel") < 0f && mouseScrollValue > 0)
        {
            mouseScrollValue--;

            if (mouseScrollValue == 0) { }
        }

        if (mouseScrollValue == 1)
        {
            weaponActive = true;
            playerWeapon.SetActive(true);
            weaponImage.SetActive(true);

            //Debug.Log("Weapon activate");
        }
        else
        {
            weaponActive = false;
            playerWeapon.SetActive(false);
            weaponImage.SetActive(false);
        }

        #endregion
    }
}
