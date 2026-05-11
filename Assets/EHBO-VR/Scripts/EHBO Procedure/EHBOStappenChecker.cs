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

    [Header("Fysieke Objecten - Zones")]
    [Tooltip("De Master Zone voor beide schouders met het basisdetectiescript")]
    [SerializeField] private GameObject HandenDetectieSchouders;
    [SerializeField] private GameObject HandenDetectieKinlift;

    [Header("Fysieke Objecten - Overig")]
    [SerializeField] private VictimInteraction victim;
    [SerializeField] private NPCInteraction omstanderNPC; 

    [Header("Ghost Hands")]
    [SerializeField] private GameObject ghostHandsSchudden; 
    [SerializeField] private GameObject ghostHandsLuchtweg; 

    [Header("NPC Settings")]
    [SerializeField] private List<Animator> npcAnimators;

    [Header("Klembord Koppeling")]
    [SerializeField] private clipboard mijnKlembord;

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
        if (HandenDetectieSchouders) HandenDetectieSchouders.SetActive(false);
        if (HandenDetectieKinlift) HandenDetectieKinlift.SetActive(false);

        // Ghost Hands uitzetten
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
        
        if (mijnKlembord != null) 
            mijnKlembord.RegisterTaskCompletion(stepName);

        Debug.Log("<color=green>STAP VOLTOOID:</color> " + stepName);
        DeactiveerAlleInteracties();
        TriggerFaseLogica(stepName);

        if (completedSteps.Count >= correctOrder.Count)
            ValidateOrder();
    }
}

private void TriggerFaseLogica(string stepName)
{
    switch (stepName)
    {
        case "Start Incident":
            DeactiveerAlleInteracties();
            NPCInteraction[] alleNPCs = Object.FindObjectsByType<NPCInteraction>(FindObjectsSortMode.None);
            foreach (NPCInteraction npc in alleNPCs) npc.EnableOutlineCapability();
            break;

        case "Omstanders Aangesproken": 
            DeactiveerAlleInteracties();
            if (HandenDetectieSchouders) HandenDetectieSchouders.SetActive(true);
            if (ghostHandsSchudden) ghostHandsSchudden.SetActive(true);
            break;

        case "Bewustzijn Check":
            DeactiveerAlleInteracties();
            if (omstanderNPC != null) omstanderNPC.ResetForPhoneCall();
            break;

        case "112 Bellen":
            DeactiveerAlleInteracties();

            if (HandenDetectieKinlift != null) 
            {
                HandenDetectieKinlift.SetActive(true);
            }
            if (ghostHandsLuchtweg != null) 
            {
                ghostHandsLuchtweg.SetActive(true);
            }
            Debug.Log("112 Bellen voltooid. Kinlift zones zijn nu direct actief.");
            break;

        case "Luchtweg Openen":
            // Deze stap wordt bereikt nadat je 2 seconden de kinlift zone hebt aangeraakt
            DeactiveerAlleInteracties(); 
            Debug.Log("Luchtweg voltooid. Volgende fase voorbereiden...");
            break;
    }
}

    // Callback voor als de NPC klaar is met bellen of animatie voltooid is
    public void StartPhoneTimer() => Invoke("OnPhoneCallFinished", 5f);
    private void OnPhoneCallFinished() => RegisterStep("Luchtweg Openen");

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
        DeactiveerAlleInteracties();
        if (summaryPanel) summaryPanel.SetActive(true);
        if (summaryText)
        {
            string result = isCorrect ? "<color=green>Correct uitgevoerd!</color>" : "<color=red>Foutieve volgorde!</color>";
            summaryText.text = $"Eindresultaat: {result}\nStappen: {completedSteps.Count}/{correctOrder.Count}";
        }
        if (mijnUIManager) mijnUIManager.LaatEindSchermZien(isCorrect, completedSteps);
    }

    public string GetCurrentStep()
    {
        if (completedSteps.Count > 0)
        {
            return completedSteps[completedSteps.Count - 1];
        }
        return ""; // Geef een lege tekst terug als er nog geen stappen zijn
    }
}