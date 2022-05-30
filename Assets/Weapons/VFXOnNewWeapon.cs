using DG.Tweening;
using UnityEditor;
using UnityEngine;
using Weapons.Player;

namespace Bullets
{
    public class VFXOnNewWeapon : MonoBehaviour
    {
        [SerializeField] private ParticleSystem particles;
        [SerializeField] private ParticleSystem endParticles;
        [SerializeField] private WeaponHandler weapons;
        private void OnEnable()
        {
            WeaponOnKillEvent.UnlockWeapon += UnlockWeapon;
        }

        private void OnDisable()
        {
            WeaponOnKillEvent.UnlockWeapon -= UnlockWeapon;
        }

        private void UnlockWeapon(WeaponBase weapon, bool highPriority, Transform obj)
        {
            //if (weapons.HasUnlockedWeapon(weapon)) return;
            
            particles.Stop();
            particles.transform.position = obj.position;
            particles.Play();
            Vector3 prevPos = particles.transform.position;
            var originalPos = obj.position;
            DOVirtual.Float(0, 1, 1f, val =>
            {
                particles.transform.position = Vector3.Lerp(originalPos, transform.position, val);
                particles.transform.right = prevPos - particles.transform.position;
                prevPos = particles.transform.position;
            }).SetEase(Ease.InQuint).OnComplete(() => endParticles.Emit(100));
        }
    }
}