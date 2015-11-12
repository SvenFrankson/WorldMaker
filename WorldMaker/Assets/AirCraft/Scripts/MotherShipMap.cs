using UnityEngine;
using System.Collections;

public class MotherShipMap : MonoBehaviour {


	public MotherShip targetMotherShip;
	private MotherShip TargetMotherShip {
		get {
			if (this.targetMotherShip == null) {
				this.targetMotherShip = this.GetComponentInParent<MotherShip> ();
			}

			return targetMotherShip;
		}
	}

	public float radius;
	public float height;

	public GameObject planetIconPrefab;
	public GameObject selectedPlanetIconPrefab;
	public GameObject shipIconPrefab;

	private GameObject[] planetsIcons;
	private GameObject shipIcon;
	private GameObject selectedPlanetIcon;

	public void Start () {
		this.InstantiateIcons ();
	}

	public void Update () {
		this.shipIcon.transform.localPosition = Vector3.up * height + TargetMotherShip.TruePos.TruePos * this.radius / 1000000f;
		this.shipIcon.transform.localRotation = TargetMotherShip.transform.rotation;
		this.selectedPlanetIcon.transform.localPosition = Vector3.up * height + TargetMotherShip.SelectedPlanet.TruePos.TruePos * this.radius / 1000000f;
	}

	public void InstantiateIcons () {
		this.planetsIcons = new GameObject[this.TargetMotherShip.Planets.Count];

		for (int i = 0; i < this.planetsIcons.Length; i++) {
			Planet p = this.TargetMotherShip.Planets [i].Key;

			this.planetsIcons [i] = GameObject.Instantiate<GameObject> (this.planetIconPrefab);
			this.planetsIcons [i].transform.parent = this.transform;
			this.planetsIcons [i].transform.localPosition = Vector3.up * height + p.TruePos.TruePos * this.radius / 1000000f;
		}

		this.shipIcon = GameObject.Instantiate<GameObject> (this.shipIconPrefab);
		this.shipIcon.transform.parent = this.transform;
		this.shipIcon.transform.localPosition = Vector3.up * height + TargetMotherShip.TruePos.TruePos * this.radius / 1000000f;
		this.shipIcon.transform.localRotation = TargetMotherShip.transform.rotation;
		
		this.selectedPlanetIcon = GameObject.Instantiate<GameObject> (this.selectedPlanetIconPrefab);
		this.selectedPlanetIcon.transform.parent = this.transform;
		this.selectedPlanetIcon.transform.localPosition = Vector3.up * height + TargetMotherShip.SelectedPlanet.TruePos.TruePos * this.radius / 1000000f;
	}
}
