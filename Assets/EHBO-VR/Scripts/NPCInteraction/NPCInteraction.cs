using UnityEngine;
using System.Collections;

public class NPCInteraction : MonoBehaviour
{
    private Outline outline;
    private Animator animator;
    private bool hasBeenAddressed = false;
    private bool arrivalShown = false;

    [Header("Phone Settings")]
    [SerializeField] private GameObject phone; // Sleep hier je 'phone' model in via de Inspector

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

        // Zorg dat de telefoon onzichtbaar is bij de start van de game
        if (phone != null)
        {
            phone.SetActive(false);
        }
    }

    public void EnableOutlineCapability()
    {
        canShowOutline = true;
    }

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

    // --- HOVER LOGICA ---
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

    // --- SELECT LOGICA ---
    public void AddressNPC()
    {
        if (UIManager.Instance != null && !UIManager.Instance.CanInteract()) return;
        if (hasBeenAddressed || !canShowOutline) return;
        
        hasBeenAddressed = true;

        if (outline != null) 
        {
            outline.enabled = false;
        }

        if (EHBOStappenChecker.Instance != null)
        {
            string currentStep = EHBOStappenChecker.Instance.GetCurrentStep();

            if (currentStep == "112 Bellen")
            {
                // --- TWEEDE INTERACTIE: Telefoon activeren ---
                if (animator != null)
                {
                    animator.SetTrigger("startPhoneCall");
                }

                if (phone != null)
                {
                    phone.SetActive(true);
                }

                // FIX: Start de timer in de EHBOStappenChecker script!
                EHBOStappenChecker.Instance.StartPhoneTimer();
            }
            else
            {
                // EERSTE INTERACTIE: Kennismaking
                EHBOStappenChecker.Instance.RegisterStep("Omstanders Aangesproken");
            }
        }
    }
}