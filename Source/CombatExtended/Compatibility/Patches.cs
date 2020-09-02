using Verse;
using System;
using Multiplayer.API;

namespace CombatExtended.Compatibility
{
    // See: https://github.com/Parexy/Multiplayer/wiki/API
    [StaticConstructorOnStartup]
    static class Patches
    {
		static Patches()
		{
			if(!MP.enabled)
				return;

			// This is where the magic happens and your attributes
			// auto register, similar to Harmony's PatchAll.
			MP.RegisterAll();

			// You can choose to not auto register and do it manually
			// with the MP.Register* methods.

			// Use MP.IsInMultiplayer to act upon it in other places
			// user can have it enabled and not be in session
		}

		public static void Init()
        {
            if (EDShields.CanInstall())
            {
                EDShields.Install();
            }

            Log.Message($"VanillaFurnitureExpandedShields.CanInstall() - {VanillaFurnitureExpandedShields.CanInstall()}");
            if (VanillaFurnitureExpandedShields.CanInstall())
            {
                VanillaFurnitureExpandedShields.Install();
            }
        }
    }
}
