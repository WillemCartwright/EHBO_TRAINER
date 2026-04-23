using UnityEngine;

public class ShoulderSequenceManager : MonoBehaviour
{
    public static ShoulderSequenceManager Instance;

    [Header("Instellingen")]
    [SerializeField] private float requiredTime = 2.0f;
    [SerializeField] private Animator victimAnimator;

    private bool leftHandIn = false;
    private bool rightHandIn = false;
    private float combinedTimer = 0f;
    private bool sequenceFinished = false;

    void Awake() { Instance = this; }

    public void UpdateHandStatus(string zoneName, bool isIn)
    {
        if (sequenceFinished) return;

        string nameLower = zoneName.ToLower();
        if (nameLower.Contains("l")) leftHandIn = isIn;
        if (nameLower.Contains("r")) rightHandIn = isIn;

        // We roepen geen UpdateVisuals meer aan omdat er geen outlines zijn.
    }

    void Update()
    {
        if (sequenceFinished) return;

        // Logica blijft: beide controllers moeten in de zones blijven
        if (leftHandIn && rightHandIn)
        {
            combinedTimer += Time.deltaTime;

            if (UIManager.Instance != null) UIManager.Instance.StartSchudTekst();

            if (combinedTimer >= requiredTime)
            {
                FinishSequence();
            }
        }
        else
        {
            combinedTimer = 0f; // Reset als je één hand weghaalt
        }
    }

    private void FinishSequence()
    {
        sequenceFinished = true;
        
        // Trigger de animatie van het slachtoffer
        if (victimAnimator != null) victimAnimator.SetTrigger("StartShaking");

        Debug.Log("Succes: Schudden voltooid zonder outlines!");
        
        // De Invoke voor CleanUpOutlines is verwijderd, 
        // de EHBOStappenChecker handelt nu het uitzetten van de hand-models af.
    }
}