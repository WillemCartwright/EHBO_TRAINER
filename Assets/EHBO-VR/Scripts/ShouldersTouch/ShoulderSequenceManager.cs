using UnityEngine;

public class ShoulderSequenceManager : MonoBehaviour
{
    public static ShoulderSequenceManager Instance;

    [Header("Instellingen")]
    [SerializeField] private float requiredTime = 2.0f; // Hoe lang moet je schudden?
    [SerializeField] private Animator victimAnimator;

    private bool leftHandIn = false;
    private bool rightHandIn = false;
    private float combinedTimer = 0f;
    private bool sequenceFinished = false;

    void Awake() 
    { 
        Instance = this; 
    }

    /// <summary>
    /// Wordt aangeroepen door de ShoulderTouchLogic op de schouder-zones.
    /// </summary>
    public void UpdateHandStatus(string zoneName, bool isIn)
    {
        if (sequenceFinished) return;

        string nameLower = zoneName.ToLower();
        
        // Check of de zone links of rechts is op basis van de naam van het object
        if (nameLower.Contains("l")) leftHandIn = isIn;
        if (nameLower.Contains("r")) rightHandIn = isIn;
    }

    void Update()
    {
        // Als we al klaar zijn, of de checker is nog niet bij deze stap, doe niets
        if (sequenceFinished) return;

        // Logica: beide handen moeten tegelijkertijd in de zones zijn
        if (leftHandIn && rightHandIn)
        {
            combinedTimer += Time.deltaTime;

            // Update de tekst in de UI naar "Slachtoffer wordt nu geschud"
            if (UIManager.Instance != null) 
            {
                UIManager.Instance.StartSchudTekst();
            }

            // Als de tijd verstreken is, voltooi de reeks
            if (combinedTimer >= requiredTime)
            {
                FinishSequence();
            }
        }
        else
        {
            // Reset de timer zodra één van de twee handen de zone verlaat
            combinedTimer = 0f; 
        }
    }

    private void FinishSequence()
    {
        sequenceFinished = true;
        
        // Start de animatie waarbij het slachtoffer reageert (of juist niet)
        if (victimAnimator != null) 
        {
            victimAnimator.SetTrigger("StartShaking");
        }

        Debug.Log("Succes: Schudden voltooid! Signaal sturen naar EHBOStappenChecker...");

        // GEEF HET SEINTJE AAN DE CHECKER
        // Dit zorgt ervoor dat de game nu pas doorgaat naar de stap "112 Bellen"
        if (EHBOStappenChecker.Instance != null)
        {
            EHBOStappenChecker.Instance.OnSchudAnimatieKlaar();
        }
        
        // Optioneel: zet dit script uit om performance te sparen
        this.enabled = false;
    }
}