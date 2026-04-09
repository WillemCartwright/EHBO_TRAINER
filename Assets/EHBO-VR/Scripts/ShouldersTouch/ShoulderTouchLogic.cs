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
            shoulderOutline.OutlineColor = Color.white; // Gids-kleur
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check of interactie mag (niet op hond geklikt)
        if (UIManager.Instance != null && !UIManager.Instance.CanInteract()) return;

        isHandInside = true;
        ReportToManager();
    }

    private void OnTriggerExit(Collider other)
    {
        // Bij exit laten we de logica vaak wel doorlopen (om de handstatus te resetten),
        // maar we checken het voor de zekerheid toch om vreemde UI-updates te voorkomen.
        isHandInside = false;
        ReportToManager();
    }

    private void ReportToManager()
    {
        // Ook hier checken we of de game niet al 'over' is door de hond
        if (UIManager.Instance != null && !UIManager.Instance.CanInteract()) return;

        if (ShoulderSequenceManager.Instance != null)
        {
            ShoulderSequenceManager.Instance.UpdateHandStatus(this.gameObject.name, isHandInside);
        }
    }

    public void SetColor(Color c)
    {
        // De manager kan de kleur nog steeds aanpassen, tenzij we ook hier blokkeren
        if (UIManager.Instance != null && !UIManager.Instance.CanInteract()) return;
        
        if (shoulderOutline != null) shoulderOutline.OutlineColor = c;
    }

    public void HideOutline()
    {
        if (shoulderOutline != null) shoulderOutline.enabled = false;
    }
}