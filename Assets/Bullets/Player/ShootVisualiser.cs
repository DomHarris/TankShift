using UnityEngine;

namespace Bullets.Player
{
    [RequireComponent(typeof(PlayerShootInput))]
    public class ShootVisualiser : MonoBehaviour
    {
        // Serialized Fields - set in the Unity Inspector
        #region SerializedFields
        [SerializeField, Tooltip("Where should the bullet shoot from?")] 
        private Transform shootPoint;

        [Header("Visualisation"), SerializeField, Tooltip("The spacing between dots in the visualiser")]
        private float shootDisplayTimeDelta = 0.1f;
        
        [SerializeField, Tooltip("The camera used for the trajectory visualisation")] 
        private Camera cam;
        
        [SerializeField, Tooltip("The mesh used to draw each point in the trajectory visualisation")] 
        private Mesh shootDisplayMesh;
        
        [SerializeField, Tooltip("The material used to draw each point in the trajectory visualisation")]
        private Material shootDisplayMaterial;

        [SerializeField, Tooltip("The size of the visualisation points")]
        private float size = 0.5f;
        #endregion
        
        // Private fields - only used in this script
        #region PrivateFields
        private PlayerShootInput _shoot;
        private static readonly int ChargeAmount = Shader.PropertyToID("_ChargeAmount");

        #endregion
        
        
        /// <summary>
        /// Called when the game loads
        /// Grab any references we need, and pre-calculate anything we know won't change
        /// </summary>
        private void Awake()
        {
            _shoot = GetComponent<PlayerShootInput>();
        }

        /// <summary>
        /// Called when the object is enabled
        /// Start listening to any events
        /// </summary>
        private void OnEnable()
        {
            _shoot.ShootHold += OnShootHold; 
        }

        /// <summary>
        /// Called when the object is disabled
        /// Stop listening to any events
        /// </summary>
        private void OnDisable()
        {
            _shoot.ShootHold -= OnShootHold;
        }
    
        /// <summary>
        /// Called when the shoot button is being held down
        /// </summary>
        /// <param name="percentage">How "charged" the shot is</param>
        private void OnShootHold(float percentage)
        {
            var currentShootForce = _shoot.GetShootForce();
            // magic maths function to get the bullet's time of flight
            var time = Ballistics.GetTimeOfFlight(currentShootForce, shootPoint.right, shootPoint.position.y);
            
            // visualise the trajectory for the bullet
            // complex mathematical wizardry that gives us a bunch of Vector3 positions
            var points = Ballistics.GetBallisticPath(shootPoint.position, shootPoint.right, currentShootForce, shootDisplayTimeDelta, time);
            
            shootDisplayMaterial.SetFloat(ChargeAmount, percentage);
            
            // for each of the points, immediately draw a mesh.
            // Doing it this way means we don't need to bother with gameobjects!
            // Gameobjects have a lot of overhead, so immediately drawing meshes saves a lot of performance
            foreach (var p in points)
                Graphics.DrawMesh(shootDisplayMesh, Matrix4x4.TRS(p, Quaternion.identity, Vector3.one * size), shootDisplayMaterial, 0, cam);
        }
    }
}