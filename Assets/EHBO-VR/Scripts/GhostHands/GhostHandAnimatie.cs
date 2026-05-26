using System.Collections;
using UnityEngine;

public class GhostHandAnimatie : MonoBehaviour
{
    [Header("Animatie Settings")]
    [SerializeField] private Animator mijnAnimator;
    [SerializeField] private string animationTriggerName = "PlayAnim";
    
    [Tooltip("Hoeveel seconden duurt jouw animatie écht?")]
    [SerializeField] private float handmatigeAnimatieDuur = 10.0f; // <-- HIER STAAT HIJ NU OP 10!

    [Header("Afronding Settings")]
    [SerializeField] private string taskToComplete = "Voer 30 borstcompressies uit met een snelheid van 2 compressies per seconde";
    [SerializeField] private NPCInteraction bystanderNPC;

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

        // We negeren wat Unity denkt, we wachten nu ECHT 10 seconden!
        Debug.Log($"<color=yellow>[GHOST HANDS] Handen pompen nu voor {handmatigeAnimatieDuur} seconden...</color>");
        yield return new WaitForSeconds(handmatigeAnimatieDuur);

        // Pas na 10 seconden de taak afronden
        Debug.Log("<color=green>[GHOST HANDS] 10 seconden voorbij. Animatie klaar, Checker updaten!</color>");
        
        if (EHBOStappenChecker.Instance != null)
        {
            EHBOStappenChecker.Instance.RegisterStep(taskToComplete);
        }

        if (bystanderNPC != null)
        {
            bystanderNPC.ResetForPhoneCall();
        }
        
        // De handen weer uitzetten na de reanimatie
        this.gameObject.SetActive(false); 
    }
}  