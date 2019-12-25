using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CixaDialogo : MonoBehaviour
{

    public List<string> textos;
    int textIndex;
    int characterIndex;
    float characterTimer;
    public Text textUI;

    public Image downArrow;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        characterTimer += Time.deltaTime;
        if(characterTimer > 0.1f) {
            characterTimer = 0;
            characterIndex++;
            if(characterIndex >= textos[textIndex].Length) {
                characterIndex = textos[textIndex].Length-1;
            }
        }
        textUI.text = textos[textIndex].Substring(0, characterIndex+1);

        if(characterIndex >= textos[textIndex].Length - 1) {
            downArrow.gameObject.SetActive(true);
        }
        else {
            downArrow.gameObject.SetActive(false);
        }

    }

    public void Advance(Animator cameraAnimator)
    {
        if (characterIndex >= textos[textIndex].Length-1 && textIndex < textos.Count-1) {
            characterIndex = 0;
            textIndex++;
            if (textIndex >= textos.Count) {
                textIndex = textos.Count - 1;
            }
        }
        else if(characterIndex < textos[textIndex].Length - 1) {
            characterIndex = textos[textIndex].Length - 1;
        }
        else {
            gameObject.SetActive(false);
            cameraAnimator.SetTrigger("Intro");
        }
    }


}
