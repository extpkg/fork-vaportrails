using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(BeaconWrapper))]
public class Door : Interactable {
	public Sprite icon;

	protected override void Start() {
		base.Start();
		// load the mapIcon prefab base, then set it as the sprite
		if (icon != null) {
			GameObject g = Instantiate(Resources.Load<GameObject>("Runtime/MapIconBase"), transform);
			g.transform.position = transform.position;
			g.GetComponent<SpriteRenderer>().sprite = icon;
		}
	}

	public override void OnInteract(EntityController player) {
		FindObjectOfType<TransitionManager>().BeaconTransition(GetComponent<BeaconWrapper>().GetBeacon);
	}
}
