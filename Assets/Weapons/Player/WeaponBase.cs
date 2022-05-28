using System;
using System.Collections.Generic;
using Bullets.Player;
using Entity.Stats;
using UnityEngine;
using Utils;

namespace Weapons.Player
{
    
    [Serializable]
    public class WeaponFeature
    {
        public Sprite Type;
        public string Description;
        public StatModifier[] StatModifiers;
        
    }
    public abstract class WeaponBase : ScriptableObject
    {
        [SerializeField] private string shortDescription;
        public string ShortDescription => shortDescription;
        [SerializeField, TextArea] private string description;
        public string Description => description;
        
        [SerializeField] private List<WeaponFeature> features;
        public List<WeaponFeature> Features => features;
        [SerializeField] private Sprite uiSprite;
        public Sprite UISprite => uiSprite;
        
        [NonSerialized] protected BaseShootInput _input;
        [NonSerialized] protected Transform _shootPoint;
        [NonSerialized] protected StatController _stats;
        [NonSerialized] protected float _previousFireTime;
        [NonSerialized] protected float _fireRate;
        
        public virtual void Init(BaseShootInput input, Transform shootPoint, float fireRate, StatController stats)
        {
            _previousFireTime = Time.time;
            
            _input = input;
            _shootPoint = shootPoint;
            _input.Shoot += ShootWeapon;
            _stats = stats;
            _fireRate = fireRate;

            foreach (var feature in features)
                foreach (var modifier in feature.StatModifiers)
                    _stats.AddModifier(modifier);
        }

        public virtual void Dispose()
        {
            _input.Shoot -= ShootWeapon;
            foreach (var feature in features)
                foreach (var modifier in feature.StatModifiers)
                    _stats.RemoveModifier(modifier);
        }

        protected void ShootWeapon()
        {
            if (Time.time < _previousFireTime + _fireRate)
                return;
            _previousFireTime = Time.time;
            OnShoot();
        }

        protected abstract void OnShoot();
    }
}