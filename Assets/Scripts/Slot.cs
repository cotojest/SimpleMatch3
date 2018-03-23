using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

[System.Serializable]
public class SlotUnityEvent : UnityEvent<Slot> {
}

public class Slot : MonoBehaviour, IBlockSupplier, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler {
	public Slot right;
	public Slot left;
	public Slot down;
	public Slot up;
	public Slot[] receivers;
	public UnityEvent OnContentEmpty;
	public UnityEvent OnBlockDestroyed;
	public UnityEvent OnReady;
	public SlotUnityEvent OnMoveBegin;
	public SlotUnityEvent OnMoveEnd;

	public State state;
	bool pressed;

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
				OnContentEmpty.Invoke();
			}
		}
	}
	Block _content;

	 void Awake() {
		state = State.Empty;
		foreach (Slot s in receivers) {
			s.OnContentEmpty.AddListener(() => CheckReceivers());
		}
	}		

	public void OnPointerUp(PointerEventData data) {
		pressed = false;
	}

	public void OnPointerDown(PointerEventData data) {
		pressed = true;
		if (state == State.BlockReady) {
			OnMoveBegin.Invoke (this);
		}
	}
		
	public void OnPointerExit(PointerEventData data) {
		if (pressed && state == State.BlockReady) {
			Vector2 pointerMovement = data.position - data.pressPosition;
			if (Mathf.Abs (pointerMovement.x) > Mathf.Abs (pointerMovement.y)) {
				if (right && pointerMovement.x > 0) {
					OnMoveEnd.Invoke (right);
				} else if (left){
					OnMoveEnd.Invoke (left);
				}
			} else {
				if (up && pointerMovement.y > 0) {
					OnMoveEnd.Invoke (up);
				} else if (down) {
					OnMoveEnd.Invoke (down);
				}
			}
			pressed = false;
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
