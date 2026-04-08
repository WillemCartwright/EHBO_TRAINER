using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("UI Elementen")]
    [SerializeField] private TextMeshProUGUI uiText;
    [SerializeField] private CanvasGroup uiGroup; 

    [Header("Fase 1: Het Ongeluk")]
    public string instructieAankomst = "Er is een man op de grond gevallen!\nKlik de omstanders aan.";
    public string afterClickMessage = "De NPC blijft nu bij je in de buurt.";
    public string finalInstruction = "Klik het slachtoffer aan om te zien of hij reageert";

    [Header("Fase 2: Slachtoffer Reactie")]
    public string victimCallMessage = "Jij: Hallo meneer, kunt u mij horen?";
    // Stap B: Vertel de speler WAT hij moet doen
    public string victimNoResponse = "... Het slachtoffer geeft geen reactie.\nKlik op zijn schouders om hem zachtjes te schudden.";

    [Header("Fase 3: Schudden")]
    // Stap C: Bevestig DAT de speler het nu aan het doen is
    public string instructieSchuddenActive = "Het slachtoffer wordt nu rustig bij zijn schouders geschud.";

    [Header("Fase 3: Objecten")]
    [SerializeField] private GameObject leftShoulderZone;
    [SerializeField] private GameObject rightShoulderZone;

    private bool isShowingText = false;

    void Awake()
    {
        Instance = this;
        
        if (uiGroup != null) uiGroup.alpha = 0;

        // Schouderzones staan uit bij de start
        if (leftShoulderZone != null) leftShoulderZone.SetActive(false);
        if (rightShoulderZone != null) rightShoulderZone.SetActive(false);
    }

    // --- FASE 1: Omstanders ---
    public void ToonInstructieAankomst()
    {
        if (isShowingText) return;

        NPCInteraction[] alleNPCs = Object.FindObjectsByType<NPCInteraction>(FindObjectsSortMode.None);
        foreach (NPCInteraction npc in alleNPCs)
        {
            npc.EnableOutlineCapability();
        }

        StopAllCoroutines();
        StartCoroutine(FadeInText(instructieAankomst));
    }

    public void ShowInteractionSequence()
    {
        StopAllCoroutines();
        StartCoroutine(InteractionSequence());
    }

    private IEnumerator InteractionSequence()
    {
        isShowingText = true;
        yield return StartCoroutine(ShowTextAsync(afterClickMessage, 3f));
        yield return new WaitForSeconds(0.5f);

        VictimInteraction victim = Object.FindAnyObjectByType<VictimInteraction>();
        if (victim != null) victim.EnableVictimInteraction();
        
        yield return StartCoroutine(FadeInText(finalInstruction));
    }

    // --- FASE 2: Reactie checken ---
    public void ShowVictimReactionSequence()
    {
        StopAllCoroutines();
        StartCoroutine(VictimSequence());
    }

    private IEnumerator VictimSequence()
    {
        isShowingText = true;

        // 1. "Hallo meneer?" (5 seconden)
        yield return StartCoroutine(ShowTextAsync(victimCallMessage, 5f));
        
        yield return new WaitForSeconds(0.5f);
        
        // 2. Toon Stap B: "Geen reactie. Klik op zijn schouders..." (Blijft staan)
        yield return StartCoroutine(FadeInText(victimNoResponse));

        // 3. Nu pas worden de triggers actief
        if (leftShoulderZone != null) leftShoulderZone.SetActive(true);
        if (rightShoulderZone != null) rightShoulderZone.SetActive(true);
    }

    // --- FASE 3: Schudden (Direct aangeroepen door OnTriggerEnter in ShoulderTouchLogic) ---
    public void StartSchudTekst()
    {
        // Alleen updaten als we niet al de schud-tekst tonen
        if (uiText.text != instructieSchuddenActive)
        {
            StopAllCoroutines();
            StartCoroutine(UpdateInstructie(instructieSchuddenActive));
        }
    }

    private IEnumerator UpdateInstructie(string nieuweTekst)
    {
        isShowingText = true;
        yield return StartCoroutine(Fade(uiGroup.alpha, 0, 0.5f));
        uiText.text = nieuweTekst;
        yield return StartCoroutine(Fade(0, 1, 0.5f));
    }

    // --- FADE ENGINES ---
    private IEnumerator FadeInText(string message)
    {
        isShowingText = true;
        uiText.text = message;
        yield return StartCoroutine(Fade(uiGroup.alpha, 1, 0.8f));
    }

    public void VerbergTekst()
    {
        StopAllCoroutines();
        StartCoroutine(TextFadeOutEnStop());
    }

    private IEnumerator TextFadeOutEnStop()
    {
        yield return StartCoroutine(Fade(uiGroup.alpha, 0, 0.8f));
        isShowingText = false;
        uiText.text = "";
    }

    public IEnumerator ShowTextAsync(string message, float duration)
    {
        uiText.text = message;
        yield return StartCoroutine(Fade(uiGroup.alpha, 1, 0.8f));
        yield return new WaitForSeconds(duration);
        yield return StartCoroutine(Fade(1, 0, 0.8f));
        isShowingText = false;
    }

    private IEnumerator Fade(float start, float end, float time)
    {
        if (uiGroup == null) yield break;

        float elapsed = 0; 
        while (elapsed < time) 
        { 
            elapsed += Time.deltaTime; 
            uiGroup.alpha = Mathf.Lerp(start, end, elapsed / time); 
            yield return null; 
        }
        uiGroup.alpha = end;
    }
}