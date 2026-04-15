using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "New Character", menuName = "Character")]
public class CharacterSelectiion : ScriptableObject
{
    public string characterName;
    public Sprite characterSprite;
}
