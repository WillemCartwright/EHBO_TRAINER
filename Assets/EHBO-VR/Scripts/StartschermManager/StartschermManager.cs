using UnityEngine;
using UnityEngine.SceneManagement;

public class StartschermManager : MonoBehaviour
{
    [Header("UI Elementen")]
    [Tooltip("Sleep hier het hele Startscherm Canvas object in")]
    [SerializeField] private GameObject startschermCanvas;

    [Header("Scene Namen")]
    [Tooltip("De EXACTE naam van je tutorial scene in de Build Settings")]
    [SerializeField] private string tutorialSceneNaam = "TutorialScene";

    [Header("Timer / Start Referentie")]
    [Tooltip("Sleep hier het TimerParkscene GameObject in (zodat we de timer pas starten als de speler op Start klikt)")]
    [SerializeField] private TimerParkscene parkTimer;

    void Start()
    {
        // Het startscherm moet bij het opstarten natuurlijk ALTIJD zichtbaar zijn!
        if (startschermCanvas != null)
        {
            startschermCanvas.SetActive(true);
        }

        // Zorg ervoor dat de reanimatietimer nog even NIET gaat lopen op de achtergrond
        // We zetten de component even uit totdat de speler op 'Start' drukt.
        if (parkTimer != null)
        {
            parkTimer.enabled = false;
        }
    }

    // --- GEKOPPELD AAN KNOP 1: Start de reanimatie ---
    public void KnopStartScenario()
    {
        Debug.Log("<color=cyan>[MENU]</color> Scenario gestart!");

        // 1. Verberg het startscherm direct
        if (startschermCanvas != null)
        {
            startschermCanvas.SetActive(false);
        }

        // 2. Zet de timer-component aan en trigger de echte aftelling
        if (parkTimer != null)
        {
            parkTimer.enabled = true;
            parkTimer.StartRealTimer(); 
        }
    }

    // --- GEKOPPELD AAN KNOP 2: Ga naar de tutorial scene ---
    public void KnopGaNaarTutorial()
    {
        Debug.Log("<color=cyan>[MENU]</color> Laden van de tutorial scene: " + tutorialSceneNaam);
        
        // Laad de tutorial scene in
        SceneManager.LoadScene(tutorialSceneNaam);
    }
}