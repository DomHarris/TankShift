using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Entity.Damage
{
    /// <summary>
    /// Show a visual representation of the health
    /// </summary>
    public class HealthBar : MonoBehaviour
    {
        // Serialized Fields - set in the Unity Inspector
        #region SerializedFields
        [SerializeField, Tooltip("The UI Image that displays the health percentage as a bar")]
        private Image bar;
        
        [SerializeField, Tooltip("The UI Image that displays the previous health percentage as a bar")]
        private Image prevBar;

        [SerializeField, Tooltip("How long do we wait before moving the secondary bar?")] 
        private float delay = 0.1f;
        
        [SerializeField, Tooltip("How long should the secondary bar take to move?")] 
        private float timeToFill = 0.2f;
        [SerializeField, Tooltip("The health object that tells us how much health we've got")]
        private Health health;
        #endregion

        
        /// <summary>
        /// Called when the object is enabled
        /// Start listening to any events
        /// </summary>
        private void OnEnable()
        {
            health.OnHit += OnHealthUpdate;
        }

        
        /// <summary>
        /// Called when the object is disabled
        /// Stop listening to any events, because if this object is disabled we don't care
        /// </summary>
        private void OnDisable()
        {
            health.OnHit -= OnHealthUpdate;
        }

        /// <summary>
        /// When the health is updated, update the UI accordingly
        /// </summary>
        /// <param name="currentHealth">The numeric value of the current health</param>
        /// <param name="previousHealth">The numeric value of the previous health</param>
        /// <param name="maxHealth">The numeric value of the max health</param>
        /// <param name="healthPercentage">The percentage value of currentHealth / maxHealth</param>
        private void OnHealthUpdate(float currentHealth, float previousHealth, float maxHealth, float healthPercentage)
        {
            // immediately set the main health bar's fill amount
            bar.fillAmount = healthPercentage;
            
            // a white, secondary health bar smoothly animates to the current health percentage after a delay
            // this helps the player visualise how much damage has been done
            // and also looks really cool
            DOVirtual.Float(prevBar.fillAmount, healthPercentage, timeToFill, val => prevBar.fillAmount = val).SetEase(Ease.InOutQuint).SetDelay(delay);
        }
    }
}