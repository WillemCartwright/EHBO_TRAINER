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
    // DIT GAAT ONS VERTELLEN OF DE SCHAKELAAR GOED STAAT:
    Debug.Log($"<color=orange>[CHECK]</color> Animatie start. Is dit de herhalingsstap? ANTWOORD: {isHerhalingsStap}");
    
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
                Debug.Log("<color=green>[GHOST HANDS]</color> Herhaal borstcompressies succesvol AFGEVINKT op klembord!");
                // Meld de stap aan bij de checker
                EHBOStappenChecker.Instance.RegisterStep("Herhaal borstcompressies");
            }
            else
            {
                // Ronde 1 (vinkt de eerste lange reanimatiestap af)
                Debug.Log("<color=green>[GHOST HANDS]</color> Ronde 1 succesvol AFGEVINKT op klembord!");
                EHBOStappenChecker.Instance.RegisterStep(taskToComplete);

                if (bystanderNPC != null)
                {
                    bystanderNPC.ResetForPhoneCall();
                }
            }
        }
        
        // VEILIGHEID: Wacht 1 frame zodat EHBOStappenChecker de switch-case en de deactivatie 
        // van de zones rustig kan afronden voordat dit object hard op onactief wordt gezet.
        yield return null;

        // Schakel daarna pas de mesh of het object uit
        this.gameObject.SetActive(false); 
    }
}