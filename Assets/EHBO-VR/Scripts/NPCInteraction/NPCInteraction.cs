using UnityEngine;
using System.Collections;

public class NPCInteraction : MonoBehaviour
{
    private Outline outline;
    private Animator animator;
    private bool hasBeenAddressed = false;
    private bool arrivalShown = false;

    [Header("Phone Settings")]
    [SerializeField] private GameObject phone; 

    private bool canShowOutline = false; 

    void Awake()
    {
        outline = GetComponent<Outline>();
        animator = GetComponent<Animator>();

        if (outline != null) 
        {
            outline.enabled = false;
        }

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
    if (hasBeenAddressed || !canShowOutline) return;

    if (EHBOStappenChecker.Instance != null)
    {
        string currentStep = EHBOStappenChecker.Instance.GetCurrentStep();

        // Als we net klaar zijn met Omstanders, is de volgende logische klik voor 112
        if (currentStep == "Bewustzijn Check") 
        {
            hasBeenAddressed = true;
            if (outline != null) outline.enabled = false;

            // We vertellen de checker direct: de 112 stap is nu gedaan!
            EHBOStappenChecker.Instance.RegisterStep("112 Bellen");
            StartCalling112();
        }
        else if (currentStep == "Start Incident" || string.IsNullOrEmpty(currentStep))
        {
            hasBeenAddressed = true;
            if (outline != null) outline.enabled = false;
            EHBOStappenChecker.Instance.RegisterStep("Omstanders Aangesproken");
        }
    }
}

    public void StartCalling112()
    {
        if (animator != null)
        {
            animator.SetTrigger("startPhoneCall");
        }

        if (phone != null)
        {
            phone.SetActive(true);
        }
    }
}