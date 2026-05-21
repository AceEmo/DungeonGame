using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class StandardInputProvider : MonoBehaviour, IInputProvider
{
    public float GetAxisRaw(string axisName)
    {
        if (Keyboard.current == null)
        {
            return 0f;
        }

        return axisName switch
        {
            "Horizontal" => GetKeyAxis(Keyboard.current.dKey, Keyboard.current.aKey),
            "Vertical" => GetKeyAxis(Keyboard.current.wKey, Keyboard.current.sKey),
            "HorizontalArrows" => GetKeyAxis(Keyboard.current.rightArrowKey, Keyboard.current.leftArrowKey),
            "VerticalArrows" => GetKeyAxis(Keyboard.current.upArrowKey, Keyboard.current.downArrowKey),
            _ => LogUnsupportedAxis(axisName)
        };
    }

    public bool GetButtonDown(string buttonName)
    {
        if (Keyboard.current == null)
        {
            return false;
        }

        return buttonName switch
        {
            "Interact" => Keyboard.current.eKey.wasPressedThisFrame,
            "Map" => Keyboard.current.tabKey.wasPressedThisFrame,
            _ => false
        };
    }

    public bool GetButton(string buttonName)
    {
        if (Keyboard.current == null)
        {
            return false;
        }

        return buttonName switch
        {
            "Interact" => Keyboard.current.eKey.isPressed,
            "Map" => Keyboard.current.tabKey.isPressed,
            _ => false
        };
    }

    private float GetKeyAxis(ButtonControl positiveKey, ButtonControl negativeKey)
    {
        if (positiveKey.isPressed) return 1f;
        if (negativeKey.isPressed) return -1f;
        return 0f;
    }

    private float LogUnsupportedAxis(string axisName)
    {
        Debug.LogWarning($"Axis '{axisName}' is not supported.");
        return 0f;
    }
}