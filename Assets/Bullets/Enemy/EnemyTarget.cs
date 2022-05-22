using UnityEngine;

namespace Bullets.Enemy
{
    /// <summary>
    /// Target the target using projectile motion equations
    /// </summary>
    public class EnemyTarget : MonoBehaviour
    {
        // Serialized Fields - set in the Unity Inspector
        #region SerializedFields
        
        [SerializeField, Tooltip("Where should the bullet shoot from?")] 
        private Transform shootPoint;

        [SerializeField, Tooltip("How much force should we apply to the bullet when it is fired?")] 
        private float force;

        [SerializeField, Tooltip("The target object")] 
        private Transform target;

        [Header("Visualisation"), SerializeField, Tooltip("The spacing between dots in the visualiser")]
        private float shootDisplayTimeDelta = 0.1f;
        
        [SerializeField, Tooltip("The camera used for the trajectory visualisation")] 
        private Camera cam;
        
        [SerializeField, Tooltip("The mesh used to draw each point in the trajectory visualisation")] 
        private Mesh shootDisplayMesh;
        
        [SerializeField, Tooltip("The material used to draw each point in the trajectory visualisation")]
        private Material shootDisplayMaterial;

        [SerializeField, Tooltip("Damping speed for the targeting. Higher = will take more time to move to the target")] 
        private float targetSpeed = 0.2f;
        #endregion
        
        // Private fields - only used in this script
        #region PrivateFields
        private Vector3 _velocityDamp; // used for smoothly moving to the target, so we don't instantly target the player
        private Vector3 _targetPos; // the current target position
        #endregion
        
        
        private void Update()
        {
            Shoot();
        }

        private void Shoot()
        {
            // smoothly move towards the target, to give the player a chance to avoid
            _targetPos = Vector3.SmoothDamp(_targetPos, target.position, ref _velocityDamp, targetSpeed);
            
            // mathematical wizardry that tells us what angle to rotate the turret to
            var canShoot = Ballistics.CalculateTrajectory(shootPoint.position, _targetPos, force, out var angle);
            
            // if the above function failed, we don't need to do anything else 
            if (!canShoot) return;
            
            // which direction is the player?
            var dirX = Mathf.Sign(target.position.x - transform.position.x);

            // rotate the turret by -angle (because Unity rotates the wrong way ugh)
            var right = Quaternion.Euler(0, 0, -angle) * Vector3.right;
            
            // the angle is only ever going to point to the right 
            // so multiply the x component by the direction we found earlier - this corrects that
            right.x *= dirX;
            transform.right = right;

            // visualise the trajectory for the bullet
            // complex mathematical wizardry that gives us a bunch of Vector3 positions
            var points = Ballistics.GetBallisticPath(shootPoint.position, shootPoint.right, force, shootDisplayTimeDelta, 2);

            // for each of the points, immediately draw a mesh.
            // Doing it this way means we don't need to bother with gameobjects!
            // Gameobjects have a lot of overhead, so immediately drawing meshes saves a lot of performance
            foreach (var p in points)
                Graphics.DrawMesh(shootDisplayMesh, Matrix4x4.TRS(p, Quaternion.identity, Vector3.one * 0.25f), shootDisplayMaterial, 0, cam);

        }
    }
}
