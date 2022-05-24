using UnityEngine;

namespace Entity.Stats
{
    public class StatController : MonoBehaviour
    {
        [SerializeField] private StatCollection stats;
        public StatCollection Stats => stats;
    }
}