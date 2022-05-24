using DG.Tweening;
using Entity.Damage;
using UnityEditor.U2D;
using UnityEngine;

namespace Bullets
{
    public class ProjectileMovePlatform : MonoBehaviour, IHitReceiver
    {
        [SerializeField] private Transform platform;
        [SerializeField] private Vector3 startPos;
        [SerializeField] private Vector3 endPos;
        [SerializeField] private float moveDuration;
        [SerializeField] private Ease ease = Ease.InOutQuint;

        private bool _moving = false;
        
        public void ReceiveHit(HitData data)
        {
            if (data.DamageType == DamageType.Projectile)
                Toggle();
        }
        
        [ContextMenu("Move To Start")]
        public void MoveToStartPos()
        {
            if (_moving) return;
            _moving = true;
            platform.DOMove(startPos, moveDuration).SetEase(ease).OnComplete(() => _moving = false);
        }

        [ContextMenu("Move To End")]
        public void MoveToEndPos()
        {
            if (_moving) return;
            _moving = true;
            platform.DOMove(endPos, moveDuration).SetEase(ease).OnComplete(() => _moving = false);
        }

        public void Toggle()
        {
            if (platform.position == endPos)
                MoveToStartPos();
            else
                MoveToEndPos();
        }

        private void OnDrawGizmos()
        {
            Debug.DrawLine(transform.position, platform.position);
        }
    }
}