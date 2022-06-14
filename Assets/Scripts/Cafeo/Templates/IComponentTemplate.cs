using System;
using UnityEngine;

namespace Cafeo.Templates
{
    public interface IComponentTemplate<out T> : ITemplate<GameObject>
    {
        GameObject ITemplate<GameObject>.Generate()
        {
            throw new NotImplementedException();
        }

        public T AddToGameObjet(GameObject gameObject);

        public T GenerateGameObject()
        {
            var go = new GameObject();
            var comp = AddToGameObjet(go);
            return comp;
        }
    }
}