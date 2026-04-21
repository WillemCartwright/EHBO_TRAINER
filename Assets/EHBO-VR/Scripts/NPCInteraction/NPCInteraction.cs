using UnityEngine;
using System.Collections;

public class NPCInteraction : MonoBehaviour
{
    private Outline outline;
    private Animator animator;
    private bool hasBeenAddressed = false;
    private bool arrivalShown = false;
    
    // Bepaalt of de NPC interactief mag zijn
    private bool canShowOutline = false; 

    void Awake()
    {
        outline = GetComponent<Outline>();
        animator = GetComponent<Animator>();

        if (outline != null) 
        {
            outline.enabled = false;
        }
    }

    /// <summary>
    /// Maakt de NPC de allereerste keer klikbaar (bij aankomst bij het slachtoffer).
    /// </summary>
    public void EnableOutlineCapability()
    {
        canShowOutline = true;
    }

    /// <summary>
    /// Wordt aangeroepen door de EHBOStappenChecker in de fase "112 Bellen".
    /// Hiermee maken we de NPC opnieuw klikbaar voor de tweede interactie.
    /// </summary>
    public void ResetForPhoneCall()
    {
        hasBeenAddressed = false; 
        canShowOutline = true; 
        Debug.Log("NPC gereset: Speler kan nu de opdracht geven om 112 te bellen.");
    }

    public void TriggerArrivalText()
    {
        EnableOutlineCapability();

        if (arrivalShown) return;
        arrivalShown = true;

        if (EHBOStappenChecker.Instance != null)
        {
            EHBOStappenChecker.Instance.RegisterStep("Start Incident");
        }
    }

    // --- HOVER LOGICA (Oculus Interaction SDK) ---
    public void OnHoverEnter()
    {
        if (UIManager.Instance != null && !UIManager.Instance.CanInteract()) return;

        if (outline != null && canShowOutline && !hasBeenAddressed)
        {
            outline.enabled = true;
        }
    }

    public void OnHoverExit()
    {
        if (outline != null)
        {
            outline.enabled = false;
        }
    }

    // --- SELECT LOGICA (Bijv. bij klik/select van de NPC) ---
    public void AddressNPC()
    {
        // 1. Veiligheidscheck: Mag interactie?
        if (UIManager.Instance != null && !UIManager.Instance.CanInteract()) return;

        // 2. Is de NPC al afgehandeld of mag hij nog niet oplichten?
        if (hasBeenAddressed || !canShowOutline) return;
        
        hasBeenAddressed = true;

        if (outline != null) 
        {
            outline.enabled = false;
        }

        // 3. Logica bepalen op basis van de voortgang in de StappenChecker
        if (EHBOStappenChecker.Instance != null)
        {
            // We kijken of we al eens eerder hebben gesproken (was hij al gearriveerd?)
            // Als de cursist nog niet bij "112 Bellen" is, is dit de eerste ontmoeting.
            // Als hij er wel is, is dit de bel-opdracht.
            
            string currentStep = EHBOStappenChecker.Instance.GetCurrentStep();

            if (currentStep == "112 Bellen")
            {
                // TWEEDE INTERACTIE: Opdracht geven
                if (animator != null)
                {
                    animator.SetTrigger("startPhoneCall"); // Zorg dat deze trigger in je Animator zit
                }
                EHBOStappenChecker.Instance.RegisterStep("112 Opdracht Gegeven");
            }
            else
            {
                // EERSTE INTERACTIE: Kennismaking
                EHBOStappenChecker.Instance.RegisterStep("Omstanders Aangesproken");
            }
        }
    }
}