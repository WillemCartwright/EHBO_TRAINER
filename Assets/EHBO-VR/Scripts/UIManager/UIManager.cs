using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("UI Elementen")]
    [SerializeField] private TextMeshProUGUI uiText;
    [SerializeField] private CanvasGroup uiGroup; 

    [Header("Instructie Teksten")]
    public string instructieAankomst = "Er is een man op de grond gevallen!\nKlik de omstanders aan.";
    public string afterClickMessage = "De NPC blijft nu bij je in de buurt.";
    public string finalInstruction = "Klik het slachtoffer aan om te zien of hij reageert";
    public string victimCallMessage = "Jij: Hallo meneer, kunt u mij horen?";
    public string victimNoResponse = "... Het slachtoffer geeft geen reactie.\nSchud zachtjes aan de schouders.";
    public string instructieSchuddenActive = "Het slachtoffer wordt nu rustig bij zijn schouders geschud.";
    public string dogWarningText = "Er is weinig tijd, richt je aandacht niet op de hond.";

    void Awake()
    {
        if (Instance == null) Instance = this;
        if (uiGroup != null) uiGroup.alpha = 0;
    }

    public bool CanInteract() => true;

    public void ToonTekst(string bericht)
    {
        StopAllCoroutines();
        StartCoroutine(ShowTextAsync(bericht, 3f));
    }

    public void ToonInstructieAankomst()
    {
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
        yield return StartCoroutine(ShowTextAsync(afterClickMessage, 3f));
        uiText.text = finalInstruction;
        yield return StartCoroutine(Fade(uiGroup.alpha, 1, 0.6f));
    }

    public void ShowVictimReactionSequence()
    {
        StopAllCoroutines();
        StartCoroutine(VictimSequence());
    }

    private IEnumerator VictimSequence()
    {
        uiText.text = victimCallMessage;
        yield return StartCoroutine(Fade(uiGroup.alpha, 1, 0.6f));
        yield return new WaitForSeconds(4f);
        uiText.text = victimNoResponse;
    }

    public void StartSchudTekst()
    {
        if (uiText.text != instructieSchuddenActive) uiText.text = instructieSchuddenActive;
    }

    public void ShowDogWarning()
    {
        StopAllCoroutines();
        StartCoroutine(DogFailSequence());
    }

    private IEnumerator DogFailSequence()
    {
        uiText.text = dogWarningText;
        yield return StartCoroutine(Fade(uiGroup.alpha, 1, 0.4f));
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public IEnumerator ShowTextAsync(string message, float duration)
    {
        uiText.text = message;
        yield return StartCoroutine(Fade(uiGroup.alpha, 1, 0.6f));
        yield return new WaitForSeconds(duration);
        yield return StartCoroutine(Fade(1, 0, 0.6f));
    }

    private IEnumerator FadeInText(string message)
    {
        uiText.text = message;
        yield return StartCoroutine(Fade(uiGroup.alpha, 1, 0.6f));
    }

    private IEnumerator Fade(float start, float end, float time)
    {
        float elapsed = 0; 
        while (elapsed < time) 
        { 
            elapsed += Time.deltaTime; 
            uiGroup.alpha = Mathf.Lerp(start, end, elapsed / time); 
            yield return null; 
        }
        uiGroup.alpha = end;
    }

    public void LaatEindSchermZien(bool isCorrect, List<string> stappen) { /* ... */ }
}