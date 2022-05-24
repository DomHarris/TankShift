using System;
using Bullets.Player;
using UnityEngine;

namespace Weapons.Player
{
    public abstract class WeaponBase : ScriptableObject
    {
        [NonSerialized] protected BaseShootInput _input;
        [NonSerialized] protected Transform _shootPoint;

        public virtual void Init(BaseShootInput input, Transform shootPoint)
        {
            _input = input;
            _shootPoint = shootPoint;
            _input.Shoot += OnShoot;
        }

        public virtual void Dispose()
        {
            _input.Shoot -= OnShoot;
        }

        protected abstract void OnShoot();
    }
}