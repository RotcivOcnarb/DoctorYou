using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{

    public GameObject toFollow;
    public Rect limit;
    Camera mainCamera;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 position = transform.position;
        position.y += (toFollow.transform.position.y - position.y) / 10f;
        position.z = -10;
        transform.position = position;

        if(transform.position.y - mainCamera.orthographicSize < limit.position.y - limit.height / 2f) {
            Vector3 v = transform.position;
            v.y = limit.position.y - limit.height / 2f + mainCamera.orthographicSize;
            transform.position = v;
        }
        if (transform.position.y + mainCamera.orthographicSize > limit.position.y + limit.height / 2f) {
            Vector3 v = transform.position;
            v.y = limit.position.y + limit.height / 2f - mainCamera.orthographicSize;
            transform.position = v;
        }

    }

    private void OnDrawGizmosSelected()
    {

        Vector2 c0 = limit.position + new Vector2(-limit.width / 2f, -limit.height / 2f);
        Vector2 c1 = limit.position + new Vector2(limit.width / 2f, -limit.height / 2f);
        Vector2 c2 = limit.position + new Vector2(limit.width / 2f, limit.height / 2f);
        Vector2 c3 = limit.position + new Vector2(-limit.width / 2f, limit.height / 2f);

        Gizmos.DrawLine(c0, c1);
        Gizmos.DrawLine(c1, c2);
        Gizmos.DrawLine(c2, c3);
        Gizmos.DrawLine(c3, c0);

    }
}
