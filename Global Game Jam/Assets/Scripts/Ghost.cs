using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour
{
    public float fadeTime;

    private float fadeCounter;
    private SpriteRenderer render;

    // Start is called before the first frame update
    void Start()
    {
        fadeCounter = fadeTime;
        render = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (fadeCounter < fadeTime)
        {
            fadeCounter += Time.deltaTime;

            render.color = new Color(render.color.r, render.color.g, render.color.b, render.color.a - Mathf.Lerp(0, 1, fadeTime));
        }
        else if (render.color.a < 1)
            Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Player")
        {
            fadeCounter = 0;
            Destroy(GetComponent<Collider2D>());
        }
    }
}
