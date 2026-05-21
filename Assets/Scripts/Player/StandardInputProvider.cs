using UnityEngine;
using UnityEngine.InputSystem;

public class StandardInputProvider : MonoBehaviour, IInputProvider
{
    public float GetAxisRaw(string axisName)
    {
        if (Keyboard.current == null)
        {
            return 0f;
        }

        switch (axisName)
        {
            case "Horizontal":
                if (Keyboard.current.dKey.isPressed) return 1f;
                if (Keyboard.current.aKey.isPressed) return -1f;
                return 0f;

            case "Vertical":
                if (Keyboard.current.wKey.isPressed) return 1f;
                if (Keyboard.current.sKey.isPressed) return -1f;
                return 0f;

            case "HorizontalArrows":
                if (Keyboard.current.rightArrowKey.isPressed) return 1f;
                if (Keyboard.current.leftArrowKey.isPressed) return -1f;
                return 0f;

            case "VerticalArrows":
                if (Keyboard.current.upArrowKey.isPressed) return 1f;
                if (Keyboard.current.downArrowKey.isPressed) return -1f;
                return 0f;

            default:
                Debug.LogWarning($"Axis '{axisName}' is not supported.");
                return 0f;
        }
    }

    public bool GetButtonDown(string buttonName)
    {
        if (Keyboard.current == null)
        {
            return false;
        }

        switch (buttonName)
        {
            case "Interact":
                return Keyboard.current.eKey.wasPressedThisFrame;
            default:
                return false;
        }
    }
}