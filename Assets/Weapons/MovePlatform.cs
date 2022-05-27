using DG.Tweening;
using UnityEngine;

namespace Weapons
{
    public class MovePlatform : MonoBehaviour
    {
        [SerializeField] private Vector3 moveAmount;
        [SerializeField] private float moveDuration;
        [SerializeField] private Ease ease = Ease.InOutQuint;


        private void Start()
        {
            transform.DOMove(moveAmount + transform.position, moveDuration).SetEase(ease).SetLoops(-1, LoopType.Yoyo);
        }

        private void OnDrawGizmos()
        {
            var box = GetComponent<BoxCollider2D>().bounds;
            Gizmos.DrawWireCube(moveAmount + transform.position, box.size);
        }
    }
}