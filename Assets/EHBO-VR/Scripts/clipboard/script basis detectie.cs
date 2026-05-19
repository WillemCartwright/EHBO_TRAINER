using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scriptbasisdetectie : MonoBehaviour
{
    [SerializeField] private float requiredDuration = 4.0f;
    
    [Header("Koppeling met Handen")]
    [Tooltip("Sleep hier de Ghost Hands in waar je animatie-script op staat")]
    [SerializeField] private GhostHandAnimatie ghostHandScript; 

    [Header("Afronding Settings (Voor als er GEEN animatie is)")]
    [SerializeField] private string taskToComplete;
    [SerializeField] private NPCInteraction bystanderNPC; 

    [Header("Trigger Settings")]
    [SerializeField] private List<GameObject> objectsToDeActivateOnEnter;
    [SerializeField] private List<GameObject> objectsToActivateOnExit;

    private float elapsedActionTime = 0.0f;
    private bool isCountingActionTime = false;
    private bool isTaskFinished = false; 

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("GameController"))
        {
            if (isTaskFinished) return;

            isCountingActionTime = true;
            Debug.Log("<color=cyan>[ZONE]</color> GameController gedetecteerd! Timer loopt...");

            foreach (GameObject obj in objectsToDeActivateOnEnter)
                if (obj != null) obj.SetActive(false);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("GameController"))
        {
            isCountingActionTime = false;
            elapsedActionTime = 0.0f;
            Debug.Log("<color=cyan>[ZONE]</color> GameController heeft de zone verlaten. Timer gereset.");

            if (!isTaskFinished)
            {
                foreach (GameObject obj in objectsToActivateOnExit)
                    if (obj != null) obj.SetActive(true);
            }
        }
    }

    void Update()
    {
        if (isCountingActionTime && !isTaskFinished)
        {
            elapsedActionTime += Time.deltaTime;

            if (elapsedActionTime >= requiredDuration)
            {
                TriggerAnimatieOfRondAf();
            }
        }
    }

    private void TriggerAnimatieOfRondAf()
    {
        isTaskFinished = true;
        isCountingActionTime = false;
        
        if (ghostHandScript != null)
        {
            // SITUATIE 1: Er is een animatie gekoppeld (bijv. Hartcompressie).
            // Zet het object waar het script op zit eerst weer AAN!
            ghostHandScript.gameObject.SetActive(true);
            
            Debug.Log("<color=green>[ZONE]</color> Timer gehaald! Handen geactiveerd, seintje sturen naar de animatie...");
            ghostHandScript.StartDeAnimatieEnRondAf();
        }
        else
        {
            // SITUATIE 2: Vakje is LEEG (bijv. Bewustzijn Check). We ronden direct zelf af!
            Debug.Log("<color=green>[ZONE]</color> Geen animatie gekoppeld. Taak direct afronden!");
            RondTaakDirectAf();
        }

        // Zet de zone uit, zijn werk zit erop
        this.gameObject.SetActive(false);
    }

    private void RondTaakDirectAf()
    {
        if (EHBOStappenChecker.Instance != null)
        {
            EHBOStappenChecker.Instance.RegisterStep(taskToComplete);
        }

        if (bystanderNPC != null)
        {
            bystanderNPC.ResetForPhoneCall();
        }
    }
}