using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthCollectible : MonoBehaviour
{
    public int healValue;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            if (other.GetComponent<Unit>().currentHealth < other.GetComponent<Unit>().maxHealth)
            {
                other.GetComponent<Unit>().Heal(healValue);
                SoundManager.Instance.PlaySound("HealPickupSound");
                transform.parent.gameObject.SetActive(false);
            }
            else
            {
                CombatTextManager.Instance.DisplayText(this.transform.position, "Health full!");
            }
        }
    }
}
