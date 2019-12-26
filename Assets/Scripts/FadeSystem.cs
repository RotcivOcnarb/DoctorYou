using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FadeSystem : MonoBehaviour
{
    //"Graphics" é qualquer componente de UI que desenhe algo na tela
    //Como por exemplo "Image" ou "Text"
    //Eu defino isso pra poder aplicar efeito de fade em qualquer UI q eu quiser
    public Graphic fadable;
    public bool intro, outro; //Intro é no começo (alpha de 1 até 0), outro é pra sair da fase (alpha de 0 até 1)
    float alpha = 1;
    int nextScene; //aqui é o build index da cena que vai trocar depois de terminar o outro

    void Update()
    {
        if (intro) {
            alpha -= Time.deltaTime; //Diminui o alpha até chegar em 0 e desliga o intro
            if(alpha < 0) {
                alpha = 0;
                intro = false;
            }
        }
        if (outro) { //aumenta o alpha até chegar em 1 e muda de cena
            alpha += Time.deltaTime;
            if(alpha > 1) {
                alpha = 1;
                outro = false;
                if(nextScene != -1)
                    SceneManager.LoadScene(nextScene);
            }
        }

        fadable.color = new Color(fadable.color.r, fadable.color.g, fadable.color.b, alpha); //muda o alpha do graphics pra o alpha calculado
    }

    public void ExitTo(int scene) //Auto explicativo
    {
        outro = true;
        intro = false;
        nextScene = scene;
    }

    public void Intro() //Auto explicativo
    {
        intro = true;
        outro = false;
    }
}
