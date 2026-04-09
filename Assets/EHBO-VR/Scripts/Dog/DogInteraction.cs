using UnityEngine;
using UnityEngine.SceneManagement;

public class DogInteraction : MonoBehaviour
{
    private Outline dogOutline;

    void Start()
    {
        // We zoeken de component op, maar we zetten 'enabled' NIET meer op true.
        // De Meta Interaction SDK (Hover events) bepaalt nu wanneer de outline aan gaat.
        dogOutline = GetComponent<Outline>();

        if (dogOutline != null)
        {
            // We stellen alleen de kleur en dikte alvast in, 
            // zodat hij er goed uitziet als hij later aan gaat.
            dogOutline.OutlineColor = Color.white;
            dogOutline.OutlineWidth = 5f;
            
            // Zorg dat hij echt uit staat bij het opstarten
            dogOutline.enabled = false;
        }
    }

    // Wordt aangeroepen door de Pointable Unity Event Wrapper (When Select)
    public void OnDogClicked()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowDogWarning();
        }
    }
}