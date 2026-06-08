using UnityEngine;

public class CrowdManager : MonoBehaviour
{
    // Dit zoekt automatisch alle actieve scripts van de characters in de scene
    public void TriggerAmbush()
    {
        CharacterSwarmer[] allCharacters = FindObjectsByType<CharacterSwarmer>(FindObjectsSortMode.None);

        foreach (CharacterSwarmer character in allCharacters)
        {
            character.StartSurrounding();
        }
    }
}