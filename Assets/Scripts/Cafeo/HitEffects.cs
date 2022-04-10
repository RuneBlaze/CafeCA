using System;
using System.Collections.Generic;
using Cafeo.Data;
using UnityEngine;

namespace Cafeo
{
    [Serializable]
    public class HitEffects
    {
        // anything that is derived from str, def, etc.
        public enum SecondaryAttr
        {
            Atk,
            Def,
            Mat,
            Mdf,
            Spd,
            HpPerSec,
            MpPerSec,
            CpPerSec,
            Shield,
            ShieldPerc,
        }
        
        [HideInInspector] public IStatusTag statusTag;

        [Serializable]
        public class BuffExpr
        {
            public SecondaryAttr dst;
            public string calcExpr;
            public float duration;
            public string buffNameOverride;
            public string BuffName => buffNameOverride ?? dst.ToString();
            public PresetSpecifier presetSpecifier;
            public bool eternal;

            public BuffExpr(SecondaryAttr dst, string calcExpr, float duration)
            {
                this.dst = dst;
                this.calcExpr = calcExpr;
                this.duration = duration;
            }

            public float CalcValue(BattleVessel src, BattleVessel target, int levelOffset = 0)
            {
                var state = new Dictionary<string, float>();
                state["atk"] = src.Atk;
                state["def"] = src.Def;
                state["mat"] = src.Mat;
                state["mdf"] = src.Mdf;
                state["level"] = levelOffset;
                float v = ArithmeticEval.EvaluateArithmeticExpression(calcExpr.ToLowerInvariant(), state);
                return v;
            }

            public StatusEffect CalcStatus(BattleVessel src, BattleVessel target, int levelOffset = 0)
            {
                float v = CalcValue(src, target, levelOffset);
                var s = new StatusEffect(target, duration, eternal);
                if (presetSpecifier is { IsEmpty: false })
                {
                    s.passiveEffect = presetSpecifier.Generate();
                }
                bool isDebuff = v < 0;
                string buffHumanize = isDebuff ? "debuff" : "buff";
                s.displayName = $"{dst} {buffHumanize}";
                switch (dst)
                {
                    case SecondaryAttr.Atk:
                        s.atk = v;
                        break;
                    case SecondaryAttr.Def:
                        s.def = v;
                        break;
                    case SecondaryAttr.Mat:
                        s.mat = v;
                        break;
                    case SecondaryAttr.Mdf:
                        s.mdf = v;
                        break;
                    case SecondaryAttr.Spd:
                        s.spd = v;
                        break;
                    case SecondaryAttr.HpPerSec:
                        s.hpChangeSec = v;
                        break;
                    case SecondaryAttr.MpPerSec:
                        s.mpChangeSec = v;
                        break;
                    case SecondaryAttr.CpPerSec:
                        s.cpChangeSec = v;
                        break;
                    case SecondaryAttr.Shield:
                        s.shield = v;
                        break;
                    case SecondaryAttr.ShieldPerc:
                        s.shieldPerc = v;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                return s;
            }
        }
        public List<BuffExpr> buffs = new ();
        
        public void Apply(BattleVessel user, BattleVessel target, int levelOffset = 0)
        {
            foreach (var buff in buffs)
            {
                var statusEffect = buff.CalcStatus(user, target, levelOffset);
                statusEffect.statusTag = statusTag;
                target.AddStatus(statusEffect);
            }
        }

        public void TearDown(BattleVessel target)
        {
            target.RemoveStatus(it => it.statusTag.CompareStatusTag(statusTag));
        }
    }
}