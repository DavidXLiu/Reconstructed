using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndGame : MonoBehaviour
{
    public Text winText;
    public float fadeTime;

    private GameObject player;
    private bool end;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        end = false;
    }

    private void FixedUpdate()
    {
        if (end && winText.enabled && winText.color.a != 1)
        {
            winText.color = new Color(winText.color.r, winText.color.g, winText.color.b, winText.color.a + Mathf.Lerp(0, 1, fadeTime));
        }
    }

    // Update is called once per frame
    void Update()
    {
        /*if (end && winText.enabled && winText.color.a != 1)
        {
            winText.color = new Color(winText.color.r, winText.color.g, winText.color.b, winText.color.a + Mathf.Lerp(0, 1, fadeTime));
        }*/
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Player")
        {
            player.GetComponent<SpriteRenderer>().color = Color.white;
            Destroy(player.GetComponent<PlayerController>());
            Destroy(player.GetComponent<PlayerStats>());

            winText.enabled = true;
            end = true;
        }
    }
}
