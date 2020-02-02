using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateSprite : MonoBehaviour
{
    #region Inspector Variables
    public string name;
    public float fps; // Frames per sprite
    public bool playOnce;
    public List<Sprite> sprites;
    #endregion

    private int spriteIndex;
    private float timer;

    // Start is called before the first frame update
    void Start()
    {
        spriteIndex = 0;
        timer = 0;
    }

    private void FixedUpdate()
    {
        // Don't repeat
        if (playOnce && timer >= fps)
            return;

        timer++;

        if (timer >= fps)
        {
            // Change sprite index
            spriteIndex++;
            if (spriteIndex >= sprites.Count)
                spriteIndex = 0;

            // Don't update if only played once
            if (spriteIndex == 0 && playOnce)
            {
                if (name == "Death")
                    Destroy(gameObject);
                return;
            }

            // Update sprite
            gameObject.GetComponent<SpriteRenderer>().sprite = sprites[spriteIndex];

            // Reset timer
            timer = 0;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Don't repeat
        /*if (playOnce && timer >= fps)
            return;

        timer++;

        if (timer >= fps)
        {
            // Change sprite index
            spriteIndex++;
            if (spriteIndex >= sprites.Count)
                spriteIndex = 0;

            // Don't update if only played once
            if (spriteIndex == 0 && playOnce)
            {
                if (name == "Death")
                    Destroy(gameObject);
                return;
            }

            // Update sprite
            gameObject.GetComponent<SpriteRenderer>().sprite = sprites[spriteIndex];

            // Reset timer
            timer = 0;
        }*/
    }

    private void OnEnable()
    {
        if (playOnce)
        {
            spriteIndex = 0;
            timer = 0;
        }
    }
}
