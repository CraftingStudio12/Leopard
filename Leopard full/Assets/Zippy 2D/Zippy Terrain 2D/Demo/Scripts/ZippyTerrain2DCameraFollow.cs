using UnityEngine;
using System.Collections;

public class ZippyTerrain2DCameraFollow : MonoBehaviour {
	public Transform target;
	public bool followAxisY;

	void Update () {	
		if(followAxisY) transform.position = new Vector3(target.position.x, target.position.y, transform.position.z);
		else
		transform.position = new Vector3(target.position.x, transform.position.y, transform.position.z);
	}
}
