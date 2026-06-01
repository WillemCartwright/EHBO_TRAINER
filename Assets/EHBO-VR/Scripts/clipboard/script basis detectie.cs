using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scriptbasisdetectie : MonoBehaviour
{
    [SerializeField] private float requiredDuration = 4.0f; // 4 seconden wachten
    
    [Header("Koppeling met Handen")]
    [Tooltip("Sleep hier de Ghost Hands in waar je animatie-script op staat")]
    [SerializeField] private GhostHandAnimatie ghostHandScript; 

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

            // --- TIJDELIJK UITGEZET VOOR TESTEN VAN DE POSITIE ---
            // if (ProgressBarUI.Instance != null)
            // {
            //     ProgressBarUI.Instance.StopProgressBar();
            // }
        }
    }

    void Update()
    {
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
        
        // --- TIJDELIJK UITGEZET VOOR TESTEN VAN DE POSITIE ---
        // if (ProgressBarUI.Instance != null)
        // {
        //     ProgressBarUI.Instance.StopProgressBar();
        // }

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