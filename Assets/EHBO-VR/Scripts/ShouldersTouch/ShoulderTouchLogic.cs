using UnityEngine;

public class ShoulderTouchLogic : MonoBehaviour
{
    private bool isHandInside = false;

    private void OnTriggerEnter(Collider other)
    {
        // Check of interactie mag
        if (UIManager.Instance != null && !UIManager.Instance.CanInteract()) return;

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
        if (UIManager.Instance != null && !UIManager.Instance.CanInteract()) return;

        if (ShoulderSequenceManager.Instance != null)
        {
            // Geeft de status door aan de manager
            ShoulderSequenceManager.Instance.UpdateHandStatus(this.gameObject.name, isHandInside);
        }
    }

    // De HideOutline en SetColor functies zijn verwijderd omdat we geen outlines meer gebruiken.
}