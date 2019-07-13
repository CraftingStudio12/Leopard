using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]

public class ZippyTerrain2D : MonoBehaviour {
	[Header("Appearance settings")]
	[Tooltip("Curve used to generate terrain.")]
	public AnimationCurve terrainCurve;
	[Tooltip("Repeat the curve.")]
	public int curveRepeat = 1;
	[Tooltip("Top mesh vertex color.")]
	public Color topColor = Color.white;
	[Tooltip("Bottom mesh vertex color.")]
	public Color bottomColor = Color.white;
	[Tooltip("Resolution multiplier.")]
	[Range(1, 15)]
	public int resolution = 5;
	
	[Header("Size settings")]
	[Tooltip("Terrain width.")]
	public float width = 10f;
	[Tooltip("Terrain height.")]
	public float height = 10f;

	[Header("UV & Material Settings")]
	[Tooltip("Disable distortion on texture.")]
	public bool staticTexture;
	[Tooltip("Stretch UV horizontally.")]
	public float UVScaleX = 1f;
	[Tooltip("Stretch UV vertically.")]
	public float UVScaleY = 1f;
	[Tooltip("Offset UV horizontally.")]
	public float UVOffsetX = 0f;
	[Tooltip("Offset UV Vertically.")]
	public float UVOffsetY = 0f;


	[Header("Sprite Sorting")]
	[Tooltip("2D sorting order of the terrain.")]
	public int sortingOrder = 0;
	[Tooltip("2D sorting layer of the terrain.")]
	[UnluckSoftware.SortingLayer]
	public int sortingLayer;

	[Header("Collider Type")]
	[Tooltip("2D Collider component of the terrain.")]
	public ColliderEnum colliderType;
	public enum ColliderEnum {
		None,
		Edge,
		Polygon
	};

#if UNITY_EDITOR
	public float handleScale = 1f;
	[HideInInspector] public float smoothValue = 0f;
	[HideInInspector] public bool smoothAll = true;
#endif
	[HideInInspector] public bool updateCollider = true;

	//Component references
	[HideInInspector]	public Transform cacheTransform;
	[HideInInspector]	public MeshFilter cacheMeshFilter;
	[HideInInspector]	public MeshRenderer cacheMeshRenderer;
	[HideInInspector]	public BoxCollider2D cacheCollider;

	//Arrays
	[HideInInspector]	public Vector2[] points;
	[HideInInspector]	public float[] posX;
	[HideInInspector]	public float[] posY;
	[HideInInspector]	public int colliderAmount;
	[HideInInspector]	public int pointAmount;
	[HideInInspector]	public Mesh groundMesh;
	[HideInInspector]	public Transform parent;
	[HideInInspector]   public int selectedPoint = -1;

	void CacheComponents() {
		if (this == null) return;
		if (!cacheMeshFilter) cacheMeshFilter = GetComponent<MeshFilter>();
		if (!cacheTransform) cacheTransform = transform;
		if (!cacheMeshRenderer) cacheMeshRenderer = GetComponent<MeshRenderer>();
	}

	void PositionChildren() {
		ZippyTerrain2DChild[] c = transform.GetComponentsInChildren<ZippyTerrain2DChild>();
		for(int i = 0; i < c.Length; i++) {
			c[i].PositionOnTerrain();
		}
	}

	public void Init() {
		colliderAmount = Mathf.RoundToInt(width) * resolution;
		pointAmount = colliderAmount + 1;
		posX = new float[pointAmount];
		posY = new float[pointAmount];
		points = new Vector2[pointAmount * 2];
		for (int i = 0; i < pointAmount; i++) {
			posY[i] = cacheTransform.position.y;
			posX[i] = cacheTransform.position.x + width * i / colliderAmount;
		}
		groundMesh = new Mesh();
	}

	public void CreateGround () {
		CacheComponents();
		Init();
		SpriteSorting();
		Wave();
		UpdateTerrain();
		GenerateMesh();
		PositionChildren();
#if UNITY_EDITOR
		EditorUtility.SetDirty(this);
#endif
	}

	void GenerateMeshColors() {
		List<Color> colors = new List<Color>();
		for (var i = 0; i < points.Length * .5f; i++) {
			colors.Add(bottomColor);
			colors.Add(topColor);
		}
		groundMesh.colors = colors.ToArray();
	}

	void GenerateMesh() {
		List<Vector3> verts = new List<Vector3>();
		for (var i = points.Length -1; i >= 0; i--) verts.Add(cacheTransform.InverseTransformPoint(points[i]));
		if(groundMesh.vertices.Length != verts.Count) groundMesh = new Mesh();
		groundMesh.vertices = verts.ToArray();
		if(groundMesh.colors.Length == 0) { 
			GenerateMeshColors();
			List<int> tris = new List<int>();
				for (var i = 0; i < verts.Count - 2; i++) {
					if (i % 2 == 0) {
						tris.Add(i + 2);
						tris.Add(i + 1);
						tris.Add(i);
					} else {
						tris.Add(i + 2);
						tris.Add(i);
						tris.Add(i + 1);
					}
				}		
			groundMesh.triangles = tris.ToArray();
			cacheMeshFilter.mesh = groundMesh;
		}
		groundMesh.RecalculateBounds();
		Vector2[] u = new Vector2[verts.Count];
		if (staticTexture) { 
			for (int i = 0; i < u.Length; i++) {
				u[i] = new Vector2(verts[i].x * UVScaleX +UVOffsetX, verts[i].y * UVScaleY +UVOffsetY) *.05f;
			}
		} else {
			int ui =0;
			for (int i = 0; i < verts.Count; i++) {
				if (i % 2 == 1) {
					u[i] = new Vector2(verts[i].x / width * UVScaleX + UVOffsetX, UVScaleY + UVOffsetY);
					ui++;
				} else {
					u[i] = new Vector2(verts[i].x / width * UVScaleX + UVOffsetX,  0 + UVOffsetY);
				}
			}
		}
		groundMesh.uv = u;
	}

	public void UpdateTerrain() {
		for (int i = 0; i < pointAmount; i++) posX[i] = cacheTransform.position.x + width * i / colliderAmount;
		int ii = 0;
		for (int i = 0; i < pointAmount; i++) {
			points[i + ii] = new Vector3(posX[i], posY[i] + height + cacheTransform.position.y);
			points[i + ii + 1] = new Vector3(posX[i], cacheTransform.position.y);
			ii++;
		}
		EdgeCollider2D e = GetComponent<EdgeCollider2D>();
		PolygonCollider2D p = GetComponent<PolygonCollider2D>();
		
		if (p) {
			p.points = null;
			p.enabled = false;
		}
		if (e) {
			e.points = new Vector2[2];
			e.enabled = false;
		}
		if (!updateCollider) {
			return;
		}

		if (colliderType == ColliderEnum.Edge) {
			if (e == null) e = gameObject.AddComponent<EdgeCollider2D>();
			e.enabled = true;
			List<Vector2> foo = new List<Vector2>();
			float prevY = 100000000f;
			for (int i = 0; i < posX.Length; i++) {
				if (Mathf.Clamp(posY[i], prevY - .01f, prevY + .01f) != posY[i] || i == posX.Length - 1) {
					foo.Add(new Vector2(posX[i] - cacheTransform.position.x, posY[i] + height));
					prevY = posY[i];
				}
			}
			foo.Add(new Vector2(posX[posX.Length-1] - cacheTransform.position.x, 0));
			foo.Add(new Vector2(0, 0));
			foo.Add(new Vector2(0, posY[0] + height));
			e.points = foo.ToArray();
		}

		if (colliderType == ColliderEnum.Polygon) {
			if (p == null) p = gameObject.AddComponent<PolygonCollider2D>();
			p.enabled = true;
			Vector2[] foo = new Vector2[posX.Length +2];
			foo[0] = new Vector2(0f, 0f);		
			for (int i = 0; i < posX.Length; i++) {
					foo[i + 1] = new Vector2(posX[i] - cacheTransform.position.x, posY[i] + height);
			}
			foo[foo.Length - 1] = new Vector2(width, 0);
			p.SetPath(0, foo);
		}
	}

	void Wave() {
		float f =  resolution * width /curveRepeat;
		for (int i = 0; i < pointAmount; i++) {
			float n = i/ f ;
			posY[i] = terrainCurve.Evaluate(n);
		}
	}

	void SpriteSorting() {
		cacheMeshRenderer.sortingOrder = sortingOrder;
		cacheMeshRenderer.sortingLayerID = sortingLayer;
	}
}

#if UNITY_EDITOR
[CustomEditor(typeof(ZippyTerrain2D))]
[CanEditMultipleObjects]
public class ZippyTerrain2DEditor : Editor {
	ZippyTerrain2D tar;
	bool undoRegistered;
	bool shiftDown;
	bool mouseDrag;
	bool mouseDown;
	bool mouseUp;
	bool ctrlDown;

	public override void OnInspectorGUI() {
		DrawDefaultInspector();
		tar.height = Mathf.Clamp(tar.height, 0, Mathf.Infinity);
		tar.width = Mathf.Clamp(tar.width, 5, Mathf.Infinity);
		if (GUI.changed) {
			tar.CreateGround();
		}
		GUILayout.Space(10);
		tar.smoothAll = EditorGUILayout.Toggle("Smooth Alwayes", tar.smoothAll);
		tar.smoothValue = EditorGUILayout.FloatField("Smooth Value", tar.smoothValue);
		if (GUILayout.Button("Smooth Curves")) {
			SmoothCurves();
		}

		if (GUILayout.Button("Update All Zippy Grounds")) {
			ZippyTerrain2D[] z = FindObjectsOfType<ZippyTerrain2D>();
			for(int i = 0; i < z.Length; i++) {
				z[i].CreateGround();
			}
		}
		GUILayout.Space(10);
		EditorGUILayout.HelpBox("Shift key to delete point\nControl key to smooth point", MessageType.Info);

		EditorUtility.SetSelectedWireframeHidden((target as ZippyTerrain2D).cacheMeshRenderer, true);
	}


	private void OnEnable() {
		tar = (target as ZippyTerrain2D);
		Undo.undoRedoPerformed += UndoRedoPerformed;
		tar.Init();	
		tar.CreateGround();
	}

	private void OnDisable() {
		Undo.undoRedoPerformed -= UndoRedoPerformed;
	}

	private void UndoRedoPerformed() {
		tar.CreateGround();
	}

	private void SmoothCurves() {
		Undo.RegisterCompleteObjectUndo(tar, "Zippy Smooth Curves");
		for (int i = 0; i < tar.terrainCurve.length; i++) {
			tar.terrainCurve.SmoothTangents(i, tar.smoothValue);
			tar.CreateGround();
		}
	}

	private void OnSceneGUI() {
		Event guiEvent = Event.current;
		mouseUp = guiEvent.type == EventType.MouseUp;
		shiftDown = guiEvent.modifiers == EventModifiers.Shift;
		mouseDrag = guiEvent.type == EventType.MouseDrag;
		ctrlDown = guiEvent.modifiers == EventModifiers.Control;

		if (mouseUp) {
			undoRegistered = false;
			tar.updateCollider = true;
			tar.UpdateTerrain();
		}
				
		float xx = tar.transform.position.x;
		float yy = tar.transform.position.y;

		if (tar.terrainCurve == null) {
			tar.terrainCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
			tar.CreateGround();
		}
		

		for (int i = 0; i < tar.terrainCurve.length; i++) {

			if(mouseDrag && !undoRegistered) {
			//	Debug.Log("undoRegistered");
				undoRegistered = true;
				Undo.RegisterCompleteObjectUndo(tar, "Zippy Drag Point");
				tar.updateCollider = false;
			}

			Vector2 v = new Vector2(tar.terrainCurve[i].time * tar.width +xx, tar.terrainCurve[i].value + tar.height+yy);
			Vector2 mPos = Handles.FreeMoveHandle(v, Quaternion.identity, .5f * tar.handleScale, Vector3.zero, Handles.CircleCap);
			if(i > 0 && i < tar.terrainCurve.length-1) {
				Handles.color = Color.red;
				if (shiftDown && Handles.Button(v, Quaternion.identity, .8f * tar.handleScale, 1 * tar.handleScale, Handles.SphereCap)) {
					Undo.RegisterCompleteObjectUndo(tar, "Zippy Delete Key");
					tar.terrainCurve.RemoveKey(i);
					tar.terrainCurve.SmoothTangents(i, 0f);
					tar.CreateGround();
					return;
				}
				Handles.color = Color.white;
			}

			Handles.color = Color.cyan;
			if (ctrlDown && Handles.Button(v, Quaternion.identity, .8f * tar.handleScale, 1 * tar.handleScale, Handles.SphereCap)) {
				Undo.RegisterCompleteObjectUndo(tar, "Zippy Smooth Key");
				tar.terrainCurve.SmoothTangents(i, 0f);
				tar.CreateGround();
				return;
			}

			Handles.color = Color.white;
			Keyframe newKey = new Keyframe();
			bool changed =false;
			Vector2 x = new Vector2(Mathf.Clamp01((mPos.x -xx) / tar.width ), Mathf.Clamp(mPos.y - tar.height -yy, 0, Mathf.Infinity));
			if (new Vector2(tar.terrainCurve[i].time, tar.terrainCurve[i].value) != new Vector2(x.x, x.y)) {
				newKey.time = x.x;
				newKey.value = x.y;
				if (i != 0 && i != tar.terrainCurve.length - 1) {
					if (x.x >= tar.terrainCurve[i + 1].time) return;
					if (x.x <= tar.terrainCurve[i - 1].time) return;
				}
				changed = true;
			}
			if (changed) {
				tar.terrainCurve.RemoveKey(i);
				tar.terrainCurve.AddKey(newKey);
				tar.terrainCurve.SmoothTangents(i, 0f);
				if (tar.smoothAll) {
					if(i>0) tar.terrainCurve.SmoothTangents(i - 1, 0f);
					if(i<tar.terrainCurve.length -1) tar.terrainCurve.SmoothTangents(i + 1, 0f);
				}
				tar.CreateGround();
			}
			if (i < tar.terrainCurve.length - 1) {
				Vector2 v2 = new Vector2(tar.terrainCurve[i].time * tar.width +xx, tar.terrainCurve[i].value + tar.height+yy);
				Vector2 vv2 = new Vector2(tar.terrainCurve[i + 1].time * tar.width +xx, tar.terrainCurve[i + 1].value + tar.height+yy);
				Vector2 vvv2 = Vector2.Lerp(v2, vv2, .5f);
				Handles.color = new Color(1, 1, 1, .25f);
				Handles.DrawLine(new Vector2(tar.terrainCurve[i].time * tar.width +xx, tar.terrainCurve[i].value + tar.height +yy), new Vector2(tar.terrainCurve[i + 1].time * tar.width +xx, tar.terrainCurve[i + 1].value + tar.height+yy));
				Handles.color = Color.white;
				if (Handles.Button(vvv2, Quaternion.identity, .4f * tar.handleScale, .4f * tar.handleScale, Handles.CubeCap)) {
					Undo.RegisterCompleteObjectUndo(tar, "Zippy Create Key");
					tar.terrainCurve.AddKey(new Keyframe((vvv2.x - xx) / tar.width , vvv2.y - tar.height-yy));
					tar.terrainCurve.SmoothTangents(i, 0f);
					if (tar.smoothAll) {
						if (i > 0) tar.terrainCurve.SmoothTangents(i - 1, 0f);
						if (i < tar.terrainCurve.length - 1) tar.terrainCurve.SmoothTangents(i + 1, 0f);
					}
					tar.CreateGround();
				}
			}
		}
		
	}
}
#endif