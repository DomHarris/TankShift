using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Entity
{
    public class PlatformController : RaycastController
    {
        [SerializeField] private LayerMask passengerMask;
        
        // Constants: values that don't really ever need to change
        #region Constants
        // how many objects do we want to hit? I only ever use the first one, so I set it to 1
        private const int NumRaycastResults = 1;
        #endregion
        
        // Private fields - only used in this script
        #region PrivateFields
        // The cached results from the raycast - cached here so we don't allocate every frame 
        private RaycastHit2D[] _results = new RaycastHit2D[NumRaycastResults];


        private HashSet<CollisionEntity> _passengers = new HashSet<CollisionEntity>();
        private Vector3 _previousPosition;
        private Vector3 _velocity;
        #endregion
        
        protected override void Awake()
        {
            base.Awake();
            _previousPosition = transform.position;
        }

        private void Update()
        {
            _velocity = (transform.position - _previousPosition);
            UpdateRaycastOrigins();
            MovePassengers(_velocity);
            _previousPosition = transform.position;
        }


        private void MovePassengers(Vector3 velocity)
        {
            _passengers.Clear();
            float directionX = Mathf.Sign(velocity.x);
            float directionY = Mathf.Sign(velocity.y);

            if (!Mathf.Approximately(velocity.y, 0))
            {
                // how far should we check? 
                float rayLength = Mathf.Abs(velocity.y) + SkinWidth;

                // for each ray
                for (int i = 0; i < verticalRayCount; ++i)
                {
                    // if we're going up, use the bottom left corner. If we're going down, use the top right corner
                    var origin = Mathf.Approximately(directionY, -1)
                        ? _raycastOrigins.BottomLeft
                        : _raycastOrigins.TopLeft;

                    // space the rays out based on out vertical ray spacing
                    origin += (Vector2)transform.right * (_verticalRaySpacing * i);

                    // fire the ray, see what it hits
                    Debug.DrawRay(origin, transform.up * directionY);
                    var numHits = Physics2D.RaycastNonAlloc(origin, transform.up * directionY, _results, rayLength, passengerMask);
                    if (numHits > 0) // we hit something! 
                    {
                        var entity = _results[0].transform.GetComponent<CollisionEntity>();
                        if (entity != null && !_passengers.Contains(entity))
                        {
                            _passengers.Add(entity);
                            var pushX = Mathf.Approximately(directionY, 1) ? velocity.x : 0;
                            var pushY = velocity.y - (_results[0].distance - SkinWidth) * directionY;
                            entity.Move(new Vector3(pushX, pushY));
                        }
                    }
                }
            }

            if (Mathf.Approximately(velocity.y, -1) ||
                Mathf.Approximately(velocity.y, 0) && !Mathf.Approximately(velocity.x, 0))
            {
                // how far should we check? 
                float rayLength = SkinWidth * 2;
                // for each ray
                for (int i = 0; i < verticalRayCount; ++i)
                {

                    // if we're going up, use the bottom left corner. If we're going down, use the top right corner
                    var origin = _raycastOrigins.TopLeft + (Vector2)transform.right * (_verticalRaySpacing * i);

                    // fire the ray, see what it hits
                    Debug.DrawRay(origin, transform.up * directionY);
                    var numHits = Physics2D.RaycastNonAlloc(origin, transform.up, _results, rayLength, passengerMask);
                    if (numHits > 0) // we hit something! 
                    {
                        var entity = _results[0].transform.GetComponent<CollisionEntity>();
                        if (entity != null && !_passengers.Contains(entity))
                        {
                            _passengers.Add(entity);
                            var pushX = velocity.x;
                            var pushY = velocity.y;
                            entity.Move(new Vector3(pushX, pushY));
                        }
                    }
                }
            }
        }
    }
}