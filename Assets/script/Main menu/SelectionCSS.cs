using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class SelectionCSS : MonoBehaviour
{
    public List<Character> characters = new List<Character>();
    public GameObject charCellPrefab;

    void Start()
    {
        foreach (Character character in characters)
        {
            SpawnCharacterCell(character);
        }
    }
    
    void SpawnCharacterCell(Character character)
    {
        GameObject charCell = Instantiate(charCellPrefab, transform);
        
        charCell.name = character.characterName;

        Image artwork = charCell.transform.Find("artwork").GetComponent<Image>();
        TextMeshProUGUI name = charCell.transform.Find("nameRect").GetComponentInChildren<TextMeshProUGUI>();

        artwork.sprite = character.characterSprite;
        name.text = character.characterName;

    }

}
