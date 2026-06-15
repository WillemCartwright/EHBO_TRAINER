using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scriptbasisdetectie : MonoBehaviour
{
    [SerializeField] private float requiredDuration = 4.0f; // 4 seconden wachten
    
    [Header("Koppeling met Handen")]
    [Tooltip("Sleep hier de Ghost Hands in waar je animatie-script op staat")]
    [SerializeField] private GhostHandAnimatie ghostHandScript; 

    [Tooltip("Sleep hier de child-mesh (Renderer) van de LINKER ghost hand in")]
    [SerializeField] private Renderer linkeHandVanDezeStap;
    [Tooltip("Sleep hier de child-mesh (Renderer) van de RECHTER ghost hand in")]
    [SerializeField] private Renderer rechterHandVanDezeStap;

    [Header("Afronding Settings")]
    [SerializeField] private string taskToComplete;

    [HideInInspector] public float elapsedActionTime = 0.0f;
    
    [HideInInspector] public bool isCountingActionTime = false;
    [HideInInspector] public bool isTaskFinished = false; 

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("GameController"))
        {
            if (isTaskFinished) return;
            isCountingActionTime = true;
            Debug.Log("<color=cyan>[ZONE]</color> Handen gedetecteerd! Timer loopt...");

            if (ProgressBarUI.Instance != null)
            {
                ProgressBarUI.Instance.StartProgressBar(requiredDuration);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("GameController"))
        {
            isCountingActionTime = false;
            elapsedActionTime = 0.0f;
            Debug.Log("<color=cyan>[ZONE]</color> Handen weggehaald. Timer gereset.");

            if (ProgressBarUI.Instance != null)
            {
                ProgressBarUI.Instance.StopProgressBar();
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isTaskFinished) return;
            isCountingActionTime = true;
            ProgressBarUI.Instance?.StartProgressBar(requiredDuration);
        }

        if (isCountingActionTime && !isTaskFinished)
        {
            elapsedActionTime += Time.deltaTime;

            if (elapsedActionTime >= requiredDuration)
            {
                VerwerkVierSecondenBereikt();
            }
        }
    }

    private void VerwerkVierSecondenBereikt()
    {
        isTaskFinished = true;
        isCountingActionTime = false;
        
        if (ProgressBarUI.Instance != null)
        {
            ProgressBarUI.Instance.StopProgressBar();
        }

        // 1. Als er handen zijn, start de animatie (deze handelt via GhostHandAnimatie ZELF het klembord af)
        if (ghostHandScript != null)
        {
            ghostHandScript.gameObject.SetActive(true);
            ghostHandScript.StartDeAnimatieEnRondAf();
            Debug.Log("<color=green>[ZONE]</color> 4 seconden gehaald! Handen geactiveerd, animatie start.");
        }
        // 2. VOOR ALLES ZONDER ANIMATIE (Zoals de allereerste schouder-klop stap!): Meld direct de taak af!
        else
        {
            Debug.Log("<color=green>[ZONE]</color> 4 seconden gehaald! Geen animatie (vroege stap), direct afronden.");
            if (EHBOStappenChecker.Instance != null)
            {
                EHBOStappenChecker.Instance.RegisterStep(taskToComplete);
            }
        }

        this.gameObject.SetActive(false);
    }
}