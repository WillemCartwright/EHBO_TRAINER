using UnityEngine;
using UnityEngine.InputSystem;

public class ClipboardScaleController : MonoBehaviour
{
    [Header("Anchors")]
    public Transform normalAnchor;
    public Transform focusAnchor;

    [Header("Instellingen")]
    public float transitionSpeed = 15f; // Iets sneller voor minder 'lag'
    public InputActionProperty toggleAction;

    private bool isFocused = false;

    void OnEnable()
    {
        if (toggleAction.action != null)
        {
            toggleAction.action.Enable();
        }
    }

    void Start()
    {
        // Extra veiligheid: Mocht het Input System bij OnEnable nog niet klaar zijn,
        // dan forceren we de knop hier bij de start nogmaals aan.
        if (toggleAction.action != null && !toggleAction.action.enabled)
        {
            toggleAction.action.Enable();
        }

        // Zorg dat het klembord direct naar zijn beginpositie schiet
        Transform target = isFocused ? focusAnchor : normalAnchor;
        if (target != null)
        {
            transform.position = target.position;
            transform.rotation = target.rotation;
            transform.localScale = target.localScale;
        }
    }

    void LateUpdate() // LateUpdate werkt beter voor objecten die de camera volgen
    {
        if (toggleAction.action.WasPressedThisFrame())
        {
            isFocused = !isFocused;
            Debug.Log("Switching Clipboard. Focused: " + isFocused);
        }

        Transform target = isFocused ? focusAnchor : normalAnchor;

        if (target != null)
        {
            // Gebruik MoveTowards of een snelle Lerp voor stabiliteit
            transform.position = Vector3.Lerp(transform.position, target.position, Time.deltaTime * transitionSpeed);
            transform.rotation = Quaternion.Slerp(transform.rotation, target.rotation, Time.deltaTime * transitionSpeed);
            transform.localScale = Vector3.Lerp(transform.localScale, target.localScale, Time.deltaTime * transitionSpeed);
        }
    }
}