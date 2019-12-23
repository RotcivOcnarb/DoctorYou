using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeSystem : MonoBehaviour
{
    Image image;
    public bool intro, outro;
    float alpha = 1;
    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if (intro) {
            alpha -= Time.deltaTime;
            if(alpha < 0) {
                alpha = 0;
                intro = false;
            }
        }
        if (outro) {
            alpha += Time.deltaTime;
            if(alpha > 1) {
                alpha = 1;
                outro = false;
            }
        }

        image.color = new Color(0, 0, 0, alpha);
    }
}
