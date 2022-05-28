using System;
using System.Collections.Generic;
using Bullets.Player;
using Entity.Stats;
using JetBrains.Annotations;
using UnityEngine;

namespace Weapons.Player
{
    public class WeaponHandler : MonoBehaviour
    {
        [SerializeField] private WeaponBase startWeapon;
        [SerializeField] private BaseShootInput input;
        [SerializeField] private Transform shootPoint;
        [SerializeField] private StatController stats;
        [SerializeField, StatTypeWithParent]
        private StatType fireRate;

        public float FireRate => _stats.GetStat(fireRate);
        
        private StatController _stats;

        [CanBeNull] private WeaponBase _currentWeapon;
        private List<WeaponBase> _unlockedWeapons = new List<WeaponBase>();
        public List<WeaponBase> UnlockedWeapons => _unlockedWeapons;
        
        private void Awake()
        {
            _stats = GetComponentInParent<StatController>();
            _unlockedWeapons.Add(startWeapon);
            SetWeapon(0);
        }

        public void SetWeapon(int idx)
        {
            if (_currentWeapon != null)
                _currentWeapon.Dispose();

            _currentWeapon = _unlockedWeapons[idx];
            if (_currentWeapon != null)
                _currentWeapon.Init(input, shootPoint, FireRate, stats);
        }
        
        public void SetWeapon(WeaponBase weapon)
        {
            if (_currentWeapon != null)
                _currentWeapon.Dispose();

            _currentWeapon = weapon;
            if (_currentWeapon != null)
                _currentWeapon.Init(input, shootPoint, FireRate, stats);
        }

        public bool UnlockWeapon(WeaponBase weapon)
        {
            if (_unlockedWeapons.Contains(weapon))
                return false;
            
            _unlockedWeapons.Add(weapon);
            return true;
        }

        public bool HasUnlockedWeapon(WeaponBase weapon)
        {
            return _unlockedWeapons.Contains(weapon);
        }

        public bool IsCurrentWeapon(WeaponBase weapon)
        {
            return _currentWeapon == weapon;
        }

        private void OnDisable()
        {
            if (_currentWeapon != null)
                _currentWeapon.Dispose();
        }
    }
}