using UnityEngine;

public class PersistentMainCamera : MonoBehaviour
{
    private void Awake()
    {
        var cameras = Object.FindObjectsByType<Camera>(FindObjectsSortMode.None);

        foreach (var cam in cameras)
        {
            if (cam != GetComponent<Camera>() && cam.CompareTag("MainCamera"))
            {
                Destroy(gameObject);
                return;
            }
        }

        DontDestroyOnLoad(gameObject);
    }
}
