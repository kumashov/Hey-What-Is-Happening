using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PrefabPlacement : MonoBehaviour
{
    [Serializable]
    public class PlaceablePrefab
    {
        public GameObject Prefab;
        public bool Place;
        public float OffsetY;
    }

    [HideInInspector]
    public PlaceablePrefab[] Prefabs = new PlaceablePrefab[0];

    public Transform PrefabParent;
    public Vector3 RotationVariation;
    public float ScaleVariation;

    public bool PlacementMode { get; set; }
    public Vector3 LastHit { get; private set; }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

#if UNITY_EDITOR
    public void TogglePlacementMode()
    {
        PlacementMode = !PlacementMode;
    }

    public void PlaceAtScreenPosition(Vector2 position)
    {
        if (Camera.current == null)
        {
            return;
        }

        position.y = Camera.current.pixelHeight - position.y;
        Ray ray = Camera.current.ScreenPointToRay(position);

        RaycastHit hit;
        TerrainCollider collider = this.GetComponent<TerrainCollider>();

        if (collider.Raycast(ray, out hit, Mathf.Infinity))
        {
            Place(hit.point);
            LastHit = hit.point;
        }
    }

    public void Place(Vector3 position)
    {
        List<PlaceablePrefab> availablePrefabs = new List<PlaceablePrefab>(Prefabs);
        availablePrefabs.RemoveAll(p => !p.Place);

        if (availablePrefabs.Count > 0)
        {
            PlaceablePrefab placeable = availablePrefabs[UnityEngine.Random.Range(0, availablePrefabs.Count)];
            GameObject instance = null;

            if (placeable.Prefab != null)
            {
                instance = UnityEditor.PrefabUtility.InstantiatePrefab(placeable.Prefab) as GameObject;//GameObject.InstantiatePrefab(placeable.Prefab, PrefabParent != null ? PrefabParent : transform);
            }

            if (instance != null)
            {
                UnityEditor.Undo.RegisterCreatedObjectUndo(instance, "Instantiate Object");

                instance.transform.SetParent(PrefabParent != null ? PrefabParent : transform);

                position.y += placeable.OffsetY;
                instance.transform.position = position;

                instance.transform.rotation = Quaternion.Euler(
                    UnityEngine.Random.Range(0, RotationVariation.x),
                    UnityEngine.Random.Range(0, RotationVariation.y),
                    UnityEngine.Random.Range(0, RotationVariation.z));

                float scale = Mathf.Max(0.01f, 1.0f - UnityEngine.Random.Range(-Mathf.Abs(ScaleVariation), Mathf.Abs(ScaleVariation)));
                instance.transform.localScale = new Vector3(scale, scale, scale);
            }
        }

    }
#endif
}
