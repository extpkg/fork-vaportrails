using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class AIEngine : MonoBehaviour, IHitListener, IAttackLandListener {
	public enum State {
		NEUTRAL,
		ADVANTAGE,
		DISADVANTAGE
	}
	PlayerSnapshotInfo player;
	PlayerSnapshotInfo self;
	AIRoutine currentRoutine = null;
	Dictionary<State, List<AIRoutine>> decisions = new();
	Queue<AIRoutine> routines = new();
	ComputerController controller;

	bool playing = false;

	void Start() {
		PlayerInput p = PlayerInput.GetPlayerOneInput();
		if (!p.GetComponent<PlayerSnapshotInfo>()) {
			player = p.gameObject.AddComponent<PlayerSnapshotInfo>();
		}
		player = p.gameObject.GetComponent<PlayerSnapshotInfo>();
		
		if (!GetComponent<PlayerSnapshotInfo>()) {
			gameObject.AddComponent<PlayerSnapshotInfo>();
		}
		self = gameObject.GetComponent<PlayerSnapshotInfo>();

		controller = GetComponent<PlayerInput>().comControl;

		InitializeEngine();
	}

	protected abstract void InitializeEngine();

	public void Play() {
		playing = true;
		StartRoutine(decisions[State.NEUTRAL][0]);
	}

	public void Halt() {
		playing = false;
		StopRoutine();
	}

	void StartRoutine(AIRoutine routine) {
		currentRoutine = routine;
		currentRoutine.SetInfo(self, player, controller);
		currentRoutine.Start();
	}

	void StopRoutine() {
		currentRoutine = null;
		controller.Zero();
	}

	void Update() {
		if (!playing) return;
		currentRoutine?.Update();
		if (currentRoutine?.IsDone() ?? false) {
			StopRoutine();
			if (routines.Count > 0) {
				StartRoutine(routines.Dequeue());
				return;
			}
		}
		if (currentRoutine == null) {
			// then pick a random one, from neutral for now
			// TODO: pick advantage and disadvantage
			StartRoutine(PickRoutine(State.NEUTRAL));
		}
	}

	AIRoutine PickRoutine(State currentState) {
		return decisions[currentState][Random.Range(0, decisions[currentState].Count-1)];
	}

	protected AIRoutine Add(State state, AIRoutine routine) {
		if (!decisions.ContainsKey(state)) {
			decisions[state] = new List<AIRoutine>();
		}
		decisions[state].Add(routine);
		routine.SetInfo(GetComponent<PlayerSnapshotInfo>(), player, controller);
		return routine;
	}

	public void OnHit(AttackHitbox attacker) {
		if (GetComponent<Entity>().staggerable) {
			StopRoutine();
			routines.Clear();
		}
	}

	public void OnAttackLand(AttackHitbox hitbox, Hurtbox hurtbox) {

	}
}
