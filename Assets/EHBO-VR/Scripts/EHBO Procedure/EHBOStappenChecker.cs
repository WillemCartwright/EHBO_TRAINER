using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EHBOStappenChecker : MonoBehaviour
{
    [SerializeField] private List<string> correctOrder;
    private List<string> completedSteps = new List<string>();
    
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI debugPanelText;
    [SerializeField] private GameObject summaryPanel;
    [SerializeField] private TextMeshProUGUI summaryText;

    [Header("NPC Settings")]
    // SLEEP HIER JE NPC'S IN (die de Animator hebben)
    [SerializeField] private List<Animator> npcAnimators;

    void Start()
    {
        // Veiligheidscheck: alleen SetActive doen als het panel is ingevuld
        if (summaryPanel != null) summaryPanel.SetActive(false);
        
        DisplayDebugInfo();
    }

    // DEZE FUNCTIE MOET WORDEN AANGEROEPEN ALS DE MAN VALT
    public void VictimHasFallen()
    {
        Debug.Log("Victim is gevallen! NPC's worden geactiveerd.");
        foreach (Animator anim in npcAnimators)
        {
            if (anim != null)
            {
                anim.SetBool("shocked", true);
            }
        }
    }

    public void RegisterStep(string stepName)
    {
        if (completedSteps.Count == 0 || completedSteps[completedSteps.Count - 1] != stepName)
        {
            completedSteps.Add(stepName);
            DisplayDebugInfo();

            if (stepName == "hart compressie" || completedSteps.Count == correctOrder.Count)
            {
                ValidateOrder();
            }
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
        if (debugPanelText == null) return; // Voorkom error als vakje leeg is

        string debugPanelString = "Steps Completed:\n";
        for (int i = 0; i < completedSteps.Count; i++)
        {
            debugPanelString += $"{i + 1}. {completedSteps[i]}\n";
        }
        debugPanelText.text = debugPanelString;
    }

    private void ShowSummary(bool isCorrect)
    {
        if (summaryPanel != null) summaryPanel.SetActive(true);
        if (summaryText == null) return;

        string result = isCorrect ? "Correct Order!" : "Incorrect Order!";
        string summary = "Order of Steps Completed:\n";

        for (int i = 0; i < completedSteps.Count; i++)
        {
            summary += $"{i + 1}. {completedSteps[i]}\n";
        }

        if (!isCorrect)
        {
            summary += "\nCorrect Order:\n";
            for (int i = 0; i < correctOrder.Count; i++)
            {
                summary += $"{i + 1}. {correctOrder[i]}\n";
            }
        }

        summary += $"\nResult: {result}";
        summaryText.text = summary;
    }
}