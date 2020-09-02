using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

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

		public static Vector2 InsideUnitCircle()
		{
			return new Vector2(0.0f, 1.0f);
		}

		// TODO: Implement Seeds
		public static float ValueSeeded(int seed)
		{
			return Value();
		}

		public static float RangeSeeded(double min, double max, int seed)
		{
			return Range(min, max);
		}

		public static bool ChanceSeeded(double chance, int seed)
		{
			return Chance(chance);
		}
	}
}
