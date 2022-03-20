using UnityEngine;

namespace Cafeo.Templates
{
    public interface IComponentTemplate<out T>
    {
        public T AddToGameObjet(GameObject gameObject);

        public T GenerateGameObject()
        {
            var go = new GameObject();
            var comp = AddToGameObjet(go);
            return comp;
        }
    }
}