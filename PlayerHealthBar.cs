using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{

    public GameObject player;
    public Image healthBar;
    public Image healthBarOutline;

    private Player playerScript;
    private float playerMaxHealth;
    private float playerCurrentHealth;

    void Start()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }

        playerScript = player.GetComponent<Player>();
        playerMaxHealth = PlayerData.playerMaxHealth;
        playerCurrentHealth = PlayerData.playerCurrentHealth;
        OnPlayerHealthChange();

        playerScript.PlayerHealthChange += this.OnPlayerHealthChange;
    }

    public void OnPlayerHealthChange()
    {
        playerCurrentHealth = PlayerData.playerCurrentHealth;

        healthBar.fillAmount = playerCurrentHealth / playerMaxHealth;
        healthBarOutline.fillAmount = playerCurrentHealth / playerMaxHealth;
    }
}
