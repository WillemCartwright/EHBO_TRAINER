using UnityEngine;

public class AEDInteraction : MonoBehaviour
{
    [Header("Outline Settings")]
    [SerializeField] private MonoBehaviour outlineComponent; 

    [Header("Electrodes in VR Hands")]
    [SerializeField] private GameObject elektrodeLinksInHand;  
    [SerializeField] private GameObject elektrodeRechtsInHand; 

    [Header("Electrodes on Chest (Targets)")]
    [SerializeField] private GameObject elektrodeLinksOpBorst;  
    [SerializeField] private GameObject elektrodeRechtsOpBorst; 

    [Header("Victim Settings (NEW)")]
    // Sleep hier de Animator van het slachtoffer in (de liggende man)
    [SerializeField] private Animator victimAnimator; 

    private bool aedGeactiveerd = false;
    private bool linksGeplakt = false;
    private bool rechtsGeplakt = false;
    private bool scenarioAfgerond = false; // Extra check zodat je niet oneindig kunt blijven klikken

    void Start()
    {
        if (outlineComponent != null) outlineComponent.enabled = false;
        
        if (elektrodeLinksInHand != null) elektrodeLinksInHand.SetActive(false);
        if (elektrodeRechtsInHand != null) elektrodeRechtsInHand.SetActive(false);
        if (elektrodeLinksOpBorst != null) elektrodeLinksOpBorst.SetActive(false);
        if (elektrodeRechtsOpBorst != null) elektrodeRechtsOpBorst.SetActive(false);
    }

    public void OnRaycastHoverEnter()
    {
        // De outline mag ook weer oplichten als de elektroden geplakt zijn en we wachten op de laatste klik
        if (aedGeactiveerd && (!linksGeplakt || !rechtsGeplakt)) return; 
        if (scenarioAfgerond) return;

        if (outlineComponent != null) outlineComponent.enabled = true;
    }

    public void OnRaycastHoverExit()
    {
        if (outlineComponent != null) outlineComponent.enabled = false;
    }

    // --- KLIK LOGICA OP DE AED ---
    public void OnAEDClicked()
    {
        if (scenarioAfgerond) return;

        // EERSTE KLIK: AED openen en elektroden geven
        if (!aedGeactiveerd)
        {
            aedGeactiveerd = true;
            Debug.Log("[AED] Eerste klik: AED geopend. Elektroden verschijnen.");
            
            if (outlineComponent != null) outlineComponent.enabled = false;
            if (elektrodeLinksInHand != null) elektrodeLinksInHand.SetActive(true);
            if (elektrodeRechtsInHand != null) elektrodeRechtsInHand.SetActive(true);
            return; // Stop de functie hier zodat hij niet meteen doorloopt naar de tweede klik!
        }

        // TWEEDE KLIK: Alleen mogelijk als BEIDE elektroden op de borst zitten
        // TWEEDE KLIK: Alleen mogelijk als BEIDE elektroden op de borst zitten
    if (aedGeactiveerd && linksGeplakt && rechtsGeplakt)
    {
        scenarioAfgerond = true; 
        Debug.Log("[AED] Tweede klik: Elektroden zijn geplakt. Schok wordt toegediend, slachtoffer begint te schudden!");

        if (outlineComponent != null) outlineComponent.enabled = false;

        // 1. Trigger de animatie op het slachtoffer
        if (victimAnimator != null)
        {
            victimAnimator.SetBool("shaking", true);
        }

        // 2. NIEUW: Meld de stap aan de stappenchecker zodat het klembord afvinkt!
        if (EHBOStappenChecker.Instance != null)
        {
            // LET OP: Deze tekst moet EXACT zo in je 'correctOrder' lijst staan in Unity!
            EHBOStappenChecker.Instance.RegisterStep("AED Aansluiten");
        }
        else
        {
            Debug.LogError("[AED] EHBOStappenChecker Instance is niet gevonden in de scene!");
        }
    }
    }

    // --- PLAK LOGICA ---
    public void PlakElektrode(bool isLinkerHand)
    {
        if (!aedGeactiveerd) return;

        if (isLinkerHand && !linksGeplakt)
        {
            linksGeplakt = true;
            if (elektrodeLinksInHand != null) elektrodeLinksInHand.SetActive(false); 
            if (elektrodeLinksOpBorst != null) elektrodeLinksOpBorst.SetActive(true); 
            Debug.Log("[AED] Links geplakt!");
        }
        else if (!isLinkerHand && !rechtsGeplakt)
        {
            rechtsGeplakt = true;
            if (elektrodeRechtsInHand != null) elektrodeRechtsInHand.SetActive(false); 
            if (elektrodeRechtsOpBorst != null) elektrodeRechtsOpBorst.SetActive(true); 
            Debug.Log("[AED] Rechts geplakt!");
        }

        // Als ze nu allebei geplakt zijn, geven we een seintje dat de speler weer op de AED mag klikken
        if (linksGeplakt && rechtsGeplakt)
        {
            Debug.Log("[AED] Beide elektroden zitten erop! Richt je Raycast weer op de AED en klik om de schok te geven.");
            // Optioneel: hier kun je de outline alvast weer aanzetten als hint, of een geluidje afspelen
        }
    }
}