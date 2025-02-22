using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AttackBuffer : MonoBehaviour {

	EntityController player;
    PlayerInput inputManager;
    CombatController combatController;

	// treat it as sort-of a queue
    List<BufferedAttack> bufferedAttacks = new List<BufferedAttack>();

    bool punch, kick;
    const float inputThreshold = 0.2f;

	void Start() {
		player = GetComponent<EntityController>();
        inputManager = GetComponent<PlayerInput>();
        combatController = GetComponent<CombatController>();
	}

    void Update() {
        punch = inputManager.ButtonDown(Buttons.PUNCH);
        kick = inputManager.ButtonDown(Buttons.KICK);
        if (punch || kick) {
			AttackType attackType;
			Vector2Int attackDirection;

            if (punch) attackType = AttackType.PUNCH;
            else attackType = AttackType.KICK;

            Vector2 ls = inputManager.LeftStick();

            attackDirection = new Vector2Int(
                inputManager.HasHorizontalInput() ? (int) Mathf.Sign(ls.x * player.Forward()) : 0,
                (int) (Mathf.Approximately(ls.y, 0) ? 0 : ClampZero(ls.y))
            );
            attackDirection *= new Vector2Int(1, 2);
			BufferedAttack attack = new BufferedAttack(attackType, attackDirection);
			AddBufferedAttack(attack);
            // if it enters a graph without a buffered attack (i.e. it's read first)
            combatController.CheckAttackInputs();
        }
    }

    public void AddBufferedAttack(BufferedAttack attack) {
        bufferedAttacks.Add(attack);
        StartCoroutine(RemoveAction(attack));
    }

    float ClampZero(float input) {
        if (Mathf.Abs(input) < inputThreshold) {
            return 0;
        }
        return Mathf.Sign(input);
    }

    IEnumerator RemoveAction(BufferedAttack attack) {
        yield return new WaitForSecondsRealtime(PlayerInput.GetInputBufferDuration());
		bufferedAttacks.Remove(attack);
    }

    public BufferedAttack Peek() {
        return bufferedAttacks[0];
    }

	public BufferedAttack Consume() {
		BufferedAttack a = bufferedAttacks[0];
		bufferedAttacks.RemoveAt(0);
		return a;
	}

    public void Refund(BufferedAttack attack) {
        AddBufferedAttack(attack);
    }

	public bool Ready() {
		return bufferedAttacks.Count > 0;
	}

    public static bool Match(BufferedAttack attack, AttackLink link) {
        return link.type == attack.type && attack.HasDirection(link.direction);
    }
}

public class BufferedAttack {
	public AttackType type;
	public Vector2Int attackDirection;

	public BufferedAttack(AttackType t, Vector2Int d) {
		type = t;
		attackDirection = d;
	}
	
    public bool HasDirection(AttackDirection d) {
        if (d == AttackDirection.ANY) return true;
        return (d==(AttackDirection)attackDirection.x || d==(AttackDirection)attackDirection.y);
    }
}

public enum AttackDirection {
    ANY = 0,
    FORWARD = 1,
    BACKWARD = -1,
    UP = 2,
    DOWN = -2,
}

public enum AttackType {
    NONE = 0,
    PUNCH = 1,
    KICK = 2,
}
