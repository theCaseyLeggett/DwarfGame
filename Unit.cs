using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Diagnostics;

public abstract class Unit : MonoBehaviour
{
    public GameObject deathSprite;
    public int maxHealth;
    public float moveSpeed;
    public int currentHealth;
    public float flashSpeed;
    public string damagedAudioName;
    [HideInInspector]
    public bool isAlive = true;

    private Color normalColor;
    private Material material;
    private Animator animator;
    private float normalMoveSpeed;
    private WaitForSeconds flashWaitTime;
    private WaitForSeconds flinchWaitTime;


    private void Awake()
    {
        material = GetComponent<SpriteRenderer>().material;
        normalColor = material.color;
        normalMoveSpeed = moveSpeed;
        // Cache these so we don't have to make new ones every time something is damaged
        flashWaitTime = new WaitForSeconds(0.26f);
        flinchWaitTime = new WaitForSeconds(0.15f);
    }

    protected void CheckAlive()
    {
        if (currentHealth <= 0 && isAlive)
        {
            Die();
        }
    }

    private void OnEnable()
    {
        StartCoroutine(ResetColor(normalColor));
        StartCoroutine(ResetMoveSpeed(normalMoveSpeed));
        currentHealth = maxHealth;
        isAlive = true;
    }

    public virtual void TakeDamage(int damage)
    {
        currentHealth -= damage;
        CombatTextManager.Instance.DisplayDamageText(transform.position, damage);
        SoundManager.Instance.PlaySound(damagedAudioName);
        CheckAlive();

        if (isAlive == true)
        {
            //Unit will flash red briefly upon taking damage
            material.color = new Color(1, 0.1f, 0.1f, 1);
            StartCoroutine(ResetColor(normalColor));

            // Unit will briefly flinch and slow down after taking damage
            StartCoroutine(ResetMoveSpeed(normalMoveSpeed));
            moveSpeed *= 0.25f;
        }
    }

    IEnumerator ResetColor(Color color)
    {
        yield return flashWaitTime;
        Material material = GetComponent<SpriteRenderer>().material;
        material.color = new Color(color.r, color.g, color.b, color.a);
        yield return null;
    }

    IEnumerator ResetMoveSpeed(float normalMoveSpeed)
    {
        yield return flinchWaitTime;
        moveSpeed = normalMoveSpeed;
        yield return null;
    }

    public virtual void Heal(int healValue)
    {
        currentHealth += healValue;
        CombatTextManager.Instance.DisplayHealText(transform.position, healValue);
        if (currentHealth >= maxHealth)
        {
            currentHealth = maxHealth;
        }
    }

    protected virtual void Die()
    {
        isAlive = false;
          
        float directionX = Mathf.Sign(transform.position.x);
        GameObject deathChunks = Instantiate(deathSprite, transform.position, transform.rotation);
        deathChunks.transform.localScale = new Vector3(directionX * transform.localScale.x, transform.localScale.y, transform.localScale.z);

        gameObject.SetActive(false);
    }
}
