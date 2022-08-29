using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public class DelayedEvent : MonoBehaviour {

	public TimedEvent[] events;

	public void Invoke() {
		foreach (TimedEvent timedEvent in events) {
			StartCoroutine(DelayAndFire(timedEvent.delay, timedEvent.callback));
		}
	}

	IEnumerator DelayAndFire(float delay, UnityEvent callback) {
		yield return new WaitForSeconds(delay);
		callback.Invoke();
	}

	[System.Serializable]
	public class TimedEvent {
		public UnityEvent callback;
		public float delay;
	}	
}
