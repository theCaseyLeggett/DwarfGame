using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Require a BoxCollider2D script on any object using this script, to avoid null reference errors and confusion
[RequireComponent(typeof(BoxCollider2D))]
public class RaycastController : MonoBehaviour
{
    // Creates an option in the inspector to select Layers, which we will use to determine what game objects will be involved in collision detections
    public LayerMask collisionMask;
    // Instance of CollisionInfo struct that can call Reset() method and store/alter our variables as we move through the game world

    // skinDepth determines how far inside of the box collider our rays will originate, RayCount ints determine how many rays we will have emanating from the Player object
    protected const float skinDepth = 0.015f;
    private const float distBetweenRays = 0.065f;
    public int horizontalRayCount;
    public int verticalRayCount;
        
    // Used to determine how far apart rays ought to be from one another based on number of them and size, see UpdateRaycastOrigins for utilization
    protected float horizontalRaySpacing;
    protected float verticalRaySpacing;

    // Instance used to access our BoxCollider2D on the object
    protected BoxCollider2D collidey;
    // Instance of RaycastOrigins struct that can call the four corner locations on our BoxCollider2D
    protected RaycastOrigins raycastOrigins;

    public virtual void Awake()
    {
        collidey = GetComponent<BoxCollider2D>();
    }

    public virtual void Start()
    {
        CalculateRaySpacing();
    }
    // Sets the position of each of the four corners in RaycastOrigins struct instance, which will constrain the Raycasts added to the box collider
    // Starts the rays **INSIDE** the BoxCollider2D on our player object, based on assigned skinDepth value
    protected void UpdateRaycastOrigins()
    {
        Bounds bounds = collidey.bounds;
        bounds.Expand(skinDepth * -1);

        raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
    }

    // Calculates how far apart the rays should be from one another based on the number of rays and the size of the BoxCollider2D
    protected void CalculateRaySpacing()
    {
        Bounds bounds = collidey.bounds;
        bounds.Expand(skinDepth * -1);

        float boundsWidth = bounds.size.x;
        float boundsHeight = bounds.size.y;

        horizontalRayCount = Mathf.RoundToInt(boundsHeight / distBetweenRays);
        verticalRayCount = Mathf.RoundToInt(boundsWidth / distBetweenRays);

        horizontalRaySpacing = boundsHeight / (horizontalRayCount - 1);
        verticalRaySpacing = boundsWidth / (verticalRayCount - 1);
    }

    // A struct for the four corner positions of our Player object's 2D box collider
    protected struct RaycastOrigins
    {
        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, bottomRight;
    }
}

