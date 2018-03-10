using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimations : MonoBehaviour
{
    public GameObject noriMeleeLight;
    [HideInInspector]
    public Animator playerAnimator;
    private GameObject player;
    public PolygonCollider2D meleeHitBox;
    public CircleCollider2D meleePhysics;
    

    private void Awake()
    {
        playerAnimator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");
        meleeHitBox.enabled = false;
        meleePhysics.enabled = false;
    }

    public void HammerThrowAnimation()
    {
        playerAnimator.SetTrigger("ThrowHammer");
    }

    public void AxeChop()
    {
        EnableCollider();
        playerAnimator.SetTrigger("AxeChop");
        SoundManager.Instance.PlaySound("NoriAxeSwing");
        Invoke("DisableCollider", 0.15f);
    }

    public void JumpAnimation()
    {
        playerAnimator.SetTrigger("Jumped");
        SoundManager.Instance.PlaySound("NoriJumpSound");
    }

    public void DoubleJumpAnimation()
    {
        playerAnimator.SetTrigger("DoubleJumped");
        SoundManager.Instance.PlaySound("NoriDoubleJumpSound");
    }

    public void ThrowBomb()
    {
        playerAnimator.SetTrigger("ThrowBomb");
    }

    public void Damaged()
    {
        playerAnimator.SetTrigger("Damaged");
    }

    private void EnableCollider()
    {
        meleeHitBox.enabled = true;
        noriMeleeLight.SetActive(true);
        meleePhysics.enabled = true;
    }

    private void DisableCollider()
    {
        meleeHitBox.enabled = false;
        noriMeleeLight.SetActive(false);
        meleePhysics.enabled = false;
    }
}
