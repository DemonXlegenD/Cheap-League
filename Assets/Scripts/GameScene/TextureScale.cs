using System;
using UnityEngine;

public class TextureScale : MonoBehaviour
{
    private Renderer rend;
    public float scale;
    void Start()
    {
        rend = gameObject.GetComponent<Renderer>();

        rend.material.mainTextureScale = new Vector2(Math.Max(gameObject.transform.localScale.x, gameObject.transform.localScale.z), gameObject.transform.localScale.y) * 1/scale;
    }
}
