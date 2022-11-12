using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSwap : MonoBehaviour
{
    [SerializeField] AudioSource _audioSource;
    [SerializeField] AudioClip _swapSound;
    
    [Space]

    [SerializeField] private Transform character;
    [SerializeField] private List<Transform> possibleCharacters;
    [SerializeField] private int whichCharacter;

    // Start is called before the first frame update
    void Start()
    {
        if (character == null && possibleCharacters.Count >= 1)
        {
            character = possibleCharacters[0]; 
        }
        Swap();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (whichCharacter == 0)
            {
                whichCharacter = possibleCharacters.Count - 1;
            }
            else
            {
                whichCharacter -= 1;
            }
            Swap();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (whichCharacter == possibleCharacters.Count - 1)
            {
                whichCharacter = 0;
            }
            else
            {
                whichCharacter += 1;
            }
            Swap();
        }
    }

    private void Swap()
    {
        character = possibleCharacters[whichCharacter];
        character.GetComponent<PlayerController2d>().enabled = true;

        character.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePositionX;
        character.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;

        _audioSource.PlayOneShot(_swapSound);

        for (int i = 0; i < possibleCharacters.Count; i++)
        {
            if(possibleCharacters[i] != character)
            {
                possibleCharacters[i].GetComponent<PlayerController2d>().enabled = false;
                possibleCharacters[i].GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePositionX;
            }
        }
    }

}
