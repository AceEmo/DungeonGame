using UnityEngine;
using TMPro;

public class TerminalUIController : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private TextMeshProUGUI contentText;
    [SerializeField] private GameObject terminalPanel;

    private bool isAtHomeLog = true;

    private string homeLogText = 
        "<color=#50C8FF><b>>>> SYSTEM LOG: STARDATE 2451.03.12 <<<</b></color>\n" +
        "<color=#404040>---------------------------------------</color>\n\n" +
        "<color=#808080>[08:15]</color> <color=#FFCC00>WARNING:</color> Power surge detected in <color=#FFFFFF>Sector B4</color>.\n" +
        "<color=#808080>[08:17]</color> <color=#FF3333>ALERT:</color> Hull breach confirmed. Evacuation initiated.\n" +
        "<color=#808080>[08:30]</color> <color=#50C8FF>STATUS:</color> Personnel evacuated. Sector B4 sealed.\n\n" +
        "<color=#404040>.......................................</color>\n\n" +
        "<color=#808080>[09:12]</color> <color=#FF3333><b>>>> CRITICAL ANOMALY <<<</b></color>\n" +
        "<color=#FF9999>Unidentified biological signature detected within containment area.</color>\n\n" +
        "<color=#404040>---------------------------------------</color>\n" +
        "<color=#50C8FF>SYSTEM IDLE... AWAITING COMMAND_</color>\n" +
        "<size=75%><color=#808080>(Use terminal buttons for Manual Override or Data Retrieval)</color></size>";

    private string missionText = 
        "<color=#FFCC00><b>>>> MISSION PARAMETERS: SECTOR ALPHA <<<</b></color>\n" +
        "<color=#404040>---------------------------------------</color>\n\n" +
        "<color=#FFFFFF>[OBJ-01]</color> Locate and eliminate the <color=#FF3333>SECTOR BOSS</color>.\n" +
        "<color=#808080>Note: Boss elimination grants access to the next large sector.</color>\n\n" +
        "<color=#FFFFFF>[OBJ-02]</color> Accumulate <color=#50C8FF>SCRAP METAL</color> from debris and enemies.\n" +
        "<color=#808080>Use resources for ship-wide upgrades in specialized zones.</color>\n\n" +
        "<color=#404040>.......................................</color>\n\n" +
        "<color=#50C8FF>STRATEGIC ADVISORY:</color>\n" +
        "<color=#FFFFFF>Room clearance is NOT mandatory for deck progression.</color>\n" +
        "<color=#FFFFFF>Direct pathing to the boss is authorized for high-risk pilots.</color>\n\n" +
        "<color=#404040>---------------------------------------</color>\n" +
        "<color=#FFCC00>MISSION STATUS: IN PROGRESS...</color>";

    private string manualsText = 
        "<color=#50C8FF><b>>>> ENGINEERING MANUAL: PILOT INTERFACE <<<</b></color>\n" +
        "<color=#404040>---------------------------------------</color>\n\n" +
        "<color=#FFFFFF>[NAV]</color> Use <color=#50C8FF>WASD Keys</color> for omni-directional movement.\n" +
        "<color=#FFFFFF>[ATK]</color> Use <color=#FF3333>ARROW Keys</color> for multi-vector plasma fire.\n" +
        "<color=#FFFFFF>[ACT]</color> Use <color=#FFCC00>[E] Key</color> for environmental interaction.\n\n" +
        "<color=#404040>.......................................</color>\n\n" +
        "<color=#FF3333><b>HAZARD ALERT: THE GALACTIC ROGUE</b></color>\n" +
        "<color=#FFFFFF>Entity may offer BLACKJACK games in exchange for SCRAP.</color>\n" +
        "<color=#FF9999>Warning: Losing a hand impacts VITAL BIOMETRICS (HP).</color>\n\n" +
        "<color=#404040>---------------------------------------</color>\n" +
        "<color=#50C8FF>HARDWARE STATUS: OPTIMAL</color>";

    private void OnEnable()
    {
        ShowHome();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (isAtHomeLog)
            {
                GameManager.Instance.CloseTerminal();
            }
            else
            {
                ShowHome();
            }
        }
    }

    public void ShowMission()
    {
        isAtHomeLog = false;
        contentText.text = missionText;
    }

    public void ShowManuals()
    {
        isAtHomeLog = false;
        contentText.text = manualsText;
    }

    public void ShowHome()
    {
        isAtHomeLog = true;
        contentText.text = homeLogText;
    }
}