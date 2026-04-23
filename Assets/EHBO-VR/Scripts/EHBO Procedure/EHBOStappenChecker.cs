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

    [Header("Fysieke Objecten - Zones")]
    [SerializeField] private GameObject leftShoulderZone;
    [SerializeField] private GameObject rightShoulderZone;
    [SerializeField] private GameObject zoneVoorhoofd;
    [SerializeField] private GameObject zoneKin;

    [Header("Fysieke Objecten - Overig")]
    [SerializeField] private VictimInteraction victim;
    [SerializeField] private NPCInteraction omstanderNPC; 

    [Header("Ghost Hands")]
    [SerializeField] private GameObject ghostHandsSchudden; 
    [SerializeField] private GameObject ghostHandsLuchtweg; 

    [Header("NPC Settings")]
    [SerializeField] private List<Animator> npcAnimators;

    void Awake()
    {
        if (Instance == null) Instance = this;
        DeactiveerAlleInteracties();
    }

    void Start()
    {
        if (summaryPanel != null) summaryPanel.SetActive(false);
        DisplayDebugInfo();
    }

    private void DeactiveerAlleInteracties()
    {
        // Zones
        if (leftShoulderZone) leftShoulderZone.SetActive(false);
        if (rightShoulderZone) rightShoulderZone.SetActive(false);
        if (zoneVoorhoofd) zoneVoorhoofd.SetActive(false);
        if (zoneKin) zoneKin.SetActive(false);

        // Ghost Hands
        if (ghostHandsSchudden) ghostHandsSchudden.SetActive(false);
        if (ghostHandsLuchtweg) ghostHandsLuchtweg.SetActive(false);
    }

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
        if (completedSteps.Count == 0 || completedSteps[completedSteps.Count - 1] != stepName)
        {
            completedSteps.Add(stepName);
            DisplayDebugInfo();
            TriggerFaseLogica(stepName);

            if (completedSteps.Count >= correctOrder.Count)
            {
                ValidateOrder();
            }
        }
    }

    private void TriggerFaseLogica(string stepName)
    {
        if (mijnUIManager == null) return;

        // Maak eerst het veld schoon voordat we de nieuwe fase opbouwen
        DeactiveerAlleInteracties();

        switch (stepName)
        {
            case "Start Incident":
                mijnUIManager.ToonInstructieAankomst();
                NPCInteraction[] alleNPCs = Object.FindObjectsByType<NPCInteraction>(FindObjectsSortMode.None);
                foreach (NPCInteraction npc in alleNPCs) npc.EnableOutlineCapability();
                break;

            case "Omstanders Aangesproken": 
                mijnUIManager.ShowInteractionSequence(); 
                if (victim != null) victim.EnableVictimInteraction(); 
                break;

            case "Bewustzijn Check":
                mijnUIManager.ShowVictimReactionSequence();
                if (leftShoulderZone) leftShoulderZone.SetActive(true);
                if (rightShoulderZone) rightShoulderZone.SetActive(true);
                if (ghostHandsSchudden) ghostHandsSchudden.SetActive(true);
                break;

            case "112 Bellen":
                mijnUIManager.ToonTekst("Het slachtoffer is bewusteloos. Vertel de omstander om 112 te bellen.");
                if (omstanderNPC != null) omstanderNPC.ResetForPhoneCall();
                break;

            case "Luchtweg Openen":
                if (zoneVoorhoofd) zoneVoorhoofd.SetActive(true);
                if (zoneKin) zoneKin.SetActive(true);
                if (ghostHandsLuchtweg) ghostHandsLuchtweg.SetActive(true);
                
                mijnUIManager.ToonTekst("Plaats één hand op het voorhoofd en twee vingers onder de kin.");
                break;
                
            case "Hart Compressie":
                mijnUIManager.ToonTekst("Start nu met 30 borstcompressies.");
                // Hier komen later je reanimatie-zones
                break;
        }
    }

    public void OnSchudAnimatieKlaar() => RegisterStep("112 Bellen");

    public void StartPhoneTimer() => Invoke("OnPhoneCallFinished", 5f);

    private void OnPhoneCallFinished() => RegisterStep("Luchtweg Openen");

    public string GetCurrentStep() => completedSteps.Count > 0 ? completedSteps[completedSteps.Count - 1] : "";

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
        DeactiveerAlleInteracties(); // Alles uit bij einde
        if (summaryPanel) summaryPanel.SetActive(true);
        if (summaryText)
        {
            string result = isCorrect ? "<color=green>Correct!</color>" : "<color=red>Foutieve volgorde!</color>";
            summaryText.text = $"Eindresultaat: {result}\nStappen voltooid: {completedSteps.Count}";
        }
        if (mijnUIManager) mijnUIManager.LaatEindSchermZien(isCorrect, completedSteps);
    }
}