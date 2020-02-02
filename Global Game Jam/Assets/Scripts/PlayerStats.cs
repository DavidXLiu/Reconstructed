using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerStats : MonoBehaviour
{
    #region Inspector Variables
    public int damage;
    public float invincibleTime;
    public float deathTime;
    public float deathFade;
    public Image deathShader;

    // Body Parts
    public Transform headPos;
    public Transform leftArmPos;
    public Transform rightArmPos;
    public Transform bodyPos;
    public Transform legsPos;
    #endregion

    [HideInInspector] public bool canBeAttacked;
    [HideInInspector] public bool isDead;

    // Body Parts
    [HideInInspector] public GameObject head;
    [HideInInspector] public GameObject leftArm;
    [HideInInspector] public GameObject rightArm;
    [HideInInspector] public GameObject body;
    [HideInInspector] public GameObject legs;

    private PlayerController controller;
    private float invincibleTimer;
    private bool isInvincible;
    private float deathCounter;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<PlayerController>();
        canBeAttacked = true;
        isDead = false;
        isInvincible = false;
        invincibleTimer = 0;
        deathCounter = 0;

        // Body is the main controller
        body = gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead)
        {
            deathCounter += Time.deltaTime;

            // Fade to black
            if (deathCounter >= deathTime)
            {
                deathShader.color = new Color(deathShader.color.r, deathShader.color.g, deathShader.color.b, deathShader.color.a + Mathf.Lerp(0, 1, deathFade));

                // Restart
                if (deathShader.color.a >= 1)
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
        else if (isInvincible)
        {
            invincibleTimer += Time.deltaTime;

            // Check if no longer invincible
            if (invincibleTimer >= invincibleTime)
            {
                isInvincible = false;
                canBeAttacked = true;
            }
        }
        else if (deathShader.color.a > 0) // Starting the game scene
        {
            // Fade in
            deathShader.color = new Color(deathShader.color.r, deathShader.color.g, deathShader.color.b, deathShader.color.a - Mathf.Lerp(0, 1, deathFade));
        }
    }

    public void TakeDamage(GameObject part, RaycastHit2D hit, Vector2 dir, float force)
    {
        switch (controller.bodyState)
        {
            case PlayerController.BodyState.Full:
                RemovePart(rightArm, hit, dir, force);
                break;
            case PlayerController.BodyState.OneArm:
                RemovePart(leftArm, hit, dir, force);
                break;
            case PlayerController.BodyState.NoArms:
                RemovePart(head, hit, dir, force);
                break;
            case PlayerController.BodyState.NoHead:
                Die(dir, force);
                return;
            default:
                return;
        }

        controller.bodyState++;
        controller.ChangeAnimState(controller.anim);

        // Make invincible
        isInvincible = true;
        invincibleTimer = 0;
    }

    public void Die(Vector2 dir, float force)
    {
        isDead = true;

        Rigidbody2D rigidbody = GetComponent<Rigidbody2D>();
        rigidbody.freezeRotation = false;
        rigidbody.AddForceAtPosition(dir * force, transform.position - new Vector3(dir.x, dir.y));
        controller.DisableAnimations();
        controller.enabled = false;

        controller.walkSound1.Stop();
        controller.walkSound2.Stop();
    }

    private void RemovePart(GameObject partObj, RaycastHit2D hit, Vector2 dir, float force)
    {
        partObj.GetComponent<BodyPart>().Detach();
        partObj.GetComponent<Rigidbody2D>().AddForceAtPosition(dir * force, hit.point);
    }
}
