using Sirenix.OdinInspector;

namespace Cafeo.Templates
{
    public class JobTemplate : WithDisplayName
    {
        [BoxGroup("Job Specs", centerLabel: true)]
        private SkillTreeTemplate skillTree;
    }
}