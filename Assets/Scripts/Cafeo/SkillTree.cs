using System;
using System.Collections.Generic;
using Cafeo.Castable;

namespace Cafeo
{
    [Serializable]
    public class SkillTree
    {
        public List<List<UsableItem>> data;

        public SkillTree()
        {
            data = new List<List<UsableItem>>();
        }
    }
}