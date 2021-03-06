using System;
using Bullets.Player;
using UnityEngine;

namespace Bullets.Enemy
{
    /// <summary>
    /// Target the target using projectile motion equations
    /// </summary>
    /// Lines added by Ryu, 24-25(aggroRange), 
    public class EnemyTarget : BaseShootInput
    {
        // Serialized Fields - set in the Unity Inspector

        #region SerializedFields

        [SerializeField, Tooltip("Where should the bullet shoot from?")]
        private Transform shootPoint;

        [SerializeField, Tooltip("How much force should we apply to the bullet when it is fired?")]
        private float force;

        [SerializeField, Tooltip("The target object")]
        private Transform target;

        [SerializeField, Tooltip("Modifies range of aggression in relation to player")]
        private float aggroRange;

        [SerializeField, Tooltip("How many seconds to target the player before shooting?")]
        private float secondsOfTargeting = 3;

        [SerializeField, Tooltip("How many seconds to target the player before shooting?")]
        private float secondsBeforeShoot = 1;

        [SerializeField, Tooltip("How many seconds to wait after shooting?")]
        private float secondsAfterShoot = 3;

        [Header("Visualisation"), SerializeField, Tooltip("The spacing between dots in the visualiser")]
        private float shootDisplayTimeDelta = 0.1f;

        [SerializeField, Tooltip("Damping speed for the targeting. Higher = will take more time to move to the target")]
        private float targetSpeed = 0.2f;

        [SerializeField, Tooltip("The line renderer used to display the targeting")]
        private LineRenderer line;

        #endregion

        // Private fields - only used in this script

        #region PrivateFields

        private Vector3
            _velocityDamp; // used for smoothly moving to the target, so we don't instantly target the player

        private Vector3 _targetPos; // the current target position
        private float _timer;
        private bool _hasShot;
        private Vector3 _shootForce;
        #endregion


        private void Update()
        {
            //distance to target
            float distToTarget = Vector2.Distance(transform.position, target.position);
            // 25-27 seems to be the max range on SampleScene at start
            if (distToTarget < aggroRange)
            {
                // code to chase target
                TargetPlayer();
            }
            else
            {
                // stop chasing target
                line.positionCount = 0;
            }
        }

        private void OnDrawGizmos()
        {
            
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(target.position, 0.1f);
            Gizmos.color = Color.white;
            var canShoot = Ballistics.CalculateTrajectory(transform.position, target.position, force, out Vector3 direction);
            if (!canShoot) return;
            // which direction is the player?
            direction /= force;
            var dirX = Mathf.Sign(target.position.x - transform.position.x);

            // the angle is only ever going to point to the right 
            // so multiply the x component by the direction we found earlier - this corrects that
            //direction.x *= dirX;
            Gizmos.DrawLine(transform.position, transform.position + (direction * 2f));
            var points = Ballistics.GetBallisticPath(transform.position, direction, force, shootDisplayTimeDelta, 100);
            foreach (var point in points)
            {
                Gizmos.DrawSphere(point, 0.25f);
            }
            
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(transform.position + (direction * 2f), 0.1f);
            Gizmos.color = Color.white;
        }

        void TargetPlayer()
        {
            _timer += Time.deltaTime;

            // only update the target position if we're targeting
            if (_timer < secondsOfTargeting)
            {
                // smoothly move towards the target, to give the player a chance to avoid
                _targetPos = Vector3.SmoothDamp(_targetPos, target.position, ref _velocityDamp, targetSpeed);
            }

            // only draw/update the line if we're not on cooldown
            if (_timer < secondsOfTargeting + secondsBeforeShoot)
            {
                // mathematical wizardry that tells us what angle to rotate the turret to
                var canShoot = Ballistics.CalculateTrajectory(shootPoint.position, _targetPos, force, out _shootForce);

                // if the above function passed, draw the line 
                if (canShoot)
                {

                    // which direction is the player?
                    //var dirX = Mathf.Sign(target.position.x - transform.position.x);

                    // the angle is only ever going to point to the right 
                    // so multiply the x component by the direction we found earlier - this corrects that
                    //right.x *= dirX;
                    transform.right = _shootForce;

                    // visualise the trajectory for the bullet
                    // complex mathematical wizardry that gives us a bunch of Vector3 positions
                    var points = Ballistics.GetBallisticPath(shootPoint.position, _shootForce,
                        shootDisplayTimeDelta, 5);
                    line.positionCount = points.Length;
                    // set the positions on the line renderer
                    line.SetPositions(points);
                }
                else // don't draw the line
                {
                    line.positionCount = 0;
                }
            }
            else
            {
                line.positionCount = 0;
                if (!_hasShot)
                {
                    _hasShot = true;
                    InvokeShootEvent();
                }

                if (_timer >= secondsOfTargeting + secondsBeforeShoot + secondsAfterShoot)
                {
                    _timer = 0;
                    _hasShot = false;
                }
            }


        }

        public override Vector3 GetShootForce()
        {
            return _shootForce;
        }
    }
}
