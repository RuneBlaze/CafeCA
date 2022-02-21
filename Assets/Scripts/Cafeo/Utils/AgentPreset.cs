using UnityEngine;

namespace Cafeo.Utils
{
    [CreateAssetMenu(fileName = "NewAgentData", menuName = "Presets/AgentPreset")]
    public class AgentPreset : ScriptableObject
    {
        public float str = 10f;
        public float con = 10f;
        public float dex = 10f;
        public float per = 10f;
        public float lea = 10f;
        public float wil = 10f;
        public float mag = 10f;
        public float cut = 10f;
        public float awe = 10f;
        public float life = 10f;
        public float mana = 10f;
    }
}