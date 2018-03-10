using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RuneGolemTrigger : MonoBehaviour
{
    public GameObject runeGolem;
    public GameObject forceField;
    public GameObject mainCamera;
    public Image bossHealthBar;
    public Image bossHealthBarBackground;
    public Image bossHealthBarOverlay;
    public Text bossNameText;
    public Transform runeGolemStart;

    private GameObject playerObject;
    private Player playerScript;
    private bool hasEntered = false;
    private bool isDead;

    private void Start()
    {
        runeGolem.SetActive(false);
        bossHealthBar.enabled = false;
        bossHealthBarBackground.enabled = false;
        bossHealthBarOverlay.enabled = false;
        bossNameText.enabled = false;
        forceField.SetActive(false);
        isDead = false;

        playerObject = GameObject.FindGameObjectWithTag("Player");
        playerScript = playerObject.GetComponent<Player>(); // publisher
        playerScript.PlayerDied += this.OnPlayerDeath; // subscriber
    }

    private void OnPlayerDeath()
    {
        if (hasEntered == true)
        {
            Invoke("ResetGolem", 5.0f);
        }
    }

    private void ResetGolem()
    {
        GetComponent<AudioSource>().Stop();
        runeGolem.GetComponent<Unit>().currentHealth = runeGolem.GetComponent<Unit>().maxHealth;
        runeGolem.GetComponent<GolemBoss>().ResetWaypoints();
        runeGolem.transform.position = runeGolemStart.position;
        runeGolem.SetActive(false);
        bossHealthBar.enabled = false;
        bossHealthBarBackground.enabled = false;
        bossHealthBarOverlay.enabled = false;
        bossNameText.enabled = false;
        GetComponent<BoxCollider2D>().enabled = true;
        forceField.SetActive(false);
        ResetCamera();
        hasEntered = false;
        isDead = false;
    }

    private void Update()
    {
        if (hasEntered == true && runeGolem.gameObject.activeInHierarchy == false)
        {
            GetComponent<AudioSource>().Stop();
            bossHealthBar.enabled = false;
            bossHealthBarBackground.enabled = false;
            bossHealthBarOverlay.enabled = false;
            bossNameText.enabled = false;
            Invoke("ResetCamera", 1.5f);
            isDead = true;
            forceField.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player" && hasEntered == false)
        {
            GetComponent<AudioSource>().Play();
            hasEntered = true;
            runeGolem.SetActive(true);
            forceField.SetActive(true);
            bossHealthBar.enabled = true;
            bossHealthBarBackground.enabled = true;
            bossHealthBarOverlay.enabled = true;
            bossNameText.enabled = true;
            mainCamera.GetComponent<CameraFollow>().minX = 38.25f;
            mainCamera.GetComponent<CameraFollow>().maxX = 38.25f;
            mainCamera.GetComponent<CameraFollow>().minY = 9.5f;

            GetComponent<BoxCollider2D>().enabled = false;
        }
    }

    private void ResetCamera()
    {
        mainCamera.GetComponent<CameraFollow>().minX = -4;
        mainCamera.GetComponent<CameraFollow>().maxX = 42;
        mainCamera.GetComponent<CameraFollow>().minY = 0.5f;
    }

    private void OnEnable()
    {
        hasEntered = false;
    }
}
