using Sirenix.OdinInspector;
using UnityEngine;

namespace Cafeo.Templates
{
    public class JobTemplate : WithDisplayName
    {
        [BoxGroup("Job Specs", centerLabel: true)]
        private SkillTreeTemplate skillTree;
    }
}