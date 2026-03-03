using UnityEngine;

public class StaticBackground : MonoBehaviour
{
    private Vector3 initialLocalPosition;

    private void Start()
    {
        initialLocalPosition = new Vector3(0, 0, 10);
    }

    private void LateUpdate()
    {
        transform.localPosition = initialLocalPosition;
    }
}