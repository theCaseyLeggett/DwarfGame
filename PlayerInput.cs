using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Player))]
public class PlayerInput : MonoBehaviour
{

    Player player;

    void Start()
    {
        player = GetComponent<Player>();
    }

    void Update()
    {
        Vector2 directionalInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        player.SetDirectionalInput(directionalInput);

        if (Input.GetButtonDown("Jump") && Input.GetAxis("Vertical") > -0.2f)
        {
            player.OnJumpInput();
        }

        if (Input.GetButtonUp("Jump"))
        {
            player.OnReleaseJumpButton();
        }
        // Used to change layer mask if standing on a platform.  If Player is on a pass-through platform or 
        // other terrain element then they will fall through upon hitting the "down" key.  They will still be 
        // stopped by solid terrain because every piece of solid terrain has a Solid Obstacles BoxCollider2D and
        // a Pass Through Obstacles BoxCollider2D on one of their children; thus even if the player is only
        // colliding with Pass Through Obstacles after hitting down, they will still hit non-passable colliders.
        // At present I have it set up a bit weirdly in that the player collides with Pass Through Obstacles when
        // dropping down, but it gets the job done and isn't relevant to the functionality.  Since platforms will 
        // have a trigger that sets collisionMask on Player beneath them, any time a player does drop through such
        // a platform they will be returned to their default collisionMask layer as they fall through.
        if (Input.GetAxis("Vertical") < -0.6f && player.GetComponent<Controller2D>().collisions.below
            && Input.GetButtonDown("Jump"))
        {
            int desiredLayer = 11;
            int desiredMask = 1 << desiredLayer;
            player.GetComponent<RaycastController>().collisionMask = desiredMask;
        }

        if (Input.GetButtonDown("Fire1") && player.nextAttack <= Time.time && player.wallGrab == false)
        {
            player.BasicAttack();
        }

        if (Input.GetButtonDown("Fire2") && player.nextMeleeAttack <= Time.time && player.wallGrab == false)
        {
            player.MeleeAttack();
        }

        if (Input.GetButtonDown("Fire3") && player.nextExplosiveAttack <= Time.time && player.wallGrab == false)
        {
            player.ThrowBomb();
        }
    }
}
