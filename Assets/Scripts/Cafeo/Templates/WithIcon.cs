using Sirenix.OdinInspector;
using UnityEngine;

namespace Cafeo.Templates
{
    public class WithIcon : WithDisplayName
    {
        [BoxGroup("Basic Info", centerLabel: true)]
        [HideLabel, PreviewField(55)]
        public Texture icon;

        [BoxGroup("Basic Info", centerLabel: true)] [ProgressBar(0, 255)]
        public int tint;
    }
}