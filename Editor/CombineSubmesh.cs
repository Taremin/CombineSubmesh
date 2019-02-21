namespace CombineSubmesh {
	using System.Collections.Generic;
	using System.Collections;
	using System.IO;
	using UnityEditor;
	using UnityEngine;

	public class CombineSubmeshWindow : EditorWindow {
		static GameObject activeObject;

		void OnEnable () { }

		void OnSelectionChange () {
			var editorEvent = EditorGUIUtility.CommandEvent ("ChangeActiveObject");
			editorEvent.type = EventType.Used;
			SendEvent (editorEvent);
		}

		// Use this for initialization
		[MenuItem ("GameObject/CombineSubmesh", false, 20)]
		public static void ShowWindow () {
			activeObject = Selection.activeGameObject;
			EditorWindow.GetWindow (typeof (CombineSubmeshWindow));
		}

		static void CombineSubmesh (GameObject go) {
			Component[] comps = go.GetComponentsInChildren<Component> ();
			foreach (Component comp in comps) {
				if (comp is SkinnedMeshRenderer) {
					OptimizeSkinnedMeshRenderer (comp as SkinnedMeshRenderer);
				}
			}
		}

		static Transform[] GetChildren (GameObject go) {
			int count = go.transform.childCount;
			var children = new Transform[count];

			for (int i = 0; i < count; ++i) {
				children[i] = go.transform.GetChild (i);
			}

			return children;
		}

		static void OptimizeSkinnedMeshRenderer (SkinnedMeshRenderer smr) {
			var materialToNewIndexTable = new Dictionary<Material, int> ();
			var newMaterialList = new List<Material> ();
			var newTriangles = new List<List<int>> ();
			int uniqueCount = 0;
			var mesh = smr.sharedMesh;
			var newMesh = Instantiate (mesh);
			var indexToNewIndexTable = new Dictionary<int, int> ();

			// prepare
			for (int i = 0, il = mesh.subMeshCount; i < il; ++i) {
				Material mat = smr.sharedMaterials[i];
				if (!materialToNewIndexTable.ContainsKey (mat)) {
					materialToNewIndexTable[mat] = uniqueCount++;
					newTriangles.Add (new List<int> ());
					newMaterialList.Add (mat);
				}
				indexToNewIndexTable[i] = materialToNewIndexTable[mat];
			}

			// combine submesh
			for (int i = 0, il = newMesh.subMeshCount; i < il; ++i) {
				var submesh = newMesh.GetTriangles (i);
				var mat = smr.sharedMaterials[i];
				newTriangles[indexToNewIndexTable[i]].AddRange (submesh);
			}

			// set submesh
			for (int i = 0; i < uniqueCount; ++i) {
				newMesh.SetTriangles (newTriangles[i], i);
			}
			newMesh.subMeshCount = uniqueCount;

			// save new mesh
			var path = AssetDatabase.GetAssetPath (mesh);
			AssetDatabase.CreateAsset (
				newMesh,
				Path.GetDirectoryName (path) + "/" +
				Path.GetFileName (path) + ".OptimizedMesh"
			);
			smr.sharedMaterials = newMaterialList.ToArray ();
			smr.sharedMesh = newMesh;
		}

		private void OnGUI () {
			activeObject = Selection.activeGameObject;

			EditorGUILayout.LabelField ("アクティブなオブジェクト");
			using (new GUILayout.VerticalScope (GUI.skin.box)) {
				EditorGUILayout.LabelField (activeObject ? activeObject.name : "");
			}

			if (!activeObject || !activeObject.activeInHierarchy) {
				return;
			}

			if (GUILayout.Button ("Combine Submesh")) {
				CombineSubmesh (activeObject);
			}
		}
	}
}