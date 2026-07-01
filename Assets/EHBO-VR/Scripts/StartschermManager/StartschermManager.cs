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
    [Tooltip("Sleep hier het TimerParkscene GameObject in")]
    [SerializeField] private TimerParkscene parkTimer;

    [Header("Locomotor Instellingen")]
    [Tooltip("Sleep hier het GameObject in WAAR de FirstPersonLocomotor op staat")]
    [SerializeField] private GameObject locomotorObject;

    private Component locomotorComponent;
    private System.Reflection.FieldInfo speedField;
    private System.Reflection.PropertyInfo speedProperty;
    private float origineleSnelheid = 45f; // Komt overeen met jouw foto (Speed Factor: 45)
    private bool initialized = false;

    void Start()
    {
        if (startschermCanvas != null)
        {
            startschermCanvas.SetActive(true);
        }

        if (parkTimer != null)
        {
            parkTimer.enabled = false;
        }

        // We initialiseren de snelheids-bevriezing direct bij Start
        Invoke("FreezeSpeedAtStart", 0.1f); // Heel klein uitstel zodat Meta de boel eerst kan opstarten
    }

    private void FreezeSpeedAtStart()
    {
        SetupLocomotorReflection();
        SetLocomotorSpeed(0f); // Zet loopsnelheid op 0 (hoogte blijft werken!)
    }

    public void KnopStartScenario()
    {
        Debug.Log("<color=cyan>[MENU]</color> Scenario gestart!");

        if (startschermCanvas != null)
        {
            startschermCanvas.SetActive(false);
        }

        if (parkTimer != null)
        {
            parkTimer.enabled = true;
            parkTimer.StartRealTimer(); 
        }

        // Herstel de loopsnelheid naar de originele 45!
        SetLocomotorSpeed(origineleSnelheid);
    }

    public void KnopGaNaarTutorial()
    {
        SceneManager.LoadScene(tutorialSceneNaam);
    }

    // Zoekt via reflectie naar de 'SpeedFactor' variabele op jouw locomotor
    private void SetupLocomotorReflection()
    {
        if (initialized || locomotorObject == null) return;

        // Zoek naar de FirstPersonLocomotor component op het object
        Component[] components = locomotorObject.GetComponents<Component>();
        foreach (Component c in components)
        {
            if (c != null && c.GetType().Name.Contains("Locomotor"))
            {
                locomotorComponent = c;
                
                // We zoeken naar 'SpeedFactor' (zoals op jouw foto te zien is)
                speedField = c.GetType().GetField("SpeedFactor", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (speedField == null) speedField = c.GetType().GetField("_speedFactor", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                
                if (speedField != null)
                {
                    origineleSnelheid = (float)speedField.GetValue(locomotorComponent);
                }
                
                initialized = true;
                break;
            }
        }
    }

    private void SetLocomotorSpeed(float targetSpeed)
    {
        if (locomotorComponent != null && speedField != null)
        {
            speedField.SetValue(locomotorComponent, targetSpeed);
            Debug.Log($"[MENU] Locomotor Speed Factor aangepast naar: {targetSpeed}");
        }
        else
        {
            // Mocht de automatische code de variabele niet kunnen vinden, 
            // dan kun je dit object alsnog deactiveren als noodgreep:
            if (locomotorObject != null && targetSpeed == 0f) 
            {
                // Als back-up (als de code hierboven faalt), maar we proberen dit te vermijden ivm het vallen
                // locomotorObject.SetActive(false); 
            }
        }
    }
}