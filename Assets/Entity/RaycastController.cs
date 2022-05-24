using UnityEngine;

namespace Entity
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class RaycastController : MonoBehaviour
    {
        // Constants: values that don't really ever need to change
        #region Constants
        // skin width - we want a little bit of a buffer around the object, otherwise there are certain places where the controller could break and flicker
        protected const float SkinWidth = 0.015f;
        #endregion
        
        
        // Serialized Fields - set in the Unity Inspector
        #region SerializedFields

        [SerializeField, Tooltip("How far apart should the rays be for horizontal collision detection?")]
        protected float distanceBetweenRaysHorizontal = 0.25f;
        [SerializeField, Tooltip("How far apart should the rays be for vertical collision detection?")]
        protected float distanceBetweenRaysVertical = 0.25f;
        #endregion
        
        // Collider
        private BoxCollider2D _collider; // the collider that is attached to this object
        private Bounds _bounds; // the bounds for the collider, cached as a local object so we don't have to create a new one every frame
        
        // Raycasts
        protected RaycastOrigins _raycastOrigins; // the origin points for the raycasts
        protected int _horizontalRayCount;
        protected int _verticalRayCount;
        protected float _horizontalRaySpacing; // how far apart should the rays be, horizontally? Calculated using ray count in Awake
        protected float _verticalRaySpacing; // how far apart should the rays be, horizontally?
        
        
        /// <summary>
        /// Called when the game loads
        /// Grab any references we need, and pre-calculate anything we know won't change
        /// </summary>
        protected virtual void Awake()
        {
            _collider = GetComponent<BoxCollider2D>();
            CalculateRaySpacing();
            if (!Mathf.Approximately(transform.eulerAngles.z, 0))
                Debug.LogWarning($"{gameObject.name} has a rotation of {transform.eulerAngles.z}, it should be 0");
        }
        
        /// <summary>
        /// Update the origin points for the raycasts 
        /// </summary>
        protected void UpdateRaycastOrigins()
        {
            _bounds = _collider.bounds;
            _bounds.Expand(SkinWidth * -2f);
            _raycastOrigins.SetFromBounds(_bounds);
        }

        /// <summary>
        /// Calculate how far apart the rays should be based on the ray count and the collider size
        /// IMPORTANT: If you update the collider size, call this function
        /// </summary>
        private void CalculateRaySpacing()
        {
            _bounds = _collider.bounds;
            _bounds.Expand(SkinWidth * -2f);
            
            
            
            _horizontalRayCount = Mathf.Max(2, Mathf.RoundToInt(_bounds.size.y / distanceBetweenRaysHorizontal));
            _verticalRayCount = Mathf.Max(2, Mathf.RoundToInt(_bounds.size.x / distanceBetweenRaysVertical));

            _horizontalRaySpacing = _bounds.size.y / (_horizontalRayCount - 1);
            _verticalRaySpacing = _bounds.size.x / (_verticalRayCount - 1);
        }
    
        /// <summary>
        /// Helpful container struct for raycast origins
        /// </summary>
        public struct RaycastOrigins
        {
            public Vector2 TopLeft, TopRight;
            public Vector2 BottomLeft, BottomRight;

            public void SetFromBounds(Bounds bounds)
            {
                BottomLeft = new Vector2(bounds.min.x, bounds.min.y);
                BottomRight = new Vector2(bounds.max.x, bounds.min.y);
                TopLeft = new Vector2(bounds.min.x, bounds.max.y);
                TopRight = new Vector2(bounds.max.x, bounds.max.y);
            }
        }
    }
}