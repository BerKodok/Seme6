using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class SelectionCSS : MonoBehaviour
{
    public List<Character> characters = new List<Character>();
    public GameObject charChellPrefab;

    void Start()
    {
        foreach (Character character in characters)
        {
            SpawnCharacterCell(character);
        }
    }
    
    void SpawnCharacterCell(Character character)
    {
        GameObject charCell = Instantiate(charChellPrefab, transform);
        
        charCell.name = character.characterName;

        RawImage artwork = charCell.transform.Find("artwork").GetComponent<RawImage>();
        TextMeshProUGUI name = charCell.transform.Find("nameRect").GetComponentInChildren<TextMeshProUGUI>();

        artwork.texture = character.characterTexture;
        name.text = character.characterName;

    }

}
