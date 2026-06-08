using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class IncidentCountdown : MonoBehaviour
{
    [Header("Countdown Settings")]
    [SerializeField] private bool IsActive = false;
    [SerializeField] private float countdownTime = 5f;

    [Header("Victim (De man die valt)")]
    [SerializeField] private Animator victimAnimator; 

    [Header("Responders (De 2 speciale NPC's die rennen)")]
    [SerializeField] private GameObject[] responders; 

    [Header("Other References")]
    [SerializeField] private AudioSource countdownEndSound; 
    [SerializeField] private TimerParkscene mainTimer; 

    private float currentTime;
    private bool countdownFinished = false;

    void Start()
    {
        currentTime = countdownTime;
        
        // Zorg dat de responders bij de start ECHT stilstaan en hun script uit staat
        foreach (GameObject responder in responders)
        {
            if (responder != null)
            {
                var movement = responder.GetComponent<NPCMovement>();
                if (movement != null) movement.enabled = false;
                
                Animator anim = responder.GetComponent<Animator>();
                if (anim != null) anim.SetBool("shocked", false);
            }
        }
    }

    void Update()
    {
        if (IsActive && !countdownFinished)
        {
            currentTime -= Time.deltaTime;

            if (currentTime <= 0f)
            {
                ActivateIncident();
            }
        }
    }

    void ActivateIncident()
    {
        countdownFinished = true;

        // 1. Start de 300s timer bovenin
        if (mainTimer != null) mainTimer.StartRealTimer();

        // 2. LAAT DE VICTIM DIRECT VALLEN
        if (victimAnimator != null)
        {
            victimAnimator.SetBool("shocked", true);
            Debug.Log("Victim valt nu!");
        }

        // 3. START DE DELAY VAN 3 SECONDEN VOOR DE NPC'S
        StartCoroutine(WaitAndThenRun(3f));

        // Stop omstanders op de achtergrond
        NavMeshAgent[] navAgents = FindObjectsOfType<NavMeshAgent>();
        foreach (NavMeshAgent agent in navAgents) { agent.speed = 0; }

        if (countdownEndSound != null) countdownEndSound.Play();
    }

    private IEnumerator WaitAndThenRun(float delay)
    {
        Debug.Log("Wachten op collapse... (3 seconden)");
        yield return new WaitForSeconds(delay);

        Debug.Log("Delay voorbij! NPC's gaan nu rennen.");

        // --- DEEL A: Jouw eigen 2 speciale responders activeren ---
        foreach (GameObject responder in responders)
        {
            if (responder != null)
            {
                // Zet het vinkje 'shocked' aan bij de NPC
                Animator anim = responder.GetComponent<Animator>();
                if (anim != null) anim.SetBool("shocked", true);

                // Zet het script aan zodat ze gaan bewegen
                var movement = responder.GetComponent<NPCMovement>();
                if (movement != null) movement.enabled = true;
            }
        }

        // --- DEEL B: NIEUW - Stuur alle overige achtergrond-idlers aan ---
        CharacterSwarmer[] allIdlers = FindObjectsByType<CharacterSwarmer>(FindObjectsSortMode.None);
        foreach (CharacterSwarmer idler in allIdlers)
        {
            idler.StartSurrounding();
        }
        
        Debug.Log($"Geactiveerd: {allIdlers.Length} extra idlers lopen nu naar het slachtoffer!");
    }

    public void Activate() { IsActive = true; }
}