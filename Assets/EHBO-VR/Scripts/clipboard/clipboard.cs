using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class clipboard : MonoBehaviour
{
    [System.Serializable]
    public class Task
    {
        [Tooltip("De INTERNE NAAM die de code gebruikt (b v. 'Bewustzijn Check'). MOET matchen met de stappenchecker!")] 
        public string taskID;

        [Tooltip("De VISUELE TEKST die de speler op het klembord ziet zodra de stap is GEHAALD.")] 
        public string displayOriginalText;

        [Tooltip("Image to display task state")] public RawImage taskImage;
        [Tooltip("Text for the task description")] public TextMeshProUGUI taskText;
        [Tooltip("Placeholder text for unrevealed tasks")] public string placeholderText;
        [Tooltip("Texture for completed task")] public Texture completedTexture;

        [Header("Voice-over")]
        public AudioClip voiceOver;
    }

    [Header("Task Settings")]
    [SerializeField] private List<Task> tasks = new List<Task>();
    private int currentTaskIndex = 0;

    [Header("Voice-over Settings")]
    [SerializeField] private GameObject voiceOverManager;

    [SerializeField] private Achievement achievementManager;

    private AudioSource audioSource;

    void Start()
    {
        InitializeTasks();

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    public void StartVRTutorialVoiceOver()
    {
        StartCoroutine(PlayVoiceOverWithDelay(tasks[0].voiceOver, 4f));
    }

    // Iedere taak start netjes met de placeholderText ("?")
    private void InitializeTasks()
    {
        for (int i = 0; i < tasks.Count; i++)
        {
            tasks[i].taskText.text = tasks[i].placeholderText;
        }
    }

    // --- GEWIJZIGD: We controleren nu op 'taskID' in plaats van de visuele tekst! ---
    public void RegisterTaskCompletion(string taskName)
    {
        if (currentTaskIndex < tasks.Count && tasks[currentTaskIndex].taskID == taskName)
        {
            CompleteCurrentTask();
        }
        else
        {
            Debug.Log($"Task '{taskName}' ignored. It is not the current task.");
        }
    }

    private void CompleteCurrentTask()
    {
        Task currentTask = tasks[currentTaskIndex];

        VoiceOver voiceOverComponent = voiceOverManager.GetComponent<VoiceOver>();
        if (voiceOverComponent != null)
        {
            voiceOverComponent.StopVoiceOver();
        }

        if (achievementManager != null)
        {
            achievementManager.UnlockAchievement(Achievement.TutorialType.VR, currentTaskIndex + 1);
        }

        // --- NIEUW: Nu de taak GEHAALD is, tonen we de mooie 'displayOriginalText'! ---
        currentTask.taskText.text = currentTask.displayOriginalText;

        if (currentTask.completedTexture != null)
            currentTask.taskImage.texture = currentTask.completedTexture;

        currentTaskIndex++;
        if (currentTaskIndex < tasks.Count)
        {
            Task nextTask = tasks[currentTaskIndex];
            
            // De volgende taak blijft uiteraard op de placeholder ("?") staan
            nextTask.taskText.text = nextTask.placeholderText;

            if (voiceOverComponent != null)
            {
                StartCoroutine(PlayVoiceOverWithDelay(nextTask.voiceOver, 2f));
            }
        }
        else
        {
            Debug.Log("All tasks completed!");
        }
    }

    private IEnumerator PlayVoiceOverWithDelay(AudioClip clip, float delay)
    {
        if (clip == null || voiceOverManager == null)
            yield break;

        yield return new WaitForSeconds(delay);

        VoiceOver voiceOverComponent = voiceOverManager.GetComponent<VoiceOver>();
        if (voiceOverComponent != null)
            voiceOverComponent.PlayVoiceOver(clip);
    }
}