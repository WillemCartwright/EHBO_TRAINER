using UnityEngine;
using TMPro;

public class TimerParkscene : MonoBehaviour
{
    [SerializeField] GameObject timerCanvas;
    [SerializeField] TextMeshProUGUI timerparksceneText;
    [SerializeField] float remainingTime = 300f;
    [SerializeField] RectTransform redBar;
    
    [Header("References")]
    [SerializeField] IncidentCountdown incidentScript; 
    [SerializeField] EHBOStappenChecker stappenChecker; // SLEEP HIER JE STAPPENCHECKER IN

    private bool isTimerRunning = false;
    private float totalTime;

    void Start()
    {
        if (timerCanvas != null)
            timerCanvas.SetActive(false);

        totalTime = remainingTime;
        UpdateTimerDisplay();
    }

    public void StartRealTimer()
    {
        isTimerRunning = true;
        if (timerCanvas != null) 
            timerCanvas.SetActive(true);
        
        // --- NIEUW: Vertel de stappenchecker dat de victim is gevallen ---
        if (stappenChecker != null)
        {
            stappenChecker.VictimHasFallen();
        }

        Debug.Log("De 300 seconden timer is gestart!");
    }

    void Update()
    {
        if (isTimerRunning)
        {
            if (remainingTime > 0)
            {
                remainingTime -= Time.deltaTime;
                UpdateTimerDisplay();
                UpdateRedBar();
            }
            else
            {
                remainingTime = 0;
                isTimerRunning = false;
                timerparksceneText.color = Color.red;
                UpdateTimerDisplay();
                UpdateRedBar();
            }
        }
    }

    void UpdateRedBar()
    {
        float fraction = remainingTime / totalTime;
        redBar.localScale = new Vector3(fraction, 1f, 1f);
    }

    void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(remainingTime / 60);
        int seconds = Mathf.FloorToInt(remainingTime % 60);
        timerparksceneText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (incidentScript != null)
            {
                incidentScript.Activate();
            }
            GetComponent<Collider>().enabled = false;
        }
    }
}