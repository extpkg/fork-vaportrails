using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Achievement : ScriptableObject {
	[SerializeField] Sprite icon;
	[SerializeField][TextArea] string description;
	[SerializeField] bool rare = false;
	[SerializeField] bool secret = true;
	[SerializeField] string titleOverride;

	public Sprite Icon => icon;
	public string Description => description;
	public bool Rare => rare;
	public bool Secret => secret;

	public void Get() {
		FindObjectOfType<Achievements>(includeInactive: true).Get(this);
	}

	public string GetName() {
		if (!string.IsNullOrEmpty(titleOverride)) return titleOverride;
		return this.name;
	}
}
