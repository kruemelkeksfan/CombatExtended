using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using UnityEngine;

namespace CombatExtended
{
    public class ShiftVecReport
    {
        public LocalTargetInfo target = null;
        public Pawn TargetPawn
        {
            get
            {
                return target.Thing as Pawn;
            }
        }
        public float aimingAccuracy = 1f;
        public float sightsEfficiency = 1f;

        private float accuracyFactorInt = -1f;
        public float AccuracyFactor
        {
            get
            {
                if (accuracyFactorInt < 0)
                {
                    accuracyFactorInt = (1.5f - aimingAccuracy) / sightsEfficiency;
                }
                return accuracyFactorInt;
            }
        }

        public float circularMissRadius = 0f;
        public float indirectFireShift = 0f;

        // Visibility variables
        public float lightingShift = 0f;
        public float weatherShift = 0f;
        private float visibilityShiftInt = -1f;
        public float VisibilityShift
        {
            get
            {
                if (visibilityShiftInt < 0)
                {
                    visibilityShiftInt = (lightingShift + weatherShift + smokeDensity) * (shotDist / 50 / sightsEfficiency) * (2 - aimingAccuracy);
                }
                return visibilityShiftInt;
            }
        }

        // Leading variables
        public float shotSpeed = 0f;
        private bool TargetIsMoving
        {
            get
            {
                return TargetPawn != null && TargetPawn.pather != null && TargetPawn.pather.Moving;
            }
        }
        private float leadDistInt = -1f;
        public float LeadDist
        {
            get
            {
                if (leadDistInt < 0)
                {
                    if (TargetIsMoving)
                    {
                        float targetSpeed = CE_Utility.GetMoveSpeed(TargetPawn);
                        float timeToTarget = shotDist / shotSpeed;
                        leadDistInt = targetSpeed * timeToTarget;
                    }
                    else
                    {
                        leadDistInt = 0f;
                    }
                }
                return leadDistInt;
            }
        }
        public float LeadShift
        {
            get
            {
                return LeadDist * Mathf.Min(AccuracyFactor * 0.25f, 3);
            }
        }

        // Range variables
        public float shotDist = 0f;
        public float maxRange;
        public float DistShift
        {
            get
            {
                return shotDist * (shotDist / maxRange) * Mathf.Min(AccuracyFactor * 0.5f, 0.8f);
            }
        }

        public bool isAiming = false;
        public float swayDegrees = 0f;
        public float spreadDegrees = 0f;
        public Thing cover = null;
        public float smokeDensity = 0f;

        // Copy-constructor
        public ShiftVecReport(ShiftVecReport report)
        {
            target = report.target;
            sightsEfficiency = report.sightsEfficiency;
            aimingAccuracy = report.aimingAccuracy;
            circularMissRadius = report.circularMissRadius;
            indirectFireShift = report.indirectFireShift;
            lightingShift = report.lightingShift;
            shotSpeed = report.shotSpeed;
            shotDist = report.shotDist;
            maxRange = report.maxRange;
            isAiming = report.isAiming;
            swayDegrees = report.swayDegrees;
            spreadDegrees = report.spreadDegrees;
            cover = report.cover;
            smokeDensity = report.smokeDensity;
        }

        public ShiftVecReport()
        {
        }

        public Vector2 GetRandCircularVec()
        {
            Vector2 vec = CE_Utility.GenRandInCircle(VisibilityShift + circularMissRadius + indirectFireShift);
            return vec;
        }

        public float GetRandDist()
        {
            float dist = shotDist + Compatibility.Random.Range(-DistShift, DistShift);
            return dist;
        }

        public Vector2 GetRandLeadVec()
        {
            Vector3 moveVec = new Vector3();
            if (TargetIsMoving)
            {
            	moveVec = (TargetPawn.pather.nextCell - TargetPawn.Position).ToVector3() * (LeadDist + Compatibility.Random.Range(-LeadShift, LeadShift));
            }
            return new Vector2(moveVec.x, moveVec.z);
        }

        /// <returns>Angle Vector2 in degrees</returns>
        public Vector2 GetRandSpreadVec()
        {
            Vector2 vec = Compatibility.Random.InsideUnitCircle() * spreadDegrees;
            return vec;
        }

        public static string AsPercent(float pct)
        {
            return Mathf.RoundToInt(100f * pct) + "%";
        }

        public string GetTextReadout()
        {
            StringBuilder stringBuilder = new StringBuilder();
            if (VisibilityShift > 0)
            {
                stringBuilder.AppendLine("   " + "CE_VisibilityError".Translate() + "\t" + GenText.ToStringByStyle(VisibilityShift, ToStringStyle.FloatTwo) + " " + "CE_cells".Translate());

                if (lightingShift > 0)
                {
                    stringBuilder.AppendLine("      " + "Darkness".Translate() + "\t" + AsPercent(lightingShift));
                }
                if (weatherShift > 0)
                {
                    stringBuilder.AppendLine("      " + "Weather".Translate() + "\t" + AsPercent(weatherShift));
                }
                if (smokeDensity > 0)
                {
                    stringBuilder.AppendLine("      " + "CE_SmokeDensity".Translate() + "\t" + AsPercent(smokeDensity));
                }
            }
            if (LeadShift > 0)
            {
                stringBuilder.AppendLine("   " + "CE_LeadError".Translate() + "\t" + GenText.ToStringByStyle(LeadShift, ToStringStyle.FloatTwo) + " " + "CE_cells".Translate());
            }
            if(DistShift > 0)
            {
                stringBuilder.AppendLine("   " + "CE_RangeError".Translate() + "\t" + GenText.ToStringByStyle(DistShift, ToStringStyle.FloatTwo) + " " + "CE_cells".Translate());
            }
            if (swayDegrees > 0)
            {
                stringBuilder.AppendLine("   " + "CE_Sway".Translate() + "\t\t" + GenText.ToStringByStyle(swayDegrees, ToStringStyle.FloatTwo) + "CE_degrees".Translate());
            }
            if (spreadDegrees > 0)
            {
                stringBuilder.AppendLine("   " + "CE_Spread".Translate() + "\t\t" + GenText.ToStringByStyle(spreadDegrees, ToStringStyle.FloatTwo) + "CE_degrees".Translate());
            }
            // Don't display cover and target size if our weapon has a CEP
            if (circularMissRadius > 0)
            {
                stringBuilder.AppendLine("   " + "CE_MissRadius".Translate() + "\t" + GenText.ToStringByStyle(circularMissRadius, ToStringStyle.FloatTwo) + " "+ "CE_cells".Translate());
                if (indirectFireShift > 0)
                {
                    stringBuilder.AppendLine("   " + "CE_IndirectFire".Translate() + "\t" + GenText.ToStringByStyle(indirectFireShift, ToStringStyle.FloatTwo) + " " + "CE_cells".Translate());
                    PlayerKnowledgeDatabase.KnowledgeDemonstrated(CE_ConceptDefOf.CE_MortarDirectFire, KnowledgeAmount.FrameDisplayed); // Show we learned about indirect fire penalties
                }
            }
            else
            {
                if (cover != null)
                {
                	stringBuilder.AppendLine("   " + "CE_CoverHeight".Translate() + "\t" + new CollisionVertical(cover).Max * CollisionVertical.MeterPerCellHeight + " "+ "CE_meters".Translate());
                }
                if (target.Thing != null)
                {
                    stringBuilder.AppendLine("   " + "CE_TargetHeight".Translate() + "\t" + GenText.ToStringByStyle(new CollisionVertical(target.Thing).HeightRange.Span * CollisionVertical.MeterPerCellHeight, ToStringStyle.FloatTwo) + " " + "CE_meters".Translate());
                    stringBuilder.AppendLine("   " + "CE_TargetWidth".Translate() + "\t" + GenText.ToStringByStyle(CE_Utility.GetCollisionWidth(target.Thing) * CollisionVertical.MeterPerCellHeight, ToStringStyle.FloatTwo) + " " + "CE_meters".Translate());
                    var pawn = target.Thing as Pawn;
                    if (pawn != null && pawn.IsCrouching())
                    {
                    	LessonAutoActivator.TeachOpportunity(CE_ConceptDefOf.CE_Crouching, OpportunityType.GoodToKnow);
                    }
                }
                PlayerKnowledgeDatabase.KnowledgeDemonstrated(CE_ConceptDefOf.CE_AimingSystem, KnowledgeAmount.FrameDisplayed); // Show we learned about the aiming system
            }
            return stringBuilder.ToString();
        }
    }
}
