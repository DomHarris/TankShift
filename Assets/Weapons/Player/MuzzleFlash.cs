using DG.Tweening;
using UnityEngine;

namespace Bullets.Player
{
    public class MuzzleFlash : BaseShootListener
    {
        // Serialized Fields - set in the Unity Inspector
        #region SerializedFields
        [SerializeField, Tooltip("A sprite for the muzzle flash")] 
        private SpriteRenderer muzzleFlash;
        #endregion

        protected override void OnShoot()
        {
            muzzleFlash.color = Color.black;
            muzzleFlash.DOColor(Color.white, 0.1f).SetDelay(0.1f);
            muzzleFlash.DOColor(new Color(1, 1, 1, 0), 0.2f).SetDelay(0.2f);
        }
    }
}