using System.Collections.Generic;
using UnityEngine;

namespace Cafeo.Templates
{
    public class SkillTreeTemplate : WithDisplayName
    {
        public List<SkillTemplate> level1;
        public List<SkillTemplate> level2;
        public List<SkillTemplate> level3;
        public List<SkillTemplate> level4;
        public List<SkillTemplate> level5;
    }
}