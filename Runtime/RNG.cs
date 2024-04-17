// using System.Collections;
// using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GiveUsComponents {
	public static class RNG {

		public static uint Xorshift32(ref uint state) {
	        /* Algorithm "xor" from p. 4 of Marsaglia, "Xorshift RNGs" */
	        uint x = state;
	        x ^= x << 13;
	        x ^= x >> 17;
	        x ^= x << 5;
	        return state = x;
	    }

	    public static int RandInt(ref uint state, int from, int toExclusive) {
	        return from + (int)(
	            RNG.Xorshift32(ref state) % (uint)(toExclusive - from)
	        );
	    }

	    public static float Randf01(ref uint state) {
	        return (RNG.Xorshift32(ref state) % 1000000) / 1000000f;
	    }
	}
}