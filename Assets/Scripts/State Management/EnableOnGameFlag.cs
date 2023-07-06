using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnableOnGameFlag : GameFlagChangeListener{
	public GameFlag flag;
	public bool disableOnFlag = false;

	GameFlags flags;

	void Start() {
		flags = FindObjectOfType<GameFlags>();
		CheckEnabled();
	}

	public override void CheckEnabled() {
		if (disableOnFlag) {
			gameObject.SetActive(!flags.Has(flag));
		} else {
			gameObject.SetActive(flags.Has(flag));	
		}
	}
}
