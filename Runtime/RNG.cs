// using System.Collections;
// using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GiveUsComponents {

	public static class RNG {

		[System.Serializable]
		public class State { 
			[SerializeField] public bool seedRandomly = true;
			[SerializeField] public uint value = 0xAABB;

			[HideInInspector] public bool seeded = false;
		}

		public static uint Xorshift32(RNG.State state) {
			
			if (state.seedRandomly && !state.seeded) {
				state.value = (uint)Mathf.RoundToInt(Random.value * 100000f);
				state.seeded = true;
			}

	        /* Algorithm "xor" from p. 4 of Marsaglia, "Xorshift RNGs" */
	        uint x = state.value;
	        x ^= x << 13;
	        x ^= x >> 17;
	        x ^= x << 5;
	        return state.value = x;
	    }

	    public static int RandInt(RNG.State state, int from, int toExclusive) {
	        return from + (int)(
	            RNG.Xorshift32(state) % (uint)(toExclusive - from)
	        );
	    }

	    public static int Dice(RNG.State state, int nSides) {
	        return RNG.RandInt(state, 1, nSides + 1);
	    }

	    public static float Randf01(RNG.State state) {
	        return (RNG.Xorshift32(state) % 1000000) / 1000000f;
	    }

	}


}