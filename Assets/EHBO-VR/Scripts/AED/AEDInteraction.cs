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

    [Header("AED Kleding Wissel")]
    [Tooltip("Sleep hier alleen de kledingstukken in die UIT moeten (bijv. T-shirt, jas, rits). NIET het hele lichaam van de man!")]
    [SerializeField] private GameObject[] objectenDieUitMoeten; 
    
    [Tooltip("Sleep hier the nieuwe blote borst in die AAN moet gaan")]
    [SerializeField] private GameObject bloteBorstMesh;    

    [Header("Victim Settings & Animation")]
    [Tooltip("Sleep hier de Animator van het slachtoffer in")]
    [SerializeField] private Animator victimAnimator; 

    [Header("Audio Settings")]
    [SerializeField] private AudioClip schokAudioClip;
    [SerializeField] private AudioSource audioSource;

    private bool aedGeactiveerd = false;
    private bool linksGeplakt = false;
    private bool rechtsGeplakt = false;
    private bool scenarioAfgerond = false; 

    void Start()
    {
        if (outlineComponent != null) outlineComponent.enabled = false;
        
        if (elektrodeLinksInHand != null) elektrodeLinksInHand.SetActive(false);
        if (elektrodeRechtsInHand != null) elektrodeRechtsInHand.SetActive(false);
        if (elektrodeLinksOpBorst != null) elektrodeLinksOpBorst.SetActive(false);
        if (elektrodeRechtsOpBorst != null) elektrodeRechtsOpBorst.SetActive(false);

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        // Basisstand voor de kledingstukken bij de start
        if (objectenDieUitMoeten != null)
        {
            foreach (GameObject obj in objectenDieUitMoeten)
            {
                if (obj != null) obj.SetActive(true);
            }
        }
        
        if (bloteBorstMesh != null) bloteBorstMesh.SetActive(false);
    }

    public void OnRaycastHoverEnter()
    {
        if (aedGeactiveerd && (!linksGeplakt || !rechtsGeplakt)) return; 
        if (scenarioAfgerond) return;

        if (outlineComponent != null) outlineComponent.enabled = true;
    }

    public void OnRaycastHoverExit()
    {
        if (outlineComponent != null) outlineComponent.enabled = false;
    }

    public void OnAEDClicked()
    {
        if (scenarioAfgerond) return;

        // EERSTE KLIK: AED openen, kleding uit, blote borst aan
        if (!aedGeactiveerd)
        {
            aedGeactiveerd = true;
            Debug.Log("[AED] Eerste klik: AED geopend. Kleding gaat uit.");
            
            if (outlineComponent != null) outlineComponent.enabled = false;
            if (elektrodeLinksInHand != null) elektrodeLinksInHand.SetActive(true);
            if (elektrodeRechtsInHand != null) elektrodeRechtsInHand.SetActive(true);

            if (objectenDieUitMoeten != null)
            {
                foreach (GameObject obj in objectenDieUitMoeten)
                {
                    if (obj != null) obj.SetActive(false); 
                }
            }

            if (bloteBorstMesh != null) bloteBorstMesh.SetActive(true); 

            return; 
        }

        // TWEEDE KLIK: Schok toedienen
        if (aedGeactiveerd && linksGeplakt && rechtsGeplakt)
        {
            scenarioAfgerond = true; 
            Debug.Log("[AED] Tweede klik: Schok wordt toegediend!");

            if (outlineComponent != null) outlineComponent.enabled = false;

            // Trigger het schudden (als de animator is ingevuld)
            if (victimAnimator != null)
            {
                victimAnimator.gameObject.SetActive(true); 
                victimAnimator.SetBool("shaking", true);   
            }

            // Speel geluid af en bereken de lengte van de audio
            float wachtTijd = 0f;
            if (audioSource != null && schokAudioClip != null)
            {
                audioSource.PlayOneShot(schokAudioClip);
                wachtTijd = schokAudioClip.length; // Pakt exact de seconden van de audio clip
                Debug.Log($"[AED] Audio gestart. Lengte: {wachtTijd} seconden.");
            }

            // Start de timer die wacht tot de audio klaar is
            Invoke("ActiveerBorstcompressieHerstart", wachtTijd);

            // Meld de AED stap aan de stappenchecker (Exact met Hoofdletters!)
            if (EHBOStappenChecker.Instance != null)
            {
                EHBOStappenChecker.Instance.RegisterStep("AED aansluiten");
            }
        }
    }

    // --- DE TIMING FUNCTIE DIE GEACTIVEERD WORDT ALS DE AUDIO KLAAR IS ---
    private void ActiveerBorstcompressieHerstart()
    {
        Debug.Log("<color=green>[AED]</color> Audio is klaar! Schudden stoppen en seintje naar Stappenchecker sturen...");

        // Stop het schudden van het slachtoffer
        if (victimAnimator != null)
        {
            victimAnimator.SetBool("shaking", false);
        }

        // Meld de herhaal-stap aan de stappenchecker. Die zet nu de Ghost Hands en zones aan!
        if (EHBOStappenChecker.Instance != null)
        {
            EHBOStappenChecker.Instance.RegisterStep("Herhaal borstcompressies");
        }
    }

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

        if (linksGeplakt && rechtsGeplakt)
        {
            Debug.Log("[AED] Beide elektroden zitten erop!");
        }
    }
}