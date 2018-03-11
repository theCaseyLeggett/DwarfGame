using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller2D : RaycastController
{
    // Sets the max angles possible for climbing and descending sloped terrain
    private float maxSlopeAngle = 70f;

    public CollisionInfo collisions;
    [HideInInspector]
    public Vector2 playerInput;

    public override void Start()
    {
        base.Start();
        collisions.facingDirection = 1;
    }

    public void Move(Vector2 moveAmount, bool standingOnPlatform)
    {
        Move(moveAmount, Vector2.zero, standingOnPlatform);
    }

    public void Move(Vector2 moveAmount, Vector2 input, bool standingOnPlatform = false)
    {
        UpdateRaycastOrigins();
        collisions.Reset();
        collisions.moveAmountPrevious = moveAmount;
        playerInput = input;

        if (moveAmount.x != 0)
        {
            collisions.facingDirection = (int)Mathf.Sign(moveAmount.x);
        }
        // If we are moving in a downward direction, check to see if we need to be descending a slope
        if (moveAmount.y < 0) DescendSlope(ref moveAmount);
        // Check for horizontal collisions (no X moveAmount check b/c of wall sliding)
        HorizontalCollisions(ref moveAmount);
        // If we are moving at all in the Y direction, check for vertical collisions
        if (moveAmount.y != 0) VerticalCollisions(ref moveAmount);
        // Move our Player object according to operations on our Vector2 above
        transform.Translate(moveAmount);

        if (standingOnPlatform)
        {
            collisions.below = true;
        }
    }

    private void HorizontalCollisions(ref Vector2 moveAmount)
    {
        // Stores whether we are moving in a positive (rightward) or negative (leftward) X direction
        float directionX = collisions.facingDirection;
        // Used to determine length of Raycast used for detection: equal to the X component of our vector plus skinDepth
        float rayLength = Mathf.Abs(moveAmount.x) + skinDepth;

        if (Mathf.Abs(moveAmount.x) < skinDepth)
        {
            rayLength = 2 * skinDepth;
        }


        //Detect horizontal collisions with all rays for normal collisions
        for (int i = 0; i < horizontalRayCount; i++)
        {
            // If we are moving right (positive X) Raycast from bottom right corner of BoxCollider2D
            // If we are moving left (negative X) Raycast from bottom left corner of BoxCollider2D
            Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
            rayOrigin += Vector2.up * (horizontalRaySpacing * i);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

            if (hit)
            {
                if (hit.distance == 0)
                {
                    continue;
                }
                // Gets the slope of any obstacles we encounter
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

                if (i == 0 && slopeAngle <= maxSlopeAngle)
                {
                    if (collisions.descendingSlope)
                    {
                        collisions.descendingSlope = false;
                        moveAmount = collisions.moveAmountPrevious;
                    }
                    float distanceToSlopeStart = 0;

                    if (slopeAngle != collisions.slopeAnglePrevious)
                    {
                        distanceToSlopeStart = hit.distance - skinDepth;
                        moveAmount.x -= distanceToSlopeStart * directionX;
                    }

                    ClimbSlope(ref moveAmount, slopeAngle);
                    moveAmount.x += distanceToSlopeStart * directionX;
                }
                if (!collisions.climbingSlope || slopeAngle > maxSlopeAngle)
                {
                    moveAmount.x = (hit.distance - skinDepth) * directionX;
                    rayLength = hit.distance;

                    if (collisions.climbingSlope)
                    {
                        moveAmount.y = Mathf.Tan((collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(moveAmount.x));
                    }

                    collisions.left = directionX == -1;
                    collisions.right = directionX == 1;
                }
            }
            Debug.DrawRay(rayOrigin, Vector2.right * directionX, Color.red);
        }
    }

    private void VerticalCollisions(ref Vector2 moveAmount)
    {
        float directionY = Mathf.Sign(moveAmount.y);
        float rayLength = Mathf.Abs(moveAmount.y) + skinDepth;

        for (int i = 0; i < verticalRayCount; i++)
        {
            Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
            rayOrigin += Vector2.right * (verticalRaySpacing * i + moveAmount.x);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);

            if (hit)
            {
                moveAmount.y = (hit.distance - skinDepth) * directionY;
                rayLength = hit.distance;

                if (collisions.climbingSlope)
                {
                    moveAmount.x = moveAmount.y / Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(moveAmount.x);
                }

                collisions.above = (directionY == 1);
                collisions.below = (directionY == -1);
            }
            Debug.DrawRay(rayOrigin, Vector2.up * directionY, Color.red);
        }

        if (collisions.climbingSlope)
        {
            float directionX = Mathf.Sign(moveAmount.x);
            rayLength = Mathf.Abs(moveAmount.x) + skinDepth;
            Vector2 rayOrigin = ((directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight) + Vector2.up * moveAmount.y;
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

            if (hit)
            {
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if (slopeAngle != collisions.slopeAngle)
                {
                    moveAmount.x = (hit.distance - skinDepth) * directionX;
                    collisions.slopeAngle = slopeAngle;
                }
            }
        }
    }

    private void ClimbSlope(ref Vector2 moveAmount, float slopeAngle)
    {

        float moveDistance = Mathf.Abs(moveAmount.x);
        float climbmoveAmountY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;

        // If our current Y moveAmount is less than or equal to slope climbing moveAmount Y, then we know that the 
        // Player is not jumping. 
        if (moveAmount.y <= climbmoveAmountY)
        {
            moveAmount.y = climbmoveAmountY;
            moveAmount.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(moveAmount.x);

            // Safe to assume if we are climbing a slope that we are colliding with an object below our character.
            // Since we are moving upwards we are detecting for collisions ABOVE the character, not BELOW
            // Ergo we need to set collisions.below to true so that we can still jump when climbing slopes
            collisions.below = true;
            collisions.climbingSlope = true;
            collisions.slopeAngle = slopeAngle;
        }
    }

    void DescendSlope(ref Vector2 moveAmount)
    {
        // Stores whether our player is moving in the positive (1) or negative (-1) X direction. 
        float directionX = Mathf.Sign(moveAmount.x);

        // If we are moving in negative X direction, our descending slope ray will be shot from the bottom right,
        // because that is what would be in contact with a slope when moving leftward across the scree.
        // If we are moving in positive X direction our ray will originate in the bottom left corner,
        // because when moving down a slope to the right our bottom left corner contacts the terrain.
        Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomRight : raycastOrigins.bottomLeft;

        // Fires a ray from the above rayOrigin in the downward direction, down to infinity,
        //// sensitive to whatever BoxCollider2D-equipped-objects reside on the layers we select in the Unity Inspector.
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, Mathf.Infinity, collisionMask);

        // When our raycast hits something we check to see if has a non-0 slope and if it is <= our maxDescendAngle value.
        // This will eschew flat terrain and terrain that is too steeply angled for controlled descent.
        if (hit)
        {
            float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
            if (slopeAngle != 0 && slopeAngle <= maxSlopeAngle)
            {
                // Check to see if the slope orientation (pos/neg of X component of a vector fired out from object surface at 90 degrees)
                // is the same as the direction in which we are currently moving.  E.g. negative X slope would be " / ", positive X " \ "
                // This matters because if we are going in the positive direction we can only be descending positive X slopes, and vice versa.
                // The opposite would be true for ascending slopes (i.e. we could only ascend positive X slopes if moving in a negative X direction).
                if (Mathf.Sign(hit.normal.x) == directionX)
                {
                    // Check to see if the distance from our Player's BoxCollider2D to the hit object is <= 
                    // 
                    if (hit.distance - skinDepth <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(moveAmount.x))
                    {
                        float moveDistance = Mathf.Abs(moveAmount.x);
                        float descendmoveAmountY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
                        moveAmount.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(moveAmount.x);
                        moveAmount.y -= descendmoveAmountY;

                        collisions.slopeAngle = slopeAngle;
                        collisions.descendingSlope = true;
                        collisions.below = true;
                    }
                }
            }
        }
    }
    public struct CollisionInfo
    {
        public bool above, below, left, right;
        public bool climbingSlope, descendingSlope;
        public float slopeAngle, slopeAnglePrevious;
        public Vector2 moveAmountPrevious;
        public int facingDirection;

        // Resets all bools to false to refresh collision detection each frame 
        // Sets slopeAngle to 0 after storing its value, which is used to detect a change in slope angle frame by frame
        public void Reset()
        {
            above = below = left = right = false;
            climbingSlope = false;
            descendingSlope = false;
            slopeAnglePrevious = slopeAngle;
            slopeAngle = 0;
        }
    }
}
