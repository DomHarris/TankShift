using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Bullets.Player
{
    public class PlayerShootInput : BaseShootInput
    {
        // Events - broadcast a message to any objects that are listening
        #region Events
        public event Action<float> ShootHold;
        #endregion
        
        // Serialized Fields - set in the Unity Inspector
        #region SerializedFields
        [SerializeField, Tooltip("The easing function to use for lerping between the two shoot forces")] 
        private Ease ease;

        [Header("Shooting"), SerializeField, Tooltip("How long will it take to charge the shot?")] 
        private float maxChargeTime = 3f;
        
        [SerializeField, Tooltip("What shoot force should we use if the player immediately releases the button?")] 
        private float minShootForce = 10f;
        [SerializeField, Tooltip("What shoot force should we use if the player holds the button for MaxChargeTime?")] 
        private float maxShootForce = 15f;
        #endregion

        // Private fields - only used in this script
        #region PrivateFields
        private float _timer; // the current amount of time the player has held the shoot key 
        private bool _mouseDown; // is the mouse currently being held down?
        private bool _mouseDownThisFrame; // was the mouse first pressed this frame?
        private bool _mouseUpThisFrame; // was the mouse released this frame?
        #endregion
        
        /// <summary>
        /// Get the current (eased) percentage of the time the player has held the mouse down
        /// </summary>
        /// <returns></returns>
        public float GetPercentage()
        {
            var percentage = _timer / maxChargeTime;
            percentage = Mathf.Clamp01(percentage);
            percentage = DOVirtual.EasedValue(0, 1, percentage, ease);
            return percentage;
        }

        /// <summary>
        /// Get the current force to use for shooting the bullet
        /// </summary>
        /// <returns></returns>
        public override float GetShootForce()
        {
            return Mathf.Lerp(minShootForce, maxShootForce, GetPercentage());
        }
        
        /// <summary>
        /// Called once per frame
        /// Get and act on input from the user
        /// </summary>
        void Update()
        {
            // if we pressed the shoot button this frame, reset the timer
            if (_mouseDownThisFrame)
            {
                _timer = 0;
                _mouseDownThisFrame = false;
            }
        
            // if the shoot button is being held down, increase the timer, update the percentage and broadcast the "hold" event
            if (_mouseDown)
            {
                _timer += Time.deltaTime;
                var percentage = GetPercentage();
                if (ShootHold != null)
                    ShootHold.Invoke(percentage);
            }
            else if (_mouseUpThisFrame)
            {
                _mouseUpThisFrame = false;
                // if the mouse was released this frame, tell the base class to broadcast the "shoot" event
                InvokeShootEvent();
            }
        }

        /// <summary>
        /// Get the shoot input
        /// </summary>
        /// <param name="ctx">the input actions context, used to get the values for the input</param>
        public void MouseDown(InputAction.CallbackContext ctx)
        {
            if (!_mouseDown && ctx.ReadValueAsButton())
                _mouseDownThisFrame = true;
            if (_mouseDown && !ctx.ReadValueAsButton())
                _mouseUpThisFrame = true;
            _mouseDown = ctx.ReadValueAsButton();
        }
    }
}