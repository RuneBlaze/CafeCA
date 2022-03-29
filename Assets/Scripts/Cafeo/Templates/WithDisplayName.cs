using Sirenix.OdinInspector;
using UnityEngine;

namespace Cafeo.Templates
{
    public class WithDisplayName : ScriptableObject
    {
        [BoxGroup("Basic Info", centerLabel: true)]
        public string displayName;
        [BoxGroup("Basic Info", centerLabel: true)]
        public string id;
    }
}