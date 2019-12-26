using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

//É o mesmo sistema de diálogo do Jantar de aparencias
public class CixaDialogo : MonoBehaviour
{

    public List<string> textos; //Todos os dialogos
    int textIndex; //index de qual diálogo tá sendo mostrado
    int characterIndex; //Index de qual caracter q tá sendo mostrado
    float characterTimer; //um temporizador pra fazer o efeito de "maquina de escrever"
    public Text textUI; //O componente de texto q vai receber o texto

    public Image downArrow; //Uma setinha pra baixo q ativa quando o texto tiver terminado de animar

    void Update()
    {
        characterTimer += Time.deltaTime; //incrementa o temporizador
        if(characterTimer > 0.1f) { //a cada 100ms aumenta 1 caracter
            characterTimer = 0;
            characterIndex++;
            if(characterIndex >= textos[textIndex].Length) {//chegou no final do texto, impede de continuar aumentando
                characterIndex = textos[textIndex].Length-1;
            }
        }

        textUI.text = textos[textIndex].Substring(0, characterIndex+1); //coloca o texto no componente

        if(characterIndex >= textos[textIndex].Length - 1) //se tá no final do texto, liga a setinha
            downArrow.gameObject.SetActive(true);
        else
            downArrow.gameObject.SetActive(false); //senão desliga
    }

    public void Advance(Animator cameraAnimator) //quando o player clica com o mouse
    {
        if (characterIndex >= textos[textIndex].Length-1 && textIndex < textos.Count-1) { //Se chegou no final do texto, mas ainda não
            //é o ultimo texto, reseta a contagem de caracter pra 0 (volta pro começo)
            //e incrementa o index do texto pra ir pro próximo texto
            characterIndex = 0;
            textIndex++;
            if (textIndex >= textos.Count) {//evita de passar do ultimo texto
                textIndex = textos.Count - 1;
            }
        }
        else if(characterIndex < textos[textIndex].Length - 1) {//se ainda não chegou no final do texto, pula direto pro final
            characterIndex = textos[textIndex].Length - 1;
        }
        else { //se tá no final do texto, e do ultimo texto, começa a animação da camera de dar zoom na cabeça do paciente
            //(e desliga o diálogo)
            gameObject.SetActive(false);
            cameraAnimator.SetTrigger("Intro");
        }
    }


}
