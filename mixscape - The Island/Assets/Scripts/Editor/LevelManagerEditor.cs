using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LevelManager), true)]
public class LevelManagerEditor : Editor
{
    public LevelManager TargetLevelManager { get { return target as LevelManager; } }

    private LevelInfo _levelInfo = null;
    private int _currentSkybox;

    private List<GameObject> _allObjects = new List<GameObject>();

    void OnEnable()
    {
        LoadLevelSettings();
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.Separator();
        EditorGUILayout.LabelField("Level Editing", EditorStyles.boldLabel);

        if (GUILayout.Button("Select Terrain"))
        {
            Select(GetTerrain());
        }

        if (GUILayout.Button("Set to Next Skybox"))
        {
            NextSkybox();
        }

        if (_levelInfo != null)
        {
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Prefab Select", EditorStyles.boldLabel);
            
            _allObjects.Clear();

            DoPrefabHelperButtons("Blocking Rocks", _levelInfo.LevelBlockingObjects);
            DoPrefabHelperButtons("Trees", _levelInfo.TreeObjects);
            DoPrefabHelperButtons("Bushes", _levelInfo.BushObjects);
            DoPrefabHelperButtons("Mushrooms", _levelInfo.MushroomObjects);
            DoPrefabHelperButtons("Rocks", _levelInfo.RockObjects);
            DoPrefabHelperButtons("Human Made", _levelInfo.HumanMadeObjects);

            if (_allObjects.Count > 0)
            {
                DoPrefabPlacerModeButton("All", _allObjects.ToArray());
            }
        }
    }

    public void DoPrefabHelperButtons(string label, GameObject[] objectArray)
    {
        GUILayout.BeginHorizontal();

        if (GUILayout.Button(label))
        {
            Select(PullRandom(objectArray));
        }

        DoPrefabPlacerModeButton(label, objectArray);

        GUILayout.EndHorizontal();

        _allObjects.AddRange(objectArray);
    }

    public void DoPrefabPlacerModeButton(string label, GameObject[] objectArray)
    {
        if (GUILayout.Button("Placement Mode: " + label))
        {
            if (objectArray.Length > 0)
            {
                var terrain = GetTerrain();
                var placer = terrain.GetComponent<PrefabPlacement>();
                if (placer)
                {
                    placer.Prefabs = new PrefabPlacement.PlaceablePrefab[objectArray.Length];
                    for (int i = 0; i < objectArray.Length; ++i)
                    {
                        GameObject prefab = objectArray[i];
                        if (prefab != null)
                        {
                            if (PrefabUtility.GetPrefabType(prefab) != PrefabType.Prefab)
                            {
                                prefab = null;
                            }
                        }

                        placer.Prefabs[i] = new PrefabPlacement.PlaceablePrefab
                        {
                            Prefab = prefab,
                            OffsetY = -0.15f,
                            Place = prefab != null
                        };
                    }

                    Select(terrain);
                }
            }
        }
    }

    public static void Select(Component component)
    {
        if (component != null)
        {
            Selection.activeGameObject = component.gameObject;
        }
    }

    public static void Select(GameObject obj)
    {
        if (obj != null)
        {
            Selection.activeGameObject = obj;
        }
    }

    public void LoadLevelSettings()
    {
        _levelInfo = Resources.Load<LevelInfo>("LevelSettings");
        if (_levelInfo == null)
        {
            _levelInfo = new LevelInfo();
            AssetDatabase.CreateAsset(_levelInfo, "Assets/Resources/LevelSettings.asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        InitSkyboxSettings();
    }

    public void InitSkyboxSettings()
    {
        if (_levelInfo == null) return;

        for (int i = 0; i < _levelInfo.SkyboxMaterials.Length; ++i)
        {
            if (RenderSettings.skybox == _levelInfo.SkyboxMaterials[i])
            {
                _currentSkybox = i;
                break;
            }
        }
    }

    public void NextSkybox()
    {
        if (_levelInfo == null) return;

        if (++_currentSkybox >= _levelInfo.SkyboxMaterials.Length)
        {
            _currentSkybox = 0;
        }

        if (_currentSkybox < _levelInfo.SkyboxMaterials.Length)
        {
            RenderSettings.skybox = _levelInfo.SkyboxMaterials[_currentSkybox];

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
            }
        }
#endif
    }

    public Terrain GetTerrain()
    {
        Terrain terrain = TargetLevelManager.transform.GetComponentInChildren<Terrain>();
        if (terrain == null) terrain = GameObject.FindObjectOfType<Terrain>();
        return terrain;
    }

    public GameObject PullRandom(GameObject[] objectArray)
    {
        if (objectArray != null && objectArray.Length > 0)
        {
            return objectArray[Random.Range(0, objectArray.Length)];
        }
        return null;
    }
}
