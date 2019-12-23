using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveablePlatform : MonoBehaviour
{

    public GameObject start;
    public GameObject end;
    public GameObject playerSnap;

    float prealpha = -1;
    public float speed;
    public bool on;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(on)
            prealpha += Time.deltaTime * speed;

        if (prealpha > 1) prealpha = -1;
        float alpha = Mathf.Abs(prealpha);

        transform.position = Vector3.Lerp(start.transform.position, end.transform.position, alpha);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawLine(start.transform.position, end.transform.position);
    }

    public void SetOn(bool on)
    {
        this.on = on;
    }
}
