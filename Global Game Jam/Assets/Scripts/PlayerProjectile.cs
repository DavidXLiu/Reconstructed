using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProjectile : MonoBehaviour
{
    #region Inspector Variables
    public int damage;
    public float projectileSpeed;
    public float collideDistance;
    #endregion

    [HideInInspector] public bool moveLeft;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void FixedUpdate()
    {
        if (moveLeft)
        {
            // We do our own collision because Unity collision sucks!
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.left, collideDistance);
            Debug.DrawLine(transform.position, transform.position - (Vector3.left * collideDistance));
            if (hit.collider != null)
                CheckCollision(hit.collider);
            else
                transform.Translate(-projectileSpeed, 0, 0);
        }
        else
        {
            // We do our own collision because Unity collision sucks!
            RaycastHit2D hit = Physics2D.Raycast(transform.position, -Vector2.left, collideDistance);
            Debug.DrawLine(transform.position, transform.position + (Vector3.left * collideDistance));
            if (hit.collider != null)
                CheckCollision(hit.collider);
            else
                transform.Translate(projectileSpeed, 0, 0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        /*if (moveLeft)
        {
            // We do our own collision because Unity collision sucks!
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.left, collideDistance);
            Debug.DrawLine(transform.position, transform.position - (Vector3.left * collideDistance));
            if (hit.collider != null)
                CheckCollision(hit.collider);
            else
                transform.Translate(-projectileSpeed, 0, 0);
        }            
        else
        {
            // We do our own collision because Unity collision sucks!
            RaycastHit2D hit = Physics2D.Raycast(transform.position, -Vector2.left, collideDistance);
            Debug.DrawLine(transform.position, transform.position + (Vector3.left * collideDistance));
            if (hit.collider != null)
                CheckCollision(hit.collider);
            else
                transform.Translate(projectileSpeed, 0, 0);
        }    */        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        CheckCollision(collision.collider);
    }

    #region Helper Methods
    private void CheckCollision(Collider2D collider)
    {
        if (collider.tag == "Enemy")
            collider.gameObject.GetComponent<EnemyStats>().TakeDamage(damage);

        // Destroy bullet when colliding with anything
        Destroy(gameObject);
    }
    #endregion
}
