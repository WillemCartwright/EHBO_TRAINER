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
    [SerializeField] private GameObject leftShoulderZone;
    [SerializeField] private GameObject rightShoulderZone;

    [Header("Ghost Hands")]
    [SerializeField] private GameObject ghostHandsSchudden; 
    [SerializeField] private GameObject ghostHandsLuchtweg; 

    [Header("NPC Settings")]
    [SerializeField] private List<Animator> npcAnimators;

    [Header("NPC Acties")]
    [SerializeField] private NPCInteraction omstanderNPC; 

    void Awake()
    {
        if (Instance == null) Instance = this;

        // Zet zones en ghost hands standaard uit bij de start
        if (leftShoulderZone != null) leftShoulderZone.SetActive(false);
        if (rightShoulderZone != null) rightShoulderZone.SetActive(false);
        if (ghostHandsSchudden != null) ghostHandsSchudden.SetActive(false);
        if (ghostHandsLuchtweg != null) ghostHandsLuchtweg.SetActive(false);
    }

    void Start()
    {
        if (summaryPanel != null) summaryPanel.SetActive(false);
        DisplayDebugInfo();
    }

    /// <summary>
    /// Wordt aangeroepen als het incident begint (bijv. man valt neer).
    /// </summary>
    public void VictimHasFallen()
    {
        foreach (Animator anim in npcAnimators)
        {
            if (anim != null) anim.SetBool("shocked", true);
        }
        RegisterStep("Start Incident");
    }

    public void RegisterStep(string stepName)
    {
        // Alleen toevoegen als de stap niet al de huidige actieve stap is
        if (completedSteps.Count == 0 || completedSteps[completedSteps.Count - 1] != stepName)
        {
            completedSteps.Add(stepName);
            DisplayDebugInfo();
            TriggerFaseLogica(stepName);

            // Controleer of we klaar zijn (bijv. bij de laatste stap in de lijst)
            if (completedSteps.Count >= correctOrder.Count)
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
                // Maak alle NPC's klikbaar
                NPCInteraction[] alleNPCs = Object.FindObjectsByType<NPCInteraction>(FindObjectsSortMode.None);
                foreach (NPCInteraction npc in alleNPCs) 
                { 
                    npc.EnableOutlineCapability(); 
                }
                break;

            case "Omstanders Aangesproken": 
                mijnUIManager.ShowInteractionSequence(); 
                if (victim != null) victim.EnableVictimInteraction(); 
                break;

            case "Bewustzijn Check":
                mijnUIManager.ShowVictimReactionSequence();
                // Activeer de schouder-interactie
                if (leftShoulderZone != null) leftShoulderZone.SetActive(true);
                if (rightShoulderZone != null) rightShoulderZone.SetActive(true);
                if (ghostHandsSchudden != null) ghostHandsSchudden.SetActive(true);
                
                // Na 5 seconden schudden gaan we automatisch door naar 112
                Invoke("OnSchudAnimatieKlaar", 5f);
                break;

            case "112 Bellen":
                // Ruim de schud-fase op
                if (leftShoulderZone != null) leftShoulderZone.SetActive(false);
                if (rightShoulderZone != null) rightShoulderZone.SetActive(false);
                if (ghostHandsSchudden != null) ghostHandsSchudden.SetActive(false);

                mijnUIManager.ToonTekst("Klik op de omstander en zeg: 'Bel 112, we hebben een hartstilstand!'");

                // Maak de NPC weer klikbaar voor de telefoon actie
                if (omstanderNPC != null) omstanderNPC.ResetForPhoneCall();
                break;

            case "Luchtweg Openen":
                mijnUIManager.ToonTekst("Pas de kinlift toe om de luchtweg te openen.");
                if (ghostHandsLuchtweg != null) ghostHandsLuchtweg.SetActive(true);
                break;
        }
    }

    public void OnSchudAnimatieKlaar()
    {
        RegisterStep("112 Bellen");
    }

    public void StartPhoneTimer()
    {
        // Start de timer van 5 seconden voor de telefoon-actie (animatie van NPC)
        Invoke("OnPhoneCallFinished", 5f);
    }

    private void OnPhoneCallFinished()
    {
        RegisterStep("Luchtweg Openen");
    }

    public string GetCurrentStep()
    {
        return completedSteps.Count > 0 ? completedSteps[completedSteps.Count - 1] : "";
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

    private void ValidateOrder()
    {
        bool isCorrect = true;
        // Simpele check: komt onze lijst overeen met de correctOrder lijst?
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

    private void ShowSummary(bool isCorrect)
    {
        if (summaryPanel != null) summaryPanel.SetActive(true);
        if (summaryText != null)
        {
            string result = isCorrect ? "<color=green>Correct!</color>" : "<color=red>Foutieve volgorde!</color>";
            summaryText.text = $"Eindresultaat: {result}\nStappen voltooid: {completedSteps.Count}";
        }
        
        if (mijnUIManager != null) 
        {
            mijnUIManager.LaatEindSchermZien(isCorrect, completedSteps);
        }
    }
}