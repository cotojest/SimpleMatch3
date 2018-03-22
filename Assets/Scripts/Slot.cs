using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class Slot : MonoBehaviour, IBlockSupplier {
	[SerializeField]
	public Slot[] verticalNeighbours;
	[SerializeField]
	public Slot[] horizontalNeighbours;
	[SerializeField]
	public Slot[] receivers;
	EventTrigger eventTrigger;
	public UnityEvent OnContentEmpty;
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
				state = State.Empty;
				OnContentEmpty.Invoke ();
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
	}

	public void DestroyContent() {
		GameObject toDestroy = content.gameObject;
		content = null;
	}
		
	private void FallingAnimation() {
		state = State.WaitingForBlockAnimation;
		LeanTween.move (content.gameObject, transform.position, 0.3f)
			.setEaseOutCirc().setOnComplete(FallingAnimationComplete);

	}

	private void FallingAnimationComplete() {
		state = State.BlockReady;
		CheckReceivers ();
	}

	private void AddEventTriggerEntry(EventTriggerType eventType, UnityAction<BaseEventData> action) {
		EventTrigger.Entry newEntry = new EventTrigger.Entry ();
		newEntry.eventID = eventType;
		newEntry.callback.AddListener(action);
		eventTrigger.triggers.Add (newEntry);
	}
		
	public bool CheckReceivers() {
		bool generatedBlocks = false;
		foreach (Slot s in receivers) {
			if (s.state == Slot.State.Empty) {
				GiveBlock(s);
				return true;
			}
		}
		return false;
	}

	private void GiveBlock(Slot receiver) {
		receiver.content = content;
		content = null;
	}

	public enum State {BlockReady, WaitingForBlockAnimation, Empty}
}
