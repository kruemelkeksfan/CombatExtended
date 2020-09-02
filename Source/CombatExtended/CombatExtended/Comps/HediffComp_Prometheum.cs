﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using UnityEngine;

namespace CombatExtended
{
    public class HediffComp_Prometheum : HediffComp
    {
        private const float InternalFireDamage = 2;

        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);

            if (Pawn.IsHashIntervalTick(GenTicks.TicksPerRealSecond))
            {
                Fire fire = Pawn.GetAttachment(ThingDefOf.Fire) as Fire;
                if (fire == null && Pawn.Spawned)
                {
                    Pawn.TryAttachFire(parent.Severity * 0.5f);
                }
                else if (fire != null)
                {
                    fire.fireSize = Mathf.Min(fire.fireSize + parent.Severity * 0.5f, 1.75f);  // Clamped at max fire size
                }

                // Apply to internal parts
                if (Pawn.def.race.IsMechanoid)
                {
                    List<BodyPartRecord> availableParts = Pawn.health.hediffSet.GetNotMissingParts(BodyPartHeight.Undefined, BodyPartDepth.Inside).ToList(); 
                    var internalPart = availableParts[(int) (Compatibility.Random.Value() * availableParts.Count)];
                    if (internalPart == null)
                        return;
                    Pawn.TakeDamage(new DamageInfo(CE_DamageDefOf.Flame_Secondary, InternalFireDamage * Pawn.BodySize * parent.Severity, 0, -1, null,
                        internalPart));
                }
            }
        }
    }
}
