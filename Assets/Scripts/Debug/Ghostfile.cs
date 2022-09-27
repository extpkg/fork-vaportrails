using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Ghostfile {

	// TODO: manually set tech percentage (store this in a scriptableobject i guess)

	public Dictionary<int, List<WeightedFrameInput>> ghost;

	public Ghostfile(Dictionary<int, List<WeightedFrameInput>> inputs) {
		this.ghost = inputs;
	}
}
