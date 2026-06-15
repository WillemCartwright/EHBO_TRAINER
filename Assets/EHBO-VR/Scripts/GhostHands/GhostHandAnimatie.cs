using System.Collections;
using UnityEngine;

public class GhostHandAnimatie : MonoBehaviour
{
    [Header("Animatie Settings")]
    [SerializeField] private Animator mijnAnimator;
    [SerializeField] private string animationTriggerName = "PlayAnim";
    
    [Tooltip("Hoeveel seconden duurt jouw animatie écht?")]
    [SerializeField] private float handmatigeAnimatieDuur = 15.0f; 

    [Header("Afronding Settings")]
    [SerializeField] private string taskToComplete = "Voer 30 borstcompressies uit met een snelheid van 2 compressies per seconde";
    [SerializeField] private NPCInteraction bystanderNPC;

    // --- DE CRUCIALE SCHAKELAAR DIE JE NOG MISTE ---
    [HideInInspector] public bool isHerhalingsStap = false;

    public void StartDeAnimatieEnRondAf()
    {
        StartCoroutine(SpeelAnimatieRoutine());
    }

    private IEnumerator SpeelAnimatieRoutine()
    {
        Debug.Log("<color=yellow>[GHOST HANDS] Opdracht ontvangen! Animatie start...</color>");

        if (mijnAnimator != null)
        {
            mijnAnimator.SetTrigger(animationTriggerName);
        }
        else
        {
            Debug.LogWarning("Geen Animator gekoppeld aan GhostHandAnimatie!");
        }

        Debug.Log($"<color=yellow>[GHOST HANDS] Handen pompen nu voor {handmatigeAnimatieDuur} seconden...</color>");
        yield return new WaitForSeconds(handmatigeAnimatieDuur);

        Debug.Log("<color=green>[GHOST HANDS] Animatietijd voorbij. Checker updaten!</color>");
        
        // --- DE DEFINITIEVE CLIPBOARD FIX ---
        if (EHBOStappenChecker.Instance != null)
        {
            if (isHerhalingsStap)
            {
                // Vink NU de herhaalstap af op het klembord (dit triggert daarna automatisch de ambulance-case!)
                EHBOStappenChecker.Instance.RegisterStep("Herhaal borstcompressies");
                Debug.Log("<color=green>[GHOST HANDS]</color> Herhaal borstcompressies succesvol AFGEVINKT op klembord!");
            }
            else
            {
                // Ronde 1 (vinkt de eerste lange reanimatiestap af)
                EHBOStappenChecker.Instance.RegisterStep(taskToComplete);
                Debug.Log("<color=green>[GHOST HANDS]</color> Ronde 1 succesvol AFGEVINKT op klembord!");

                if (bystanderNPC != null)
                {
                    bystanderNPC.ResetForPhoneCall();
                }
            }
        }
        
        // De handen weer uitzetten na de reanimatie
        this.gameObject.SetActive(false); 
    }
}