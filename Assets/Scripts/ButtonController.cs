using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ButtonController : MonoBehaviour
{
    public UnityEvent onPress; //um código q é executado quando o botão é apertado (é configurável pelo editor)
    public GameObject buttonObj; //a imagem do botãozinho, pra recolher quando o botão tiver apertado
    public bool pressed;

    Vector3 startLocalPos;

    void Start()
    {
        startLocalPos = buttonObj.transform.localPosition;
    }

    void Update()
    {
        if (pressed) {
            buttonObj.transform.localPosition = startLocalPos + new Vector3(0, -.3f); //a imagem do botão recolhida
        }
        else {
            buttonObj.transform.localPosition = startLocalPos;
        }
    }

    public void PressButton() //A raiva chama esse metodo quando detecta colisão com o botão enquanto estiver correndo
    {
        if (!pressed) {
            onPress.Invoke(); //chama o método q foi configurado e seta como pressionado
            pressed = true;
        }
    }
}
