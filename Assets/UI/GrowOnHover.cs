using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Little helper class to make UI elements grow when hovered
/// </summary>
public class GrowOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private float size = 1.25f;
    [SerializeField] private float time = 0.25f;
    [SerializeField] private Ease ease = Ease.InOutQuint;
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.DOScale(Vector3.one * size, time).SetEase(ease);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.DOScale(Vector3.one, time).SetEase(ease);
    }
}