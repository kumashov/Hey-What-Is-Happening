using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TerrainAudioTextureSwitch
{
    public string TextureName;
    public AkSwitch Switch;
}

public class TerrainAudioSwitcher : MonoBehaviour
{
    public TerrainAudioTextureSwitch[] TextureSwitchMap;
    public AkSwitch StandingInWaterSwitch;
    public AkSwitch UnderwaterSwitch;
    public bool DebugDisplayCurrentTexture;

    private Terrain terrain;
    private TerrainData terrainData;
    private Vector3 terrainPos;
    private int _currTextureIndex = -1;
    private WaterState _currWaterState;
    private MixscapePlayer _player;

    // Use this for initialization
    void Start()
    {
        _player = GetComponent<MixscapePlayer>();
        
        terrain = Terrain.activeTerrain;
        terrainData = terrain.terrainData;
        terrainPos = terrain.transform.position;

    }

    // Update is called once per frame
    void Update()
    {
        bool needsSwitch = false;
        WaterState lastWaterState = _currWaterState;
        _currWaterState = _player.WaterState;
        if(lastWaterState != _currWaterState)
        {
            needsSwitch = true;
        }
        
        int lastTextureIndex = _currTextureIndex;
        _currTextureIndex = GetMainTexture(transform.position);
        if(lastTextureIndex != _currTextureIndex)
        {
            needsSwitch = true;
        }

        if(needsSwitch)
        {
            switch(_currWaterState)
            {
                case WaterState.None:
                    if(_currTextureIndex >= 0 && _currTextureIndex < terrainData.splatPrototypes.Length)
                    {
                        foreach(TerrainAudioTextureSwitch audioTextureSwitch in TextureSwitchMap)
                        {
                            if(audioTextureSwitch != null && string.Equals(audioTextureSwitch.TextureName, terrainData.splatPrototypes[_currTextureIndex].texture.name))
                            {
                                if(audioTextureSwitch.Switch != null)
                                {
                                    audioTextureSwitch.Switch.HandleEvent(null);
                                }
                            }
                        }
                    }
                    break;
                case WaterState.StandingInWater:
                    if(StandingInWaterSwitch != null)
                    {
                        StandingInWaterSwitch.HandleEvent(null);
                    }
                    break;
                case WaterState.Underwater:
                    if(UnderwaterSwitch != null)
                    {
                        UnderwaterSwitch.HandleEvent(null);
                    }
                    break;
            }
        }
    }

    void OnGUI()
    {
        if(DebugDisplayCurrentTexture)
        {
            if(terrainData.splatPrototypes.Length > 0 && _currTextureIndex >= 0 && _currTextureIndex < terrainData.splatPrototypes.Length)
            {
                GUI.Box(new Rect(100, 100, 200, 25), "index: " + _currTextureIndex.ToString() + ", name: " + terrainData.splatPrototypes[_currTextureIndex].texture.name);
            }
        }
    }

    private float[] GetTextureMix(Vector3 WorldPos)
    {
        // returns an array containing the relative mix of textures
        // on the main terrain at this world position.

        // The number of values in the array will equal the number
        // of textures added to the terrain.

        // calculate which splat map cell the worldPos falls within (ignoring y)
        int mapX = (int)(((WorldPos.x - terrainPos.x) / terrainData.size.x) * terrainData.alphamapWidth);
        int mapZ = (int)(((WorldPos.z - terrainPos.z) / terrainData.size.z) * terrainData.alphamapHeight);

        if(mapX < 0 || mapX >= terrainData.alphamapWidth || mapZ < 0 || mapZ >= terrainData.alphamapWidth)
        {
            return new float[0];
        }

        // get the splat data for this cell as a 1x1xN 3d array (where N = number of textures)
        float[,,] splatmapData = terrainData.GetAlphamaps(mapX, mapZ, 1, 1);

        // extract the 3D array data to a 1D array:
        float[] cellMix = new float[splatmapData.GetUpperBound(2) + 1];

        for(int n = 0; n < cellMix.Length; n++)
        {
            cellMix[n] = splatmapData[0, 0, n];
        }
        return cellMix;
    }

    private int GetMainTexture(Vector3 WorldPos)
    {
        // returns the zero-based index of the most dominant texture
        // on the main terrain at this world position.
        float[] mix = GetTextureMix(WorldPos);

        float maxMix = 0;
        int maxIndex = -1;

        // loop through each mix value and find the maximum
        for(int n = 0; n < mix.Length; n++)
        {
            if(mix[n] > maxMix)
            {
                maxIndex = n;
                maxMix = mix[n];
            }
        }
        return maxIndex;
    }
}
