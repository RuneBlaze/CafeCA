using Cafeo.Templates;
using UnityEngine;

namespace Cafeo.Entities
{
    public class EnemySpawner : BaseSpawner
    {
        public string enemyId;

        protected override void Activate()
        {
            base.Activate();
            Debug.Log("Spawning enemy");
            var finder = TemplateFinder.Instance;
            MyNode.PlaceEnemy(finder.RetrieveTemplate<EnemyTemplate>(enemyId), RelPos);
        }
    }
}