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
                EHBOStappenChecker.Instance.RegisterStep("Het slachtoffer is bewusteloos. Laat de omstander 112 voor je bellen");
                StartCalling112();
            }
            else if (currentStep == "Start Incident" || string.IsNullOrEmpty(currentStep))
            {
                hasBeenAddressed = true;
                if (outline != null) outline.enabled = false;
                EHBOStappenChecker.Instance.RegisterStep("Tik de omstander aan zodat hij in de buurt blijft");
            }
        }
    }

    public void StartCalling112()
    {
        // Haal de animator en het bewegingsscript op
        Animator npcAnimator = GetComponent<Animator>();
        NPCMovement movementScript = GetComponent<NPCMovement>();

        // FORCEER DE ANIMATOR: Zet de snelheid op 0 zodat hij stopt met de Blend Tree,
        // en activeer de belfunctie.
        if (npcAnimator != null)
        {
            npcAnimator.SetFloat("Speed", 0f);
            npcAnimator.SetTrigger("startPhoneCall");
            Debug.Log("[INTERACTION] Trigger 'startPhoneCall' verzonden naar Animator.");
        }

        if (phone != null)
        {
            phone.SetActive(true);
        }

        // Start de timer. Na 7 seconden bellen stopt de animatie en rent hij weg!
        // (Verander de 7f gerust naar de lengte van jouw specifieke bel-animatie)
        StartCoroutine(WachtTijdensBellenEnGaRennen(7f));
    }

    private IEnumerator WachtTijdensBellenEnGaRennen(float belDuur)
    {
        yield return new WaitForSeconds(belDuur);

        // Doe de telefoon weer in de zak
        if (phone != null) phone.SetActive(false);

        // Geef het seintje aan het NPCMovement script dat we zojuist hebben aangepast!
        NPCMovement movementScript = GetComponent<NPCMovement>();
        if (movementScript != null)
        {
            movementScript.StartRennenNaarAED();
        }
        else
        {
            Debug.LogError("[NPC] NPCMovement component niet gevonden op dit object!");
        }
    }
}