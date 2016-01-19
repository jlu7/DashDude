/*using UnityEngine;
using System.Collections.Generic;
using System.Linq;


[AddComponentMenu("2D Toolkit/Sprite/tk2dRuntimeSpriteBatcher")]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
[ExecuteInEditMode]
/// <summary>
/// Static sprite batcher builds a collection of sprites, textmeshes into one
/// static mesh for better performance.
/// </summary>
public class tk2dRuntimeSpriteBatcher : MonoBehaviour, tk2dRuntime.ISpriteCollectionForceBuild
{
	public static int CURRENT_VERSION = 3;
	
	public int version = 0;
	/// <summary>
	/// The list of batched sprites. Fill this and call Build to build your mesh.
	/// </summary>
	public tk2dBatchedSprite[] batchedSprites = null;
	public tk2dTextMeshData[] allTextMeshData = null;
	public tk2dSpriteCollectionData spriteCollection = null;

	// Flags
	public enum Flags {
		None = 0,
		GenerateCollider = 1,
		FlattenDepth = 2,
		SortToCamera = 4,
	}
	
	// default to keep backwards compatibility
	[SerializeField]
	Flags flags = Flags.GenerateCollider;

	public bool CheckFlag(Flags mask) { return (flags & mask) != Flags.None; }
	public void SetFlag(Flags mask, bool value) { 
		if (CheckFlag(mask) != value) {
			if (value) {
				flags |= mask; 
			}
			else {
				flags &= ~mask; 
			}
			Build();
		}
	}

	Mesh mesh = null;
	Mesh colliderMesh = null;
	
	[SerializeField] Vector3 _scale = new Vector3(1.0f, 1.0f, 1.0f);
	
#if UNITY_EDITOR
	// This is not exposed to game, as the cost of rebuilding this data is very high
	public Vector3 scale
	{
		get { UpgradeData(); return _scale; }
		set
		{
			bool needBuild = _scale != value;
			_scale = value;
			if (needBuild)
				Build();
		}
	}
#endif
	
	void Awake()
	{
		Build();
	}
	
	// Sanitize data, returns true if needs rebuild
	bool UpgradeData()
	{
		if (version == CURRENT_VERSION) {
			return false;
		}
		
		if (_scale == Vector3.zero) {
			_scale = Vector3.one;
		}
		
		if (version < 2)
		{
			if (batchedSprites != null)
			{
				// Parented to this object
				foreach (var sprite in batchedSprites)
					sprite.parentId = -1;
			}
		}

		if (version < 3)
		{
			if (batchedSprites != null)
			{
				foreach (var sprite in batchedSprites)
				{
					if (sprite.spriteId == -1)
					{
						sprite.type = tk2dBatchedSprite.Type.EmptyGameObject;
					}
					else {
						sprite.type = tk2dBatchedSprite.Type.Sprite;
						if (sprite.spriteCollection == null) { 
							sprite.spriteCollection = spriteCollection;
						}
					}
				}

				UpdateMatrices();
			}

			spriteCollection = null;
		}
		
		version = CURRENT_VERSION;

#if UNITY_EDITOR
		UnityEditor.EditorUtility.SetDirty(this);
#endif
		
		return true;
	}
	
	protected void OnDestroy()
	{
		if (mesh)
		{
#if UNITY_EDITOR
			DestroyImmediate(mesh);
#else
			Destroy(mesh);
#endif
		}
		
		if (colliderMesh)
		{
#if UNITY_EDITOR
			DestroyImmediate(colliderMesh);
#else
			Destroy(colliderMesh);
#endif
		}
	}

	/// <summary>
	/// Update matrices, if the sprite batcher has been built using .position, etc.
	/// It is far more efficient to simply set the matrices when building at runtime
	/// so do that if possible.
	/// </summary>
	public void UpdateMatrices() {
		bool hasParentIds = false;
		foreach (var sprite in batchedSprites)
		{
			if (sprite.parentId != -1) {
				hasParentIds = true;
				break;
			}
		}

		if (hasParentIds) {
			// Reconstruct matrices from TRS, respecting hierarchy
			Matrix4x4 tmpMatrix = new Matrix4x4();
			List<tk2dBatchedSprite> parentSortedSprites = new List<tk2dBatchedSprite>( batchedSprites );
			parentSortedSprites.Sort((a, b) => a.parentId.CompareTo(b.parentId) );
			foreach (tk2dBatchedSprite sprite in parentSortedSprites) {
				tmpMatrix.SetTRS( sprite.position, sprite.rotation, sprite.localScale );
				sprite.relativeMatrix = ((sprite.parentId == -1) ? Matrix4x4.identity : batchedSprites[ sprite.parentId ].relativeMatrix) * tmpMatrix;
			}
		}
		else {
			foreach (tk2dBatchedSprite sprite in batchedSprites) {
				sprite.relativeMatrix.SetTRS( sprite.position, sprite.rotation, sprite.localScale );
			}
		}		
	}

	/// <summary>
	/// Build a static sprite batcher's geometry and collider
	/// </summary>
	public void Build()
	{
		UpgradeData();

		if (mesh == null)
		{
			mesh = new Mesh();
			mesh.hideFlags = HideFlags.DontSave;
			GetComponent<MeshFilter>().mesh = mesh;
		}
		else
		{
			// this happens when the sprite rebuilds
			mesh.Clear();
		}
		
		if (colliderMesh)
		{
#if UNITY_EDITOR
			DestroyImmediate(colliderMesh);
#else
			Destroy(colliderMesh);
#endif
			colliderMesh = null;
		}
		
		if (batchedSprites == null || batchedSprites.Length == 0)
		{
		}
		else
		{
			SortBatchedSprites();
			BuildRenderMesh();
			BuildPhysicsMesh();
		}
	}
	
	void SortBatchedSprites()
	{
		List<tk2dBatchedSprite> solidBatches = new List<tk2dBatchedSprite>();
		List<tk2dBatchedSprite> otherBatches = new List<tk2dBatchedSprite>();
		List<tk2dBatchedSprite> undrawnBatches = new List<tk2dBatchedSprite>();
		foreach (tk2dBatchedSprite batchedSprite in batchedSprites)
		{
			if (!batchedSprite.IsDrawn)
			{
				undrawnBatches.Add(batchedSprite);
				continue;				
			}

			Material material = GetMaterial(batchedSprite);
			if (material != null) {
				if (material.renderQueue == 2000) {
					solidBatches.Add(batchedSprite);
				}
				else {
					otherBatches.Add(batchedSprite);
				}
			}
			else
			{
				solidBatches.Add(batchedSprite);
			}
		}
		
		List<tk2dBatchedSprite> allBatches = new List<tk2dBatchedSprite>(solidBatches.Count + otherBatches.Count + undrawnBatches.Count);
		allBatches.AddRange(solidBatches);
		allBatches.AddRange(otherBatches);
		allBatches.AddRange(undrawnBatches);
		
		// Re-index parents
		Dictionary<tk2dBatchedSprite, int> lookup = new Dictionary<tk2dBatchedSprite, int>();
		int index = 0;
		foreach (var v in allBatches)
			lookup[v] = index++;
		
		foreach (var v in allBatches)
		{
			if (v.parentId == -1)
				continue;
			v.parentId = lookup[ batchedSprites[v.parentId] ];
		}
		
		batchedSprites = allBatches.ToArray();
	}

	Material GetMaterial(tk2dBatchedSprite bs) {
		switch (bs.type) {
			case tk2dBatchedSprite.Type.EmptyGameObject: return null;
			case tk2dBatchedSprite.Type.TextMesh: return allTextMeshData[bs.xRefId].font.materialInst;
			default: return bs.GetSpriteDefinition().materialInst;
		}
	}

	void BuildRenderMesh()
	{
		List<Material> materials = new List<Material>();
		List<List<int>> indices = new List<List<int>>();
		
		bool needNormals = false;
		bool needTangents = false;
		bool needUV2 = false;
		bool flattenDepth = CheckFlag(Flags.FlattenDepth);

		foreach (var bs in batchedSprites)
		{
			var spriteDef = bs.GetSpriteDefinition();
			if (spriteDef != null)
			{
				needNormals |= (spriteDef.normals != null && spriteDef.normals.Length > 0); 
				needTangents |= (spriteDef.tangents != null && spriteDef.tangents.Length > 0);
			}
			if (bs.type == tk2dBatchedSprite.Type.TextMesh)
			{
				tk2dTextMeshData textMeshData = allTextMeshData[bs.xRefId];
				if ((textMeshData.font != null) && textMeshData.font.inst.textureGradients)
				{
					needUV2 = true;
				}
			}
		}

		// just helpful to have these here, stop code being more messy
		List<int> bsNVerts = new List<int>();
		List<int> bsNInds = new List<int>();
		
		int numVertices = 0;
		foreach (var bs in batchedSprites) 
		{
			if (!bs.IsDrawn) // when the first non-drawn child is found, it signals the end of the drawn list
				break;

			var spriteDef = bs.GetSpriteDefinition();
			int nVerts = 0;
			int nInds = 0;
			switch (bs.type)
			{
			case tk2dBatchedSprite.Type.EmptyGameObject:
				break;
			case tk2dBatchedSprite.Type.Sprite:
				if (spriteDef != null) tk2dSpriteGeomGen.GetSpriteGeomDesc(out nVerts, out nInds, spriteDef);
				break;
			case tk2dBatchedSprite.Type.TiledSprite:
				if (spriteDef != null) tk2dSpriteGeomGen.GetTiledSpriteGeomDesc(out nVerts, out nInds, spriteDef, bs.Dimensions);
				break;
			case tk2dBatchedSprite.Type.SlicedSprite:
				if (spriteDef != null) tk2dSpriteGeomGen.GetSlicedSpriteGeomDesc(out nVerts, out nInds, spriteDef, bs.CheckFlag(tk2dBatchedSprite.Flags.SlicedSprite_BorderOnly));
				break;
			case tk2dBatchedSprite.Type.ClippedSprite:
				if (spriteDef != null) tk2dSpriteGeomGen.GetClippedSpriteGeomDesc(out nVerts, out nInds, spriteDef);
				break;
			case tk2dBatchedSprite.Type.TextMesh:
				{
					tk2dTextMeshData textMeshData = allTextMeshData[bs.xRefId];
					tk2dTextGeomGen.GetTextMeshGeomDesc(out nVerts, out nInds, tk2dTextGeomGen.Data(textMeshData, textMeshData.font.inst, bs.FormattedText));
					break;
				}
			}
			numVertices += nVerts;

			bsNVerts.Add(nVerts);
			bsNInds.Add(nInds);
		}
		
		Vector3[] meshNormals = needNormals?new Vector3[numVertices]:null;
		Vector4[] meshTangents = needTangents?new Vector4[numVertices]:null;
		Vector3[] meshVertices = new Vector3[numVertices];
		Color32[] meshColors = new Color32[numVertices];
		Vector2[] meshUvs = new Vector2[numVertices];
		Vector2[] meshUv2s = needUV2 ? new Vector2[numVertices] : null;
		
		int currVertex = 0;

		Material currentMaterial = null;
		List<int> currentIndices = null;

		Matrix4x4 scaleMatrix = Matrix4x4.identity;
		scaleMatrix.m00 = _scale.x;
		scaleMatrix.m11 = _scale.y;
		scaleMatrix.m22 = _scale.z;

		int bsIndex = 0;
		foreach (var bs in batchedSprites)
		{
			if (!bs.IsDrawn) // when the first non-drawn child is found, it signals the end of the drawn list
				break;

			if (bs.type == tk2dBatchedSprite.Type.EmptyGameObject)
			{
				++bsIndex; // watch out for this
				continue;
			}

			var spriteDef = bs.GetSpriteDefinition();
			int nVerts = bsNVerts[bsIndex];
			int nInds = bsNInds[bsIndex];

			Material material = GetMaterial(bs);

			// should have a material at this point
			if (material != currentMaterial)
			{
				if (currentMaterial != null)
				{
					materials.Add(currentMaterial);
					indices.Add(currentIndices);
				}
				
				currentMaterial = material;
				currentIndices = new List<int>();
			}

			Vector3[] posData = new Vector3[nVerts];
			Vector2[] uvData = new Vector2[nVerts];
			Vector2[] uv2Data = needUV2 ? new Vector2[nVerts] : null;
			Color32[] colorData = new Color32[nVerts];
			Vector3[] normalData = needNormals ? new Vector3[nVerts] : null;
			Vector4[] tangentData = needTangents ? new Vector4[nVerts] : null;
			int[] indData = new int[nInds];

			Vector3 boundsCenter = Vector3.zero;
			Vector3 boundsExtents = Vector3.zero;

			switch (bs.type)
			{
			case tk2dBatchedSprite.Type.EmptyGameObject:
				break;
			case tk2dBatchedSprite.Type.Sprite:
				if (spriteDef != null) {
					tk2dSpriteGeomGen.SetSpriteGeom(posData, uvData, normalData, tangentData, 0, spriteDef, Vector3.one);
					tk2dSpriteGeomGen.SetSpriteIndices(indData, 0, currVertex, spriteDef);
				}
				break;
			case tk2dBatchedSprite.Type.TiledSprite:
				if (spriteDef != null) {
					tk2dSpriteGeomGen.SetTiledSpriteGeom(posData, uvData, 0, out boundsCenter, out boundsExtents, spriteDef, Vector3.one, bs.Dimensions, bs.anchor, bs.BoxColliderOffsetZ, bs.BoxColliderExtentZ);
					tk2dSpriteGeomGen.SetTiledSpriteIndices(indData, 0, currVertex, spriteDef, bs.Dimensions);
				}
				break;
			case tk2dBatchedSprite.Type.SlicedSprite:
				if (spriteDef != null) {
					tk2dSpriteGeomGen.SetSlicedSpriteGeom(posData, uvData, 0, out boundsCenter, out boundsExtents, spriteDef, Vector3.one, bs.Dimensions, bs.SlicedSpriteBorderBottomLeft, bs.SlicedSpriteBorderTopRight, bs.anchor, bs.BoxColliderOffsetZ, bs.BoxColliderExtentZ);
					tk2dSpriteGeomGen.SetSlicedSpriteIndices(indData, 0, currVertex, spriteDef, bs.CheckFlag(tk2dBatchedSprite.Flags.SlicedSprite_BorderOnly));
				}
				break;
			case tk2dBatchedSprite.Type.ClippedSprite:
				if (spriteDef != null) {
					tk2dSpriteGeomGen.SetClippedSpriteGeom(posData, uvData, 0, out boundsCenter, out boundsExtents, spriteDef, Vector3.one, bs.ClippedSpriteRegionBottomLeft, bs.ClippedSpriteRegionTopRight, bs.BoxColliderOffsetZ, bs.BoxColliderExtentZ);
					tk2dSpriteGeomGen.SetClippedSpriteIndices(indData, 0, currVertex, spriteDef);
				}
				break;
			case tk2dBatchedSprite.Type.TextMesh:
				{
					tk2dTextMeshData textMeshData = allTextMeshData[bs.xRefId];
					var geomData = tk2dTextGeomGen.Data(textMeshData, textMeshData.font.inst, bs.FormattedText);
					int target = tk2dTextGeomGen.SetTextMeshGeom(posData, uvData, uv2Data, colorData, 0, geomData);
					if (!geomData.fontInst.isPacked) {
						Color32 topColor = textMeshData.color;
						Color32 bottomColor = textMeshData.useGradient ? textMeshData.color2 : textMeshData.color;
						for (int i = 0; i < colorData.Length; ++i) {
							Color32 c = ((i % 4) < 2) ? topColor : bottomColor;
							byte red = (byte)(((int)colorData[i].r * (int)c.r) / 255);
							byte green = (byte)(((int)colorData[i].g * (int)c.g) / 255);
							byte blue = (byte)(((int)colorData[i].b * (int)c.b) / 255);
							byte alpha = (byte)(((int)colorData[i].a * (int)c.a) / 255);
							if (geomData.fontInst.premultipliedAlpha) {
								red = (byte)(((int)red * (int)alpha) / 255);
								green = (byte)(((int)green * (int)alpha) / 255);
								blue = (byte)(((int)blue * (int)alpha) / 255);
							}
							colorData[i] = new Color32(red, green, blue, alpha);
						}
					}
					tk2dTextGeomGen.SetTextMeshIndices(indData, 0, currVertex, geomData, target);
					break;
				}
			}
			
			bs.CachedBoundsCenter = boundsCenter;
			bs.CachedBoundsExtents = boundsExtents;

			if (nVerts > 0 && bs.type != tk2dBatchedSprite.Type.TextMesh)
			{
				bool premulAlpha = (bs.spriteCollection != null) ? bs.spriteCollection.premultipliedAlpha : false;
				tk2dSpriteGeomGen.SetSpriteColors(colorData, 0, nVerts, bs.color, premulAlpha);
			}

			Matrix4x4 mat = scaleMatrix * bs.relativeMatrix;
			for (int i = 0; i < nVerts; ++i)
			{
				Vector3 pos = Vector3.Scale(posData[i], bs.baseScale);
				pos = mat.MultiplyPoint(pos);
				if (flattenDepth) {
					pos.z = 0;
				}
				
				meshVertices[currVertex + i] = pos;

				meshUvs[currVertex + i] = uvData[i];
				if (needUV2) meshUv2s[currVertex + i] = uv2Data[i];
				meshColors[currVertex + i] = colorData[i];

				if (needNormals)
				{
					meshNormals[currVertex + i] = bs.rotation * normalData[i];
				}
				if (needTangents)
				{
					Vector3 tang = new Vector3(tangentData[i].x, tangentData[i].y, tangentData[i].z);
					tang = bs.rotation * tang;
					meshTangents[currVertex + i] = new Vector4(tang.x, tang.y, tang.z, tangentData[i].w);
				}
			}

			currentIndices.AddRange (indData);

			currVertex += nVerts;

			++bsIndex;
		}
		
		if (currentIndices != null)
		{
			materials.Add(currentMaterial);
			indices.Add(currentIndices);
		}
		
		if (mesh)
		{
			mesh.vertices = meshVertices;
	        mesh.uv = meshUvs;
			if (needUV2)
				mesh.uv2 = meshUv2s;
	        mesh.colors32 = meshColors;
			if (needNormals)
				mesh.normals = meshNormals;
			if (needTangents)
				mesh.tangents = meshTangents;
			
			mesh.subMeshCount = indices.Count;
			for (int i = 0; i < indices.Count; ++i)
				mesh.SetTriangles(indices[i].ToArray(), i);
			
			mesh.RecalculateBounds();
		}
		
		renderer.sharedMaterials = materials.ToArray();
	}

	void BuildPhysicsMesh()
	{
		// Check if the Generate Colliders flag is cleared
		// Delete existing colliders and return otherwise
		MeshCollider meshCollider = GetComponent<MeshCollider>();
		if (meshCollider != null)
		{
			if (collider != meshCollider) {
				// Already has a collider
				return;
			}

			if (!CheckFlag(Flags.GenerateCollider)) {
#if UNITY_EDITOR
				DestroyImmediate(meshCollider);
#else
				Destroy(meshCollider);
#endif
			}
		}

#if !(UNITY_3_5 || UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2)
		EdgeCollider2D[] edgeColliders = GetComponents<EdgeCollider2D>();
		if (!CheckFlag(Flags.GenerateCollider)) {
			foreach (EdgeCollider2D ec in edgeColliders) {
	#if UNITY_EDITOR
					DestroyImmediate(ec);
	#else
					Destroy(ec);
	#endif
			}
		}
#endif

		if (!CheckFlag(Flags.GenerateCollider)) {
			return;
		}

		bool flattenDepth = CheckFlag(Flags.FlattenDepth);
		int numIndices = 0;
		int numVertices = 0;
		int numEdgeColliders = 0;
		bool physics3D = true;
		
		// first pass, count required vertices, indices and edges
		foreach (var bs in batchedSprites) 
		{
			if (!bs.IsDrawn) // when the first non-drawn child is found, it signals the end of the drawn list
				break;

			tk2dSpriteDefinition spriteDef = bs.GetSpriteDefinition();

			bool buildSpriteDefinitionMesh = false;
			bool buildBox = false;
			switch (bs.type)
			{
			case tk2dBatchedSprite.Type.Sprite:
				if (spriteDef != null && spriteDef.colliderType == tk2dSpriteDefinition.ColliderType.Mesh)
				{
					buildSpriteDefinitionMesh = true;
				}
				if (spriteDef != null && spriteDef.colliderType == tk2dSpriteDefinition.ColliderType.Box)
				{
					buildBox = true;
				}
				break;
			case tk2dBatchedSprite.Type.ClippedSprite:
			case tk2dBatchedSprite.Type.SlicedSprite:
			case tk2dBatchedSprite.Type.TiledSprite:
				buildBox = bs.CheckFlag(tk2dBatchedSprite.Flags.Sprite_CreateBoxCollider);
				break;
			case tk2dBatchedSprite.Type.TextMesh:
				//...
				break;
			}

			// might want to return these counts from SpriteGeomGen? (tidier...?)
			if (buildSpriteDefinitionMesh)
			{
				numIndices += spriteDef.colliderIndicesFwd.Length;
				numVertices += spriteDef.colliderVertices.Length;
				numEdgeColliders += spriteDef.edgeCollider2D.Length;
				numEdgeColliders += spriteDef.polygonCollider2D.Length;
			}
			else if (buildBox)
			{
				numIndices += 6 * 6;
				numVertices += 8;
				numEdgeColliders++;
			}

			if (spriteDef.physicsEngine == tk2dSpriteDefinition.PhysicsEngine.Physics2D) {
				physics3D = false;
			}
		}
		
		// Destroy existing collider if not required
		if ((physics3D && numIndices == 0) || (!physics3D && numEdgeColliders == 0))
		{
			if (colliderMesh != null)
			{
#if UNITY_EDITOR
				DestroyImmediate(colliderMesh);
#else
				Destroy(colliderMesh);
#endif
				colliderMesh = null;
			}
			if (meshCollider != null) {
#if UNITY_EDITOR
				DestroyImmediate(meshCollider);
#else
				Destroy(meshCollider);
#endif
			}			

#if !(UNITY_3_5 || UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2)
			foreach (EdgeCollider2D ec in edgeColliders) {
#if UNITY_EDITOR
				DestroyImmediate(ec);
#else
				Destroy(ec);
#endif
			}
#endif

			return;
		}

		// Sanitize for chosen physics engine
		if (physics3D) {
#if !(UNITY_3_5 || UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2)
			foreach (EdgeCollider2D ec in edgeColliders) {
#if UNITY_EDITOR
				DestroyImmediate(ec);
#else
				Destroy(ec);
#endif
			}
#endif
		}
		else {
			if (colliderMesh != null) {
#if UNITY_EDITOR
				DestroyImmediate(colliderMesh);
#else
				Destroy(colliderMesh);
#endif
			}
			if (meshCollider != null) {
#if UNITY_EDITOR
				DestroyImmediate(meshCollider);
#else
				Destroy(meshCollider);
#endif
			}			
		}
		
		// Delegate to appropriate builder function
		if (physics3D) {
			BuildPhysicsMesh3D(meshCollider, flattenDepth, numVertices, numIndices);
		}
#if !(UNITY_3_5 || UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2)
		else {
			BuildPhysicsMesh2D(edgeColliders, numEdgeColliders);
		}
#endif
	}

#if !(UNITY_3_5 || UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2)
	void BuildPhysicsMesh2D(EdgeCollider2D[] edgeColliders, int numEdgeColliders) {
		
		// Delete surplus
		for (int i = numEdgeColliders; i < edgeColliders.Length; ++i) {
#if UNITY_EDITOR
			DestroyImmediate(edgeColliders[i]);
#else
			Destroy(edgeColliders[i]);
#endif
		}

		Vector2[] boxPos = new Vector2[5];

		// Fill in missing, only do this if necessary
		if (numEdgeColliders > edgeColliders.Length) {
			EdgeCollider2D[] allEdgeColliders = new EdgeCollider2D[numEdgeColliders];
			int numToFill = Mathf.Min(numEdgeColliders, edgeColliders.Length);
			for (int i = 0; i < numToFill; ++i) {
				allEdgeColliders[i] = edgeColliders[i];
			}
			for (int i = numToFill; i < numEdgeColliders; ++i) {
				allEdgeColliders[i] = gameObject.AddComponent<EdgeCollider2D>();
			}
			edgeColliders = allEdgeColliders;
		}

		// second pass, build composite mesh
		Matrix4x4 scaleMatrix = Matrix4x4.identity;
		scaleMatrix.m00 = _scale.x;
		scaleMatrix.m11 = _scale.y;
		scaleMatrix.m22 = _scale.z;

		int currEdgeCollider = 0;
		foreach (tk2dBatchedSprite bs in batchedSprites) 
		{
			if (!bs.IsDrawn) // when the first non-drawn child is found, it signals the end of the drawn list
				break;

			tk2dSpriteDefinition spriteDef = bs.GetSpriteDefinition();

			bool buildSpriteDefinitionMesh = false;
			bool buildBox = false;
			Vector3 boxOrigin = Vector3.zero;
			Vector3 boxExtents = Vector3.zero;
			switch (bs.type)
			{
			case tk2dBatchedSprite.Type.Sprite:
				if (spriteDef != null && spriteDef.colliderType == tk2dSpriteDefinition.ColliderType.Mesh)
				{
					buildSpriteDefinitionMesh = true;
				}
				if (spriteDef != null && spriteDef.colliderType == tk2dSpriteDefinition.ColliderType.Box)
				{
					buildBox = true;
					boxOrigin = spriteDef.colliderVertices[0];
					boxExtents = spriteDef.colliderVertices[1];
				}
				break;
			case tk2dBatchedSprite.Type.ClippedSprite:
			case tk2dBatchedSprite.Type.SlicedSprite:
			case tk2dBatchedSprite.Type.TiledSprite:
				buildBox = bs.CheckFlag(tk2dBatchedSprite.Flags.Sprite_CreateBoxCollider);
				if (buildBox)
				{
					boxOrigin = bs.CachedBoundsCenter;
					boxExtents = bs.CachedBoundsExtents;
				}
				break;
			case tk2dBatchedSprite.Type.TextMesh:
				break;
			}

			Matrix4x4 mat = scaleMatrix * bs.relativeMatrix;
			if (buildSpriteDefinitionMesh)
			{
				foreach (tk2dCollider2DData dat in spriteDef.edgeCollider2D) {
					Vector2[] vertices = new Vector2[ dat.points.Length ];
					for (int i = 0; i < dat.points.Length; ++i) {
						vertices[i] =  mat.MultiplyPoint( dat.points[i] );
					}
					edgeColliders[currEdgeCollider].points = vertices;
				}

				foreach (tk2dCollider2DData dat in spriteDef.polygonCollider2D) {
					Vector2[] vertices = new Vector2[ dat.points.Length + 1 ];
					for (int i = 0; i < dat.points.Length; ++i) {
						vertices[i] = mat.MultiplyPoint( dat.points[i] );
					}
					vertices[dat.points.Length] = vertices[0]; // manual wrap around for poly collider
					edgeColliders[currEdgeCollider].points = vertices;
				}

				currEdgeCollider++;
			}
			else if (buildBox)
			{
				Vector3 min = boxOrigin - boxExtents;
				Vector3 max = boxOrigin + boxExtents;
				boxPos[0] = mat.MultiplyPoint( new Vector2(min.x, min.y) );
				boxPos[1] = mat.MultiplyPoint( new Vector2(max.x, min.y) );
				boxPos[2] = mat.MultiplyPoint( new Vector2(max.x, max.y) );
				boxPos[3] = mat.MultiplyPoint( new Vector2(min.x, max.y) );
				boxPos[4] = boxPos[0];
				edgeColliders[currEdgeCollider].points = boxPos;
				currEdgeCollider++;
			}
		}
	}
#endif

	void BuildPhysicsMesh3D(MeshCollider meshCollider, bool flattenDepth, int numVertices, int numIndices) {
		if (meshCollider == null)
		{
			meshCollider = gameObject.AddComponent<MeshCollider>();
		}
	
		if (colliderMesh == null)
		{
			colliderMesh = new Mesh();
			colliderMesh.hideFlags = HideFlags.DontSave;
		}
		else
		{
			colliderMesh.Clear();
		}
		
		// second pass, build composite mesh
		int currVertex = 0;
		Vector3[] vertices = new Vector3[numVertices];
		int currIndex = 0;
		int[] indices = new int[numIndices];

		Matrix4x4 scaleMatrix = Matrix4x4.identity;
		scaleMatrix.m00 = _scale.x;
		scaleMatrix.m11 = _scale.y;
		scaleMatrix.m22 = _scale.z;

		foreach (var bs in batchedSprites) 
		{
			if (!bs.IsDrawn) // when the first non-drawn child is found, it signals the end of the drawn list
				break;

			var spriteDef = bs.GetSpriteDefinition();

			bool buildSpriteDefinitionMesh = false;
			bool buildBox = false;
			Vector3 boxOrigin = Vector3.zero;
			Vector3 boxExtents = Vector3.zero;
			switch (bs.type)
			{
			case tk2dBatchedSprite.Type.Sprite:
				if (spriteDef != null && spriteDef.colliderType == tk2dSpriteDefinition.ColliderType.Mesh)
				{
					buildSpriteDefinitionMesh = true;
				}
				if (spriteDef != null && spriteDef.colliderType == tk2dSpriteDefinition.ColliderType.Box)
				{
					buildBox = true;
					boxOrigin = spriteDef.colliderVertices[0];
					boxExtents = spriteDef.colliderVertices[1];
				}
				break;
			case tk2dBatchedSprite.Type.ClippedSprite:
			case tk2dBatchedSprite.Type.SlicedSprite:
			case tk2dBatchedSprite.Type.TiledSprite:
				buildBox = bs.CheckFlag(tk2dBatchedSprite.Flags.Sprite_CreateBoxCollider);
				if (buildBox)
				{
					boxOrigin = bs.CachedBoundsCenter;
					boxExtents = bs.CachedBoundsExtents;
				}
				break;
			case tk2dBatchedSprite.Type.TextMesh:
				break;
			}

			Matrix4x4 mat = scaleMatrix * bs.relativeMatrix;
			if (flattenDepth) {
				mat.m23 = 0;
			}
			if (buildSpriteDefinitionMesh)
			{
				tk2dSpriteGeomGen.SetSpriteDefinitionMeshData(vertices, indices, currVertex, currIndex, currVertex, spriteDef, mat, bs.baseScale);
				currVertex += spriteDef.colliderVertices.Length;
				currIndex += spriteDef.colliderIndicesFwd.Length;
			}
			else if (buildBox)
			{
				tk2dSpriteGeomGen.SetBoxMeshData(vertices, indices, currVertex, currIndex, currVertex, boxOrigin, boxExtents, mat, bs.baseScale);
				currVertex += 8;
				currIndex += 6 * 6;
			}
		}
		
		colliderMesh.vertices = vertices;
		colliderMesh.triangles = indices;
		
		meshCollider.sharedMesh = colliderMesh;
	}
	
	public bool UsesSpriteCollection(tk2dSpriteCollectionData spriteCollection)
	{
		return this.spriteCollection == spriteCollection;	
	}
	
	public void ForceBuild()
	{
		Build();
	}

    public void Commit()
    {
            tk2dRuntimeSpriteBatcher batcher = this;
            // Select all children, EXCLUDING self
            Transform[] allTransforms = transform.GetComponentsInChildren<Transform>();
            allTransforms = (from t in allTransforms where t != transform select t).ToArray();

            // sort sprites, smaller to larger z
            if (CheckFlag(tk2dRuntimeSpriteBatcher.Flags.SortToCamera))
            {
                tk2dCamera tk2dCam = tk2dCamera.CameraForLayer(gameObject.layer);
                Camera cam = tk2dCam ? tk2dCam.camera : Camera.main;
                allTransforms = (from t in allTransforms orderby cam.WorldToScreenPoint((t.renderer != null) ? t.renderer.bounds.center : t.position).z descending select t).ToArray();
            }
            else
            {
                allTransforms = (from t in allTransforms orderby t.renderer.bounds.center.z descending select t).ToArray();
            }

            // and within the z sort by material
            if (allTransforms.Length == 0)
            {
                PSLog.LogError("No Transforms Found");
                return;
            }



            //MeshCollider[] childMeshColliders = GetComponentsInChildrenExcludeSelf<MeshCollider>(batcher.transform);
            //BoxCollider[] childBoxColliders = GetComponentsInChildrenExcludeSelf<BoxCollider>(batcher.transform);
            //BoxCollider2D[] childBoxCollider2Ds = GetComponentsInChildrenExcludeSelf<BoxCollider2D>(batcher.transform);
            //EdgeCollider2D[] childEdgeCollider2Ds = GetComponentsInChildrenExcludeSelf<EdgeCollider2D>(batcher.transform);
            //PolygonCollider2D[] childPolygonCollider2Ds = GetComponentsInChildrenExcludeSelf<PolygonCollider2D>(batcher.transform);

            Dictionary<Transform, int> batchedSpriteLookup = new Dictionary<Transform, int>();
            batchedSpriteLookup[transform] = -1;

            Matrix4x4 batcherWorldToLocal = transform.worldToLocalMatrix;

            spriteCollection = null;
            batchedSprites = new tk2dBatchedSprite[allTransforms.Length];
            List<tk2dTextMeshData> allTextMeshData = new List<tk2dTextMeshData>();

            int currBatchedSprite = 0;
            foreach (var t in allTransforms)
            {
                tk2dBaseSprite baseSprite = t.GetComponent<tk2dBaseSprite>();
                tk2dTextMesh textmesh = t.GetComponent<tk2dTextMesh>();

                tk2dBatchedSprite bs = new tk2dBatchedSprite();
                bs.name = t.gameObject.name;
                bs.position = t.localPosition;
                bs.rotation = t.localRotation;
                bs.relativeMatrix = batcherWorldToLocal * t.localToWorldMatrix;

                if (baseSprite)
                {
                    bs.baseScale = Vector3.one;
                    bs.localScale = new Vector3(t.localScale.x * baseSprite.scale.x, t.localScale.y * baseSprite.scale.y, t.localScale.z * baseSprite.scale.z);
                    FillBatchedSprite(bs, t.gameObject);

                    // temp redundant - just incase batcher expects to point to a valid one, somewhere we've missed
                    spriteCollection = baseSprite.Collection;
                }
                else if (textmesh)
                {
                    bs.spriteCollection = null;

                    bs.type = tk2dBatchedSprite.Type.TextMesh;
                    bs.color = textmesh.color;
                    bs.baseScale = textmesh.scale;
                    bs.renderLayer = textmesh.SortingOrder;
                    bs.localScale = new Vector3(t.localScale.x * textmesh.scale.x, t.localScale.y * textmesh.scale.y, t.localScale.z * textmesh.scale.z);
                    bs.FormattedText = textmesh.FormattedText;

                    tk2dTextMeshData tmd = new tk2dTextMeshData();
                    tmd.font = textmesh.font;
                    tmd.text = textmesh.text;
                    tmd.color = textmesh.color;
                    tmd.color2 = textmesh.color2;
                    tmd.useGradient = textmesh.useGradient;
                    tmd.textureGradient = textmesh.textureGradient;
                    tmd.anchor = textmesh.anchor;
                    tmd.kerning = textmesh.kerning;
                    tmd.maxChars = textmesh.maxChars;
                    tmd.inlineStyling = textmesh.inlineStyling;
                    tmd.formatting = textmesh.formatting;
                    tmd.wordWrapWidth = textmesh.wordWrapWidth;
                    tmd.spacing = textmesh.Spacing;
                    tmd.lineSpacing = textmesh.LineSpacing;

                    bs.xRefId = allTextMeshData.Count;
                    allTextMeshData.Add(tmd);
                }
                else
                {
                    // Empty GameObject
                    bs.spriteId = -1;
                    bs.baseScale = Vector3.one;
                    bs.localScale = t.localScale;
                    bs.type = tk2dBatchedSprite.Type.EmptyGameObject;
                }


                batchedSpriteLookup[t] = currBatchedSprite;
                batchedSprites[currBatchedSprite++] = bs;
            }
            this.allTextMeshData = allTextMeshData.ToArray();

            int idx = 0;
            foreach (var t in allTransforms)
            {
                var bs = batchedSprites[idx];

                bs.parentId = batchedSpriteLookup[t.parent];
                t.parent = transform; // unparent
                ++idx;
            }

            Transform[] directChildren = (from t in allTransforms where t.parent == transform select t).ToArray();
            foreach (var t in directChildren)
            {
                GameObject.DestroyImmediate(t.gameObject);
            }

            Vector3 inverseScale = new Vector3(1.0f / batcher.scale.x, 1.0f / batcher.scale.y, 1.0f / batcher.scale.z);
            batcher.transform.localScale = Vector3.Scale(batcher.transform.localScale, inverseScale);
            batcher.Build();
            //EditorUtility.SetDirty(target);
    }

    public static void FillBatchedSprite(tk2dBatchedSprite bs, GameObject go)
    {
        tk2dSprite srcSprite = go.transform.GetComponent<tk2dSprite>();
        tk2dTiledSprite srcTiledSprite = go.transform.GetComponent<tk2dTiledSprite>();
        tk2dSlicedSprite srcSlicedSprite = go.transform.GetComponent<tk2dSlicedSprite>();
        tk2dClippedSprite srcClippedSprite = go.transform.GetComponent<tk2dClippedSprite>();

        tk2dBaseSprite baseSprite = go.GetComponent<tk2dBaseSprite>();
        bs.spriteId = baseSprite.spriteId;
        bs.spriteCollection = baseSprite.Collection;
        bs.baseScale = baseSprite.scale;
        bs.color = baseSprite.color;
        bs.renderLayer = baseSprite.SortingOrder;
        if (baseSprite.boxCollider != null)
        {
            bs.BoxColliderOffsetZ = baseSprite.boxCollider.center.z;
            bs.BoxColliderExtentZ = baseSprite.boxCollider.size.z * 0.5f;
        }
        else
        {
            bs.BoxColliderOffsetZ = 0.0f;
            bs.BoxColliderExtentZ = 1.0f;
        }

        if (srcSprite)
        {
            bs.type = tk2dBatchedSprite.Type.Sprite;
        }
        else if (srcTiledSprite)
        {
            bs.type = tk2dBatchedSprite.Type.TiledSprite;
            bs.Dimensions = srcTiledSprite.dimensions;
            bs.anchor = srcTiledSprite.anchor;
            bs.SetFlag(tk2dBatchedSprite.Flags.Sprite_CreateBoxCollider, srcTiledSprite.CreateBoxCollider);
        }
        else if (srcSlicedSprite)
        {
            bs.type = tk2dBatchedSprite.Type.SlicedSprite;
            bs.Dimensions = srcSlicedSprite.dimensions;
            bs.anchor = srcSlicedSprite.anchor;
            bs.SetFlag(tk2dBatchedSprite.Flags.Sprite_CreateBoxCollider, srcSlicedSprite.CreateBoxCollider);
            bs.SetFlag(tk2dBatchedSprite.Flags.SlicedSprite_BorderOnly, srcSlicedSprite.BorderOnly);
            bs.SlicedSpriteBorderBottomLeft = new Vector2(srcSlicedSprite.borderLeft, srcSlicedSprite.borderBottom);
            bs.SlicedSpriteBorderTopRight = new Vector2(srcSlicedSprite.borderRight, srcSlicedSprite.borderTop);
        }
        else if (srcClippedSprite)
        {
            bs.type = tk2dBatchedSprite.Type.ClippedSprite;
            bs.ClippedSpriteRegionBottomLeft = srcClippedSprite.clipBottomLeft;
            bs.ClippedSpriteRegionTopRight = srcClippedSprite.clipTopRight;
            bs.SetFlag(tk2dBatchedSprite.Flags.Sprite_CreateBoxCollider, srcClippedSprite.CreateBoxCollider);
        }
    }
}
*/

