﻿using UnityEngine;
using Pathfinding;
using System;

namespace BehaviorDesigner.Runtime.Tasks.Movement.AstarPathfindingProject
{
    // Abstract class for any task that uses a group of IAstarAI agents
    public abstract class IAstarAIGroupMovement : GroupMovement
    {
        [Tooltip("All of the agents")]
        public SharedGameObject[] agents = null;

        public SharedGameObjectList dynamicAgents = new();
        [Tooltip("The speed of the agents")]
        public SharedFloat speed = 10;

        protected IAstarAI[] aStarAgents;
        protected Transform[] transforms;

        public override void OnStart()
        {
            if (agents.Length == 0)
            {
                if (dynamicAgents.Value.Count > 0)
                {
                    agents = new SharedGameObject[dynamicAgents.Value.Count];
                    for (int i = 0; i < dynamicAgents.Value.Count; i++)
                    {
                        // var sharedGo = new SharedGameObject();
                        agents[i] = dynamicAgents.Value[i];
                        // var go = dynamicAgents.Value[i];
                        // agents[i].SetValue(go);
                    } 
                }
            }
            aStarAgents = new IAstarAI[agents.Length];
            transforms = new Transform[agents.Length];
            // Set the speed and turning speed of all of the agents
            for (int i = 0; i < agents.Length; ++i) {
                aStarAgents[i] = agents[i].Value.GetComponent<IAstarAI>();
                transforms[i] = agents[i].Value.transform;

                aStarAgents[i].maxSpeed = speed.Value;
            }
        }

        protected override bool SetDestination(int index, Vector3 target)
        {
            aStarAgents[index].destination = target;
            aStarAgents[index].canMove = true;
            aStarAgents[index].canSearch = true;
            aStarAgents[index].SearchPath();
            return true;
        }

        protected override Vector3 Velocity(int index)
        {
            return aStarAgents[index].velocity;
        }

        public override void OnEnd()
        {
            for (int i = 0; i < agents.Length; ++i) {
                aStarAgents[i].destination = transform.position;
                aStarAgents[i].canMove = false;
            }
        }

        // Reset the public variables
        public override void OnReset()
        {
            agents = null;
            speed = 3;
        }
    }
}
