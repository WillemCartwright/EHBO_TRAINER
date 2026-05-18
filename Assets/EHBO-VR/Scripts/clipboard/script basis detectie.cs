using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scriptbasisdetectie : MonoBehaviour
{
    [SerializeField] private float requiredDuration = 2.0f;
    [SerializeField] private clipboard clipboardTasks;
    [SerializeField] private string taskToComplete;

    [Header("NPC Reset Settings")]
    [SerializeField] private NPCInteraction bystanderNPC; // Sleep hier de NPC (omstander) in!

    [Header("Trigger Settings")]
    [SerializeField] private List<GameObject> objectsToDeActivateOnEnter;
    [SerializeField] private List<GameObject> objectsToActivateOnExit;

    private float elapsedActionTime = 0.0f;
    private bool isCountingActionTime = false;
    private BoxCollider triggerCollider;
    private bool isTaskFinished = false; 

    void Start()
    {
        triggerCollider = GetComponent<BoxCollider>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("GameController"))
        {
            if (isTaskFinished) return;

            isCountingActionTime = true;
            Debug.Log("Hand gedetecteerd op schouder. Timer loopt...");

            if (triggerCollider != null)
                triggerCollider.size *= 3f;

            foreach (GameObject obj in objectsToDeActivateOnEnter)
                if (obj != null) obj.SetActive(false);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("GameController"))
        {
            isCountingActionTime = false;
            elapsedActionTime = 0.0f;
            Debug.Log("Handen van schouders afgehaald. Timer gereset.");

            if (triggerCollider != null)
                triggerCollider.size /= 3f;

            if (!isTaskFinished)
            {
                foreach (GameObject obj in objectsToActivateOnExit)
                    if (obj != null) obj.SetActive(true);
            }
        }
    }

    void Update()
    {
        if (isCountingActionTime && !isTaskFinished)
        {
            elapsedActionTime += Time.deltaTime;

            if (elapsedActionTime >= requiredDuration)
            {
                CompleteTask();
            }
        }
    }

    public void CompleteTask()
    {
        isTaskFinished = true;
        isCountingActionTime = false;
        Debug.Log("<color=green>Schudden voltooid! Seintje sturen naar Checker en NPC resetten...</color>");

        // 1. Registreer de stap in de checker
        if (EHBOStappenChecker.Instance != null)
        {
            EHBOStappenChecker.Instance.RegisterStep(taskToComplete);
        }

        // 2. Reset de NPC zodat de outline voor 112 weer werkt
        if (bystanderNPC != null)
        {
            bystanderNPC.ResetForPhoneCall();
        }
        else
        {
            Debug.LogWarning("Geen bystanderNPC toegewezen in de Inspector van de zone!");
        }

        // De zone zelf uitschakelen zodat je niet dubbel registreert
        this.gameObject.SetActive(false);
    }
}