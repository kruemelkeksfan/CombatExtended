using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CombatExtended.Compatibility
{
	class Random
	{
		public static float Value()
		{
			/*
			List of Random Number Methods (Source: https://github.com/Dunkhan/Multiplayer/wiki/Dealing-with-Randomness):

			System.Random
			Verse.Rand
			Verse.FloatRange.RandomInRange
			Verse.IntRange.RandomInRange
			UnityEngine.Random
			Verse.GenCollection.RandomElement*
			CellFinder.Random*
			*/

			Verse.Rand;

			return 0.5f;
		}

		public static float Range(double min, double max)
		{
			return (float) (Value() * (max - min) + min);
		}

		public static bool Chance(double chance)
		{
			return Value() < chance;
		}
	}
}
