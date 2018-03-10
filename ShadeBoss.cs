using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadeBoss : MonoBehaviour
{
    public float moveSpeed;
    public float meleeRange;
    public float damageDelay;
    public float meleeCooldown;
    public float summoningCooldown;
    public float summoningRate;
    public int summonNumber;
    public Collider2D meleeHitBox;
    public Collider2D summoningAuraHitBox;
    public GameObject summonAttack;

    private bool summoning;
    private bool attacking;
    private float nextMelee;
    private float nextSummoning;
    private Transform player;
    private float distanceToPlayer;
    private Vector2 directionalInput;
    private Animator anim;
    private WaitForSeconds summonRateWait;


    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        anim = GetComponent<Animator>();
        meleeHitBox.enabled = false;
        summoningAuraHitBox.enabled = false;
        summonRateWait = new WaitForSeconds(summoningRate);
    }

    void Update()
    {
        if (!attacking && !summoning)
        {
            ChasePlayer();
        }
    }

    private void ChasePlayer()
    {
        // Calculate distance between enemy and player
        distanceToPlayer = Vector2.Distance(player.position, this.transform.position);
        directionalInput = player.position - this.transform.position;

        UpdateSprite();

        if (nextSummoning <= Time.time)
        {
            StartSummoning();
            nextSummoning = Time.time + summoningCooldown;
            return;
        }
        
        if (Vector2.Distance(player.position, this.transform.position) > meleeRange)
        {
            // Normalize vector so the unit doesn't slow way down as it approaches the Player,
            // but continues at the same uniform speed.
            directionalInput.Normalize();
            transform.Translate(directionalInput.x * Time.deltaTime * moveSpeed, directionalInput.y * Time.deltaTime * moveSpeed, 0);
        }
        // Pump the brakes if we get to within our melee range, to avoid moving into the
        // middle of the player's sprite and seizuring back and forth wildly
        else if (Vector2.Distance(player.position, this.transform.position) < meleeRange)
        {
            directionalInput = new Vector2(0, 0);
            if (!summoning && nextMelee <= Time.time)
            {
                StartMeleeAttack();
                nextMelee = Time.time + meleeCooldown;
            }
        }
    }

    private void UpdateSprite()
    {
        // Tracks where the player is relative to the chaser, orients the sprite
        // so that it's always facing the player 
        if (transform.position.x > player.position.x)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (transform.position.x < player.position.x)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    private void StartMeleeAttack()
    {
        attacking = true;
        anim.SetTrigger("MeleeAttack");
        SoundManager.Instance.PlaySound("ShadeMeleeSound");
        Invoke("MeleeDamage", damageDelay);
    }

    private void MeleeDamage()
    {
        meleeHitBox.enabled = true;
        Invoke("ResetBools", 0.25f);
    }

    private void StartSummoning()
    {
        summoning = true;
        anim.SetTrigger("Summoning");
        summoningAuraHitBox.enabled = true;
        StartCoroutine(SummonRoutine());
    }

    IEnumerator SummonRoutine()
    {
        for (int i = 0; i < summonNumber; i++)
        {
            Summon();
            yield return summonRateWait;
        }

        ResetBools();
    }

    private void Summon()
    {
        Debug.Log("Summoning!");
        Instantiate(summonAttack, player.position, Quaternion.identity);
        Invoke("PlayExplosionSound", 0.65f);
    }

    private void PlayExplosionSound()
    {
        SoundManager.Instance.PlaySound("ShadeExplosionSound");
    }

    private void ResetBools()
    {
        if (attacking == true)
        {
            attacking = false;
            meleeHitBox.enabled = false;
        }
        if (summoning == true)
        {
            StopCoroutine("Summon");
            anim.SetTrigger("StopSummoning");
            summoning = false;
            summoningAuraHitBox.enabled = false;
        }
    }

    private void OnDisable()
    {
        ResetBools();
    }
}
