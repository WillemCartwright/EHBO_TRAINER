using UnityEngine;

public class KinliftTouchLogic : MonoBehaviour
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

        if (KinliftManager.Instance != null)
        {
            // Geeft de status door aan de manager
            KinliftManager.Instance.UpdateHandStatus(this.gameObject.name, isHandInside);
        }
    }

    // De HideOutline en SetColor functies zijn verwijderd omdat we geen outlines meer gebruiken.
}