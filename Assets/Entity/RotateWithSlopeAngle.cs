using System;
using UnityEngine;

namespace Entity
{
    /// <summary>
    /// Helper class to rotate and reposition an object so it lies flat on a slope 
    /// </summary>
    public class RotateWithSlopeAngle : MonoBehaviour
    {
        // Serialized Fields - set in the Unity Inspector
        #region SerializedFields
        [SerializeField, Tooltip("Which layers do we want to check against?")] 
        private LayerMask collisions;
        [SerializeField, Tooltip("Where should the raycast fire from?")] 
        private Vector3 rayOffset;
        
        [SerializeField, Tooltip("How far below should we check, in Unity units")] 
        private float rayLength = 1f;
        #endregion
        
        /// <summary>
        /// Called every physics tick.
        /// Perform any physics calculations here
        /// </summary>
        private void FixedUpdate()
        {
            // raycast down to see if there's anything below us
            // TODO: change this to RaycastNonAlloc
            var hit = Physics2D.Raycast(transform.parent.position + rayOffset, Vector2.down, 1, collisions);
            if (hit)
            {
                // if we change the "up" vector, this rotates the object
                // we want it to rotate so the green (y) axis in Unity aligns with a vector perpendicular with the surface of the slope
                // hit.normal = a vector perpendicular with the surface of the slope
                // this works quite nicely for rotating to align with the slope 😂
                transform.up = hit.normal;
                
                // move the object so it touches the slope
                // best to do this in a child object so we're not messing with the physics at all
                var pos = transform.localPosition;
                pos.y = -hit.distance;
                transform.localPosition = pos;
            }
            else // we didn't hit anything, so we want to be flat
                transform.up = Vector3.up;
        }
    }
}