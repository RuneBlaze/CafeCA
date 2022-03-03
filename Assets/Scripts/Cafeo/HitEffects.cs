using System;
using System.Collections.Generic;

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
            HpPerSec,
            MpPerSec,
            CpPerSec,
        }

        [Serializable]
        public class BuffExpr
        {
            public SecondaryAttr dst;
            public string calcExpr;
            public float duration;

            public BuffExpr(SecondaryAttr dst, string calcExpr, float duration)
            {
                this.dst = dst;
                this.calcExpr = calcExpr;
                this.duration = duration;
            }

            public float CalcValue(BattleVessel src, BattleVessel target)
            {
                var state = new Dictionary<string, float>();
                state["atk"] = src.Atk;
                state["def"] = src.Def;
                state["mat"] = src.Mat;
                state["mdf"] = src.Mdf;
                float v = ArithmeticEval.EvaluateArithmeticExpression(calcExpr.ToLowerInvariant(), state);
                return v;
            }

            public StatusEffect CalcStatus(BattleVessel src, BattleVessel target)
            {
                float v = CalcValue(src, target);
                var s = new StatusEffect(target, duration);
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
                    case SecondaryAttr.HpPerSec:
                        s.hpChangeSec = v;
                        break;
                    case SecondaryAttr.MpPerSec:
                        s.mpChangeSec = v;
                        break;
                    case SecondaryAttr.CpPerSec:
                        s.cpChangeSec = v;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                return s;
            }
        }
        public List<BuffExpr> buffs = new ();
    }
}