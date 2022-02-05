using System;
using Cafeo.Castable;
using UnityEngine;

namespace Cafeo.Aimer
{
    public class AimerGroup : MonoBehaviour
    {
        private RangedAimer _rangedAimer;
        private MeleeAimer _meleeAimer;

        public void Awake()
        {
            _rangedAimer = GetComponentInChildren<RangedAimer>();
            _rangedAimer.gameObject.SetActive(false);

            _meleeAimer = GetComponentInChildren<MeleeAimer>();
            _meleeAimer.gameObject.SetActive(false);
        }

        public void RequestAimer(UsableItem item)
        {
            _rangedAimer.gameObject.SetActive(false);
            _meleeAimer.gameObject.SetActive(false);
            switch (item)
            {
                case RangedItem rangedItem:
                    _rangedAimer.gameObject.SetActive(true);
                    _rangedAimer.Item = rangedItem;
                    break;
                case MeleeItem meleeItem:
                    _meleeAimer.gameObject.SetActive(true);
                    _meleeAimer.Item = meleeItem;
                    break;
            }
        }

        public void SetupTargetTag(string targetTag)
        {
            _rangedAimer.Setup();
           _rangedAimer.SetupTargetTag(targetTag);
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
                return _meleeAimer.transform.right;
            }
        }
    }
}