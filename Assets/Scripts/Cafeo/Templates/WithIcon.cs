using Sirenix.OdinInspector;
using UnityEngine;

namespace Cafeo.Templates
{
    public class WithIcon : WithDisplayName
    {
        [BoxGroup("Basic Info", centerLabel: true)]
        // [AssetsOnly]
        [AssetSelector(Paths = "Assets/Graphics/Icons")]
        [HideLabel, PreviewField(55)]
        public Sprite icon;
        
        [BoxGroup("Basic Info", centerLabel: true)] [ProgressBar(0, 255)]
        public int tint;
    }
}