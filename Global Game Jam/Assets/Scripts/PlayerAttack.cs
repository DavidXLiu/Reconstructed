using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    #region Inspector Variables
    public float cooldownLaser;
    public float laserTime;
    public float cooldownGun;
    public GameObject projectile;
    #endregion

    private GameObject player;
    private PlayerController controller;
    private PlayerStats stats;

    private float cooldownLaserCounter;
    private SpriteRenderer headSprite;

    private float laserCounter;
    private float cooldownGunCounter;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        controller = player.GetComponent<PlayerController>();
        stats = player.GetComponent<PlayerStats>();

        cooldownLaserCounter = cooldownLaser;
        if (stats.head != null)
            headSprite = stats.head.GetComponent<SpriteRenderer>();

        cooldownGunCounter = cooldownGun;
        laserCounter = laserTime;
    }

    // Update is called once per frame
    void Update()
    {
        #region Laser Attack
        if (laserCounter < laserTime)
            laserCounter += Time.deltaTime;
        else if (GetComponent<LineRenderer>().GetPosition(0) != Vector3.zero)
        {
            GetComponent<LineRenderer>().SetPosition(0, Vector3.zero);
            GetComponent<LineRenderer>().SetPosition(1, Vector3.zero);
        }

        // On cooldown for laser attack
        if (cooldownLaserCounter < cooldownLaser)
        {
            cooldownLaserCounter += Time.deltaTime;

            float hue;
            float sat;
            float val;
            Color.RGBToHSV(headSprite.color, out hue, out sat, out val);
            val = 0.5f + (cooldownLaserCounter / cooldownLaser * 0.5f);
            headSprite.color = Color.HSVToRGB(hue, sat, val);

            //Color currentCol = GetComponent<LineRenderer>().material.color;
            //GetComponent<LineRenderer>().material.color = new Color(currentCol.r, currentCol.g, currentCol.b, 1 - (cooldownLaserCounter / cooldownLaser));
        }
        // Can attack again
        else if (cooldownLaserCounter >= cooldownLaser)
        {
            // Remove beam
            if (GetComponent<LineRenderer>().GetPosition(0) != Vector3.zero)
            {
                GetComponent<LineRenderer>().SetPosition(0, Vector3.zero);
                GetComponent<LineRenderer>().SetPosition(1, Vector3.zero);
            }

            if (Input.GetButtonDown("Fire1") && stats.head != null)
            {
                // To Do: Charge attack
            }
            else if (Input.GetButtonUp("Fire1") && stats.head != null)
            {
                Vector3 worldMouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector3 dir = (worldMouse - stats.headPos.position).normalized;

                List<Collider2D> collidersHit = new List<Collider2D>();
                RaycastHit2D hit = Physics2D.Raycast(stats.headPos.position, dir);

                GetComponent<LineRenderer>().SetPosition(0, new Vector3(stats.headPos.position.x, stats.headPos.position.y));
                Color currentCol = GetComponent<LineRenderer>().material.color;
                GetComponent<LineRenderer>().material.color = new Color(currentCol.r, currentCol.g, currentCol.b, 1);
                laserCounter = 0;

                if (hit.collider != null)
                {
                    collidersHit.Add(hit.collider);

                    // Extend the raycast
                    while (hit.collider.tag != "Ground")
                    {
                        hit = Physics2D.Raycast(hit.point + new Vector2(dir.x, dir.y), dir);
                        if (hit.collider != null)
                            collidersHit.Add(hit.collider);
                        else // Didn't hit anything
                            break;
                    }

                    GetComponent<LineRenderer>().SetPosition(1, hit.point);

                    // Damage all enemies and destroy breakable blocks
                    foreach (Collider2D collider in collidersHit)
                    {
                        if (collider.tag == "Enemy")
                        {
                            collider.GetComponent<EnemyStats>().TakeDamage(stats.damage);
                        }
                        else if (collider.tag == "Breakable")
                            Destroy(collider.gameObject);
                    }
                }

                controller.laserSound.Play();

                // Set on cooldown
                cooldownLaserCounter = 0;
                headSprite = stats.GetComponent<SpriteRenderer>();
            }
        }
        #endregion

        // On cooldown for normal attack
        if (cooldownGunCounter < cooldownGun)
            cooldownGunCounter += Time.deltaTime;
        // Can attack again
        else if (cooldownGunCounter >= cooldownGun)
        {
            if (Input.GetButton("Fire2") && (stats.leftArm != null || stats.rightArm != null))
            {
                if (stats.leftArm != null)
                {
                    controller.shootSound.Play();

                    GameObject bullet = Instantiate(projectile, stats.leftArm.transform.position, projectile.transform.rotation);
                    bullet.GetComponentInChildren<PlayerProjectile>().moveLeft = GetComponent<SpriteRenderer>().flipX;
                    bullet.GetComponentInChildren<SpriteRenderer>().flipY = !GetComponent<SpriteRenderer>().flipX;
                }
                if (stats.rightArm != null)
                {
                    controller.shootSound.Play();

                    GameObject bullet = Instantiate(projectile, stats.rightArm.transform.position, projectile.transform.rotation);
                    bullet.GetComponentInChildren<PlayerProjectile>().moveLeft = GetComponent<SpriteRenderer>().flipX;
                    bullet.GetComponentInChildren<SpriteRenderer>().flipY = !GetComponent<SpriteRenderer>().flipX;
                }

                // Set on cooldown
                cooldownGunCounter = 0;
            }
        }
    }
}
