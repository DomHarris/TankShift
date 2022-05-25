using System;
using Bullets.Player;
using Entity.Stats;
using JetBrains.Annotations;
using UnityEngine;

namespace Weapons.Player
{
    public class WeaponHandler : MonoBehaviour
    {
        [SerializeField, CanBeNull] private WeaponBase currentWeapon;
        [SerializeField] private BaseShootInput input;
        [SerializeField] private Transform shootPoint;

        [SerializeField, StatTypeWithParent]
        private StatType fireRate;

        public float FireRate => _stats.GetStat(fireRate);
        
        private StatController _stats;
        
        private void Awake()
        {
            _stats = GetComponentInParent<StatController>();
        }

        private void OnEnable()
        {
            if (currentWeapon != null)
                currentWeapon.Init(input, shootPoint, FireRate);
        }

        private void OnDisable()
        {
            if (currentWeapon != null)
                currentWeapon.Dispose();
        }
    }
}