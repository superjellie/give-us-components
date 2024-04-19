using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GiveUsComponents {
	public class ShowWhenAttribute : PropertyAttribute {

		public string name;
		public ulong value;
		public bool flags;
		public bool bools;

		public ShowWhenAttribute(string name, ulong value, bool flags = false) {
			this.name = name;
			this.value = value;
			this.flags = flags;
			this.bools = false;
		}

		public ShowWhenAttribute(string name, bool value = true) {
			this.name = name;
			this.value = value ? (ulong)1 : (ulong)0;
			this.bools = true;
			this.flags = false;
		}
	}
}