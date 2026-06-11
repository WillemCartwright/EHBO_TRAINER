using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scriptbasisdetectie : MonoBehaviour
{
    [SerializeField] private float requiredDuration = 4.0f; // 4 seconden wachten
    
    [Header("Koppeling met Handen")]
    [Tooltip("Sleep hier de Ghost Hands in waar je animatie-script op staat")]
    [SerializeField] private GhostHandAnimatie ghostHandScript; 

    // --- NIEUWE VARIABELEN VOOR DE HAND SHADERS ---
    [Tooltip("Sleep hier de child-mesh (Renderer) van de LINKER ghost hand in")]
    [SerializeField] private Renderer linkeHandVanDezeStap;
    [Tooltip("Sleep hier de child-mesh (Renderer) van de RECHTER ghost hand in")]
    [SerializeField] private Renderer rechterHandVanDezeStap;

    [Header("Afronding Settings (Alleen voor zones ZONDER animatie)")]
    [SerializeField] private string taskToComplete;

    private float elapsedActionTime = 0.0f;
    private bool isCountingActionTime = false;
    private bool isTaskFinished = false; 

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("GameController"))
        {
            if (isTaskFinished) return;
            isCountingActionTime = true;
            Debug.Log("<color=cyan>[ZONE]</color> Handen gedetecteerd! Timer loopt...");

            // --- DE AANPASSING HIER ---
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
        // --- TIJDELIJKE TEST MET SPATIEBALK (OMZEIL DE VR CONTROLLER) ---
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isTaskFinished) return;
            isCountingActionTime = true;
            Debug.Log("<color=orange>[TEST]</color> Spatiebalk ingedrukt! Handmatig de balk gestart.");

            if (ProgressBarUI.Instance != null)
            {
                ProgressBarUI.Instance.StartProgressBar(requiredDuration, linkeHandVanDezeStap, rechterHandVanDezeStap);
            }
        }
        // -----------------------------------------------------------------

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
        
        // --- TIP: Als de shader dadelijk werkt, kun je deze weer aanzetten ---
        if (ProgressBarUI.Instance != null)
        {
            ProgressBarUI.Instance.StopProgressBar();
        }

        if (ghostHandScript != null)
        {
            ghostHandScript.gameObject.SetActive(true);
            ghostHandScript.StartDeAnimatieEnRondAf();
            Debug.Log("<color=green>[ZONE]</color> 4 seconden gehaald! Handen geactiveerd, animatie start nu.");
        }
        else
        {
            Debug.Log("<color=green>[ZONE]</color> 4 seconden gehaald! Geen animatie, direct afronden.");
            if (EHBOStappenChecker.Instance != null)
            {
                EHBOStappenChecker.Instance.RegisterStep(taskToComplete);
            }
        }

        this.gameObject.SetActive(false);
    }
}