using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureScale : MonoBehaviour
{
    private Renderer rend;
    public float scale;
    // Start is called before the first frame update
    void Start()
    {
        rend = gameObject.GetComponent<Renderer>();

        rend.material.mainTextureScale = new Vector2(Math.Max(gameObject.transform.localScale.x, gameObject.transform.localScale.z), gameObject.transform.localScale.y) * 1/scale;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
