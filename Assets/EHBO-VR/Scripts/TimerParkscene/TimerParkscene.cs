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
    [SerializeField] EHBOStappenChecker stappenChecker; 

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
            // Start het incident (vallen slachtoffer)
            // Dit zorgt ervoor dat de Animator van de man en de hond uiteindelijk naar 'Shocked' gaan
            if (incidentScript != null)
            {
                incidentScript.Activate();
            }

            // Deactiveer deze trigger zodat hij niet nog een keer afgaat
            GetComponent<Collider>().enabled = false;
        }
    }
}