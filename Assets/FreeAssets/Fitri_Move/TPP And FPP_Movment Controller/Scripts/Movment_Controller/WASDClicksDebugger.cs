using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WASDClicksDebugger : MonoBehaviour
{
    public Button W, A, S, D;
    public Button Space;

    public RectTransform DesktopKeysDebugger;


    private void Update()
    {
        if (InputSystemYT.inputsSystem){
            DesktopKeysDebugger.gameObject.SetActive(InputSystemYT.inputsSystem.controllerType == InputSystemYT.ControllerType.Desktop ? true : false);
        }

        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        W.interactable = (z > 0) ? false : true;
        S.interactable = (z < 0) ? false : true;
        A.interactable = (x < 0) ? false : true;
        D.interactable = (x > 0) ? false : true;

        Space.interactable = !(Input.GetKey(KeyCode.Space));
    }
}