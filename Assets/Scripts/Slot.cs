using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class Slot : MonoBehaviour, IBlockSupplier {
	public Slot up;
	public Slot down;
	public Slot left;
	public Slot right;
	public Slot[] receivers;
	EventTrigger eventTrigger;
	public UnityEvent OnContentEmpty;
	public UnityEvent OnBlockDestroyed;
	public UnityEvent OnReady;
	public State state;

	public Block content {
		get {
			return _content;
		}
		set {
			_content = value;
			if (value != null) {
				FallingAnimation ();
			} else {
				Debug.Log (state);
				state = State.Empty;
				OnContentEmpty.Invoke();
			}
		}
	}
	Block _content;

	public void InitEvents(UnityAction<BaseEventData> onBeginDrag, 
						UnityAction<BaseEventData> onEndDrag, 
						UnityAction<BaseEventData> onClick) {
		eventTrigger = GetComponent<EventTrigger> ();
		AddEventTriggerEntry (EventTriggerType.BeginDrag, onBeginDrag);
		AddEventTriggerEntry (EventTriggerType.EndDrag, onEndDrag);
		AddEventTriggerEntry (EventTriggerType.PointerClick, onClick);
	}

	public void Awake() {
		state = State.Empty;
		foreach (Slot s in receivers) {
			s.OnContentEmpty.AddListener(() => CheckReceivers());
		}
	}

	public void DestroyContent() {
		state = State.WaitingForDestroyAnimation;
		content.DestroyAnimation (() => {
			content = null;
		});
	}
		
	private void FallingAnimation() {
		state = State.WaitingForBlockAnimation;
		LeanTween.move (content.gameObject, transform.position, 0.3f)
			.setEaseOutSine().setOnComplete(FallingAnimationComplete);

	}

	private void FallingAnimationComplete() {
		state = Slot.State.BlockReady;
		if (!CheckReceivers ()) {
			OnReady.Invoke ();
		};
	}

	private void AddEventTriggerEntry(EventTriggerType eventType, UnityAction<BaseEventData> action) {
		EventTrigger.Entry newEntry = new EventTrigger.Entry ();
		newEntry.eventID = eventType;
		newEntry.callback.AddListener(action);
		eventTrigger.triggers.Add (newEntry);
	}
		
	public bool CheckReceivers() {
		if (state == State.BlockReady) {
			foreach (Slot s in receivers) {
				if (s.state == Slot.State.Empty) {
					GiveBlock (s);
					return true;
				}
			}
		}
		return false;
	}
		
	private void GiveBlock(Slot receiver) {
		receiver.content = content;
		content = null;
	}

	public enum State {Empty, BlockReady, WaitingForBlockAnimation, WaitingForDestroyAnimation}
}
