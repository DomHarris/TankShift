using Bullets.Player;
using JetBrains.Annotations;
using UnityEngine;

namespace Weapons.Player
{
    public class WeaponHandler : MonoBehaviour
    {
        [SerializeField, CanBeNull] private WeaponBase currentWeapon;
        [SerializeField] private BaseShootInput input;
        [SerializeField] private Transform shootPoint;
        
        private void OnEnable()
        {
            if (currentWeapon != null)
                currentWeapon.Init(input, shootPoint);
        }

        private void OnDisable()
        {
            if (currentWeapon != null)
                currentWeapon.Dispose();
        }
    }
}