using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    #region Inspector Variables
    public int health;
    public float damageShaderTime;
    #endregion

    private SpriteRenderer render;
    private float damageShaderCounter;

    // Start is called before the first frame update
    void Start()
    {
        render = GetComponent<SpriteRenderer>();
        damageShaderCounter = damageShaderTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (damageShaderCounter < damageShaderTime)
        {
            damageShaderCounter += Time.deltaTime;

            if (render.color != Color.red)
                render.color = Color.red;
        }
        else if (damageShaderCounter >= damageShaderTime && render.color == Color.red)
            render.color = Color.white;
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        damageShaderCounter = 0;

        if (health <= 0)
            Die();
    }

    public void Die()
    {
        AudioSource audio = GetComponent<AudioSource>();
        audio.clip = GetComponent<EnemyBasic>().deathSound;
        GetComponent<AudioSource>().Play();

        Destroy(GetComponent<EnemyBasic>());
        Destroy(GetComponent<Collider2D>());
        AnimateSprite[] animations = gameObject.GetComponents<AnimateSprite>();
        for (int i = 0; i < animations.Length; i++)
        {
            if (animations[i].name == "Death")
                animations[i].enabled = true;
            else
                animations[i].enabled = false;
        }
        // To Do: Destroy itself and drop parts
    }
}
