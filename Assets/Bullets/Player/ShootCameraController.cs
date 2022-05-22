using Cinemachine;
using DG.Tweening;
using UnityEngine;

namespace Bullets.Player
{
    /// <summary>
    /// A little bit of camera VFX for when the player is shooting 
    /// </summary>
    [RequireComponent(typeof(PlayerShootInput))]
    public class ShootCameraController : MonoBehaviour
    {
        // Serialized Fields - set in the Unity Inspector
        #region SerializedFields
        [SerializeField, Tooltip("The cinemachine vcam used to control the camera")] 
        private CinemachineVirtualCamera vcam;
        
        [SerializeField, Tooltip("The orthographic size of the camera when no button is pressed")] 
        private float normalOrthoSize = 10f;
        
        [SerializeField, Tooltip("The orthographic size of the camera when the shot is fully charged")] 
        private float minOrthoSize = 7.5f;
        
        [SerializeField, Tooltip("The camera's target object")] 
        private Transform cameraTarget;
        
        [SerializeField, Tooltip("How quickly should the camera move back to the normal position, in seconds")] 
        private float smoothTime = 0.35f;
        
        [SerializeField, Tooltip("How far away should the camera target be in the x direction, in Unity units, when no button is pressed")] 
        private float camTargetNormal = 5f;
        
        [SerializeField, Tooltip("How far away should the camera target be in the x direction, in Unity units, when the shot is fully charged")] 
        private float camTargetMax = 10f;
        #endregion
        
        // Private fields - only used in this script
        #region PrivateFields
        private PlayerShootInput _shoot; // the object that fires the events we want to listen to
        private CinemachineTransposer _body; // the cinemachine transposer body object
        #endregion
        
        /// <summary>
        /// Called when the game loads
        /// Grab any references we need, and pre-calculate anything we know won't change
        /// </summary>
        private void Awake()
        {
            _shoot = GetComponent<PlayerShootInput>();
            _body = vcam.GetCinemachineComponent<CinemachineTransposer>();
        }

        /// <summary>
        /// Called when the object is enabled
        /// Start listening to any events
        /// </summary>
        private void OnEnable()
        {
            _shoot.ShootHold += OnShootHold; 
            _shoot.Shoot += OnShoot;
        }
        
        /// <summary>
        /// Called when the object is disabled
        /// Stop listening to any events
        /// </summary>
        private void OnDisable()
        {
            _shoot.ShootHold -= OnShootHold;
            _shoot.Shoot -= OnShoot;
        }

        /// <summary>
        /// Event fired when the shoot button is being held down
        /// </summary>
        /// <param name="percentage">the percentage of the charged shot</param>
        private void OnShootHold(float percentage)
        {
            // lerp the camera's orthographic size between the two values
            vcam.m_Lens.OrthographicSize = Mathf.Lerp(normalOrthoSize, minOrthoSize, percentage);
            
            // lerp the camera target's position between the two values
            var localPos = cameraTarget.localPosition;
            localPos.x = Mathf.Lerp(camTargetNormal, camTargetMax, percentage);
            cameraTarget.localPosition = localPos;
            
            // lerp the camera body's vertical follow offset between the two values 
            // TODO: don't hard code these numbers, expose them to the editor
            _body.m_FollowOffset.y = Mathf.Lerp(2, 0, percentage);
            
        }

        /// <summary>
        /// Event fired when the shot is fired
        /// </summary>
        private void OnShoot()
        { 
            // Lerp the camera's orthographic size back to normalOrthoSize, over smoothTime seconds
            DOVirtual.Float(vcam.m_Lens.OrthographicSize, normalOrthoSize, smoothTime, val =>
            {
                vcam.m_Lens.OrthographicSize = val;
            }).SetEase(Ease.OutBack);
            
            // Lerp the camera body's vertical follow offset back to 2, over smoothTime seconds
            // TODO: expose "2" to the editor instead of hard coding it
            DOVirtual.Float(_body.m_FollowOffset.y, 2, smoothTime, val =>
            {
                _body.m_FollowOffset.y = val;
            }).SetEase(Ease.OutBack);

            // move the camera target back to the original position, over smoothTime seconds
            cameraTarget.DOLocalMoveX(camTargetNormal, smoothTime).SetEase(Ease.OutBack);
        }
    }
}