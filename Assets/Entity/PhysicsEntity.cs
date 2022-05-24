using UnityEngine;

namespace Entity
{
    /// <summary>
    /// An entity that is affected by physics
    /// </summary>
    [RequireComponent(typeof(CollisionEntity))]
    public class PhysicsEntity : MonoBehaviour
    {
        // Serialized Fields - set in the Unity Inspector
        #region SerializedFields
        [SerializeField, Tooltip("How much should gravity affect this object? (Percentage of Physics2D.gravity.y")] 
        private float gravityScale = 1f;
        
        [SerializeField, Tooltip("How quickly should this object slow down? (Percentage speed lost per frame)"), Range(0, 1)] 
        private float drag = 0.1f;
        #endregion
        
        // Private fields - only used in this script
        #region PrivateFields
        private Vector3 _velocity; // how fast are we currently going?
        private CollisionEntity _controller; // the collision entity for this object
        #endregion
        
        // Properties - public access functions
        #region Properties
        // public getter for velocity
        public Vector3 Velocity => _velocity;
        #endregion


        /// <summary>
        /// Called when the game loads
        /// Grab any references we need, and pre-calculate anything we know won't change
        /// </summary>
        private void Awake()
        {
            _controller = GetComponent<CollisionEntity>();
        }

        /// <summary>
        /// Add a force to the object
        /// </summary>
        /// <param name="force">The force to add</param>
        public void AddForce(Vector2 force)
        {
            _velocity += (Vector3)force;
        }
        
        /// <summary>
        /// Set the forces acting on this object to a specific value 
        /// </summary>
        /// <param name="newForce">the new force</param>
        public void SetForce(Vector2 newForce)
        {
            _velocity = newForce;
        }

        /// <summary>
        /// Called every physics tick.
        /// Perform any physics calculations here
        /// </summary>
        private void FixedUpdate()
        {
            // if we're colliding above or below, set y velocity to 0
            if (_controller.CollisionInfo.Above || _controller.CollisionInfo.Below)
                _velocity.y = 0;
            
            // reduce the velocity by drag
            _velocity *= 1-drag;
            
            // add on the gravity force - after drag, because we don't want it to be affected by drag
            _velocity.y += Physics2D.gravity.y * gravityScale;
            
            // move the entity
            _controller.Move(_velocity * Time.deltaTime);
        }
    }
}