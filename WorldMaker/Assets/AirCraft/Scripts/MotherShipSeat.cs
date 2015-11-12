using UnityEngine;
using System.Collections;

public class MotherShipSeat : MonoBehaviour {

	public MotherShip targetMotherShip;
	public MotherShip TargetMotherShip {
		get {
			if (this.targetMotherShip == null) {
				this.targetMotherShip = this.GetComponentInParent<MotherShip> ();
			}
			return targetMotherShip;
		}
	}

	public void TakeControl (Player p) {
		p.playerMode = Player.PlayerState.PilotMotherShip;
		p.shipSeat = this;
		p.transform.position = this.transform.position ;
		if (TargetMotherShip.pilotMode == MotherShip.PilotState.NoPilot) {
			TargetMotherShip.SwitchModeTo (MotherShip.PilotState.Pilot);
			TargetMotherShip.playerInput = true;
		}
	}

	public void DropControl (Player p) {
		p.playerMode = Player.PlayerState.Move;
		p.shipSeat = null;
		TargetMotherShip.SwitchModeTo (MotherShip.PilotState.NoPilot);
		TargetMotherShip.playerInput = false;
	}
}
