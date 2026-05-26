using UnityEngine;

public class KinliftManager : MonoBehaviour
{
    public static KinliftManager Instance;

    [Header("Instellingen")]
    [SerializeField] private float requiredTime = 3.0f; // Nu 3 seconden
    [SerializeField] private Animator victimAnimator; // Voor de animatie dat het hoofd kantelt

    private bool handOnForehead = false;
    private bool handOnChin = false;
    private float timer = 0f;
    private bool sequenceFinished = false;

    void Awake() { Instance = this; }

    public void UpdateHandStatus(string zoneName, bool isIn)
    {
        if (sequenceFinished) return;

        // We checken nu op de namen van je nieuwe zones
        if (zoneName.Contains("Voorhoofd")) handOnForehead = isIn;
        if (zoneName.Contains("Kin")) handOnChin = isIn;
    }

    void Update()
    {
        if (sequenceFinished) return;

        // Check of beide handen op de juiste plek zijn
        if (handOnForehead && handOnChin)
        {
            timer += Time.deltaTime;

            if (timer >= requiredTime)
            {
                FinishSequence();
            }
        }
        else
        {
            timer = 0f; // Reset als je één hand loslaat
        }
    }

    private void FinishSequence()
    {
        sequenceFinished = true;
        
        if (victimAnimator != null) 
        {
            victimAnimator.SetTrigger("KinliftDone"); // Zorg dat je deze trigger hebt in je Animator
        }

        Debug.Log("Luchtweg geopend: 3 seconden vastgehouden.");

        // Seintje naar de stappenchecker om door te gaan naar Hartcompressies
        if (EHBOStappenChecker.Instance != null)
        {
            EHBOStappenChecker.Instance.RegisterStep("Voer 30 borstcompressies uit met een snelheid van 2 compressies per seconde");
        }
    }
}