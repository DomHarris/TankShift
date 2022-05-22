using UnityEngine;
using UnityEngine.InputSystem;

namespace Entity
{
    /// <summary>
    /// Move the entity with player input
    /// TODO: probably split tank jetpack into a separate class
    /// </summary>
    [RequireComponent(typeof(CollisionEntity), typeof(PhysicsEntity))]
    public class PlayerMoveEntity : MonoBehaviour
    {
        // Serialized Fields - set in the Unity Inspector
        #region SerializedFields
        [SerializeField, Tooltip("How quickly should the object move, in Unity units per second")] 
        private float speed = 10f;
        
        [SerializeField, Tooltip("How much force should the jetpack apply, in Unity units per second per second")] 
        private float jetpackForce = 10f;
        #endregion
        
        // Private fields - only used in this script
        #region PrivateFields
        private CollisionEntity _controller; // the collision entity for this object
        private PhysicsEntity _physics; // the physics entity for this object
 
        // Input
        private Vector2 _moveAmount; // the current input values
        private bool _isJetpack = false; // is the jetpack button being held?
        private bool _isFirstFrame = false; // is it the first frame the jetpack button is being held?
        #endregion
        
        /// <summary>
        /// Called when the game loads
        /// Grab any references we need, and pre-calculate anything we know won't change
        /// </summary>
        private void Awake()
        {
            _controller = GetComponent<CollisionEntity>();
            _physics = GetComponent<PhysicsEntity>();
        }

        /// <summary>
        /// Called every frame.
        /// Use the input to move the character
        /// </summary>
        private void Update()
        {
            // move the character
            _controller.Move(_moveAmount * Time.deltaTime);
        }
        
        /// <summary>
        /// Called every physics tick.
        /// Perform any physics calculations here
        /// </summary>
        private void FixedUpdate()
        {
            // if we're currently jetpacking, add the jetpack force to the physics object
            if (_isJetpack)
            {
                // if it's the first frame, give us a little boost
                if (_isFirstFrame)
                {
                    _isFirstFrame = false;
                    _physics.SetForce(Vector2.zero);
                }
                _physics.AddForce(Vector2.up * jetpackForce * Time.deltaTime);
            }
        }

        /// <summary>
        /// Get the horizontal input
        /// </summary>
        /// <param name="ctx">the input actions context, used to get the values for the input</param>
        public void GetInput(InputAction.CallbackContext ctx)
        {
            _moveAmount = ctx.ReadValue<Vector2>() * speed;
            // this is a tank, we don't want to move upwards when we press W ;)
            _moveAmount.y = 0;
        }

        
        /// <summary>
        /// Get the jetpack input
        /// </summary>
        /// <param name="ctx">the input actions context, used to get the values for the input</param>
        public void Jetpack(InputAction.CallbackContext ctx)
        {
            //return;
            if (!_isJetpack && ctx.ReadValueAsButton())
                _isFirstFrame = true;
            _isJetpack = ctx.ReadValueAsButton();
        }
    }
}