using System;
using Cafeo.Castable;
using UnityEngine;

namespace Cafeo.Aimer
{
    public class AimerGroup : MonoBehaviour
    {
        private RangedAimer _rangedAimer;

        public void Awake()
        {
            _rangedAimer = GetComponentInChildren<RangedAimer>();
            _rangedAimer.gameObject.SetActive(false);
        }

        public void RequestAimer(UsableItem item)
        {
            if (item is RangedItem rangedItem)
            {
                _rangedAimer.gameObject.SetActive(true);
                _rangedAimer.Item = rangedItem;
            }
        }

        public Vector2 CalcDirection(UsableItem item)
        {
            if (item is RangedItem rangedItem)
            {
                return _rangedAimer.transform.right;
            }
            else
            {
                // TODO: this is a placeholder
                return _rangedAimer.transform.right;
            }
        }
    }
}