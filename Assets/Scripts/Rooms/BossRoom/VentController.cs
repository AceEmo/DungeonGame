using UnityEngine;

public class VentController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject closedVent;
    [SerializeField] private GameObject openVent;

    private void Start()
    {
        SetVentState(isClosed: true);
    }
    
    public void OpenVent()
    {
        SetVentState(isClosed: false);
    }

    private void SetVentState(bool isClosed)
    {
        if (closedVent != null)
        {
            closedVent.SetActive(isClosed);
        }

        if (openVent != null)
        {
            openVent.SetActive(!isClosed);
        }
    }
}
