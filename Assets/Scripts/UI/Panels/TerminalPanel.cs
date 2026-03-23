using UnityEngine;
using UnityEngine.UI;

public class TerminalPanel : MonoBehaviour
{
    [SerializeField] private Button missionButton;
    [SerializeField] private Button manualsButton;
    [SerializeField] private Button exitButton;

    private TerminalUIController uiController;

    private void Awake() => uiController = GetComponent<TerminalUIController>();

    private void Start()
    {
        missionButton?.onClick.AddListener(uiController.ShowMission);
        manualsButton?.onClick.AddListener(uiController.ShowManuals);
        exitButton?.onClick.AddListener(GameManager.Instance.CloseTerminal);
    }

    private void OnDestroy()
    {
        missionButton?.onClick.RemoveAllListeners();
        manualsButton?.onClick.RemoveAllListeners();
        exitButton?.onClick.RemoveAllListeners();
    }
}