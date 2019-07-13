using UnityEngine;

public class ZippyTerrain2DEndless : MonoBehaviour {
	[Header("Endless Settings")]
	public Transform groundContainer;
	public ZippyTerrain2D startGround;
	public Transform follow;
	ZippyTerrain2D leftGround;
	ZippyTerrain2D middleGround;
	ZippyTerrain2D rightGround;
	[Header("Parallax Scrolling")]
	public bool parallaxScrolling;
	public float xSpeed = 1f;
	public float ySpeed = 1f;
	public float yOffset;


	void Start () {
		GetParent();
		if (CheckForErrors()) {
			this.enabled = false;
			return;
		}
		if (!startGround) middleGround = GetRandomGround();
		else middleGround = startGround;
		middleGround.transform.parent = transform;
		middleGround.cacheTransform.position = transform.position;
		leftGround = GetRandomGround();
		leftGround.transform.position = new Vector2(middleGround.cacheTransform.position.x - leftGround.width, transform.position.y);
		rightGround = GetRandomGround();
		rightGround.transform.position = new Vector2(middleGround.cacheTransform.position.x + middleGround.width, transform.position.y);
		DisableAll();
	}

	bool CheckForErrors() {
		bool b = false;
		if(groundContainer.childCount < 3) {
			Debug.LogError(this + " Not enough terrains in container to generate endless terrain.");
			b = true;
		}
		return b;
	}

	ZippyTerrain2D GetRandomGround() {
		ZippyTerrain2D r = groundContainer.GetChild(Random.Range(0, groundContainer.childCount)).GetComponent<ZippyTerrain2D>(); ;
		r.transform.parent = transform;
		r.gameObject.SetActive(true);
		return r;
	}

	void GetParent() {
		ZippyTerrain2D[] t = groundContainer.GetComponentsInChildren<ZippyTerrain2D>();
		for (int i = 0; i < t.Length; i++) {
			t[i].parent = t[i].transform.parent;
		}
	}

	void DisableAll() {
		ZippyTerrain2D[] t = groundContainer.GetComponentsInChildren<ZippyTerrain2D>();
		for(int i=0; i < t.Length; i++) {
			t[i].gameObject.SetActive(false);
		}
	}

	void Update () {
		if(follow.position.x >= rightGround.cacheTransform.position.x) {
			// Left ground is no longer needed, it is will be replaced by the middle ground.
			// Change back to the original parent. (GroundContainer)
			leftGround.cacheTransform.parent = leftGround.parent;
			// Disable it so it doesn't show up if the player moves left.
			leftGround.gameObject.SetActive(false);
			// Rearrange terrains to the proper variables
			leftGround = middleGround;
			middleGround = rightGround;
			// Get a random ground from the GroundContainer	to place at the right side.	
			rightGround = GetRandomGround();
			// Positions the new right terrain piece next to the middle terrain.
			rightGround.transform.position = new Vector2(middleGround.cacheTransform.position.x + middleGround.width, transform.position.y);
		}else if (follow.position.x <= middleGround.cacheTransform.position.x) {
			// Pretty much the same as above.
			rightGround.cacheTransform.parent = rightGround.parent;
			rightGround.gameObject.SetActive(false);
			rightGround = middleGround;
			middleGround = leftGround;
			leftGround = GetRandomGround();
			leftGround.transform.position = new Vector2(middleGround.cacheTransform.position.x - middleGround.width, transform.position.y);
		}
		if(!parallaxScrolling) return;
		transform.position = new Vector2(follow.transform.position.x * xSpeed, follow.transform.position.y * ySpeed - yOffset);
	}
}

