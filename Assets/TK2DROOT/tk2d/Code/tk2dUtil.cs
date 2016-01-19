using UnityEngine;
using System;
using System.Collections;

public static class tk2dUtil 
{
	// The name of all subsequent undos
#pragma warning disable 414
	static string label = "";
#pragma warning restore 414

	// This can get stuck if an exception is trigerred before it reset
	// BeginGroup will reset this flag for this reason
	static bool undoEnabled = false;
	static public bool UndoEnabled {
		get {
			return undoEnabled;
		}
		set {
			undoEnabled = value;
		}
	}
	
	public static void BeginGroup(string name) {
		undoEnabled = true;
		label = name;
	}

	public static void EndGroup() {
		label = "";
	}

	public static void DestroyImmediate( UnityEngine.Object obj ) {
		if (obj == null) {
			return;
		}

#if UNITY_EDITOR && !(UNITY_3_5 || UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2)
		if (!Application.isPlaying && undoEnabled) {
			UnityEditor.Undo.DestroyObjectImmediate(obj);
		}
		else
#endif
		{
			UnityEngine.Object.DestroyImmediate(obj);
		}
	}

	public static GameObject CreateGameObject(string name) {
		GameObject go = new GameObject(name);
#if UNITY_EDITOR && !(UNITY_3_5 || UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2)
		if (!Application.isPlaying && undoEnabled) {
			UnityEditor.Undo.RegisterCreatedObjectUndo(go, label);
		}
#endif
		return go;
	}

	public static Mesh CreateMesh() {
		Mesh mesh = new Mesh();
#if UNITY_EDITOR && !(UNITY_3_5 || UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2)
		if (!Application.isPlaying && undoEnabled) {
			UnityEditor.Undo.RegisterCreatedObjectUndo(mesh, label);
		}
#endif
		return mesh;
	}

	public static T AddComponent<T>(GameObject go) where T : UnityEngine.Component {
		T t = go.AddComponent<T>();
#if UNITY_EDITOR && !(UNITY_3_5 || UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2)
		if (!Application.isPlaying && undoEnabled) {
			UnityEditor.Undo.RegisterCreatedObjectUndo(t, label);
		}
#endif
		return t;
	}

#if !UNITY_3_5
	public static void SetActive( GameObject go, bool active ) {
		if (active == go.activeSelf) {
			return;
		}

#if UNITY_EDITOR && !(UNITY_3_5 || UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2)
		if (!Application.isPlaying && undoEnabled) {
			UnityEditor.Undo.RegisterCompleteObjectUndo(go, label);
		}
#endif
		go.SetActive(active);
	}
#endif

	public static void SetTransformParent(Transform t, Transform parent) {
#if UNITY_EDITOR && !(UNITY_3_5 || UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2)
		if (!Application.isPlaying && undoEnabled) {
			UnityEditor.Undo.SetTransformParent(t, parent, label);
		}
		else
#endif
		{
			t.parent = parent;
		}
	}

    public static tk2dSpriteCollectionData GetCollection(string spriteCollectionName)
    {
        return tk2dSystem.LoadResourceByName<tk2dSpriteCollectionData>(spriteCollectionName);
    }

    public static void SetCollectionAndSprite(tk2dSprite sprite, string spriteCollectionName, string spriteName)
    {
        tk2dSpriteCollectionData spriteCollection = tk2dUtil.GetCollection(spriteCollectionName);
        if (spriteCollection != null)
        {
            sprite.SetSprite(spriteCollection, spriteName);
        }
        else
        {
           // PSLog.LogError("Unable to find SpriteCollection with name["+spriteCollectionName+"]");
        }
    }
}

