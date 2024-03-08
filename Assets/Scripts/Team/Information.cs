using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Information : MonoBehaviour
{
    public Image background;
    public TextMeshProUGUI label;

    public float fadeInDuration = 0.5f; // Durée de fondu en secondes
    public float fadeOutDelay = 2f; // Délai avant de commencer à disparaître
    public float fadeOutDuration = 0.5f; // Durée de fondu pour disparaître en secondes

    // Start is called before the first frame update
    void Start()
    {
        SetVisible(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetVisible(bool state)
    {
        background.enabled = state;
        label.enabled = state;
    }

    public void OnScored(string playerName, Color color)
    {
        Color textColor = color;
        Color backgrounColor = Color.white;
        textColor.a = 0f;
        backgrounColor.a = 0f;
        label.text = playerName;
        label.color = textColor;
        background.color = backgrounColor;
        SetVisible(true);
        // Démarrer la coroutine de fondu en entrée
        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeInDuration)
        {
            float alphaText = Mathf.Lerp(0f, 1f, elapsedTime / fadeInDuration);
            Color textColor = label.color;
            textColor.a = alphaText;
            label.color = textColor;

            float alphaBg = Mathf.Lerp(0f, 0.3f, elapsedTime / fadeInDuration);
            Color backgrounColor = background.color;
            backgrounColor.a = alphaBg;
            background.color = backgrounColor;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(fadeOutDelay);

        // Démarrer la coroutine de fondu en sortie
        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeOutDuration)
        {
            float alphaText = Mathf.Lerp(1f, 0f, elapsedTime / fadeOutDuration);
            Color textColor = label.color;
            textColor.a = alphaText;
            label.color = textColor;

            float alphaBg = Mathf.Lerp(0.3f, 0f, elapsedTime / fadeOutDuration);
            Color bgColor = background.color;
            bgColor.a = alphaBg;
            background.color = bgColor;

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        SetVisible(false);
    }

    public void OnWin(string playerName, Color color)
    {
        Color textColor = color;
        Color backgrounColor = Color.white;
        textColor.a = 0f;
        backgrounColor.a = 0f;
        label.text = playerName + " WON";
        label.color = textColor;
        background.color = backgrounColor;
        SetVisible(true);
        // Démarrer la coroutine de fondu en entrée
        StartCoroutine(FadeIn());
    }
}
