using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ButtonController : MonoBehaviour
{
    public UnityEvent onPress;
    public GameObject buttonObj;
    public bool pressed;

    Vector3 startLocalPos;

    void Start()
    {
        startLocalPos = buttonObj.transform.localPosition;
    }

    void Update()
    {
        if (pressed) {
            buttonObj.transform.localPosition = startLocalPos + new Vector3(0, -.3f);
        }
        else {
            buttonObj.transform.localPosition = startLocalPos;
        }
    }

    public void PressButton()
    {
        if (!pressed) {
            onPress.Invoke();
            pressed = true;
        }
    }
}
