using System;
using Cafeo.Aimer;
using Cafeo.Castable;
using UnityEngine;
using static System.Single;

namespace Cafeo.Utility
{
    // A wrapper for calculating utility, contains both the entire environment,
    // and also some helpers for accessing the situation
    public class UtilityEnv : MonoBehaviour
    {
        public float distanceToEnemyTarget; // body distance to the target, non-penetrating
        public float distanceToAllyTarget; // body distance to the ally target, penetrating
        public float[] surroundingAllies; // sorted distances to all allies surrounding
        public float[] surroundingEnemies; // sorted distances to all enemies surrounding
        public float[] forwardAllies; // sorted distances to all allies in front
        public float[] forwardEnemies; // sorted distances to all enemies in front
        
        public int corneringEnemies; // TODO: implement the following
        public int corneringObstacles;

        public float justUsedPenalty = 0.2f;
        public UsableItem justUsed;
        
        private int offset; // a randomized offset to distribute physics queries over multiple frames
        private int frameCnt;
        private int allyMask;
        private int enemyMask;
        private int obstacleMask;

        private ContactFilter2D allyContactFilter;
        private ContactFilter2D allyContactFilterWithObstacle;
        private ContactFilter2D enemyContactFilter;
        private ContactFilter2D enemyContactFilterWithObstacle;

        private Collider2D[] colliderCache;
        private RaycastHit2D[] hitCache;
        private BattleVessel vessel;
        private AimerGroup aimer;

        public BattleVessel targetAlly;
        public BattleVessel bestTargetEnemy;
        private GenericBrain brain;

        public bool simple;

        private void Awake()
        {
            surroundingAllies = new float[5];
            surroundingEnemies = new float[5];
            forwardAllies = new float[5];
            forwardEnemies = new float[5];
            colliderCache = new Collider2D[5];
            hitCache = new RaycastHit2D[5];
            obstacleMask = LayerMask.GetMask("Obstacle");
        }

        private void Start()
        {
            offset = UnityEngine.Random.Range(0, 100);
            Scene.rogueUpdateEvent.AddListener(RogueUpdate);
            vessel = GetComponent<BattleVessel>();
            aimer = GetComponent<AimerGroup>();
            brain = GetComponent<GenericBrain>();
            var maskPlayerSide = LayerMask.GetMask("Allies");
            var maskEnemySide = LayerMask.GetMask("Enemies");
            allyMask = vessel.IsAlly ? maskPlayerSide : maskEnemySide;
            enemyMask = vessel.IsAlly ? maskEnemySide : maskPlayerSide;
            allyContactFilter = new ContactFilter2D
            {
                layerMask = allyMask,
                useLayerMask = true,
            };
            enemyContactFilter = new ContactFilter2D
            {
                layerMask = enemyMask,
                useLayerMask = true,
            };
            
            allyContactFilterWithObstacle = new ContactFilter2D
            {
                layerMask = allyMask | LayerMask.GetMask("Obstacle"),
                useLayerMask = true,
            };
            
            enemyContactFilterWithObstacle = new ContactFilter2D
            {
                layerMask = enemyMask | LayerMask.GetMask("Obstacle"),
                useLayerMask = true,
            };
        }

        public RogueManager Scene => RogueManager.Instance;

        private void RogueUpdate()
        {
            frameCnt++;
            int f = frameCnt + offset;
            switch (f % 4)
            {
                case 0:
                    CastCircle(); // update closeUp and midRange
                    UpdateCorneringEnemies();
                    break;
                case 1:
                    if (!simple) CastForwardCircle(); // update forwardAllies and forwardEnemies
                    break;
            }

            var targetEnemy = brain.RetrieveTargetEnemy();
            if (targetEnemy != null)
            {
                distanceToEnemyTarget = vessel.BodyDistance(targetEnemy);
            }
            else
            {
                distanceToEnemyTarget = PositiveInfinity;
            }
            
            if (targetAlly != null)
            {
                distanceToAllyTarget = vessel.BodyDistance(targetAlly);
            }
            else
            {
                distanceToAllyTarget = PositiveInfinity;
            }
        }

        private void UpdateCorneringEnemies()
        {
            if (!simple)
            {
                // surrounding means less than 1 body distance away
                corneringEnemies = CountSurroundingEnemies(1);
            }
        }

        private void CastCircle()
        {
            for (int j = 0; j < 5; j++)
            {
                surroundingAllies[j] = PositiveInfinity;
                surroundingEnemies[j] = PositiveInfinity;
            }
            BattleVessel bestAlly = null;
            float bestAllyDist = PositiveInfinity;
            int cnt = Physics2D.OverlapCircleNonAlloc(transform.position, 5, colliderCache, allyMask);
            for (int i = 0; i < cnt; i++)
            {
                var c = colliderCache[i];
                var potentialAlly = Scene.GetVesselFromGameObject(c.gameObject);
                surroundingAllies[i] = (potentialAlly.transform.position - transform.position).sqrMagnitude;
                if (surroundingAllies[i] < bestAllyDist && SightCheck(potentialAlly.gameObject))
                {
                    bestAllyDist = surroundingAllies[i];
                    bestAlly = potentialAlly;
                }
                i++;
            }
            targetAlly = bestAlly;
            
            BattleVessel bestEnemy = null;
            float bestEnemyDist = PositiveInfinity;
            
            if (!simple)
            {
                cnt = Physics2D.OverlapCircleNonAlloc(transform.position, 5, colliderCache, enemyMask);
                for (int i = 0; i < cnt; i++)
                {
                    var c = colliderCache[i];
                    var potentialEnemy = Scene.GetVesselFromGameObject(c.gameObject);
                    surroundingEnemies[i] = (potentialEnemy.transform.position - transform.position).sqrMagnitude;
                    if (surroundingEnemies[i] < bestEnemyDist && SightCheck(potentialEnemy.gameObject, true))
                    {
                        bestEnemyDist = surroundingEnemies[i];
                        bestEnemy = potentialEnemy;
                    }
                    i++;
                }
                bestTargetEnemy = bestEnemy;
            }
            Array.Sort(surroundingAllies);
            if (!simple) Array.Sort(surroundingEnemies);
        }

        // <summary>checks if between this and target is clear of obstacles</summary>
        private bool SightCheck(GameObject target, bool enemiesAsObstacles = false)
        {
            var position = transform.position;
            var dir = target.transform.position - position;
            var hit = Physics2D.Raycast(position, dir, 
                dir.magnitude, enemiesAsObstacles ? enemyContactFilterWithObstacle.layerMask : obstacleMask);
            if (hit.collider != null && hit.collider.gameObject != target)
            {
                return false;
            }
            return true;
        }

        private void CastForwardCircle()
        {
            for (int j = 0; j < 5; j++)
            {
                forwardAllies[j] = PositiveInfinity;
                forwardEnemies[j] = PositiveInfinity;
            }
            int cnt = Physics2D.CircleCast(transform.position, 3, 
                aimer.RangedAimer.transform.right, allyContactFilter, hitCache, 10);
            for (int i = 0; i < cnt; i++)
            {
                var c = hitCache[i];
                var potentialAlly = Scene.GetVesselFromGameObject(c.collider.gameObject);
                if (!SightCheck(c.collider.gameObject))
                {
                    forwardAllies[i] = PositiveInfinity;
                }
                else
                {
                    forwardAllies[i] = (potentialAlly.transform.position - transform.position).sqrMagnitude;
                }
                i++;
            }
            cnt = Physics2D.CircleCast(transform.position, 3, 
                aimer.RangedAimer.transform.right, enemyContactFilter, hitCache, 10);
            for (int i = 0; i < cnt; i++)
            {
                var c = hitCache[i];
                var potentialEnemy = Scene.GetVesselFromGameObject(c.collider.gameObject);
                if (!SightCheck(c.collider.gameObject))
                {
                    forwardEnemies[i] = PositiveInfinity;
                }
                else
                {
                    forwardEnemies[i] = (potentialEnemy.transform.position - transform.position).sqrMagnitude;
                }
                i++;
            }
            Array.Sort(forwardAllies);
            Array.Sort(forwardEnemies);
        }

        public int CountSurroundingAllies(float threshold)
        {
            int cnt = 0;
            for (int i = 0; i < 5; i++)
            {
                var pow = Mathf.Pow(threshold, 2);
                if (surroundingAllies[i] < pow)
                {
                    cnt++;
                }
                else
                {
                    break;
                }
            }
            return cnt;
        }
        
        public int CountSurroundingEnemies(float threshold)
        {
            int cnt = 0;
            for (int i = 0; i < 5; i++)
            {
                var pow = Mathf.Pow(threshold, 2);
                if (surroundingEnemies[i] < pow)
                {
                    cnt++;
                }
                else
                {
                    break;
                }
            }
            return cnt;
        }
        
        public int CountForwardAllies(float threshold)
        {
            int cnt = 0;
            for (int i = 0; i < 5; i++)
            {
                var pow = Mathf.Pow(threshold, 2);
                if (forwardAllies[i] < pow)
                {
                    cnt++;
                }
                else
                {
                    break;
                }
            }
            return cnt;
        }
        
        public int CountForwardEnemies(float threshold)
        {
            int cnt = 0;
            for (int i = 0; i < 5; i++)
            {
                var pow = Mathf.Pow(threshold, 2);
                if (forwardEnemies[i] < pow)
                {
                    cnt++;
                }
                else
                {
                    break;
                }
            }
            return cnt;
        }

        public float CalcStatusEffectScore(UsableItem item)
        {
            return 1;
        }
        
        public float CalcHitConfidence(UsableItem item)
        {
            return 1;
        }
    }
}