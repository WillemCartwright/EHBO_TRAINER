using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scriptbasisdetectie : MonoBehaviour
{
    [SerializeField] private float requiredDuration = 2.0f;
    [SerializeField] private clipboard clipboardTasks;
    [SerializeField] private string taskToComplete;

    [Header("Trigger Settings")]
    [SerializeField] private List<GameObject> objectsToDeActivateOnEnter;
    [SerializeField] private List<GameObject> objectsToActivateOnExit;

    private float elapsedActionTime = 0.0f;
    private bool isCountingActionTime = false;
    private BoxCollider triggerCollider;
    private bool isTaskFinished = false; // Voorkom dat de taak meerdere keren vuurt

    void Start()
    {
        triggerCollider = GetComponent<BoxCollider>();
    }

    void OnTriggerEnter(Collider other)
    {
        // FIX 1: Luister naar beide tags
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
        // FIX 2: Ook hier beide tags controleren
        if (other.CompareTag("Player") || other.CompareTag("GameController"))
        {
            isCountingActionTime = false;
            elapsedActionTime = 0.0f;
            Debug.Log("Handen van schouders afgehaald. Timer gereset.");

            if (triggerCollider != null)
                triggerCollider.size /= 3f;

            // Alleen activeren op exit als de taak NIET af is (bijv. ghost hands terugzetten)
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
        Debug.Log("<color=green>Schudden voltooid! Seintje sturen naar Checker...</color>");

        // FIX 3: Direct de StappenChecker aanroepen in plaats van alleen het clipboard
        if (EHBOStappenChecker.Instance != null)
        {
            EHBOStappenChecker.Instance.RegisterStep(taskToComplete);
        }

        // Optioneel: Zet de schouderzone zelf uit zodat je niet per ongeluk opnieuw triggert
        this.gameObject.SetActive(false);
    }
}