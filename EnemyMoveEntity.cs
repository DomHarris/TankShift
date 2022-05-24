using UnityEngine;
using UnityEngine.InputSystem;

namespace Entity
{
    /// <summary>
    /// Enemy entity moves in relation to player
    /// </summary>
    [RequireComponent(typeof(CollisionEntity), typeof(PhysicsEntity))]
    public class EnemyMoveEntity : MonoBehaviour
    {
        // Serialized Fields - set in the Unity Inspector
        // Unsure if enemies should have Jetpack, leaving for now
        #region SerializedFields
        [SerializeField, Tooltip("Tracks the player location")]
        Transform player;

        [SerializeField, Tooltip("Modifies range of aggression in relation to player")]
        float aggroRange;

        [SerializeField, Tooltip("The target object")]
        private Transform target;

        // Rigidbody2D here may be unnecessary as stated on line 51
        // *** Rigidbody2D = rb2d;

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

            // Rigidbody2D here may be unnecessary as stated on line 21
            // rb2d = GetComponent<Rigidbody2D>();

        }

        /// <summary>
        /// Called every frame.
        /// Use the input to move the character
        /// </summary>
        private void Update()
        {
            // move the character
            _controller.Move(_moveAmount * Time.deltaTime);

            {
                //distance to target
                float distToTarget = Vector2.Distance(transform.position, target.position);
                // print("distToTarget:" + distToTarget); 25-27 seems to be the max range on SampleScene at start
                if (distToTarget < aggroRange)
                {
                    // code to chase target
                    chaseTarget();
                }
                else
                {
                    // stop chasing target
                    stopChaseTarget();
                }
            }
        }
        private void chaseTarget()
        {
            if (transform.position.x < target.position.x)
            {
                // this is where rb2d was supposed to be used, not sure how to utilize it however
                // refer to line 22 and 52 in EnemyMoveEntity script
                _controller.Move(new Vector2(speed * Time.deltaTime, 0));
            }
            else if (transform.position.x > target.position.x)
            {
                _controller.Move(new Vector2(-speed * Time.deltaTime, 0));
            }
        }
        private void stopChaseTarget()
        {
            // refer to line 72-73 above   REFER TO YouTube vid youtube.com/watch?v=nEYA3hzZHJ0
            _controller.Move(new Vector2(0, 0));
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