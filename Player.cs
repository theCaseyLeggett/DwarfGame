using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Controller2D))]
[RequireComponent(typeof(PlayerAnimations))]

public class Player : Unit
{
    public delegate void BroadcastPlayerDeath();
    public event BroadcastPlayerDeath PlayerDied;

    public delegate void BroadcastPlayerHealthChange();
    public event BroadcastPlayerHealthChange PlayerHealthChange;

    [Range(0, 1)]
    public float doubleJumpMultiplier = 0.8f;
    public float maxJumpHeight = 1.45f;
    public float minJumpHeight = 0.3f;
    public float jumpTimeToApex = 0.33f;
    public float wallSlideSpeed;

    public float basicAttackCooldown = 0.66f;
    public float basicAttackAnimation = 0.3f;

    public float meleeAttackCooldown = 0.4f;
    public float meleeAttackAnimation = 0.4f;

    public float explosiveAttackCooldown = 1.5f;

    [HideInInspector]
    public float nextAttack;
    [HideInInspector]
    public float nextMeleeAttack;
    [HideInInspector]
    public float nextExplosiveAttack;
    [HideInInspector]
    public bool wallGrab;

    public string characterName;
    [HideInInspector]
    public int moveDirectionX = 1;

    public GameObject basicAttackProjectile;
    public GameObject explosiveAttackProjectile;
    public Transform projectileSpawn;
    [HideInInspector]
    public Vector2 wallJumpClimb;
    [HideInInspector]
    public Vector2 directionalInput;

    private float accelerationTimeAirborne = .2f;
    private float accelerationTimeGrounded = .1f;
    private float maxJumpVelocity;
    private float minJumpVelocity;
    private float positionXSmoothing;
    private int wallDirX;
    private bool doubleJump = true;
    private bool grabbing = false;

    private PlayerAnimations playerAnimator;
    private Vector2 velocity;
    private Gravity worldGravity;
    private Controller2D controller;
    private Inventory inventory;
    private GameObject deathChunks;

    private void Start()
    {
        // Initialize controller and worldGravity objects, to be used by Player
        // Set currentHealth from Unit class == maxHealth
        controller = GetComponent<Controller2D>();
        worldGravity = GameObject.FindGameObjectWithTag("Gravity").GetComponent<Gravity>();
        playerAnimator = GetComponent<PlayerAnimations>();
        inventory = GetComponent<Inventory>();

        // TODO: I have no idea why this is necessary, it worked fine before but randomly
        // I couldn't jump unless I assigned this value in here rather than the Start()
        // method in the Gravity class.  It worked fine all of yesterday...  No clue!
        worldGravity.gravity = -(2 * maxJumpHeight) / Mathf.Pow(jumpTimeToApex, 2);
        /////////////////////
        maxHealth = PlayerData.playerMaxHealth;
        currentHealth = PlayerData.playerCurrentHealth;
        // Define our jump velocity in terms of gravity's magnitude and time to apex of jump
        maxJumpVelocity = Mathf.Abs(worldGravity.gravity) * jumpTimeToApex;
        minJumpVelocity = Mathf.Sqrt((2 * Mathf.Abs(worldGravity.gravity)) * minJumpHeight);
    }

    private void Update()
    {
        CalculateVelocity();
        HandleWallSliding();
        UpdateSprite();

        controller.Move(velocity * Time.deltaTime, directionalInput);
    }

    public void SetDirectionalInput(Vector2 input)
    {
        directionalInput = input;
    }

    public void OnJumpInput()
    {
        if (wallGrab)
        {
            playerAnimator.playerAnimator.SetTrigger("Jumped");
            if (wallDirX == directionalInput.x)
            {
                velocity.x = velocity.x * -wallJumpClimb.x;
                velocity.y = wallJumpClimb.y;
            }
        }
        if (controller.collisions.below)
        {
            velocity.y = maxJumpVelocity;
            doubleJump = true;
            playerAnimator.JumpAnimation();
        }
        // doubleJump bool and else if statement below allow a second jump at desired % power while airborne
        else if (doubleJump)
        {
            doubleJump = false;
            velocity.y = maxJumpVelocity * doubleJumpMultiplier;
            playerAnimator.DoubleJumpAnimation();
        }
        // TODO: Why is this needed here?  Can't jump without it, even though it's in the update method already...
        controller.Move(velocity * Time.deltaTime, directionalInput);
    }

    public void OnReleaseJumpButton()
    {
        if (velocity.y > minJumpVelocity)
        {
            velocity.y = minJumpVelocity;
        }
    }

    private void CalculateVelocity()
    {
        // If player jumps into an obstacle or lands on an obstacle, set Y velocity to 0
        if (controller.collisions.above || controller.collisions.below)
        {
            velocity.y = 0;
        }
        float targetPosX = directionalInput.x * moveSpeed;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetPosX, ref positionXSmoothing, controller.collisions.below ? accelerationTimeGrounded : accelerationTimeAirborne);
        velocity.y += worldGravity.gravity * Time.deltaTime;
    }

    private void HandleWallSliding()
    {
        wallDirX = (controller.collisions.left ? -1 : 1);
        wallGrab = false;

        // Detect a wall grab
        if ((controller.collisions.left ^ controller.collisions.right)
            && !controller.collisions.below && directionalInput.x == wallDirX)
        {
            wallGrab = true;
            if (velocity.y < 0) { velocity.y = wallSlideSpeed; }
        }
    }

    protected override void Die()
    {
        isAlive = false;

        float directionX = Mathf.Sign(transform.position.x);
        deathChunks = Instantiate(deathSprite, transform.position, transform.rotation);
        deathChunks.transform.localScale = new Vector3(directionX * transform.localScale.x, transform.localScale.y, transform.localScale.z);
        PlayerData.playerDeaths++;

        OnPlayerDeath();

        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        isAlive = true;
        if(deathChunks != null) Destroy(deathChunks);
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
        PlayerData.playerCurrentHealth = currentHealth;
        playerAnimator.Damaged(); // flinch animation
        OnPlayerHealthChange(); // broadcast a change in health for the UI
    }

    public override void Heal(int healValue)
    {
        base.Heal(healValue);
        PlayerData.playerCurrentHealth = currentHealth;
        OnPlayerHealthChange(); // broadcast a change in health for the UI
    }

    protected virtual void OnPlayerDeath()
    {
        if (PlayerDied != null)
        {
            PlayerDied();
        }
    }

    protected virtual void OnPlayerHealthChange()
    {
        if (PlayerHealthChange != null)
        {
            PlayerHealthChange();
        }
    }

    public void BasicAttack()
    {
        nextAttack = basicAttackCooldown + Time.time;
        ThrowHammer();
    }

    public void MeleeAttack()
    {
        nextMeleeAttack = meleeAttackCooldown + Time.time;
        playerAnimator.AxeChop();
    }

    public void ThrowBomb()
    {
        if (inventory.KuldoCrystalCount() > 0)
        {
            nextExplosiveAttack = explosiveAttackCooldown + Time.time;
            playerAnimator.ThrowBomb();
            SpawnBomb();
            inventory.KuldoCrystalChange(-1);
        }
        // else play some error sound to indicate no ammo left 
    }

    private void ThrowHammer()
    {
        SoundManager.Instance.PlaySound("NoriHammerThrow");
        playerAnimator.HammerThrowAnimation();
        Invoke("SpawnHammer", basicAttackAnimation);
    }

    private void SpawnHammer()
    {
        Instantiate(basicAttackProjectile, projectileSpawn.position, projectileSpawn.rotation);
    }

    private void SpawnBomb()
    {
        Instantiate(explosiveAttackProjectile, projectileSpawn.position, projectileSpawn.rotation);
    }

    private void UpdateSprite()
    {
        //Track xVelocity and give to animator to track whether to animate idling or running
        if (Mathf.Abs(directionalInput.x) > 0 && controller.collisions.below)
        {
            playerAnimator.playerAnimator.SetFloat("xVelocity", Mathf.Abs(directionalInput.x));
        }
        else
        {
            playerAnimator.playerAnimator.SetFloat("xVelocity", 0);
        }
        
        if (wallGrab == true && grabbing == false)
        {
            playerAnimator.playerAnimator.SetTrigger("GrabWall");
            grabbing = true;
            playerAnimator.playerAnimator.SetBool("WallSliding", true);
        }
        if (wallGrab == false)
        {
            grabbing = false;
            playerAnimator.playerAnimator.SetBool("WallSliding", false);
        }

        //Track yVelocity and feed it into animator to set the correct upward/falling animations when moving in the Y direction
        playerAnimator.playerAnimator.SetFloat("yVelocity", velocity.y);

        // Player object's direction of motion determines how the sprite renders, flipping it based on current X direction
        // and the Player's input
        if ((directionalInput.x > 0 && moveDirectionX == -1)
            || (directionalInput.x < 0 && moveDirectionX == 1))
        {
            FlipSprite();
        }
    }

    private void FlipSprite()
    {
        moveDirectionX = -moveDirectionX;
        Vector3 localScale = gameObject.transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }
}
