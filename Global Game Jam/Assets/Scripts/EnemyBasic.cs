using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBasic : MonoBehaviour
{
    #region Inspector Variables
    public int health;

    public float floatDistance;
    public float margin;

    // Patrol
    public float patrolSpeed;
    public Vector2 patrolWait; // x is min, y is max
    public Vector2 patrolLength; // x is min, y is max

    // Seek
    public float seekSpotTime;
    public float seekRange;
    public float seekSpeed;
    public float cautionDistance; // Distance it stops from player to attack

    // Attack
    public float chargeTime;
    public float cooldownTime;
    public float attackRange;
    public float attackForce;
    public AnimateSprite attackAnim;
    public AnimateSprite idleAnim;

    public AudioClip deathSound;
    #endregion

    private enum BehaviorState { Patrol, Seek, Attack, Look, Wait };
    private BehaviorState behavior;

    private float destination;

    #region Patrol State Variables
    private float waitTime;
    private float waitCounter;
    private float initialXPos;
    private bool faceLeft;
    private GameObject player;
    #endregion

    #region Attack State Variables
    private float attackCounter;
    private float cooldownCounter;
    private bool onCooldown;
    private PlayerStats playerStats;
    private PlayerController playerController;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        #region Patrol State Initialization
        player = GameObject.FindGameObjectWithTag("Player");
        behavior = BehaviorState.Patrol;
        initialXPos = transform.position.x;
        #endregion

        #region Attack State Initialization
        playerStats = player.GetComponent<PlayerStats>();
        playerController = player.GetComponent<PlayerController>();
        attackCounter = 0;
        cooldownCounter = 0;
        onCooldown = false;
        #endregion

        StartNewPatrol();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Player" && playerController.isCharging)
            GetComponent<EnemyStats>().Die();
    }

    private void FixedUpdate()
    {
        // Floating Animation
        transform.Translate(0, Mathf.Sin(Time.time) * floatDistance, 0);

        // Patroling state
        if (behavior == BehaviorState.Patrol)
        {
            // Check if facing the direction of the player
            if (player.transform.position.x - transform.position.x > 0 && !faceLeft)
            {
                // Check if able to "see" the player
                RaycastHit2D hit = Physics2D.Raycast(transform.position + GetComponent<Collider2D>().bounds.extents, (player.transform.position - transform.position).normalized, seekRange);
                if (hit.collider != null && hit.collider.tag == "Player")
                {
                    attackAnim.enabled = true;
                    idleAnim.enabled = false;
                    behavior = BehaviorState.Seek;

                    // Set place to head towards
                    destination = player.transform.position.x - cautionDistance;
                }
            }
            else if (player.transform.position.x - transform.position.x < 0 && faceLeft)
            {
                // Check if able to "see" the player
                RaycastHit2D hit = Physics2D.Raycast(transform.position - GetComponent<Collider2D>().bounds.extents, (player.transform.position - transform.position).normalized, seekRange);
                if (hit.collider != null && hit.collider.tag == "Player")
                {
                    attackAnim.enabled = true;
                    idleAnim.enabled = false;
                    behavior = BehaviorState.Seek;

                    // Set place to head towards
                    destination = player.transform.position.x + cautionDistance;
                }
            }

            // Wait a bit at new destination
            if (Mathf.Abs(transform.position.x - destination) <= margin)
            {
                // Check if done waiting
                if (waitCounter >= waitTime)
                    StartNewPatrol();

                waitCounter += Time.deltaTime;
            }
            // Move towards destination
            else
            {
                if (transform.position.x > destination)
                    transform.Translate(-patrolSpeed, 0, 0);
                else
                    transform.Translate(patrolSpeed, 0, 0);
            }
        }
        else if (behavior == BehaviorState.Seek)
        {
            // Reached caution distance
            if (Mathf.Abs(transform.position.x - player.transform.position.x) - cautionDistance <= margin)
            {
                behavior = BehaviorState.Attack;
            }
            // Lost sight
            else if (Mathf.Abs(transform.position.x - player.transform.position.x) - cautionDistance > seekRange)
            {
                behavior = BehaviorState.Patrol;
                attackAnim.enabled = false;
                idleAnim.enabled = true;
                destination = transform.position.x; // Ez switch to wait
            }
            else
            {
                destination = player.transform.position.x - cautionDistance;

                if (transform.position.x > destination)
                {
                    transform.Translate(-seekSpeed, 0, 0);
                    if (!faceLeft)
                    {
                        faceLeft = true;
                        GetComponent<SpriteRenderer>().flipX = false;
                    }
                }
                else
                {
                    transform.Translate(seekSpeed, 0, 0);
                    if (faceLeft)
                    {
                        faceLeft = false;
                        GetComponent<SpriteRenderer>().flipX = true;
                    }
                }
            }
        }
        else if (behavior == BehaviorState.Attack)
        {
            // Check if Player is fleeing
            if (Mathf.Abs(transform.position.x - player.transform.position.x) - cautionDistance > margin + attackRange)
            {
                GetComponent<LineRenderer>().SetPosition(0, transform.position);
                GetComponent<LineRenderer>().SetPosition(1, transform.position);

                attackCounter = 0;
                behavior = BehaviorState.Seek;
            }
            // Check if cooling down
            else if (onCooldown)
            {
                cooldownCounter += Time.deltaTime;

                if (cooldownCounter >= cooldownTime)
                {
                    GetComponent<LineRenderer>().SetPosition(0, transform.position);
                    GetComponent<LineRenderer>().SetPosition(1, transform.position);

                    // Reset to attacking
                    onCooldown = false;
                    attackCounter = 0;
                    attackAnim.enabled = true;
                    idleAnim.enabled = false;
                }
            }
            // Attacking
            else if (playerStats.canBeAttacked)
            {
                attackCounter += Time.deltaTime;

                // Able to attack
                if (attackCounter >= chargeTime)
                {
                    GameObject target = null;

                    // Get all possible target parts
                    switch (playerController.bodyState)
                    {
                        case PlayerController.BodyState.Full:
                            target = playerStats.rightArm;
                            break;
                        case PlayerController.BodyState.OneArm:
                            target = playerStats.leftArm;
                            break;
                        case PlayerController.BodyState.NoArms:
                            target = playerStats.head;
                            break;
                        case PlayerController.BodyState.NoHead:
                            target = playerStats.body;
                            break;
                        default:
                            target = playerStats.body;
                            break;
                    }

                    DisableOtherParts(target);

                    // Raycast the beam
                    Vector2 dir = (target.transform.position - transform.position).normalized;
                    RaycastHit2D hit = Physics2D.Raycast(transform.position, dir);

                    EnableOtherParts(target);

                    // Check if hit
                    if (hit.collider.gameObject == target)
                    {
                        playerStats.TakeDamage(target, hit, dir, attackForce);

                        GetComponent<LineRenderer>().SetPosition(0, transform.position);
                        GetComponent<LineRenderer>().SetPosition(1, hit.point);

                        GetComponent<AudioSource>().Play();
                    }

                    // Cooldown
                    cooldownCounter = 0;
                    onCooldown = true;
                    attackAnim.enabled = false;
                    idleAnim.enabled = true;
                }
            }
            // Don't start attacking when invincible
            else if (!playerStats.canBeAttacked && attackCounter != 0)
            {
                attackCounter = 0;
                attackAnim.enabled = false;
                idleAnim.enabled = true;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        /*// Floating Animation
        transform.Translate(0, Mathf.Sin(Time.time) * floatDistance, 0);

        // Patroling state
        if (behavior == BehaviorState.Patrol)
        {
            // Check if facing the direction of the player
            if (player.transform.position.x - transform.position.x > 0 && !faceLeft)
            {
                // Check if able to "see" the player
                RaycastHit2D hit = Physics2D.Raycast(transform.position + GetComponent<Collider2D>().bounds.extents, (player.transform.position - transform.position).normalized, seekRange);
                if (hit.collider != null && hit.collider.tag == "Player")
                {
                    attackAnim.enabled = true;
                    idleAnim.enabled = false;
                    behavior = BehaviorState.Seek;

                    // Set place to head towards
                    destination = player.transform.position.x - cautionDistance;
                }
            }
            else if (player.transform.position.x - transform.position.x < 0 && faceLeft)
            {
                // Check if able to "see" the player
                RaycastHit2D hit = Physics2D.Raycast(transform.position - GetComponent<Collider2D>().bounds.extents, (player.transform.position - transform.position).normalized, seekRange);
                if (hit.collider != null && hit.collider.tag == "Player")
                {
                    attackAnim.enabled = true;
                    idleAnim.enabled = false;
                    behavior = BehaviorState.Seek;

                    // Set place to head towards
                    destination = player.transform.position.x + cautionDistance;
                }
            }

            // Wait a bit at new destination
            if (Mathf.Abs(transform.position.x - destination) <= margin)
            {
                // Check if done waiting
                if (waitCounter >= waitTime)
                    StartNewPatrol();

                waitCounter += Time.deltaTime;
            }
            // Move towards destination
            else
            {
                if (transform.position.x > destination)
                    transform.Translate(-patrolSpeed, 0, 0);
                else
                    transform.Translate(patrolSpeed, 0, 0);
            }
        }
        else if (behavior == BehaviorState.Seek)
        {
            // Reached caution distance
            if (Mathf.Abs(transform.position.x - player.transform.position.x) - cautionDistance <= margin)
            {
                behavior = BehaviorState.Attack;
            }
            // Lost sight
            else if (Mathf.Abs(transform.position.x - player.transform.position.x) - cautionDistance > seekRange)
            {
                behavior = BehaviorState.Patrol;
                attackAnim.enabled = false;
                idleAnim.enabled = true;
                destination = transform.position.x; // Ez switch to wait
            }
            else
            {
                destination = player.transform.position.x - cautionDistance;

                if (transform.position.x > destination)
                {
                    transform.Translate(-seekSpeed, 0, 0);
                    if (!faceLeft)
                    {
                        faceLeft = true;
                        GetComponent<SpriteRenderer>().flipX = false;
                    }
                }
                else
                {
                    transform.Translate(seekSpeed, 0, 0);
                    if (faceLeft)
                    {
                        faceLeft = false;
                        GetComponent<SpriteRenderer>().flipX = true;
                    }
                }
            }
        }
        else if (behavior == BehaviorState.Attack)
        {
            // Check if Player is fleeing
            if (Mathf.Abs(transform.position.x - player.transform.position.x) - cautionDistance > margin + attackRange)
            {
                GetComponent<LineRenderer>().SetPosition(0, transform.position);
                GetComponent<LineRenderer>().SetPosition(1, transform.position);

                attackCounter = 0;
                behavior = BehaviorState.Seek;
            }
            // Check if cooling down
            else if (onCooldown)
            {
                cooldownCounter += Time.deltaTime;

                if (cooldownCounter >= cooldownTime)
                {
                    GetComponent<LineRenderer>().SetPosition(0, transform.position);
                    GetComponent<LineRenderer>().SetPosition(1, transform.position);

                    // Reset to attacking
                    onCooldown = false;
                    attackCounter = 0;
                    attackAnim.enabled = true;
                    idleAnim.enabled = false;
                }
            }
            // Attacking
            else if (playerStats.canBeAttacked)
            {
                attackCounter += Time.deltaTime;

                // Able to attack
                if (attackCounter >= chargeTime)
                {
                    GameObject target = null;

                    // Get all possible target parts
                    switch (playerController.bodyState)
                    {
                        case PlayerController.BodyState.Full:
                            target = playerStats.rightArm;
                            break;
                        case PlayerController.BodyState.OneArm:
                            target = playerStats.leftArm;
                            break;
                        case PlayerController.BodyState.NoArms:
                            target = playerStats.head;
                            break;
                        case PlayerController.BodyState.NoHead:
                            target = playerStats.body;
                            break;
                        default:
                            target = playerStats.body;
                            break;
                    }

                    DisableOtherParts(target);

                    // Raycast the beam
                    Vector2 dir = (target.transform.position - transform.position).normalized;
                    RaycastHit2D hit = Physics2D.Raycast(transform.position, dir);

                    EnableOtherParts(target);

                    // Check if hit
                    if (hit.collider.gameObject == target)
                    {
                        playerStats.TakeDamage(target, hit, dir, attackForce);

                        GetComponent<LineRenderer>().SetPosition(0, transform.position);
                        GetComponent<LineRenderer>().SetPosition(1, hit.point);

                        GetComponent<AudioSource>().Play();
                    }

                    // Cooldown
                    cooldownCounter = 0;
                    onCooldown = true;
                    attackAnim.enabled = false;
                    idleAnim.enabled = true;
                }
            }
            // Don't start attacking when invincible
            else if (!playerStats.canBeAttacked && attackCounter != 0)
            {
                attackCounter = 0;
                attackAnim.enabled = false;
                idleAnim.enabled = true;
            }
        }*/
    }

    private void StartNewPatrol()
    {
        destination = initialXPos + Random.Range(patrolLength.x, patrolLength.y); // Patrol lengths start to the right of the initial X position
        waitTime = Random.Range(patrolWait.x, patrolWait.y);
        waitCounter = 0;

        if (destination - transform.position.x > 0)
        {
            faceLeft = false;
            GetComponent<SpriteRenderer>().flipX = true;
        }
        else
        {
            faceLeft = true;
            GetComponent<SpriteRenderer>().flipX = false;
        }
    }

    private void DisableOtherParts(GameObject part)
    {
        // Disable all other player parts momentarily
        if (playerStats.head != null && part != playerStats.head)
        {
            Collider2D[] headColliders = playerStats.head.GetComponents<Collider2D>();
            for (int i = 0; i < headColliders.Length; i++)
            {
                headColliders[i].enabled = false;
            }
        }
        if (playerStats.leftArm != null && part != playerStats.leftArm)
        {
            Collider2D[] leftArmColliders = playerStats.leftArm.GetComponents<Collider2D>();
            for (int i = 0; i < leftArmColliders.Length; i++)
            {
                leftArmColliders[i].enabled = false;
            }
        }
        if (playerStats.rightArm != null && part != playerStats.rightArm)
        {
            Collider2D[] rightArmColliders = playerStats.rightArm.GetComponents<Collider2D>();
            for (int i = 0; i < rightArmColliders.Length; i++)
            {
                rightArmColliders[i].enabled = false;
            }
        }
        if (playerStats.body != null && part != playerStats.body)
        {
            Collider2D[] bodyColliders = playerStats.body.GetComponents<Collider2D>();
            for (int i = 0; i < bodyColliders.Length; i++)
            {
                bodyColliders[i].enabled = false;
            }
        }
        if (playerStats.legs != null && part != playerStats.legs)
        {
            Collider2D[] legsColliders = playerStats.legs.GetComponents<Collider2D>();
            for (int i = 0; i < legsColliders.Length; i++)
            {
                legsColliders[i].enabled = false;
            }
        }
    }

    private void EnableOtherParts(GameObject part)
    {
        // Enable all other player parts
        if (playerStats.head != null && part != playerStats.head)
        {
            Collider2D[] headColliders = playerStats.head.GetComponents<Collider2D>();
            for (int i = 0; i < headColliders.Length; i++)
            {
                headColliders[i].enabled = true;
            }
        }
        if (playerStats.leftArm != null && part != playerStats.leftArm)
        {
            Collider2D[] leftArmColliders = playerStats.leftArm.GetComponents<Collider2D>();
            for (int i = 0; i < leftArmColliders.Length; i++)
            {
                leftArmColliders[i].enabled = true;
            }
        }
        if (playerStats.rightArm != null && part != playerStats.rightArm)
        {
            Collider2D[] rightArmColliders = playerStats.rightArm.GetComponents<Collider2D>();
            for (int i = 0; i < rightArmColliders.Length; i++)
            {
                rightArmColliders[i].enabled = true;
            }
        }
        if (playerStats.body != null && part != playerStats.body)
        {
            Collider2D[] bodyColliders = playerStats.body.GetComponents<Collider2D>();
            for (int i = 0; i < bodyColliders.Length; i++)
            {
                bodyColliders[i].enabled = true;
            }
        }
        if (playerStats.legs != null && part != playerStats.legs)
        {
            Collider2D[] legsColliders = playerStats.legs.GetComponents<Collider2D>();
            for (int i = 0; i < legsColliders.Length; i++)
            {
                legsColliders[i].enabled = true;
            }
        }
    }
}
