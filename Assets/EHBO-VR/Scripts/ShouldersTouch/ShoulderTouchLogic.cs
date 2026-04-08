using UnityEngine;

public class ShoulderTouchLogic : MonoBehaviour
{
    private Outline shoulderOutline;
    private bool isHandInside = false;

    void Awake()
    {
        shoulderOutline = GetComponent<Outline>();
    }

    private void OnEnable()
    {
        if (shoulderOutline != null)
        {
            shoulderOutline.enabled = true;
            shoulderOutline.OutlineColor = Color.white; // Gids-kleur (wachten op beide handen)
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        isHandInside = true;
        ReportToManager();
    }

    private void OnTriggerExit(Collider other)
    {
        isHandInside = false;
        ReportToManager();
    }

    private void ReportToManager()
    {
        if (ShoulderSequenceManager.Instance != null)
        {
            // We geven door of er een hand in deze specifieke zone zit
            ShoulderSequenceManager.Instance.UpdateHandStatus(this.gameObject.name, isHandInside);
        }
    }

    // Functies voor de manager om de kleur van buitenaf aan te passen
    public void SetColor(Color c)
    {
        if (shoulderOutline != null) shoulderOutline.OutlineColor = c;
    }

    public void HideOutline()
    {
        if (shoulderOutline != null) shoulderOutline.enabled = false;
    }
}