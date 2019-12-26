using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ExecuteDelayed : MonoBehaviour
{
    public UnityEvent delayedEvent;
    public float delayTime;
    float timer = 0;
    bool executed = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if(timer > delayTime && !executed) {
            executed = true;
            delayedEvent.Invoke();
        }
    }
}
