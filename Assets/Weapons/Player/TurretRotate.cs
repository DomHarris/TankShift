using UnityEngine;
using UnityEngine.InputSystem;

namespace Bullets.Player
{
    /// <summary>
    /// Rotate an object based on the mouse position
    /// </summary>
    public class TurretRotate : MonoBehaviour
    {
        // Serialized Fields - set in the Unity Inspector
        #region SerializedFields
        [SerializeField] private float minAngle;
        [SerializeField] private float maxAngle = 90;
        [SerializeField] private Camera mainCam;
        #endregion
        
        // Private fields - only used in this script
        #region PrivateFields
        private Vector3 _inputPos;
        #endregion
        
        /// <summary>
        /// Called every frame.
        /// Rotate the object with the input's mouse position
        /// </summary>
        private void Update()
        {
            // transform the mouse position (e.g. (1920,1080) for the top right corner) into a world position based on the camera's position 
            var mouseWorld = mainCam.ScreenToWorldPoint(_inputPos);
            // set the Z position to this object's position to stop it rotating in other axes
            mouseWorld.z = transform.position.z;
            
            // get the direction from the mouse position to this object
            var direction = mouseWorld - transform.position;

            // that's the direction we want the turret to point
            // directly setting the transform's "right" vector to this value will do this for us
            transform.right = direction;
            
            // get the current angle between the object's up vector and this direction
            float currentAngle = Vector3.Angle(transform.parent.up, direction);
            
            // if it's less than the min angle, set the "right" vector to the minimum angle
            if (currentAngle < minAngle)
            {
                // -minAngle because Unity rotates in the opposite direction
                Vector3 minAngleDirection = Quaternion.Euler (0,0,-minAngle) * transform.parent.up; // to rotate a vector, we do quaternion * vector
                transform.right = minAngleDirection;
            }
            
            // if it's greater than the current angle, set the "right" vector to the maximum angle
            if (currentAngle > maxAngle)
            {
                // there's a bit of a fudge here, we don't know which way the angle is pointing so we use the sign of the x direction 
                // again, -direction.x because Unity rotates in the opposite direction
                Vector3 maxAngleDirection = Quaternion.Euler (0,0,maxAngle * Mathf.Sign(-direction.x)) * transform.parent.up;
                transform.right = maxAngleDirection;
            }
        }

        /// <summary>
        /// Get the current mouse position
        /// </summary>
        /// <param name="ctx">the input actions context, used to get the values for the input</param>
        public void GetMousePos(InputAction.CallbackContext ctx)
        {
            _inputPos = ctx.ReadValue<Vector2>();
            // reset the Z value, otherwise ScreenToWorldPoint is incorrect
            _inputPos.z = 0;
        }
    }
}
