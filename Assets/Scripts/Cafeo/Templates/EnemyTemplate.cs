﻿using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Cafeo.Templates
{
    public class EnemyTemplate : SoulTemplate
    {
        [BoxGroup("Hotbar", centerLabel: true)]
        [AssetSelector] public List<SkillTemplate> skills;

        [BoxGroup("Enemy", centerLabel: true)] public int dropGoldExpectation;
        [BoxGroup("Enemy", centerLabel: true)] public string aiType;

        public override AgentSoul AddToGameObjet(GameObject gameObject)
        {
            var soul = base.AddToGameObjet(gameObject);
            // soul.gold = Random.Range(0, dropGoldExpectation * 2);
            // soul.aiType = aiType;
            return soul;
        }
    }
}