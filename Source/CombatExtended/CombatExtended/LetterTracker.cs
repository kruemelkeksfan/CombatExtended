using RimWorld;
using Verse;

namespace CombatExtended
{
    public class LetterTracker : MapComponent
    {
        private static bool _sentMechWarning;

        public LetterTracker(Map map) : base(map)
        {
        }

        public override void MapComponentTick()
        {
            base.MapComponentTick();

            if (!_sentMechWarning && GenDate.DaysPassed >= Faction.OfMechanoids.def.earliestRaidDays * 0.75f)
            {
                var mappawns = Find.AnyPlayerHomeMap.mapPawns;
                var suggestingPawn = mappawns.FreeColonistsSpawnedCount != 0
                    ? mappawns.FreeColonistsSpawned[(int) (Compatibility.Random.Value() * mappawns.FreeColonistsSpawned.Count)]
                    : PawnsFinder.AllMapsCaravansAndTravelingTransportPods_Alive_Colonists[(int) (Compatibility.Random.Value() * PawnsFinder.AllMapsCaravansAndTravelingTransportPods_Alive_Colonists.Count)];

                var label = "CE_MechWarningLabel".Translate();
                var text = "CE_MechWarningText".Translate(suggestingPawn?.LabelShort ?? "CE_MechWarningText_UnnamedColonist".Translate(), CE_ThingDefOf.Mech_Centipede.GetStatValueAbstract(StatDefOf.ArmorRating_Sharp));

                Find.LetterStack.ReceiveLetter(label, text, LetterDefOf.NegativeEvent);
                _sentMechWarning = true;
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<bool>(ref _sentMechWarning, "sentMechWarning", false, false);
        }
    }
}