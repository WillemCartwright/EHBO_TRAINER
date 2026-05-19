using System.Collections;
using UnityEngine;

public class GhostHandAnimatie : MonoBehaviour
{
    [Header("Animatie Settings")]
    [SerializeField] private Animator mijnAnimator;
    [SerializeField] private string animationTriggerName = "PlayAnim";

    [Header("Afronding Settings")]
    [SerializeField] private string taskToComplete = "Hart Compressie";
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
            // 1. Start de animatie
            mijnAnimator.SetTrigger(animationTriggerName);
            
            // 2. Wacht 1 frame zodat Unity de overgang kan verwerken
            yield return null; 
            
            // 3. Vraag aan Unity hoe lang deze specifieke animatie duurt
            float exacteAnimatieTijd = mijnAnimator.GetCurrentAnimatorStateInfo(0).length;
            Debug.Log($"<color=yellow>[GHOST HANDS] Animatie duurt exact {exacteAnimatieTijd} seconden. Wachten...</color>");
            
            // 4. Wacht precies de lengte van de animatie
            yield return new WaitForSeconds(exacteAnimatieTijd);
        }
        else
        {
            Debug.LogWarning("Geen Animator gekoppeld aan GhostHandAnimatie!");
            yield return new WaitForSeconds(5.0f); // Fallback
        }

        // 5. Taak afronden!
        Debug.Log("<color=green>[GHOST HANDS] Animatie is klaar, Checker updaten!</color>");
        
        if (EHBOStappenChecker.Instance != null)
        {
            EHBOStappenChecker.Instance.RegisterStep(taskToComplete);
        }

        if (bystanderNPC != null)
        {
            bystanderNPC.ResetForPhoneCall();
        }
        
        // Optioneel: zet de handen na de hele actie uit zodat ze niet in beeld blijven hangen
        this.gameObject.SetActive(false); 
    }
}