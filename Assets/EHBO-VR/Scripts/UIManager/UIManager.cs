using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("UI Elementen")]
    [SerializeField] private TextMeshProUGUI uiText;
    [SerializeField] private CanvasGroup uiGroup; 

    [Header("Eindscherm UI")]
    [SerializeField] private GameObject eindPaneel;
    [SerializeField] private TextMeshProUGUI eindTekst;

    [Header("Instructie Teksten")]
    public string instructieAankomst = "Er is een man op de grond gevallen!\nKlik de omstanders aan.";
    public string afterClickMessage = "De NPC blijft nu bij je in de buurt.";
    public string finalInstruction = "Klik het slachtoffer aan om te zien of hij reageert";
    public string victimCallMessage = "Jij: Hallo meneer, kunt u mij horen?";
    public string victimNoResponse = "... Het slachtoffer geeft geen reaction.\nSchud zachtjes aan de schouders.";
    public string instructieSchuddenActive = "Het slachtoffer wordt nu rustig bij zijn schouders geschud.";
    public string dogWarningText = "Er is weinig tijd, richt je aandacht niet op de hond.";

    [Header("Referenties Managers")]
    [SerializeField] private EHBOStappenChecker stappenChecker;

    private bool isShowingText = false;
    private bool isGameOver = false; 

    void Awake()
    {
        if (Instance == null) Instance = this;
        
        if (uiGroup != null) uiGroup.alpha = 0;
        if (eindPaneel != null) eindPaneel.SetActive(false);
    }

    // --- EHBO LOGICA KOPPELINGEN ---

    public void LaatEindSchermZien(bool isCorrect, List<string> behaaldeStappen)
    {
        isGameOver = true;
        if (eindPaneel != null) eindPaneel.SetActive(true);
        
        string resultaat = isCorrect ? "<color=green>Correcte Volgorde!</color>" : "<color=red>Onjuiste Volgorde!</color>";
        
        if (eindTekst != null)
        {
            string overzicht = string.Join("\n", behaaldeStappen);
            eindTekst.text = $"{resultaat}\n\n<b>Jouw stappen:</b>\n{overzicht}";
        }
        
        StartCoroutine(Fade(uiGroup.alpha, 0, 0.5f)); 
    }

    public void ToonTekst(string bericht)
    {
        if (isGameOver) return;
        StopAllCoroutines();
        StartCoroutine(ShowTextAsync(bericht, 3f));
    }

    public void StapVoltooid(string stapNaam) 
    {
        if (stappenChecker != null)
        {
            stappenChecker.RegisterStep(stapNaam);
        }
        ToonTekst($"Stap voltooid: {stapNaam}");
    }

    public bool CanInteract()
    {
        return !isGameOver;
    }

    // --- HOND LOGICA (Alleen Visueel) ---
    public void ShowDogWarning()
    {
        if (isGameOver) return; 
        isGameOver = true;

        StopAllCoroutines();
        StartCoroutine(DogFailSequence());
    }

    private IEnumerator DogFailSequence()
    {
        isShowingText = true;
        uiText.text = dogWarningText;
        yield return StartCoroutine(Fade(uiGroup.alpha, 1, 0.4f));
        yield return new WaitForSeconds(3f);
        yield return StartCoroutine(Fade(1, 0, 0.4f));
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // --- FASE SEQUENCES (Alleen Tekst & Animatie triggers) ---
    public void ToonInstructieAankomst()
    {
        if (isGameOver) return;
        NPCInteraction[] alleNPCs = Object.FindObjectsByType<NPCInteraction>(FindObjectsSortMode.None);
        foreach (NPCInteraction npc in alleNPCs) { npc.EnableOutlineCapability(); }
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
        if (isGameOver) yield break;
        VictimInteraction victim = Object.FindAnyObjectByType<VictimInteraction>();
        if (victim != null) victim.EnableVictimInteraction();
        yield return StartCoroutine(FadeInText(finalInstruction));
    }

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
    }

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

    // --- FADE HULPFUNCTIES ---
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
            if (uiGroup == null) yield break; 
            uiGroup.alpha = Mathf.Lerp(start, end, elapsed / time); 
            yield return null; 
        }
        if (uiGroup != null) uiGroup.alpha = end;
    }
}