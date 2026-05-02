using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject[] characterPrefabs;
    public Transform[] spawnPoints;

    void Start()
    {
        int count = Mathf.Min(LobbyManager.FinalPlayerCount, spawnPoints.Length);

        for (int i = 0; i < count; i++)
        {
            int id = CharacterSelectManager.SelectedCharacter[i];

            if (id < 0 || id >= characterPrefabs.Length)
            {
                Debug.LogError($"Invalid character ID: {id}");
                continue;
            }

            Instantiate(
                characterPrefabs[id],
                spawnPoints[i].position,
                spawnPoints[i].rotation
            );
        }
    }
}