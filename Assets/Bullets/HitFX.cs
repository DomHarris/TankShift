using System.Collections.Generic;
using DG.Tweening;
using Entity.Damage;
using UnityEngine;

namespace Bullets
{
    public class HitFX : MonoBehaviour, IHitReceiver
    {
        [SerializeField] private float endSize = -0.1f;
        [SerializeField] private float time = 0.1f;
        [SerializeField] private List<DamageType> typesToActivateFX;

        public void ReceiveHit(HitData data)
        {
            if (typesToActivateFX.Contains(data.DamageType))
                transform.DOPunchScale(Vector3.one * endSize, time);
        }
    }
}