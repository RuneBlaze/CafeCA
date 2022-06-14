using System;
using Cafeo.Templates;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Cafeo.Data
{
    [Serializable]
    public class PresetSpecifier : ITemplate<PresetPassiveEffect>
    {
        [InfoBox("This name will be looked up in the preset database for a programmatic effect.")]
        public string presetName;

        public Vector4 userData;

        public bool IsEmpty => string.IsNullOrEmpty(presetName);

        public PresetPassiveEffect Generate()
        {
            return PresetPassiveEffect.FromPreset(presetName, userData);
        }
    }
}