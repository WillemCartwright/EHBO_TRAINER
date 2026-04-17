using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

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

    [Header("NPC Settings")]
    [SerializeField] private List<Animator> npcAnimators;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        if (summaryPanel != null) summaryPanel.SetActive(false);
        DisplayDebugInfo();
    }

    // Wordt aangeroepen wanneer het slachtoffer valt
    public void VictimHasFallen()
    {
        foreach (Animator anim in npcAnimators)
        {
            if (anim != null) anim.SetBool("shocked", true);
        }
        // Registreer de eerste stap automatisch
        RegisterStep("Start Incident");
    }

    public void RegisterStep(string stepName)
    {
        if (completedSteps.Count == 0 || completedSteps[completedSteps.Count - 1] != stepName)
        {
            completedSteps.Add(stepName);
            DisplayDebugInfo();

            // TACTISCH: Stuur de UI aan op basis van de nieuwe stap
            TriggerFaseLogica(stepName);

            // Controleer of we bij de laatste stap zijn
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

            case "Bewustzijn Check":
                mijnUIManager.ShowVictimReactionSequence();
                break;

            case "112 Bellen":
                mijnUIManager.ToonTekst("Bel 112 en zet de telefoon op luidspreker.");
                break;

            case "Luchtweg Openen":
                mijnUIManager.ToonTekst("Pas de kinlift toe om de luchtweg te openen.");
                break;

            case "Hart Compressie":
                mijnUIManager.ToonTekst("Start nu met 30 borstcompressies.");
                break;

            case "Beademing":
                mijnUIManager.ToonTekst("Geef nu 2 beademingen.");
                break;

            case "AED Aansluiten":
                mijnUIManager.ToonTekst("Bevestig de AED elektroden op de borst.");
                break;

            case "Hulpdiensten Nemen Over":
                mijnUIManager.ToonTekst("De ambulance is gearriveerd. Goed gedaan.");
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

        string debugPanelString = "<b>Voortgang EHBO:</b>\n";
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
            summaryText.text = $"Eindresultaat: {result}\nStappen voltooid: {completedSteps.Count}/{correctOrder.Count}";
        }

        if (mijnUIManager != null)
        {
            mijnUIManager.LaatEindSchermZien(isCorrect, completedSteps);
        }
    }
}