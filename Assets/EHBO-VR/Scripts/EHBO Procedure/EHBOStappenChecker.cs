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

    public WaypointMover ambulanceMovement; // Sleep hier straks de ambulance in

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
        if (HandenDetectieHartRonde2) HandenDetectieHartRonde2.SetActive(false);

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
        // Zet altijd direct alle interacties uit zodat er nooit twee zones tegelijk aan staan
        DeactiveerAlleInteracties();

        switch (stepName)
        {
            case "Start Incident":
                // Start -> Nu mag je de omstander aantikken
                break;

            case "Tik de omstander aan zodat hij in de buurt blijft": 
                // Omstander aangetikt -> Nu pas mag je de Bewustzijn Check doen!
                if (HandenDetectieSchouders) HandenDetectieSchouders.SetActive(true);
                if (ghostHandsSchudden) ghostHandsSchudden.SetActive(true);
                break;

            case "Bewustzijn Check":
                // Bewustzijn Check gedaan -> Nu start de omstander met 112 bellen
                if (omstanderNPC != null) omstanderNPC.ResetForPhoneCall();
                break;

            case "Het slachtoffer is bewusteloos. Laat de omstander 112 voor je bellen":
                // 112 bellen is KLAAR -> Nu pas activeren we de Kinlift/Luchtweg zone!
                if (HandenDetectieKinlift != null) HandenDetectieKinlift.SetActive(true);
                if (ghostHandsLuchtweg != null) ghostHandsLuchtweg.SetActive(true);
                break;

            case "Open de luchtweg van het slachtoffer door het hoofd naar achter te kantelen":
                // Luchtweg/Kinlift is NU pas echt gedaan -> Nu pas mag je gaan REANIMEREN!
                if (HandenDetectieHart) HandenDetectieHart.SetActive(true);
                if (ghostHandsHartcompressie) ghostHandsHartcompressie.SetActive(true);
                break;

            case "Voer 30 borstcompressies uit met een snelheid van 2 compressies per seconde": 
                // Eerste reanimatie klaar -> Nu pas mag je gaan BEADEMEN!
                if (HandenDetectieBeademing) HandenDetectieBeademing.SetActive(true);
                if (ghostHandsBeademing) ghostHandsBeademing.SetActive(true);
                break;

            case "Geef het slachtoffer mond-op-mondbeademing. Blaas binnen tien seconden twee keer in de mond": 
                // Beademing klaar -> Omstander rent weg voor de AED
                GameObject omstander = GameObject.Find("npc_csl_00_character_01m_01"); 
                if (omstander != null)
                {
                    var movement = omstander.GetComponent<NPCMovement>();
                    if (movement != null) movement.RentTerugMetAED();
                }
                break;

            case "AED aansluiten":
                // AED-schok en audio zijn klaar! Nu starten we direct de herhalingsronde.
                Debug.Log("<color=yellow>[STAPPENCHECKER]</color> AED klaar. Zones voor herhaling borstcompressies worden nu aangezet!");
                
                // Activeer direct de zone voor ronde 2
                if (HandenDetectieHartRonde2 != null) 
                {
                    HandenDetectieHartRonde2.SetActive(true);
                    scriptbasisdetectie zoneScript2 = HandenDetectieHartRonde2.GetComponent<scriptbasisdetectie>();
                    if (zoneScript2 != null)
                    {
                        zoneScript2.isTaskFinished = false;           
                        zoneScript2.isCountingActionTime = false;      
                        zoneScript2.elapsedActionTime = 0.0f; 
                    }
                }
                
                // Activeer de ghost hands voor ronde 2
                if (ghostHandsHartcompressie != null) 
                {
                    GhostHandAnimatie animScript = ghostHandsHartcompressie.GetComponent<GhostHandAnimatie>();
                    if (animScript != null)
                    {
                        ghostHandsHartcompressie.SetActive(true);
                        animScript.isHerhalingsStap = true; 
                    }
                }
                break;
            
            case "Herhaal borstcompressies":
                // Deze case mag nu leeg blijven. Hij wordt pas bereikt als de GhostHandAnimatie klaar is
                // en RegisterStep("Herhaal borstcompressies") heeft aangeroepen.
                Debug.Log("<color=yellow>[STAPPENCHECKER]</color> Herhaal borstcompressies is zojuist afgevinkt!");
                break;

            case "Hulpverleners nemen over":
                Debug.Log("<color=green>[FINALE]</color> Ambulance is er.");
                
                // Activeer het rijden van de ambulance!
                if (ambulanceMovement != null)
                {
                    ambulanceMovement.StartRijden();
                }
                else
                {
                    Debug.LogError("AmbulanceMovement script mist op het fasescript!");
                }
                break;
        }
    }

    public void StartPhoneTimer() => Invoke("OnPhoneCallFinished", 5f);
    
    private void OnPhoneCallFinished() 
    {
        // GECORRIGEERD: De telefoon vinkt nu netjes de 112-stap af als hij klaar is!
        RegisterStep("Het slachtoffer is bewusteloos. Laat de omstander 112 voor je bellen");
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