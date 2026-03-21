using UnityEngine;
public class StandardInputProvider : MonoBehaviour, IInputProvider
{
    public float GetAxisRaw(string axisName)
    {
        return Input.GetAxisRaw(axisName);
    }
}