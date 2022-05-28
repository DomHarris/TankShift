using System;
using Entity.Damage;
using UnityEngine;
using Weapons.Player;

namespace Bullets
{
    public class WeaponOnKillEvent : MonoBehaviour
    {
        public static Action<WeaponBase, bool, Transform> UnlockWeapon;

        [SerializeField] private bool highPriority;
        [SerializeField] private WeaponBase weaponToUnlock;
        [SerializeField] private Health health;
        
        private void OnEnable()
        {
            health.OnHit += OnHit;
        }

        private void OnHit(float currentHealth, float previousHealth, float maxHealth, float healthPercentage)
        {
            if (currentHealth <= 0)
            {
                Debug.Log(currentHealth);
                UnlockWeapon?.Invoke(weaponToUnlock, highPriority, transform);                
            }
        }
    }
}