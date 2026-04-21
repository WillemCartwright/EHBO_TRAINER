using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Oculus.Interaction; 

public class EHBOStappenChecker : MonoBehaviour
{
    public static EHBOStappenChecker Instance;

    [SerializeField] private List<string> correctOrder;
    private List<string> completedSteps = new List<string>();
    
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI debugPanelText;
    [SerializeField] private GameObject summaryPanel;
    [SerializeField] private TextMeshProUGUI summaryText;
    [SerializeField] private UIManager mijnUIManager; 

    [Header("Fysieke Objecten")]
    [SerializeField] private VictimInteraction victim;
    [SerializeField] private RayInteractable dogRayInteractable;
    [SerializeField] private GameObject leftShoulderZone;
    [SerializeField] private GameObject rightShoulderZone;

    [Header("NPC Settings")]
    [SerializeField] private List<Animator> npcAnimators;

    [Header("NPC Acties")]
    [SerializeField] private NPCInteraction omstanderNPC; 

    void Awake()
    {
        if (Instance == null) Instance = this;

        // Zet zones standaard uit bij de start
        if (leftShoulderZone != null) leftShoulderZone.SetActive(false);
        if (rightShoulderZone != null) rightShoulderZone.SetActive(false);
    }

    void Start()
    {
        if (summaryPanel != null) summaryPanel.SetActive(false);
        DisplayDebugInfo();
    }

    public void VictimHasFallen()
    {
        foreach (Animator anim in npcAnimators)
        {
            if (anim != null) anim.SetBool("shocked", true);
        }
        RegisterStep("Start Incident");
    }

    public string GetCurrentStep()
    {
        if (completedSteps.Count > 0)
        {
            return completedSteps[completedSteps.Count - 1];
        }
        return "";
    }
    
    public void RegisterStep(string stepName)
    {
        if (completedSteps.Count == 0 || completedSteps[completedSteps.Count - 1] != stepName)
        {
            completedSteps.Add(stepName);
            DisplayDebugInfo();
            TriggerFaseLogica(stepName);

            if (stepName == "Hulpdiensten Nemen Over" || completedSteps.Count >= correctOrder.Count)
            {
                ValidateOrder();
            }
        }
    }

    private void TriggerFaseLogica(string stepName)
    {
        if (mijnUIManager == null) return;

        switch (stepName)
        {
            case "Start Incident":
                mijnUIManager.ToonInstructieAankomst();
                break;

            case "Omstanders Aangesproken": 
                mijnUIManager.ShowInteractionSequence(); 
                if (victim != null) 
                {
                    victim.EnableVictimInteraction(); 
                }
                break;

            case "Bewustzijn Check":
                mijnUIManager.ShowVictimReactionSequence();
                // Activeer de schouderzones
                if (leftShoulderZone != null) leftShoulderZone.SetActive(true);
                if (rightShoulderZone != null) rightShoulderZone.SetActive(true);

                // TIJDELIJKE TIMER: Na 5 seconden wordt de volgende stap automatisch getriggerd
                // Zodra je de echte animatie-trigger hebt, kun je deze regel verwijderen.
                Invoke("OnSchudAnimatieKlaar", 5f);
                break;

            case "112 Bellen":
                // 1. Schakel schouderzones uit
                if (leftShoulderZone != null) leftShoulderZone.SetActive(false);
                if (rightShoulderZone != null) rightShoulderZone.SetActive(false);

                // 2. Toon tekst voor de speler
                mijnUIManager.ToonTekst("Klik op de omstander en zeg: 'Bel 112, we hebben een hartstilstand!'");

                // 3. Maak de NPC weer klikbaar voor de animatie
                if (omstanderNPC != null) 
                {
                    omstanderNPC.ResetForPhoneCall();
                }
                break;

            case "Luchtweg Openen":
                mijnUIManager.ToonTekst("Pas de kinlift toe om de luchtweg te openen.");
                break;

            case "Hart Compressie":
                mijnUIManager.ToonTekst("Start nu met 30 borstcompressies.");
                break;

            case "Hulpdiensten Nemen Over":
                mijnUIManager.ToonTekst("De ambulance is er. Goed gedaan!");
                break;
        }
    }

    private void ValidateOrder()
    {
        bool isCorrect = true;
        for (int i = 0; i < correctOrder.Count; i++)
        {
            if (i >= completedSteps.Count || completedSteps[i] != correctOrder[i])
            {
                isCorrect = false;
                break;
            }
        }
        ShowSummary(isCorrect);
    }

    private void DisplayDebugInfo()
    {
        if (debugPanelText == null) return;
        string debugPanelString = "<b>Voortgang:</b>\n";
        for (int i = 0; i < completedSteps.Count; i++)
        {
            debugPanelString += $"{i + 1}. {completedSteps[i]}\n";
        }
        debugPanelText.text = debugPanelString;
    }

    private void ShowSummary(bool isCorrect)
    {
        if (summaryPanel != null) summaryPanel.SetActive(true);
        if (summaryText != null)
        {
            string result = isCorrect ? "<color=green>Correct!</color>" : "<color=red>Foutieve volgorde!</color>";
            summaryText.text = $"Eindresultaat: {result}\nStappen: {completedSteps.Count}/{correctOrder.Count}";
        }
        if (mijnUIManager != null) mijnUIManager.LaatEindSchermZien(isCorrect, completedSteps);
    }

    /// <summary>
    /// Wordt aangeroepen door de timer (Invoke) of later door een Animation Event.
    /// </summary>
    public void OnSchudAnimatieKlaar()
    {
        Debug.Log("Schudden voltooid: we gaan nu over naar de 112 fase.");
        RegisterStep("112 Bellen");
    }
}