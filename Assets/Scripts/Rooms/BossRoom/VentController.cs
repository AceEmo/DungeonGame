using UnityEngine;

public class VentController : MonoBehaviour
{
    [Header("References")]
    public GameObject closedVent;
    public GameObject openVent;

    private void Start()
    {
        if (closedVent == null || openVent == null)
        {
            return;
        }

        closedVent.SetActive(true);
        openVent.SetActive(false);
    }
    
    public void OpenVent()
    {
        if (closedVent != null && openVent != null)
        {
            closedVent.SetActive(false);
            openVent.SetActive(true);
        }
    }
}