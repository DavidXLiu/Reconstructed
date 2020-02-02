using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region Inspector Variables
    public float alphaSpeed;
    public float betaSpeed;
    public float speedCap;
    public float rollForce;
    public float alphaJumpForce;
    public float omegaJumpForce;
    public float downForce;
    public List<AnimateSprite> animStates;

    public AudioSource walkSound1;
    public AudioSource walkSound2;
    public AudioSource landSound;
    public AudioSource clickSound;
    public AudioSource laserSound;
    public AudioSource shootSound;
    #endregion

    public enum BodyState { Full, OneArm, NoArms, NoHead };
    public enum AnimState { Idle, Walk, Jump };

    // Animations
    private List<AnimateSprite> animIdle;
    private List<AnimateSprite> animWalk;
    private List<AnimateSprite> animJump;
    public AnimState anim;

    private Rigidbody2D rigidbody;
    public BodyState bodyState;
    private float airTimer;
    [HideInInspector] public bool grounded = true;

    public bool hasCharge;
    public bool isCharging;

    // Start is called before the first frame update
    void Start()
    {
        hasCharge = false;
        isCharging = false;

        // Set the rigidbody
        rigidbody = GetComponent<Rigidbody2D>();
        if (rigidbody == null)
            Debug.LogError($"Rigidbody2D is not set in {gameObject.name}");

        // Get Animations
        animIdle = new List<AnimateSprite>();
        animWalk = new List<AnimateSprite>();
        animJump = new List<AnimateSprite>();
        if (animStates.Count == 0)
            animStates = GetComponentsInChildren<AnimateSprite>().ToList();
        foreach (AnimateSprite anim in animStates)
        {
            switch (anim.name)
            {
                // Idle
                case "FullIdle":
                    animIdle.Insert(0, anim);
                    break;
                case "OneArmIdle":
                    animIdle.Insert(1, anim);
                    break;
                case "NoArmsIdle":
                    animIdle.Insert(2, anim);
                    break;
                case "NoHeadIdle":
                    animIdle.Insert(3, anim);
                    break;
                // Walk
                case "FullWalk":
                    animWalk.Insert(0, anim);
                    break;
                case "OneArmWalk":
                    animWalk.Insert(1, anim);
                    break;
                case "NoArmsWalk":
                    animWalk.Insert(2, anim);
                    break;
                case "NoHeadWalk":
                    animWalk.Insert(3, anim);
                    break;
                // Jump
                case "FullJump":
                    animJump.Insert(0, anim);
                    break;
                case "OneArmJump":
                    animJump.Insert(1, anim);
                    break;
                case "NoArmsJump":
                    animJump.Insert(2, anim);
                    break;
                case "NoHeadJump":
                    animJump.Insert(3, anim);
                    break;
                default:
                    break;
            }
        }
        ChangeAnimState(AnimState.Idle);

        // Set Body State
        //bodyState = BodyState.Alpha;

        airTimer = 0;
    }

    private void FixedUpdate()
    {
        //transform.Translate(new Vector3(Input.GetAxis("Horizontal") * alphaSpeed * Time.deltaTime, 0));

        if (!grounded)
        {
            airTimer += 0.02f;

            // Own check for grounded
            if (airTimer > 0.025f)
            {
                RaycastHit2D hit = Physics2D.Raycast(transform.position - new Vector3(0, 0.25f), new Vector2(0, -1), 0.75f);
                Debug.DrawLine(transform.position - new Vector3(0, 0.25f), transform.position - new Vector3(0, 0.25f) + (new Vector3(0, -1) * 0.75f));
                if (hit.collider != null && (hit.collider.tag == "Ground" || hit.collider.tag == "Enemy"))
                {
                    landSound.volume = -rigidbody.velocity.y / 250f;
                    landSound.Play();

                    grounded = true;
                    airTimer = 0;
                }
            }
        }

        if (Input.GetAxis("Horizontal") == 0 && anim != AnimState.Idle && grounded)
        {
            ChangeAnimState(AnimState.Idle);
        }
        else if (Input.GetAxis("Horizontal") != 0)
        {
            // Check for run animation
            if (grounded && anim != AnimState.Walk)
            {
                ChangeAnimState(AnimState.Walk);
            }

            // Left/Right Movement
            transform.Translate(new Vector3(Input.GetAxis("Horizontal") * alphaSpeed, 0));

            // Sprite change
            if (Input.GetAxis("Horizontal") < 0)
                GetComponent<SpriteRenderer>().flipX = true;
            else if (Input.GetAxis("Horizontal") > 0)
                GetComponent<SpriteRenderer>().flipX = false;
        }

        if (Input.GetAxis("Vertical") < 0)
            rigidbody.AddForce(new Vector2(0, Input.GetAxis("Vertical") * downForce));

        // Jumping
        if (Input.GetButton("Jump") && !grounded && rigidbody.velocity.y > 0)
        {
            rigidbody.AddForce(new Vector2(0, alphaJumpForce / (Mathf.Pow(airTimer, 2) + (alphaJumpForce / 5))));
        }

        if (hasCharge && Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            alphaSpeed = 0.25f;
            GetComponent<PlayerStats>().canBeAttacked = false;
            GetComponent<SpriteRenderer>().color = Color.red;
            isCharging = true;
        }
        if (hasCharge && Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            alphaSpeed = 0.075f;
            GetComponent<PlayerStats>().canBeAttacked = true;
            GetComponent<SpriteRenderer>().color = Color.white;
            isCharging = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Jump") && grounded)
        {
            rigidbody.AddForce(new Vector2(0, alphaJumpForce));
            grounded = false;

            ChangeAnimState(AnimState.Jump);
        }

        /*if (!grounded)
        {
            airTimer += Time.deltaTime;

            // Own check for grounded
            if (airTimer > 0.025f)
            {
                RaycastHit2D hit = Physics2D.Raycast(transform.position - new Vector3(0, 0.25f), new Vector2(0, -1), 0.75f);
                Debug.DrawLine(transform.position - new Vector3(0, 0.25f), transform.position - new Vector3(0, 0.25f) + (new Vector3(0, -1) * 0.75f));
                if (hit.collider != null && (hit.collider.tag == "Ground" || hit.collider.tag == "Enemy"))
                {
                    landSound.volume = -rigidbody.velocity.y / 250f;
                    landSound.Play();

                    grounded = true;
                    airTimer = 0;
                }
            }
        }

        // Full Body Movement
        //if (bodyState == BodyState.Full)
        //{
            // Check for idle animation
            if (Input.GetAxis("Horizontal") == 0 && anim != AnimState.Idle && grounded)
            {
                ChangeAnimState(AnimState.Idle);
            }
            else if (Input.GetAxis("Horizontal") != 0)
            {
                // Check for run animation
                if (grounded && anim != AnimState.Walk)
                {
                    ChangeAnimState(AnimState.Walk);
                }

                // Left/Right Movement
                transform.Translate(new Vector3(Input.GetAxis("Horizontal") * alphaSpeed * Time.deltaTime, 0));

                // Sprite change
                if (Input.GetAxis("Horizontal") < 0)
                    GetComponent<SpriteRenderer>().flipX = true;
                else if (Input.GetAxis("Horizontal") > 0)
                    GetComponent<SpriteRenderer>().flipX = false;
            }

            // Fastfall
            if (Input.GetAxis("Vertical") < 0)
                rigidbody.AddForce(new Vector2(0, Input.GetAxis("Vertical") * downForce * Time.deltaTime));

            // Jumping
            if (Input.GetButtonDown("Jump") && grounded)
            {
                rigidbody.AddForce(new Vector2(0, alphaJumpForce));
                grounded = false;

                ChangeAnimState(AnimState.Jump);
            }
            // Higher jumping
            else if (Input.GetButton("Jump") && !grounded && rigidbody.velocity.y > 0)
            {
                rigidbody.AddForce(new Vector2(0, alphaJumpForce / (Mathf.Pow(airTimer, 2) + (alphaJumpForce / 5))));
            }*/
        //}
        // Broken Body Movement
        /*else if (bodyState == BodyState.Beta)
        {
            // Left/Right Movement
            transform.Translate(new Vector3(Input.GetAxis("Horizontal") * betaSpeed * Mathf.Abs(Mathf.Sin(Time.time * 4)), 0));

            // Falling
            if (Input.GetAxis("Vertical") < 0)
                rigidbody.AddForce(new Vector2(0, Input.GetAxis("Vertical") * downForce));

            // Jumping
            if (Input.GetButtonDown("Jump") && grounded)
            {
                rigidbody.AddForce(new Vector2(0, alphaJumpForce));
                grounded = false;
            }
            // Higher jumping
            else if (Input.GetButton("Jump") && !grounded && rigidbody.velocity.y > 0)
            {
                rigidbody.AddForce(new Vector2(0, alphaJumpForce / (Mathf.Pow(airTimer, 2) + (alphaJumpForce / 5))));
            }
        }*/
        // Just Head Movement
        /*else if (bodyState == BodyState.Omega)
        {
            // Left/Right Movement
            if (Mathf.Abs(rigidbody.velocity.x) < speedCap)
                rigidbody.AddForce(new Vector2(Input.GetAxis("Horizontal") * rollForce, 0));

            // Jumping
            if (Input.GetButtonDown("Jump") && grounded)
            {
                rigidbody.AddForce(new Vector2(0, omegaJumpForce));
                grounded = false;
            }
            // Higher jumping
            else if (Input.GetButton("Jump") && !grounded && rigidbody.velocity.y > 0)
            {
                rigidbody.AddForce(new Vector2(0, omegaJumpForce / (Mathf.Pow(airTimer, 2) + (omegaJumpForce / 5))));
            }
        }

        if (hasCharge && Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            alphaSpeed = 0.2f;
            GetComponent<PlayerStats>().canBeAttacked = false;
            GetComponent<SpriteRenderer>().color = Color.red;
            isCharging = true;
        }
        if (hasCharge && Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            alphaSpeed = 0.05f;
            GetComponent<PlayerStats>().canBeAttacked = true;
            GetComponent<SpriteRenderer>().color = Color.white;
            isCharging = false;
        }*/
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Hits what's labeled as the ground layer
        /*if (collision.gameObject.layer == 8)
        {
            grounded = true;
            airTimer = 0;
        }*/
    }

    #region Helper Methods
    public void ChangeAnimState(AnimState state)
    {
        switch(state)
        {
            case AnimState.Idle:
                anim = AnimState.Idle;
                DisableAnimations();
                animIdle[(int)bodyState].enabled = true;

                walkSound1.Stop();
                walkSound2.Stop();
                break;
            case AnimState.Walk:
                anim = AnimState.Walk;
                DisableAnimations();
                animWalk[(int)bodyState].enabled = true;

                if (bodyState == BodyState.Full)
                    walkSound1.Play();
                walkSound2.Play();
                break;
            case AnimState.Jump:
                //anim = AnimState.Jump;
                //DisableAnimations();
                //animJump[(int)bodyState].enabled = true;
                break;
            default:
                break;
        }
    }

    public void DisableAnimations()
    {
        foreach (AnimateSprite sprite in animStates)
            sprite.enabled = false;
    }
    #endregion
}
