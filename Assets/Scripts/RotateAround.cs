using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class RotateAround : MonoBehaviour
{
    public float angle;
    public GameObject rotatePoint;
    Vector2 originalPolar;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public Vector2 PolarToCart(Vector2 polar)
    {
        return new Vector2(
                Mathf.Cos(polar.x * Mathf.Deg2Rad) * polar.y,
                Mathf.Sin(polar.x * Mathf.Deg2Rad) * polar.y
            );
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 dif = -rotatePoint.transform.position + transform.position;
        originalPolar = new Vector2(
                Mathf.Atan2(dif.y, dif.x) * Mathf.Rad2Deg,
                dif.magnitude
            );
        originalPolar.x = angle;
        transform.position = (Vector2)(rotatePoint.transform.position) + PolarToCart(originalPolar);
        transform.rotation = Quaternion.Euler(0, 0, originalPolar.x - 90);
    }
}
