using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DisableOnButton : MonoBehaviour
{
    public void OnKeyPress(InputAction.CallbackContext ctx)
    {
        if (ctx.ReadValueAsButton()) gameObject.SetActive(false);
    }
}
