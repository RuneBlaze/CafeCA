using System;
using Cafeo.Templates;
using UnityEngine;

namespace Cafeo.Data
{
    [Serializable]
    public class PresetSpecifier : ITemplate<PresetPassiveEffect>
    {
        public string presetName;
        public Vector4 userData;

        public bool IsEmpty => string.IsNullOrEmpty(presetName);
        public PresetPassiveEffect Generate()
        {
            return PresetPassiveEffect.FromPreset(presetName, userData);
        }
    }
}