using UnityEngine;

public class ShoulderSequenceManager : MonoBehaviour
{
    public static ShoulderSequenceManager Instance;

    [Header("Instellingen")]
    [SerializeField] private float requiredTime = 2.0f;
    [SerializeField] private ShoulderTouchLogic leftLogic;
    [SerializeField] private ShoulderTouchLogic rightLogic;
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

        // Feedback kleuren aanpassen
        UpdateVisuals();
    }

    void Update()
    {
        if (sequenceFinished) return;

        // Alleen als BEIDE handen erin zitten, gaat de timer lopen
        if (leftHandIn && rightHandIn)
        {
            combinedTimer += Time.deltaTime;

            // Zodra ze beide erin zitten, triggeren we de "wordt nu geschud" tekst
            if (UIManager.Instance != null) UIManager.Instance.StartSchudTekst();

            if (combinedTimer >= requiredTime)
            {
                FinishSequence();
            }
        }
        else
        {
            // Als er één hand uitgaat, resetten we de timer (streng, maar realistisch)
            combinedVisualTimerReset();
        }
    }

    private void UpdateVisuals()
    {
        if (sequenceFinished) return;

        // Geel als beide handen erin zitten, anders wit (wachten)
        Color feedbackColor = (leftHandIn && rightHandIn) ? Color.yellow : Color.white;
        
        if (leftLogic != null) leftLogic.SetColor(feedbackColor);
        if (rightLogic != null) rightLogic.SetColor(feedbackColor);
    }

    private void combinedVisualTimerReset()
    {
        combinedTimer = 0f;
        // Optioneel: hier kun je de tekst terugzetten naar "klik op schouders" als ze loslaten
    }

    private void FinishSequence()
    {
        sequenceFinished = true;
        
        if (leftLogic != null) { leftLogic.SetColor(Color.green); }
        if (rightLogic != null) { rightLogic.SetColor(Color.green); }

        if (victimAnimator != null) victimAnimator.SetTrigger("StartShaking");

        Debug.Log("Succes: Slachtoffer is met beide handen geschud!");
        
        // Na 1.5 seconde de outlines opruimen
        Invoke("CleanUpOutlines", 1.5f);
    }

    private void CleanUpOutlines()
    {
        if (leftLogic != null) leftLogic.HideOutline();
        if (rightLogic != null) rightLogic.HideOutline();
    }
}