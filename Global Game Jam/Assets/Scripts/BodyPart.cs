using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPart : MonoBehaviour
{
    #region Inspector Variables
    public string name;
    public bool isAttached;
    public PartType type;
    public float pickupTime;
    public float pickupRadius;
    public TextMesh pickupUI;
    public float pickupUIFadeTime;
    public float pickupUIHeight;
    #endregion

    public enum PartType { Head, Body, Legs, Arm, Charge };

    private GameObject player;
    private PlayerController playerController;
    private PlayerStats playerStats;

    private Rigidbody2D rigidbody;
    private Transform partAnchor;

    private bool fadeIn;
    private bool fadeOut;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerController = player.GetComponent<PlayerController>();
        playerStats = player.GetComponent<PlayerStats>();

        rigidbody = GetComponent<Rigidbody2D>();

        fadeIn = false;
        fadeOut = false;
    }

    private void FixedUpdate()
    {
        // Always keep text upright
        if (pickupUI.gameObject.transform.rotation != Quaternion.identity)
            pickupUI.gameObject.transform.rotation = Quaternion.identity;

        // Always keep body part set onto player
        if (isAttached)
            transform.position = partAnchor.position;

        if (fadeIn && !fadeOut)
        {
            pickupUI.color = new Color(pickupUI.color.r, pickupUI.color.g, pickupUI.color.b, pickupUI.color.a + Mathf.Lerp(0, 1, pickupUIFadeTime));

            if (pickupUI.color.a >= 1)
                fadeIn = false;
        }
        else if (fadeOut)
        {
            pickupUI.color = new Color(pickupUI.color.r, pickupUI.color.g, pickupUI.color.b, pickupUI.color.a - Mathf.Lerp(0, 1, pickupUIFadeTime));

            if (pickupUI.color.a == 0)
                fadeOut = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!isAttached && // Check if not attached to player
            playerController.grounded && // Check if the player is currently on the ground
            Mathf.Abs((player.transform.position - transform.position).magnitude) <= pickupRadius && // Check if the player is within range
            OnGround() && // Part isn't moving
            CheckCanAttach()) // Can currently be equipped
        {
            // Attach to player
            if (Input.GetButtonDown("Interact"))
            {
                AttachPart(type);
            }

            // Check if displayed
            if (pickupUI.color.a != 1 && !fadeOut)
            {
                fadeIn = true;
                pickupUI.gameObject.transform.position = transform.position + new Vector3(0, pickupUIHeight);
            }
        }
        // Check if not near player
        else if (!isAttached && pickupUI.color.a != 0 && !fadeIn)
            fadeOut = true;
        // Make text transparent upon equip
        else if (isAttached && pickupUI.color.a != 0)
            pickupUI.color = new Color(pickupUI.color.r, pickupUI.color.g, pickupUI.color.b, 0);

        /*// Always keep text upright
        if (pickupUI.gameObject.transform.rotation != Quaternion.identity)
            pickupUI.gameObject.transform.rotation = Quaternion.identity;

        // Always keep body part set onto player
        if (isAttached)
            transform.position = partAnchor.position;

        if (fadeIn && !fadeOut)
        {
            pickupUI.color = new Color(pickupUI.color.r, pickupUI.color.g, pickupUI.color.b, pickupUI.color.a + Mathf.Lerp(0, 1, pickupUIFadeTime));

            if (pickupUI.color.a >= 1)
                fadeIn = false;
        }
        else if (fadeOut)
        {
            pickupUI.color = new Color(pickupUI.color.r, pickupUI.color.g, pickupUI.color.b, pickupUI.color.a - Mathf.Lerp(0, 1, pickupUIFadeTime));

            if (pickupUI.color.a == 0)
                fadeOut = false;
        }

        if (!isAttached && // Check if not attached to player
            playerController.grounded && // Check if the player is currently on the ground
            Mathf.Abs((player.transform.position - transform.position).magnitude) <= pickupRadius && // Check if the player is within range
            OnGround() && // Part isn't moving
            CheckCanAttach()) // Can currently be equipped
        {
            // Attach to player
            if (Input.GetButtonDown("Interact"))
            {
                AttachPart(type);
            }

            // Check if displayed
            if (pickupUI.color.a != 1 && !fadeOut)
            {
                fadeIn = true;
                pickupUI.gameObject.transform.position = transform.position + new Vector3(0, pickupUIHeight);
            }
        }
        // Check if not near player
        else if (!isAttached && pickupUI.color.a != 0 && !fadeIn)
            fadeOut = true;
        // Make text transparent upon equip
        else if (isAttached && pickupUI.color.a != 0)
            pickupUI.color = new Color(pickupUI.color.r, pickupUI.color.g, pickupUI.color.b, 0);*/
    }

    private void AttachPart(PartType partType)
    {
        if (CheckCanAttach())
        {
            switch (partType)
            {
                case PartType.Head:
                    playerStats.head = gameObject;
                    gameObject.transform.position = playerStats.headPos.position;
                    partAnchor = playerStats.headPos;
                    break;
                case PartType.Arm:
                    if (playerStats.leftArm == null)
                    {
                        playerStats.leftArm = gameObject;
                        gameObject.transform.position = playerStats.leftArmPos.position;
                        partAnchor = playerStats.leftArmPos;
                    }
                    else if (playerStats.rightArm == null)
                    {
                        playerStats.rightArm = gameObject;
                        gameObject.transform.position = playerStats.rightArmPos.position;
                        partAnchor = playerStats.rightArmPos;
                    }
                    break;
                case PartType.Legs:
                    playerStats.legs = gameObject;
                    gameObject.transform.position = playerStats.legsPos.position;
                    partAnchor = playerStats.legsPos;
                    break;
                case PartType.Body:
                    playerStats.body = gameObject;
                    gameObject.transform.position = playerStats.bodyPos.position;
                    partAnchor = playerStats.bodyPos;
                    break;
                case PartType.Charge:
                    playerController.hasCharge = true;
                    break;
            }
        }

        playerController.clickSound.Play();

        if (type != PartType.Charge)
        {
            isAttached = true;
            playerController.bodyState--;
            playerController.ChangeAnimState(playerController.anim);
            transform.rotation = Quaternion.identity;
            transform.SetParent(player.transform);
            Color spriteColor = GetComponent<SpriteRenderer>().color;
            GetComponent<SpriteRenderer>().color = new Color(spriteColor.r, spriteColor.g, spriteColor.b, 0); // Make transparent
            rigidbody.bodyType = RigidbodyType2D.Static;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Detach()
    {
        isAttached = false;
        Color spriteColor = GetComponent<SpriteRenderer>().color;
        GetComponent<SpriteRenderer>().color = new Color(spriteColor.r, spriteColor.g, spriteColor.b, 1); // Make visible
        rigidbody.bodyType = RigidbodyType2D.Dynamic;
        rigidbody.gravityScale = 2;
        transform.parent = null;

        switch (type)
        {
            case PartType.Head:
                playerStats.head = null;
                break;
            case PartType.Arm:
                if (playerStats.leftArm == gameObject)
                    playerStats.leftArm = null;
                else if (playerStats.rightArm == gameObject)
                    playerStats.rightArm = null;
                break;
            case PartType.Legs:
                playerStats.legs = null;
                break;
            case PartType.Body:
                playerStats.body = null;
                break;
        }
    }

    private bool OnGround()
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position + Vector3.up, Vector2.down * 0.05f);

        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].collider.tag == "Ground" &&
                rigidbody.velocity == Vector2.zero)
                return true;
        }

        return false;
    }

    private bool CheckCanAttach()
    {
        switch (type)
        {
            case PartType.Head:
                if (playerStats.head == null && playerController.bodyState == PlayerController.BodyState.NoHead)
                    return true;
                else
                    return false;
            case PartType.Arm:
                if ((playerStats.leftArm == null  && playerController.bodyState == PlayerController.BodyState.NoArms) ||
                    (playerStats.rightArm == null && playerController.bodyState == PlayerController.BodyState.OneArm))
                    return true;
                else
                    return false;
            case PartType.Charge:
                return true;
            default:
                return false;
        }
    }
}
