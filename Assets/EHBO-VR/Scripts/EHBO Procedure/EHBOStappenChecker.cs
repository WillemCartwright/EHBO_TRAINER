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
    [SerializeField] private GameObject HandenDetectieHart;
    [SerializeField] private GameObject HandenDetectieHartRonde2;
    [SerializeField] private GameObject HandenDetectieBeademing;

    [Header("Fysieke Objecten - Overig")]
    [SerializeField] private VictimInteraction victim;
    [SerializeField] private NPCInteraction omstanderNPC; 
    
    // --- NIEUW: Referentie naar de telefoon op de hand van de speler ---
    [Tooltip("Sleep hier het 'phone' GameObject uit de Hand Rig van de player in")]
    [SerializeField] private GameObject spelerTelefoon; 

    [Header("Ghost Hands")]
    [SerializeField] private GameObject ghostHandsSchudden; 
    [SerializeField] private GameObject ghostHandsLuchtweg; 
    [SerializeField] private GameObject ghostHandsHartcompressie;
    [SerializeField] private GameObject ghostHandsBeademing;

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
        if (HandenDetectieHart) HandenDetectieHart.SetActive(false);
        if (HandenDetectieBeademing) HandenDetectieBeademing.SetActive(false);

        if (ghostHandsSchudden) ghostHandsSchudden.SetActive(false);
        if (ghostHandsLuchtweg) ghostHandsLuchtweg.SetActive(false);
        if (ghostHandsHartcompressie) ghostHandsHartcompressie.SetActive(false);
        if (ghostHandsBeademing) ghostHandsBeademing.SetActive(false);
    }

    public void VictimHasFallen()
    {
        foreach (Animator anim in npcAnimators)
        {
            if (anim != null) anim.SetBool("shocked", true);
        }

        if (spelerTelefoon != null)
        {
            spelerTelefoon.SetActive(false);
            Debug.Log("[STAPPENCHECKER] Slachtoffer gevallen: telefoon van speler direct uitgeschakeld.");
        }
        else
        {
            Debug.LogWarning("[STAPPENCHECKER] spelerTelefoon is niet toegewezen in de Inspector!");
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

            DisplayDebugInfo(); // Update meteen het debug-paneel in VR

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

            case "Tik de omstander aan zodat hij in de buurt blijft": 
                DeactiveerAlleInteracties();
                if (HandenDetectieSchouders) HandenDetectieSchouders.SetActive(true);
                if (ghostHandsSchudden) ghostHandsSchudden.SetActive(true);
                break;

            case "Bewustzijn Check":
                DeactiveerAlleInteracties();
                if (omstanderNPC != null) omstanderNPC.ResetForPhoneCall();
                break;

            case "Het slachtoffer is bewusteloos. Laat de omstander 112 voor je bellen":
                DeactiveerAlleInteracties();
                if (HandenDetectieKinlift != null) HandenDetectieKinlift.SetActive(true);
                if (ghostHandsLuchtweg != null) ghostHandsLuchtweg.SetActive(true);
                Debug.Log("112 Bellen voltooid. Kinlift zones zijn nu direct actief.");
                break;

            case "Open de luchtweg van het slachtoffer door het hoofd naar achter te kantelen":
                DeactiveerAlleInteracties(); 
                if (HandenDetectieHart) HandenDetectieHart.SetActive(true);
                if (ghostHandsHartcompressie) ghostHandsHartcompressie.SetActive(true);
                Debug.Log("Luchtweg voltooid. Hartcompressie zones zijn nu actief.");
                break;

            case "Voer 30 borstcompressies uit met een snelheid van 2 compressies per seconde": 
                if (HandenDetectieBeademing) HandenDetectieBeademing.SetActive(true);
                if (ghostHandsBeademing) ghostHandsBeademing.SetActive(true);
                Debug.Log("Hartcompressie voltooid. Beademingsfase start NU!");
                break;

            case "Geef het slachtoffer mond-op-mondbeademing. Blaas binnen tien seconden twee keer in de mond": 
                DeactiveerAlleInteracties();
                Debug.Log("Beademing voltooid! Volgende cyclus voorbereiden...");
                
                GameObject omstander = GameObject.Find("npc_csl_00_character_01m_01"); 
                if (omstander != null)
                {
                    var movement = omstander.GetComponent<NPCMovement>();
                    if (movement != null)
                    {
                        movement.RentTerugMetAED();
                    }
                    else
                    {
                        Debug.LogError("[STAPPENCHECKER] NPCMovement script niet gevonden op de omstander!");
                    }
                }
                else
                {
                    Debug.LogError("[STAPPENCHECKER] Kan 'omstanderNPC' niet vinden in de Hierarchy!");
                }
                break;

            // --- HOOFDLETTER FIX VOOR DE AED STAP ---
            case "AED aansluiten":
                DeactiveerAlleInteracties();
                Debug.Log("<color=orange>[FASE]</color> AED stap is actief. We wachten tot de speler de schok toedient...");
                break;
            
            case "Herhaal borstcompressies":
                DeactiveerAlleInteracties();
                
                // 1. Zet de nieuwe zone aan voor ronde 2! 
                if (HandenDetectieHartRonde2 != null) 
                {
                    HandenDetectieHartRonde2.SetActive(true);

                    // HARD RESET VOOR DE NIEUWE ZONE:
                    scriptbasisdetectie zoneScript2 = HandenDetectieHartRonde2.GetComponent<scriptbasisdetectie>();
                    if (zoneScript2 != null)
                    {
                        zoneScript2.isTaskFinished = false;           
                        zoneScript2.isCountingActionTime = false;      
                        zoneScript2.elapsedActionTime = 0.0f; // <-- DE CRUCIALE FIX: Zet de klok voor zone 2 op 0!
                        Debug.Log("<color=orange>[RESET]</color> Zone Ronde 2 klok op 0 gezet!");
                    }
                }
                
                // 2. Zet de Ghost Hands aan
                if (ghostHandsHartcompressie) ghostHandsHartcompressie.SetActive(true);
                
                // 3. Schakel de herhaalmodus in op de Ghost Hands
                if (ghostHandsHartcompressie != null) 
                {
                    GhostHandAnimatie animScript = ghostHandsHartcompressie.GetComponent<GhostHandAnimatie>();
                    if (animScript != null)
                    {
                        animScript.gameObject.SetActive(true);
                        animScript.isHerhalingsStap = true; 
                    }
                }
                
                Debug.Log("<color=orange>[FASE]</color> Nieuwe zone voor ronde 2 start nu gegarandeerd vanaf 0 seconden.");
                break;

            case "Hulpverleners nemen over":
                DeactiveerAlleInteracties();
                Debug.Log("<color=green>[FINALE]</color> De ambulance is gearriveerd! Scenario succesvol doorlopen.");
                
                // Hier kun je eventueel nog een sirene-geluid afspelen of direct de evaluatie triggeren
                break;
        }
    }

    public void StartPhoneTimer() => Invoke("OnPhoneCallFinished", 5f);
    private void OnPhoneCallFinished() => RegisterStep("Open de luchtweg van het slachtoffer door het hoofd naar achter te kantelen");

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
        return ""; 
    }
}