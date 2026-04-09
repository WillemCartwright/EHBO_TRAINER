using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using Oculus.Interaction; // Nodig voor de RayInteractable referentie

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
    public string victimNoResponse = "... Het slachtoffer geeft geen reactie.\nKlik op zijn schouders om hem zachtjes te schudden.";

    [Header("Fase 3: Schudden")]
    public string instructieSchuddenActive = "Het slachtoffer wordt nu rustig bij zijn schouders geschud.";

    [Header("Hond Instellingen")]
    public string dogWarningText = "Er is weinig tijd, richt je aandacht niet op de hond.";
    [SerializeField] private RayInteractable dogRayInteractable; // De hond referentie

    [Header("Fase 3: Objecten")]
    [SerializeField] private GameObject leftShoulderZone;
    [SerializeField] private GameObject rightShoulderZone;

    private bool isShowingText = false;
    private bool isGameOver = false; 

    void Awake()
    {
        if (Instance == null) Instance = this;
        
        if (uiGroup != null) uiGroup.alpha = 0;

        if (leftShoulderZone != null) leftShoulderZone.SetActive(false);
        if (rightShoulderZone != null) rightShoulderZone.SetActive(false);
    }

    /// <summary>
    /// Gebruik dit in andere scripts: if(!UIManager.Instance.CanInteract()) return;
    /// </summary>
    public bool CanInteract()
    {
        return !isGameOver;
    }

    // --- HOND LOGICA ---
    public void ShowDogWarning()
    {
        if (isGameOver) return; 
        isGameOver = true;

        // Forceer alle interacties uit
        if (dogRayInteractable != null) dogRayInteractable.enabled = false;

        StopAllCoroutines();
        StartCoroutine(DogFailSequence());
    }

    private IEnumerator DogFailSequence()
    {
        isShowingText = true;
        uiText.text = dogWarningText;
        
        // Snel infaden van de waarschuwing
        yield return StartCoroutine(Fade(uiGroup.alpha, 1, 0.4f));
        
        // De speler MOET dit 3 seconden zien voor de herstart
        yield return new WaitForSeconds(3f);
        
        // Uitfaden voor een nette overgang
        yield return StartCoroutine(Fade(1, 0, 0.4f));

        // Herstart de scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // --- FASE 1: Omstanders ---
    public void ToonInstructieAankomst()
    {
        if (isGameOver) return;

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
        if (isGameOver) return;
        StopAllCoroutines();
        StartCoroutine(InteractionSequence());
    }

    private IEnumerator InteractionSequence()
    {
        isShowingText = true;
        yield return StartCoroutine(ShowTextAsync(afterClickMessage, 3f));
        
        if (isGameOver) yield break; // Veiligheidscheck

        VictimInteraction victim = Object.FindAnyObjectByType<VictimInteraction>();
        if (victim != null) victim.EnableVictimInteraction();
        
        yield return StartCoroutine(FadeInText(finalInstruction));
    }

    // --- FASE 2: Reactie checken ---
    public void ShowVictimReactionSequence()
    {
        if (isGameOver) return;
        StopAllCoroutines();
        StartCoroutine(VictimSequence());
    }

    private IEnumerator VictimSequence()
    {
        isShowingText = true;
        yield return StartCoroutine(ShowTextAsync(victimCallMessage, 4f));
        
        if (isGameOver) yield break;

        yield return StartCoroutine(FadeInText(victimNoResponse));

        if (leftShoulderZone != null) leftShoulderZone.SetActive(true);
        if (rightShoulderZone != null) rightShoulderZone.SetActive(true);
    }

    // --- FASE 3: Schudden ---
    public void StartSchudTekst()
    {
        if (isGameOver) return;
        if (uiText.text != instructieSchuddenActive)
        {
            StopAllCoroutines();
            StartCoroutine(UpdateInstructie(instructieSchuddenActive));
        }
    }

    private IEnumerator UpdateInstructie(string nieuweTekst)
    {
        isShowingText = true;
        yield return StartCoroutine(Fade(uiGroup.alpha, 0, 0.3f));
        uiText.text = nieuweTekst;
        yield return StartCoroutine(Fade(0, 1, 0.3f));
    }

    // --- HULPFUNCTIES ---
    private IEnumerator FadeInText(string message)
    {
        if (isGameOver) yield break;
        isShowingText = true;
        uiText.text = message;
        yield return StartCoroutine(Fade(uiGroup.alpha, 1, 0.6f));
    }

    public void VerbergTekst()
    {
        if (isGameOver) return;
        StopAllCoroutines();
        StartCoroutine(TextFadeOutEnStop());
    }

    private IEnumerator TextFadeOutEnStop()
    {
        yield return StartCoroutine(Fade(uiGroup.alpha, 0, 0.6f));
        isShowingText = false;
        uiText.text = "";
    }

    public IEnumerator ShowTextAsync(string message, float duration)
    {
        uiText.text = message;
        yield return StartCoroutine(Fade(uiGroup.alpha, 1, 0.6f));
        yield return new WaitForSeconds(duration);
        if (!isGameOver) yield return StartCoroutine(Fade(1, 0, 0.6f));
        isShowingText = false;
    }

    private IEnumerator Fade(float start, float end, float time)
    {
        if (uiGroup == null) yield break;
        float elapsed = 0; 
        while (elapsed < time) 
        { 
            elapsed += Time.deltaTime; 
            if (uiGroup == null) yield break; // Extra check voor scene switch
            uiGroup.alpha = Mathf.Lerp(start, end, elapsed / time); 
            yield return null; 
        }
        if (uiGroup != null) uiGroup.alpha = end;
    }
}