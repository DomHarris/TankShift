using System;
using JetBrains.Annotations;
using UnityEngine;

namespace Entity.Damage
{
    /// <summary>
    /// Enum for damage type
    /// Will be used to:
    /// - allow things to resist/be weak to certain damage types
    /// - run specific functions for different damage types, e.g. buttons that are pressed with projectiles, mirrors that reflect lasers, etc 
    /// </summary>
    [Flags]
    public enum DamageType { Projectile, Laser, Effect }
    
    
    /// <summary>
    /// Data object that holds all the relevant data for an object being hit
    /// </summary>
    public struct HitData
    {
        public float Damage; // how much damage does this cause?
        public DamageType DamageType; // what sort of damage is it?
        public GameObject IncomingObject; // the object that caused this hit
        public Vector3 IncomingDirection; // the direction the object was travelling
        [CanBeNull] public Collision2D CollisionInfo; // Collision data for rigidbody collisions
        // ^^ the [CanBeNull] tag here lets everyone know that this sometimes won't hold data
    }
}