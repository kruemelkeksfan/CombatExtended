using Verse;
using System;
using Multiplayer.API;

namespace CombatExtended.Compatibility
{
    // See: https://github.com/Parexy/Multiplayer/wiki/API
    [StaticConstructorOnStartup]
    static class Patches
    {
        class SeedHolder
		{
            [SyncField]
            public int seed;
		}

    	static SeedHolder seed = new SeedHolder();

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

			// Initialize RNGs with same Seeds on every Client
			if(MP.IsInMultiplayer)
			{

				MP.WatchBegin(); // Let's being watching

				// This is here to set the variable if it changed on other clients
				// It will update the variable and the logic will stay the same.
				MP.Watch(seed, nameof(seed.seed));
			}
			seed.seed = (int) DateTimeOffset.UtcNow.ToUnixTimeSeconds();
			if(MP.IsInMultiplayer)
			{

				MP.WatchEnd(); // We are done watching!

			}

			Verse.Rand.PushState();
			Verse.Rand.Seed = 0;//seed.seed;
			UnityEngine.Random.InitState(0);//seed.seed);
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
