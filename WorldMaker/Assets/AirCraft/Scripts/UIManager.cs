using UnityEngine;
using System.Collections;

public class UIManager : MonoBehaviour {
	
	public GUISkin skin;

	public void OnGUI () {
		GUI.skin = skin;

		GUI.TextArea (Rect.MinMaxRect (Screen.width - 310, 10, Screen.width - 10, 70), "TimeScale = " + Time.timeScale + "\n(Pav8) : Speed Up TimeScale\n(Pav2) : Slow Down TimeScale");
	}  
}
