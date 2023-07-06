using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameCheckpointLoader : SavedObject {
#if UNITY_EDITOR
	bool loadedBefore = false;
	public GameCheckpoint checkpoint;

	protected override void LoadFromProperties(bool startingUp) {
		loadedBefore = Get<bool>(nameof(loadedBefore));
	}

	void Start() {
		if (!loadedBefore) {
			// then add everything
			Inventory inventory = PlayerInput.GetPlayerOneInput().GetComponentInChildren<Inventory>();
			inventory.AddItemsQuietly(checkpoint.GetItems());

			GameFlags f = FindObjectOfType<GameFlags>();
			f.Add(checkpoint.GetGameFlags());

			loadedBefore = true;
		}
	}

	protected override void SaveToProperties(ref Dictionary<string, object> properties) {
		properties[nameof(loadedBefore)] = loadedBefore;
	}

	// don't want to always set this crap in the editor
	public override string GetObjectPath() {
		return $"global/{name}/{GetType().Name}";
	}
#endif
}
