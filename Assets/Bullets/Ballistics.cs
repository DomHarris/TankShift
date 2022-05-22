using UnityEngine;

namespace Bullets
{
    /// <summary>
    /// Projectile Motion helper functions & fixes from https://forum.unity.com/threads/projectile-trajectory-prediction.664909/
    /// </summary>
    public static class Ballistics
    {
        /// <summary>
        /// Calculate the lanch angle.
        /// </summary>
        /// <returns>Whether or not the calculation was possible</returns>
        /// <param name="start">The muzzle.</param>
        /// <param name="end">Wanted hit point.</param>
        /// <param name="velocity">Muzzle velocity.</param>
        /// <param name="angle">output angle</param>
        public static bool CalculateTrajectory(Vector3 start, Vector3 end, float velocity, out float angle)
        {
            Vector3 dir = end - start;
            float vSqr = velocity * velocity;
            float y = dir.y;
            dir.y = 0.0f;
            float x = dir.sqrMagnitude;
            float g = -Physics.gravity.y;

            float uRoot = vSqr * vSqr - g * (g * (x) + (2.0f * y * vSqr));


            if (uRoot < 0.0f)
            {

                //target out of range.
                angle = -45.0f;
                return false;
            }

            float r = Mathf.Sqrt(uRoot);
            float bottom = g * Mathf.Sqrt(x);

            angle = -(Mathf.Atan2(vSqr - r, bottom) * Mathf.Rad2Deg);
            return true;
        }

        /// <summary>
        /// Gets the ballistic path.
        /// </summary>
        /// <returns>The ballistic path.</returns>
        /// <param name="startPos">Start position.</param>
        /// <param name="forward">Forward direction.</param>
        /// <param name="velocity">Velocity.</param>
        /// <param name="timeResolution">Time from frame to frame.</param>
        /// <param name="maxTime">Max time to simulate, will be clamped to reach height 0 (aprox.).</param>

        public static Vector3[] GetBallisticPath(Vector3 startPos, Vector3 forward, float velocity, float timeResolution, float maxTime = Mathf.Infinity)
        {

            maxTime = Mathf.Min(maxTime, GetTimeOfFlight(velocity, forward, startPos.y));
            Vector3[] positions = new Vector3[Mathf.CeilToInt(maxTime / timeResolution)];
            Vector3 velVector = forward * velocity;
            int index = 0;
            Vector3 curPosition = startPos;
            for (float t = 0.0f; t < maxTime; t += timeResolution)
            {

                if (index >= positions.Length)
                    break; //rounding error using certain values for maxTime and timeResolution

                positions[index] = curPosition;
                curPosition += velVector * timeResolution;
                velVector += (Vector3)Physics2D.gravity * timeResolution;
                index++;
            }

            return positions;
        }

        /// <summary>
        /// Checks the ballistic path for collisions.
        /// </summary>
        /// <returns><c>false</c>, if ballistic path was blocked by an object on the Layermask, <c>true</c> otherwise.</returns>
        /// <param name="arc">Arc.</param>
        /// <param name="lm">Anything in this layer will block the path.</param>
        public static bool CheckBallisticPath(Vector3[] arc, LayerMask lm)
        {
            for (int i = 1; i < arc.Length; i++)
            {

                if (Physics2D.Raycast(arc[i - 1], arc[i] - arc[i - 1], (arc[i] - arc[i - 1]).magnitude, lm))
                    return false;
            }

            return true;
        }

        public static Vector3 GetHitPosition(Vector3 startPos, Vector3 forward, float velocity, LayerMask layerMask)
        {

            Vector3[] path = GetBallisticPath(startPos, forward, velocity, .35f);
        
            for (int i = 1; i < path.Length; i++)
            {

                //Debug.DrawRay (path [i - 1], path [i] - path [i - 1], Color.red, 10f);
                var hit = Physics2D.Raycast(path[i - 1], path[i] - path[i - 1], (path[i] - path[i - 1]).magnitude, layerMask);
                if (hit)
                {
                    return hit.point;
                }
            }

            return Vector3.zero;
        }


        public static float CalculateMaxRange(float muzzleVelocity)
        {
            return (muzzleVelocity * muzzleVelocity) / -Physics.gravity.y;
        }

        public static float GetTimeOfFlight(float vel, Vector3 forward, float height)
        {
            var a = vel * Mathf.Sin(Vector3.Angle (forward, Vector3.up) * Mathf.Deg2Rad);
            return Mathf.Min((a + Mathf.Sqrt(Mathf.Pow(a, 2) + 2 * -Physics.gravity.y * height)) / -Physics.gravity.y, 10);
        }

    }
}