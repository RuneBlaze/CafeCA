using UnityEngine;

namespace Cafeo.Aimer
{
    public abstract class GenericAimer<T> : MonoBehaviour
    {
        public abstract T Item { get; set; }
    }
}