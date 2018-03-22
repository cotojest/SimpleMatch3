using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class Slot : MonoBehaviour {
	[SerializeField]
	public Slot[] verticalNeighbours;
	[SerializeField]
	public Slot[] horizontalNeighbours;
	[SerializeField]
	public BlockSupplier[] blockSuppliers;
	EventTrigger eventTrigger;
	public Block content;


	public void InitEvents(UnityAction<BaseEventData> onBeginDrag, 
						UnityAction<BaseEventData> onEndDrag, 
						UnityAction<BaseEventData> onClick) {
		eventTrigger = GetComponent<EventTrigger> ();
		AddEventTriggerEntry (EventTriggerType.BeginDrag, onBeginDrag);
		AddEventTriggerEntry (EventTriggerType.EndDrag, onEndDrag);
		AddEventTriggerEntry (EventTriggerType.PointerClick, onClick);
	}
		
	public void DestroyContent() {
		GameObject toDestroy = content.gameObject;
		content = null;
		GetNewBlock ();
	}

	public void FallContent() {
		content = null;
		GetNewBlock ();
	}

	public void GetNewBlock() {
		foreach (BlockSupplier bS in blockSuppliers) {
			if (bS.hasBlock) {
				content = bS.GiveBlock ();
				break;
			}
		}
		
	}

	private void AddEventTriggerEntry(EventTriggerType eventType, UnityAction<BaseEventData> action) {
		EventTrigger.Entry newEntry = new EventTrigger.Entry ();
		newEntry.eventID = eventType;
		newEntry.callback.AddListener(action);
		eventTrigger.triggers.Add (newEntry);
	}

}
