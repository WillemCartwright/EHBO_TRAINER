using UnityEngine;

public class AEDInteraction : MonoBehaviour
{
    [Header("Outline Settings")]
    // Sleep hier het Outline-component in dat op deze AED zit
    [SerializeField] private MonoBehaviour outlineComponent; 

    [Header("Electrodes in VR Hands")]
    [SerializeField] private GameObject elektrodeLinksInHand;  // Onder de Linker VR controller
    [SerializeField] private GameObject elektrodeRechtsInHand; // Onder de Rechter VR controller

    [Header("Electrodes on Chest (Targets)")]
    [SerializeField] private GameObject elektrodeLinksOpBorst;  // De plakker alvast op de juiste plek op de borst (uitgevinkt)
    [SerializeField] private GameObject elektrodeRechtsOpBorst; // De 2e plakker alvast op de juiste plek op de borst (uitgevinkt)

    private bool aedGeactiveerd = false;
    private bool linksGeplakt = false;
    private bool rechtsGeplakt = false;

    void Start()
    {
        // Zorg dat de outline bij de start uit staat, tot de raycast kijkt
        if (outlineComponent != null) outlineComponent.enabled = false;
        
        // Zorg dat alle elektroden (handen + borst) onzichtbaar starten
        if (elektrodeLinksInHand != null) elektrodeLinksInHand.SetActive(false);
        if (elektrodeRechtsInHand != null) elektrodeRechtsInHand.SetActive(false);
        if (elektrodeLinksOpBorst != null) elektrodeLinksOpBorst.SetActive(false);
        if (elektrodeRechtsOpBorst != null) elektrodeRechtsOpBorst.SetActive(false);
    }

    // --- 1. RAYCAST HOVER LOGICA (Aangeroepen door je VR Raycast script) ---
    public void OnRaycastHoverEnter()
{
    // Als de AED al aan is, negeer de hover compleet!
    if (aedGeactiveerd) return; 

    if (outlineComponent != null)
    {
        outlineComponent.enabled = true;
    }
}

    public void OnRaycastHoverExit()
    {
        if (outlineComponent != null)
        {
            outlineComponent.enabled = false; // Outline uit als je wegkijkt
        }
    }

    // --- 2. KLIK LOGICA OP DE AED ---
    public void OnAEDClicked()
{
    // TIJDELIJKE TEST: Dit MOET in de console verschijnen als je klikt!
    Debug.Log("[TEST] OnAEDClicked is succesvol aangeroepen door de VR Controller!");

    if (aedGeactiveerd) return;

    aedGeactiveerd = true;
    Debug.Log("[AED] Speler heeft de AED geopend! Elektroden verschijnen in de handen.");

    // Zet outline permanent uit
    if (outlineComponent != null) 
    {
        outlineComponent.enabled = false;
        Debug.Log("[AED] Outline is nu uitgezet.");
    }
    else
    {
        Debug.LogWarning("[AED] OutlineComponent is NIET gekoppeld in de Inspector!");
    }

    // Geef de speler de elektroden in zijn VR handen
    if (elektrodeLinksInHand != null) 
    {
        elektrodeLinksInHand.SetActive(true);
        Debug.Log("[AED] Linker elektrode aangezet.");
    }
    if (elektrodeRechtsInHand != null) 
    {
        elektrodeRechtsInHand.SetActive(true);
        Debug.Log("[AED] Rechter elektrode aangezet.");
    }
}

    // --- 3. PLAK LOGICA OP DE BORST ---
    // Deze functie roep je aan vanuit je controllers wanneer ze de borst aanraken of erop klikken
    public void PlakElektrode(bool isLinkerHand)
    {
        if (!aedGeactiveerd) return;

        if (isLinkerHand && !linksGeplakt)
        {
            linksGeplakt = true;
            if (elektrodeLinksInHand != null) elektrodeLinksInHand.SetActive(false); // Verdwijn uit hand
            if (elektrodeLinksOpBorst != null) elektrodeLinksOpBorst.SetActive(true); // Verschijn op borst
            Debug.Log("[AED] Linker elektrode succesvol op de borst geplakt!");
        }
        else if (!isLinkerHand && !rechtsGeplakt)
        {
            rechtsGeplakt = true;
            if (elektrodeRechtsInHand != null) elektrodeRechtsInHand.SetActive(false); // Verdwijn uit hand
            if (elektrodeRechtsOpBorst != null) elektrodeRechtsOpBorst.SetActive(true); // Verschijn op borst
            Debug.Log("[AED] Rechter elektrode succesvol op de borst geplakt!");
        }

        // Check of ze allebei geplakt zijn om de volgende stap (bijv. reanimatie/schok) te triggeren
        if (linksGeplakt && rechtsGeplakt)
        {
            Debug.Log("[AED] Top! Allebei de elektroden zitten op de juiste plek. De AED kan nu gaan analyseren!");
            // Hier kun je eventueel een audiofragment starten ("Analyseert hartritme, raak het slachtoffer niet aan")
        }
    }
}