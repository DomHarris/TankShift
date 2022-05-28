using System.Collections.Generic;
using DG.Tweening;
using Entity.Damage;
using UnityEngine;

namespace Bullets
{
    public class HitFlash : MonoBehaviour, IHitReceiver
    {
        [SerializeField] private SpriteRenderer[] renderers;
        [SerializeField] private float time = 0.1f;
        [SerializeField] private List<DamageType> typesToActivateFX;

        public void ReceiveHit(HitData data)
        {
            if (typesToActivateFX.Contains(data.DamageType))
            {
                foreach (var spriteRenderer in renderers)
                {
                    DOTween.Sequence()
                        .Append(spriteRenderer.material.DOFloat(1, "_FlashAmount", time))
                        .Append(spriteRenderer.material.DOFloat(0, "_FlashAmount", time));
                }
            }
        }
    }
}