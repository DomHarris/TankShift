using System;
using UnityEngine;

namespace Entity
{   
    /// <summary>
    /// An entity that can move and will collide.
    /// Heavily based on Sebastian Lague's character controller: https://www.youtube.com/playlist?list=PLFt_AvWsXl0f0hqURlhyIoAabKPgRsqjz
    /// (with some uh minor improvements)
    /// (and a bunch of comments so you can work out what's going on)
    /// </summary>
    [HelpURL("https://github.com/DomHarris/TankShift/tree/main/Assets/Entity")]
    public class CollisionEntity : RaycastController
    {
        // Constants: values that don't really ever need to change
        #region Constants
        // how many objects do we want to hit? I only ever use the first one, so I set it to 1
        private const int NumRaycastResults = 1;
        #endregion

        // Serialized Fields - set in the Unity Inspector
        #region SerializedFields
        [SerializeField, Tooltip("Which layers should we be collide with?")] 
        private LayerMask collisionMask;
        
        [SerializeField, Tooltip("How steep are the slopes we should be able to climb?")] 
        private float maxClimbAngle = 45;
        #endregion
        
        // Private fields - only used in this script
        #region PrivateFields

        // The cached results from the raycast - cached here so we don't allocate every frame 
        private RaycastHit2D[] _resultsMain = new RaycastHit2D[NumRaycastResults];
        private RaycastHit2D[] _resultsSpare = new RaycastHit2D[NumRaycastResults];

        // the collision details for this object this frame
        private CollisionDetails _collisionInfo;
        #endregion

        // Properties - public access functions
        #region Properties
        // Public getter for collision info
        // done like this because we don't ever want to be able to write to this object outside this script
        public CollisionDetails CollisionInfo => _collisionInfo;
        #endregion

        /// <summary>
        /// Check vertical collisions
        /// </summary>
        /// <param name="velocity">How fast and in which direction are we moving - reference parameter so it will change the value that is passed in</param>
        private void VerticalCollisions(ref Vector3 velocity)
        {
            // are we moving up or down?
            var directionY = Mathf.Sign(velocity.y);
            
            // how far should we check? 
            float rayLength = Mathf.Abs(velocity.y) + SkinWidth;

            // for each ray
            for (int i = 0; i < _verticalRayCount; ++i)
            {
                // if we're going up, use the bottom left corner. If we're going down, use the top right corner
                var origin = Mathf.Approximately(directionY, -1) ? _raycastOrigins.BottomLeft : _raycastOrigins.TopLeft;
                
                // space the rays out based on out vertical ray spacing
                origin += Vector2.right * (_verticalRaySpacing * i + velocity.x);
                
                // fire the ray, see what it hits
                var numHits = Physics2D.RaycastNonAlloc(origin, Vector2.up * directionY, _resultsMain, rayLength, collisionMask);
                if (numHits > 0) // we hit something! 
                {
                    // only move to that point. 
                    velocity.y = (_resultsMain[0].distance - SkinWidth) * directionY;
                    
                    // set the ray length to this distance so the next rays don't let you move further
                    // without this you'll ignore some objects!
                    rayLength = _resultsMain[0].distance;

                    // if we're climbing a slope, alter the vertical velocity
                    if (_collisionInfo.ClimbingSlope)
                        velocity.x = velocity.y / Mathf.Tan(_collisionInfo.SlopeAngle * Mathf.Deg2Rad) * Mathf.Sign(velocity.x);

                    // we hit something, set the right flags in the collision info
                    // if we're moving up, directionY will be 1, otherwise it will be -1
                    // use Mathf.Approximately for those checks instead of "==" because floating point precision can mess things up
                    _collisionInfo.Below = Mathf.Approximately(directionY, -1);
                    _collisionInfo.Above = Mathf.Approximately(directionY, 1);
                    
                    _collisionInfo.SlopeNormal = _resultsMain[0].normal;
                }
                
                // draw a ray in the unity editor so we can see what's going on
                Debug.DrawRay(origin, Vector2.up * rayLength, Color.red);
            }

            
            // if we're climbing a slope, there's a little bit of extra we need to do to stop a specific issue
            if (_collisionInfo.ClimbingSlope)
            {
                // basically, if we're moving vertically shoot a ray horizontally to check if there's a new surface we're on
                float directionX = Mathf.Sign(velocity.x);
                rayLength = Mathf.Abs(velocity.x) + SkinWidth;
                var origin = (Mathf.Approximately(directionX, -1) ? _raycastOrigins.BottomLeft : _raycastOrigins.BottomRight) + Vector2.up * velocity.y;
                var numHits = Physics2D.RaycastNonAlloc(origin, Vector2.right * directionX, _resultsMain, rayLength, collisionMask);
                if (numHits > 0)
                {
                    // if there's a new slope, use that instead of the old one
                    float slopeAngle = Vector2.Angle(_resultsMain[0].normal, Vector2.up);
                    if (!Mathf.Approximately(slopeAngle, _collisionInfo.SlopeAngle))
                    {
                        velocity.x = (_resultsMain[0].distance - SkinWidth) * directionX;
                        _collisionInfo.SlopeAngle = slopeAngle;
                    }
                }
            }
        }
        
        /// <summary>
        /// Check horizontal collisions
        /// </summary>
        /// <param name="velocity">How fast and in which direction are we moving - reference parameter so it will change the value that is passed in</param>
        private void HorizontalCollisions(ref Vector3 velocity)
        {
            // are we moving left or right?
            var directionX = Mathf.Sign(velocity.x);
            
            // how far should we check? 
            float rayLength = Mathf.Abs(velocity.x) + SkinWidth;

            // for each ray
            for (int i = 0; i < _horizontalRayCount; ++i)
            {
                // if we're going left, use the bottom left corner. If we're going right, use the bottom right corner
                var origin = Mathf.Approximately(directionX, -1) ? _raycastOrigins.BottomLeft : _raycastOrigins.BottomRight;
                
                // space the rays out based on out horizontal ray spacing
                origin += Vector2.up * (_horizontalRaySpacing * i);
                
                // fire the ray, see what it hits
                var numHits = Physics2D.RaycastNonAlloc(origin, Vector2.right * directionX, _resultsMain, rayLength, collisionMask);
                if (numHits > 0) // we hit something!
                {
                    // ignore collisions if we're inside the object
                    if (_resultsMain[0].distance <= Mathf.Epsilon)
                        continue;
                    
                    // work out the angle of the surface we hit
                    float slopeAngle = Vector2.Angle(_resultsMain[0].normal, Vector2.up);
                    
                    // if it's a slope and this is the first ray (we only need to check the first one for slopes) and we can actually climb it
                    if (i == 0 && slopeAngle <= maxClimbAngle)
                    {
                        // if we were descending a slope, we definitely aren't now (because we're climbing one), so set that to false and reset the velocity
                        if (_collisionInfo.DescendingSlope)
                        {
                            _collisionInfo.DescendingSlope = false;
                            velocity = _collisionInfo.VelocityOld;
                        }
                    
                        // work out how far away the slope is
                        float distToSlopeStart = 0;
                        if (!Mathf.Approximately(slopeAngle, _collisionInfo.SlopeAngleOld))
                        {
                            distToSlopeStart = _resultsMain[0].distance - SkinWidth;
                            velocity.x -= distToSlopeStart * directionX;
                        }
                        
                        // climb the slope
                        ClimbSlope(ref velocity, slopeAngle, _resultsMain[0].normal);
                        
                        // fudge the position slightly so we actually end up right at the start of the slope
                        velocity.x += distToSlopeStart * directionX;
                    }

                    if (!_collisionInfo.ClimbingSlope || slopeAngle > maxClimbAngle)
                    {
                        // only move to that point. 
                        velocity.x = (_resultsMain[0].distance - SkinWidth) * directionX;
                        
                        // set the ray length to this distance so the next rays don't let you move further
                        // without this you'll ignore some objects!
                        rayLength = _resultsMain[0].distance;

                        // if we're climbing a slope, alter the vertical velocity
                        if (_collisionInfo.ClimbingSlope)
                            velocity.y = Mathf.Tan(_collisionInfo.SlopeAngle * Mathf.Deg2Rad * Mathf.Abs(velocity.x));

                        // we hit something, set the right flags in the collision info
                        _collisionInfo.Left = Mathf.Approximately(directionX, -1);
                        _collisionInfo.Right = Mathf.Approximately(directionX, 1);
                    }
                }
            }
        }

        /// <summary>
        /// Climb a slope
        /// </summary>
        /// <param name="velocity">How fast and in which direction are we moving?</param>
        /// <param name="slopeAngle">What's the angle of the slope?</param>
        private void ClimbSlope(ref Vector3 velocity, float slopeAngle, Vector2 slopeNormal)
        {
            // some funky maths here to work out new positions. For more info watch Sebastian Lague's great series, linked above.
            float moveDist = Mathf.Abs(velocity.x);
            float newVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDist;
            if (velocity.y <= newVelocityY)
            {
                velocity.y = newVelocityY;
                velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDist * Mathf.Sign(velocity.x);
                _collisionInfo.Below = true;
                _collisionInfo.ClimbingSlope = true;
                _collisionInfo.SlopeAngle = slopeAngle;
                _collisionInfo.SlopeNormal = slopeNormal;
            }
        }
        
        /// <summary>
        /// Descend a slope
        /// </summary>
        /// <param name="velocity">How fast and in which direction are we going?</param>
        private void DescendSlope(ref Vector3 velocity)
        {
            var numHitsLeft = Physics2D.RaycastNonAlloc(_raycastOrigins.BottomLeft, Vector2.down , _resultsMain, Mathf.Abs(velocity.y) + SkinWidth, collisionMask);
            var numHitsRight = Physics2D.RaycastNonAlloc(_raycastOrigins.BottomRight, Vector2.down , _resultsSpare, Mathf.Abs(velocity.y) + SkinWidth, collisionMask);

            if (numHitsLeft > 0 ^ numHitsRight > 0)
            {
                SlideDownSlope(_resultsMain[0], ref velocity);
                SlideDownSlope(_resultsSpare[0], ref velocity);
            }

            if (_collisionInfo.SlidingDownSlope) return;
            float directionX = Mathf.Sign(velocity.x);
            var origin = Mathf.Approximately(directionX, -1) ? _raycastOrigins.BottomRight : _raycastOrigins.BottomLeft;
            var numHits = Physics2D.RaycastNonAlloc(origin, -Vector2.up , _resultsMain, Mathf.Infinity, collisionMask);
            if (numHits > 0)
            {
                float slopeAngle = Vector2.Angle(_resultsMain[0].normal, Vector2.up);
                if (!Mathf.Approximately(slopeAngle, 0) && slopeAngle < maxClimbAngle)
                {
                    if (Mathf.Approximately(Mathf.Sign(_resultsMain[0].normal.x), directionX))
                    {
                        if (_resultsMain[0].distance - SkinWidth <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x))
                        {
                            float moveDist = Mathf.Abs(velocity.x);
                            float newVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDist;
                            velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDist * Mathf.Sign(velocity.x);
                            velocity.y -= newVelocityY;

                            _collisionInfo.SlopeAngle = slopeAngle;
                            _collisionInfo.DescendingSlope = true;
                            _collisionInfo.Below = true;
                            _collisionInfo.SlopeNormal = _resultsMain[0].normal;
                        }
                    }
                }
            }
        }

        private void SlideDownSlope(RaycastHit2D hit, ref Vector3 moveAmount)
        {
            if (hit)
            {
                var slopeAngle = Vector3.Angle(hit.normal, Vector3.up);
                if (slopeAngle > maxClimbAngle)
                {
                    moveAmount.x = (Mathf.Abs(moveAmount.y) - hit.distance) / Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * hit.normal.x;

                    _collisionInfo.SlopeAngle = slopeAngle;
                    _collisionInfo.SlidingDownSlope = true;
                    _collisionInfo.SlopeNormal = hit.normal;
                }
            }
        }

        /// <summary>
        /// Move this object
        /// </summary>
        /// <param name="amount">How much are we moving it by?</param>
        public void Move(Vector3 amount, bool onPlatform = false)
        {
            // update the raycast origins so we know where the raycasts should fire from
            UpdateRaycastOrigins();
            // reset the collisions, we don't need the old data any more
            _collisionInfo.Reset();
            // keep track of what the initial velocity was supposed to be
            _collisionInfo.VelocityOld = amount;
            
            // if we're going downhill, try descending a slope and update the amount we're trying to move
            if (amount.y <= 0)
                DescendSlope(ref amount);

            // if we're moving in the X direction, handle horizontal collisions and update the amount we're trying to move
            // Mathf.Abs - the absolute value, so if it's -2 this will return 2
            // Mathf.Epsilon - a really really small value, we check if the value is > epsilon rather than == 0 because floating point maths is funky and sometimes x == 0 will return false even if x is 0
            if (Mathf.Abs(amount.x) > Mathf.Epsilon)
                HorizontalCollisions(ref amount);
        
            // if we're moving in the Y direction, handle vertical collisions and update the amount we're trying to move
            // same deal as above
            if (Mathf.Abs(amount.y) > Mathf.Epsilon)
                VerticalCollisions(ref amount);
        
            // actually move the object 
            transform.Translate(amount);
            
            // set the collision direction to -1 if we're moving left, 1 if we're moving right
            _collisionInfo.Direction = Mathf.RoundToInt(Mathf.Sign(amount.x));

            if (onPlatform)
                _collisionInfo.Below = true;
            
            // if we're moving at all, update the physics transforms
            // same deal with checking against Epsilon vs == 0 here 
            // we use sqrMagnitude because finding how long a vector is uses square roots
            // we tend to want to avoid sqrt where possible, because it is a really slow operation
            if (amount.sqrMagnitude > Mathf.Epsilon)
                Physics2D.SyncTransforms();
        }


        /// <summary>
        /// Details for the current collisions
        /// </summary>
        public struct CollisionDetails
        {
            public bool Above, Below;
            public bool Left, Right;
            public bool ClimbingSlope, DescendingSlope, SlidingDownSlope;
            public float SlopeAngle, SlopeAngleOld;
            public Vector2 SlopeNormal;
            public int Direction;
            public Vector3 VelocityOld;
            
            /// <summary>
            /// Reset the collision details so we can reuse this struct
            /// </summary>
            public void Reset()
            {
                Above = Below = Left = Right = ClimbingSlope = DescendingSlope = SlidingDownSlope = false;
                SlopeAngleOld = SlopeAngle;
                SlopeAngle = 0;
                SlopeNormal = Vector2.zero;
            }
        }
    }
}