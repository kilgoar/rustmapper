using UnityEditor;
using UnityEngine;
using UnityEditor.IMGUI.Controls;
using Rotorz.ReorderableList;

public class MapIOEditor : EditorWindow
{
    string editorVersion = "v1.9.5-prerelease";

	[SerializeField] TreeViewState m_TreeViewState;

    PrefabHierachy m_TreeView;
    SearchField m_SearchField;				
						
	int htb = 0, htt = 500;
	
	int pDiam = 600, pGradient = 100, pXoff = 15, pYoff = 20, channelSize = 250;
	bool channels = true;
	int seafloor = 450;
	
	float tttWeight = .6f;

    string[] landLayers = { "Ground", "Biome", "Alpha", "Topology" };
    string loadFile = "";
    string saveFile = "";
    string mapName = "";
    string prefabSaveFile = "", mapPrefabSaveFile = "";
    //Todo: mess this up. It's coarse and rough and irritating and it's just getting worse. mmmmmmm...
	
	float trWeight = .33f;
	bool trFlat = true;
	bool trCirc = true;
	bool trPerlin = true;
	int trZ = 503;
	int trLow = 2;
	int trHigh = 20;
	int trCount = 15;
	int trDescale = 3;
	
	TerrainTopology.Enum topologyHolder = TerrainTopology.Enum.Beach;
	
	int dsR = 37;
	int dsH = 10;
	int dsW = 30;
	
	int layer = 9;
	int period = 75;
	float scaley = 75f;
	
	int thicc = 2;
	
	int p = 100;
	int min = 100;
	int max = 200;
	
	int scale = 50;
	float contrast = 1.5f;
	bool tog = false;
	bool tog1 = true;
	
	
	int cliffI = 0, cliffD = 65, cliffF = 500;
	float cliffZ = -16.8f;
	string cliffFile = "";
	Vector3 rotations = new Vector3(4, 300, 4);
	
    int mapSize = 2000, mainMenuOptions = 0, toolsOptions = 0, mapToolsOptions = 0, heightMapOptions = 0, conditionalPaintOptions = 0, prefabOptions = 0;
    float heightToSet = 450f, offset = 0f;
    //float scale = 50f;
    //float mapScale = 1f; Comment back in when used.
    bool[] sides = new bool[4]; 
    bool checkHeight = true, setWaterMap = false;
	bool strip = true;
    bool allLayers = false, ground = false, biome = false, alpha = false, topology = false, heightmap = false, prefabs = false, paths = false;
    float heightLow = 0f, heightHigh = 500f, slopeLow = 40f, slopeHigh = 60f;
    float slopeMinBlendLow = 25f, slopeMaxBlendLow = 40f, slopeMinBlendHigh = 60f, slopeMaxBlendHigh = 75f;
    float heightMinBlendLow = 0f, heightMaxBlendLow = 500f, heightMinBlendHigh = 500f, heightMaxBlendHigh = 1000f;
    float normaliseLow = 450f, normaliseHigh = 1000f;
	float z = 0f;
	int x = 0, y = 0;
    int z1 = 0, z2 = 0, x1 = 0, x2 = 0;
    bool blendSlopes = false, blendHeights = false, aboveTerrain = false;
    int textureFrom, textureToPaint, landLayerFrom, landLayerToPaint, topologyFrom, topologyToPaint;
    int layerConditionalInt, texture = 0, topologyTexture = 0, alphaTexture;
    bool deletePrefabs = false;
    bool checkHeightCndtl = false, checkSlopeCndtl = false, checkAlpha = false;
    float slopeLowCndtl = 45f, slopeHighCndtl = 60f;
    float heightLowCndtl = 500f, heightHighCndtl = 600f;
    bool autoUpdate = false;
    //string assetDirectory = "Assets/NodePresets/";
    Vector2 scrollPos = new Vector2(0, 0);
    Vector2 presetScrollPos = new Vector2(0, 0);

    float filterStrength = 1f;
    float terraceErodeFeatureSize = 150f, terraceErodeInteriorCornerWeight = 1f;
    float blurDirection = 0f;

    int[] values = { 0, 1 };
    string[] activeTextureAlpha = { "Visible", "Invisible" };
    string[] activeTextureTopo = { "Active", "Inactive" };



    [MenuItem("Rust Map Editor/Main Menu", false, 0)]
    static void Initialize()
    {
        MapIOEditor window = (MapIOEditor)EditorWindow.GetWindow(typeof(MapIOEditor), false, "Rust Map Editor");
    }
	void OnEnable()
    {
        if (m_TreeViewState == null)
            m_TreeViewState = new TreeViewState();

        m_TreeView = new PrefabHierachy(m_TreeViewState);
        m_SearchField = new SearchField();
        m_SearchField.downOrUpArrowKeyPressed += m_TreeView.SetFocusAndEnsureSelectedItem;
    }
	
    void OnGUI()
    {
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, false);
        GUIContent[] mainMenu = new GUIContent[5];
        mainMenu[0] = new GUIContent("File");
        mainMenu[1] = new GUIContent("Prefabs");
        mainMenu[2] = new GUIContent("Layers");
		mainMenu[3] = new GUIContent("Generator");
		mainMenu[4] = new GUIContent("Advanced");
        mainMenuOptions = GUILayout.Toolbar(mainMenuOptions, mainMenu);


		
        #region Menu
        switch (mainMenuOptions)
        {
            #region Main Menu
            case 0:
                
				
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button(new GUIContent("Open", "Opens a file viewer to find and open a Rust .map file."), GUILayout.MaxWidth(100)))
                {
                    loadFile = UnityEditor.EditorUtility.OpenFilePanel("Import Map File", loadFile, "map");

                    var blob = new WorldSerialization();
                    if (loadFile == "")
                    {
                        return;
                    }
                    MapIO.ProgressBar("Loading: " + loadFile, "Loading Land Heightmap Data ", 0.1f);
                    blob.Load(loadFile);
                    MapIO.loadPath = loadFile;
                    MapIO.ProgressBar("Loading: " + loadFile, "Loading Land Heightmap Data ", 0.2f);
                    MapIO.Load(blob);
                }
                if (GUILayout.Button(new GUIContent("Save As...", "Opens a file viewer to find and save a Rust .map file."), GUILayout.MaxWidth(100)))
                {
                    saveFile = UnityEditor.EditorUtility.SaveFilePanel("Export Map File", saveFile, mapName, "map");
                    if (saveFile == "")
                    {
                        return;
                    }
                    Debug.Log("Exported map " + saveFile);
                    MapIO.savePath = saveFile;
                    prefabSaveFile = saveFile;
                    MapIO.ProgressBar("Saving Map: " + saveFile, "Saving Heightmap ", 0.1f);
                    MapIO.Save(saveFile);
                }
				
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.Space();
				EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button(new GUIContent("New", "Creates a new map " + mapSize.ToString() + " metres in size."), GUILayout.MaxWidth(100)))
                {
                    int newMap = EditorUtility.DisplayDialogComplex("Warning", "Creating a new map will remove any unsaved changes to your map.", "Create New Map", "Exit", "Save and Create New Map");
                    if (mapSize < 1000 & mapSize > 6000)
                    {
                        EditorUtility.DisplayDialog("Error", "Map size must be between 1000 - 6000", "Ok");
                        return;
                    }
                    switch (newMap)
                    {
                        case 0:
                            MapIO.loadPath = "New Map";
                            MapIO.NewEmptyTerrain(mapSize);
                            break;
                        case 1:
                            // User cancelled
                            break;
                        case 2:
                            saveFile = UnityEditor.EditorUtility.SaveFilePanel("Export Map File", saveFile, mapName, "map");
                            if (saveFile == "")
                            {
                                EditorUtility.DisplayDialog("Error", "Save Path is Empty", "Ok");
                                return;
                            }
                            Debug.Log("Exported map " + saveFile);
                            MapIO.Save(saveFile);
                            MapIO.loadPath = "New Map";
                            MapIO.NewEmptyTerrain(mapSize);
                            break;
                        default:
                            Debug.Log("Create New Map option outofbounds");
                            break;
                    }
                }
                GUILayout.Label(new GUIContent("Size", "The size of the Rust Map to create upon new map."), GUILayout.MaxWidth(30));
                mapSize = EditorGUILayout.IntField(mapSize, GUILayout.MaxWidth(100));
				
					EditorGUILayout.EndHorizontal();
                

						EditorGUILayout.Space();
				        EditorGUILayout.LabelField("Asset Bundle", EditorStyles.boldLabel);
                        
						
						EditorGUILayout.BeginHorizontal();
						
                        if (GUILayout.Button(new GUIContent("Load", "Loads all the prefabs from the Rust Asset Bundle for use in the editor. Prefabs paths to be loaded can be changed in " +
                            "AssetList.txt in the root directory"), GUILayout.MaxWidth(100)))
                        {
                            if (!MapIO.bundleFile.Contains(@"steamapps/common/Rust/Bundles/Bundles"))
                            {
                                MapIO.bundleFile = MapIO.bundleFile = EditorUtility.OpenFilePanel("Select Bundle File", MapIO.bundleFile, "");
                                if (MapIO.bundleFile == "")
                                {
                                    return;
                                }
                                if (!MapIO.bundleFile.Contains(@"steamapps/common/Rust/Bundles/Bundles"))
                                {
                                    EditorUtility.DisplayDialog("ERROR: Bundle File Invalid", @"Bundle file path invalid. It should be located within steamapps\common\Rust\Bundles", "Ok");
                                    return;
                                }
                            }
                            MapIO.StartPrefabLookup();
                        }
                        if (GUILayout.Button(new GUIContent("Unload", "Unloads all the prefabs from the Rust Asset Bundle."), GUILayout.MaxWidth(100)))
                        {
                            if (MapIO.GetPrefabLookUp() != null)
                            {
                                MapIO.GetPrefabLookUp().Dispose();
                                MapIO.SetPrefabLookup(null);
                            }
                            else
                            {
                                EditorUtility.DisplayDialog("ERROR: Can't unload prefabs", "No prefabs loaded.", "Ok");
                            }
                        }
                        EditorGUILayout.EndHorizontal();
						
						
                        MapIO.bundleFile = GUILayout.TextArea(MapIO.bundleFile);
				
				
                GUILayout.Label("About", EditorStyles.boldLabel);
                GUILayout.Label("OS: " + SystemInfo.operatingSystem);
                GUILayout.Label("Unity Version: " + Application.unityVersion);
                GUILayout.Label("Editor Version: " + editorVersion);

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button(new GUIContent("Report Bug", "Opens up the editor bug report in GitHub."), GUILayout.MaxWidth(75)))
                {
                    Application.OpenURL("https://github.com/RustMapMaking/Rust-Map-Editor-Unity/issues/new?assignees=Adsitoz&labels=bug&template=bug-report.md&title=%5BBUG%5D+Bug+name+goes+here");
                }
                if (GUILayout.Button(new GUIContent("Request Feature", "Opens up the editor feature request in GitHub."), GUILayout.MaxWidth(105)))
                {
                    Application.OpenURL("https://github.com/RustMapMaking/Rust-Map-Editor-Unity/issues/new?assignees=Adsitoz&labels=enhancement&template=feature-request.md&title=%5BREQUEST%5D+Request+name+goes+here");
                }
                if (GUILayout.Button(new GUIContent("RoadMap", "Opens up the editor roadmap in GitHub."), GUILayout.MaxWidth(65)))
                {
                    Application.OpenURL("https://github.com/RustMapMaking/Rust-Map-Editor-Unity/projects/1");
                }
                if (GUILayout.Button(new GUIContent("Wiki", "Opens up the editor wiki in GitHub."), GUILayout.MaxWidth(40)))
                {
                    Application.OpenURL("https://github.com/RustMapMaking/Rust-Map-Editor-Unity/wiki");
                }
                EditorGUILayout.EndHorizontal();
                break;
            #endregion
            #region Tools
            case 1:

		
					
					DoToolbar();
					DoTreeView();
							 

                    break;
						
            
            
            #endregion
						

           
			case 2:
						
				GUIContent[] toolsOptionsMenu = new GUIContent[4];
					toolsOptionsMenu[0] = new GUIContent("Ground");
					toolsOptionsMenu[1] = new GUIContent("Biome");
					toolsOptionsMenu[2] = new GUIContent("Alpha");
					toolsOptionsMenu[3] = new GUIContent("Topology");
					toolsOptions = GUILayout.Toolbar(toolsOptions, toolsOptionsMenu);
					string formerLandLayer = MapIO.landLayer;
					
					switch (toolsOptions)
					{
						default:
							toolsOptions = 0;
							break;
										
						case 0:
						MapIO.landLayer = landLayers[0];
                        if (MapIO.landLayer != formerLandLayer)
                        {
                            MapIO.ChangeLandLayer();
                            Repaint();
                        }
						
						EditorGUILayout.Space();
						
                        if (slopeMinBlendHigh > slopeMaxBlendHigh)
                        {
                            slopeMaxBlendHigh = slopeMinBlendHigh + 0.25f;
                            if (slopeMaxBlendHigh > 90f)
                            {
                                slopeMaxBlendHigh = 90f;
                            }
                        }
                        if (slopeMinBlendLow > slopeMaxBlendLow)
                        {
                            slopeMinBlendLow = slopeMaxBlendLow - 0.25f;
                            if (slopeMinBlendLow < 0f)
                            {
                                slopeMinBlendLow = 0f;
                            }
                        }
                        if (heightMinBlendLow > heightMaxBlendLow)
                        {
                            heightMinBlendLow = heightMaxBlendLow - 0.25f;
                            if (heightMinBlendLow < 0f)
                            {
                                heightMinBlendLow = 0f;
                            }
                        }
                        if (heightMinBlendHigh > heightMaxBlendHigh)
                        {
                            heightMaxBlendHigh = heightMinBlendHigh + 0.25f;
                            if (heightMaxBlendHigh > 1000f)
                            {
                                heightMaxBlendHigh = 1000f;
                            }
                        }
                        slopeMaxBlendLow = slopeLow;
                        slopeMinBlendHigh = slopeHigh;
                        heightMaxBlendLow = heightLow;
                        heightMinBlendHigh = heightHigh;
                        if (blendSlopes == false)
                        {
                            slopeMinBlendLow = slopeMaxBlendLow;
                            slopeMaxBlendHigh = slopeMinBlendHigh;
                        }
                        if (blendHeights == false)
                        {
                            heightMinBlendLow = heightLow;
                            heightMaxBlendHigh = heightHigh;
                        }
                        #region Ground Layer
                        if (MapIO.landLayer.Equals("Ground"))
                        {
                            MapIO.terrainLayer = (TerrainSplat.Enum)EditorGUILayout.EnumPopup("Texture", MapIO.terrainLayer);
                            
							
							if (GUILayout.Button("Fill"))
                            {
                                MapIO.PaintLayer("Ground", TerrainSplat.TypeToIndex((int)MapIO.terrainLayer));
                            }
                            
							EditorGUILayout.Space();
							GUILayout.Label("Mottling", EditorStyles.boldLabel);
							p = EditorGUILayout.IntField("Patches", p);
							
							EditorGUILayout.BeginHorizontal();
							min = EditorGUILayout.IntField("Minimum size", min);
							max = EditorGUILayout.IntField("Maximum size", max);
							EditorGUILayout.EndHorizontal();
							if (GUILayout.Button("Apply"))
							{
                                MapIO.paintCrazing(p, min, max);
							}
							
							EditorGUILayout.Space();
							GUILayout.Label("Gradient Noise", EditorStyles.boldLabel);
							
							scale = EditorGUILayout.IntField("Scale", scale);
							contrast = EditorGUILayout.FloatField("Contrast", contrast);
							
							EditorGUILayout.BeginHorizontal();
							tog1 = EditorGUILayout.Toggle("Paint on Biome", tog1);
							tog = EditorGUILayout.Toggle("Invert", tog);
							EditorGUILayout.EndHorizontal();
							
							MapIO.targetBiomeLayer = (TerrainBiome.Enum)EditorGUILayout.EnumPopup("Target Biome:", MapIO.targetBiomeLayer);
							if (GUILayout.Button("Apply"))
							{
                                MapIO.paintPerlin(scale, contrast, tog, tog1);
								
							}
							
							EditorGUILayout.Space();
							GUILayout.Label("Slope Range", EditorStyles.boldLabel); // From 0 - 90
							
                            EditorGUILayout.BeginHorizontal();
                            GUILayout.Label("From: " + slopeLow.ToString() + "°", EditorStyles.boldLabel);
                            GUILayout.Label("To: " + slopeHigh.ToString() + "°", EditorStyles.boldLabel);
                            EditorGUILayout.EndHorizontal();
							
                            EditorGUILayout.MinMaxSlider(ref slopeLow, ref slopeHigh, 0f, 90f);
							
							
                            if (blendSlopes == true)
                            {
                                GUILayout.Label("Blend Low: " + slopeMinBlendLow + "°");
                                EditorGUILayout.MinMaxSlider(ref slopeMinBlendLow, ref slopeMaxBlendLow, 0f, 90f);
                                GUILayout.Label("Blend High: " + slopeMaxBlendHigh + "°");
                                EditorGUILayout.MinMaxSlider(ref slopeMinBlendHigh, ref slopeMaxBlendHigh, 0f, 90f);
                            }
							
							EditorGUILayout.BeginHorizontal();
                            if (GUILayout.Button(new GUIContent("Paint Slopes", "Paints the terrain on the " + MapIO.landLayer + " layer within the slope range.")))
                            {
                                MapIO.PaintSlope("Ground", slopeLow, slopeHigh, slopeMinBlendLow, slopeMaxBlendHigh, TerrainSplat.TypeToIndex((int)MapIO.terrainLayer));
                            }
							blendSlopes = EditorGUILayout.ToggleLeft("Blend", blendSlopes);
							EditorGUILayout.EndHorizontal();
							
							
							EditorGUILayout.Space();
                            GUILayout.Label("Height Range", EditorStyles.boldLabel); // From 0 - 90
                            
                            
							
							EditorGUILayout.BeginHorizontal();
                            GUILayout.Label("From: " + heightLow.ToString() + "m", EditorStyles.boldLabel);
                            GUILayout.Label("To: " + heightHigh.ToString() + "m", EditorStyles.boldLabel);
                            EditorGUILayout.EndHorizontal();
							
							
                            EditorGUILayout.MinMaxSlider(ref heightLow, ref heightHigh, 0f, 1000f);
							
                            if (blendHeights == true)
                            {
                                GUILayout.Label("Blend Low: " + heightMinBlendLow + "m");
                                EditorGUILayout.MinMaxSlider(ref heightMinBlendLow, ref heightMaxBlendLow, 0f, 1000f);
                                GUILayout.Label("Blend High: " + heightMaxBlendHigh + "m");
                                EditorGUILayout.MinMaxSlider(ref heightMinBlendHigh, ref heightMaxBlendHigh, 0f, 1000f);
                            }
							
							EditorGUILayout.BeginHorizontal();
                            if (GUILayout.Button(new GUIContent("Paint Heights", "Paints the terrain on the " + MapIO.landLayer + " layer within the height range.")))
                            {
                                MapIO.PaintHeight("Ground", heightLow, heightHigh, heightMinBlendLow, heightMaxBlendHigh, TerrainSplat.TypeToIndex((int)MapIO.terrainLayer));
                            }
							blendHeights = EditorGUILayout.ToggleLeft("Blend", blendHeights);
							EditorGUILayout.EndHorizontal();
							
							GUILayout.Label("River Painter", EditorStyles.boldLabel);
							
							aboveTerrain = EditorGUILayout.ToggleLeft("Paint only visible part of river.", aboveTerrain);
							
                            if (GUILayout.Button("Paint Rivers"))
                            {
                                MapIO.PaintRiver("Ground", aboveTerrain, TerrainSplat.TypeToIndex((int)MapIO.terrainLayer));
                            }
						}
						
						
						break;
						
						//biomes
						case 1:
						MapIO.landLayer = landLayers[1];
                        if (MapIO.landLayer != formerLandLayer)
                        {
                            MapIO.ChangeLandLayer();
                            Repaint();
                        }
						EditorGUILayout.Space();
						
						MapIO.paintBiomeLayer = (TerrainBiome.Enum)EditorGUILayout.EnumPopup("Biome", MapIO.paintBiomeLayer);
						
						if (GUILayout.Button("Fill"))
                            {
                            MapIO.PaintLayer("Biome", TerrainBiome.TypeToIndex((int)MapIO.paintBiomeLayer));
							}
						
						GUILayout.Label("Height Range", EditorStyles.boldLabel); // From 0 - 90
                        blendHeights = EditorGUILayout.ToggleLeft("Blend", blendHeights);
                            
							
						EditorGUILayout.BeginHorizontal();
                        GUILayout.Label("From: " + heightLow.ToString() + "m", EditorStyles.boldLabel);
                        GUILayout.Label("To: " + heightHigh.ToString() + "m", EditorStyles.boldLabel);
                        EditorGUILayout.EndHorizontal();
							
							
                        EditorGUILayout.MinMaxSlider(ref heightLow, ref heightHigh, 0f, 1000f);
							
                        if (blendHeights == true)
                            {
                                GUILayout.Label("Blend Low: " + heightMinBlendLow + "m");
                                EditorGUILayout.MinMaxSlider(ref heightMinBlendLow, ref heightMaxBlendLow, 0f, 1000f);
                                GUILayout.Label("Blend High: " + heightMaxBlendHigh + "m");
                                EditorGUILayout.MinMaxSlider(ref heightMinBlendHigh, ref heightMaxBlendHigh, 0f, 1000f);
                            }
							
                        if (GUILayout.Button(new GUIContent("Paint Heights", "Paints the terrain on the " + MapIO.landLayer + " layer within the height range.")))
                            {
                            MapIO.PaintHeight("Biome", heightLow, heightHigh, heightMinBlendLow, heightMaxBlendHigh, TerrainBiome.TypeToIndex((int)MapIO.paintBiomeLayer));
                            }
						
						if (GUILayout.Button("Gradient Biomes"))
						{
                            MapIO.biomeGradients(20,20,20,20);
						}	
							
						
						break;

						
						case 2:
						MapIO.landLayer = landLayers[2];
                        if (MapIO.landLayer != formerLandLayer)
                        {
                            MapIO.ChangeLandLayer();
                            Repaint();
                        }
						//alpha
						
						break;
						
						case 3:
						MapIO.landLayer = landLayers[3];
                        if (MapIO.landLayer != formerLandLayer)
                        {
                            MapIO.ChangeLandLayer();
                            Repaint();
							
						
                        }
						
						MapIO.oldTopologyLayer = MapIO.topologyLayer;
                            MapIO.topologyLayer = (TerrainTopology.Enum)EditorGUILayout.EnumPopup("Active topology:", MapIO.topologyLayer);
                            if (MapIO.topologyLayer != MapIO.oldTopologyLayer)
                            {
                                MapIO.ChangeLandLayer();
                                Repaint();
                            }
						//topos
						
						EditorGUILayout.Space();
						EditorGUILayout.BeginHorizontal();
						if (GUILayout.Button("Fill"))
						{
                            MapIO.clearTopology();
                            MapIO.invertTopology();
						}
						if (GUILayout.Button("Clear"))
						{
                            MapIO.clearTopology();
						}
						if (GUILayout.Button("Invert"))
						{
                            MapIO.invertTopology();
						}
						EditorGUILayout.EndHorizontal();
						
						
						
						GUILayout.Label("Heights", EditorStyles.boldLabel);
						
						EditorGUILayout.BeginHorizontal();
						htb = EditorGUILayout.IntField("Floor", htb);
						htt = EditorGUILayout.IntField("Ceiling", htt);
						EditorGUILayout.EndHorizontal();
						
						EditorGUILayout.BeginHorizontal();
						if (GUILayout.Button("Fill"))
						{
                            MapIO.paintHeight(htb,htt);
						}
						if (GUILayout.Button("Clear"))
						{
                            MapIO.eraseHeight(htb,htt);
						}
						
						EditorGUILayout.EndHorizontal();
						
						
						EditorGUILayout.Space();
						GUILayout.Label("Topology Combinator", EditorStyles.boldLabel);
						MapIO.targetTopologyLayer = (TerrainTopology.Enum)EditorGUILayout.EnumPopup("Source topology:", MapIO.targetTopologyLayer);
						
						
						
						EditorGUILayout.BeginHorizontal();
						if (GUILayout.Button("Copy"))
						{
                            MapIO.copyTopologyLayer();
						}
						
						if (GUILayout.Button("Erase overlaps"))
						{
                            MapIO.notTopologyLayer();
						}
						EditorGUILayout.EndHorizontal();
						
						
						EditorGUILayout.BeginHorizontal();
						if (GUILayout.Button("Outline"))
						{
                            MapIO.paintTopologyOutline(thicc);
						}
						thicc = EditorGUILayout.IntField("Thickness:", thicc);
						if (thicc > 6)
							thicc = 5;
						EditorGUILayout.EndHorizontal();
						
						GUILayout.Label("Ground Combinator", EditorStyles.boldLabel);
						MapIO.targetTerrainLayer = (TerrainSplat.Enum)EditorGUILayout.EnumPopup("Texture", MapIO.targetTerrainLayer);
						
						
						
						EditorGUILayout.BeginHorizontal();
						if (GUILayout.Button("Fill"))
						{
                            MapIO.terrainToTopology(tttWeight);
						}
						tttWeight = GUILayout.HorizontalSlider(tttWeight, 0.0f, .7f);
						EditorGUILayout.EndHorizontal();
						
						break;
							
					}

                        
						
						
            break;
            
			case 3:
			GUIContent[] prefabsOptionsMenu = new GUIContent[3];
                prefabsOptionsMenu[0] = new GUIContent("Heightmaps");
                prefabsOptionsMenu[1] = new GUIContent("Geology");
				prefabsOptionsMenu[2] = new GUIContent("Presets");
                //prefabsOptionsMenu[1] = new GUIContent("Spawn Prefabs");
                prefabOptions = GUILayout.Toolbar(prefabOptions, prefabsOptionsMenu);

                switch (prefabOptions)
                {
                    case 0:
					
					GUILayout.Label("Nudge", EditorStyles.boldLabel);
					
                                        EditorGUILayout.BeginHorizontal();
                                        

                                        offset = EditorGUILayout.FloatField(offset, GUILayout.MaxWidth(40));
                                        if (GUILayout.Button(new GUIContent("Apply", "Raises or lowers the height of the entire heightmap by " + offset.ToString() + " metres. " +
                                            "A positive offset will raise the heightmap, a negative offset will lower the heightmap.")))
											{
                            MapIO.OffsetHeightmap(offset, true, false);
											}

										EditorGUILayout.EndHorizontal();
					EditorGUILayout.Space();
					
					EditorGUILayout.LabelField("Generate Smooth Terrain", EditorStyles.boldLabel);
					layer = EditorGUILayout.IntField("Layers:", layer);
					period = EditorGUILayout.IntField("Period:", period);
					scaley = EditorGUILayout.FloatField("Scale:", scaley);
					
							if (GUILayout.Button("Apply"))
							{
                            MapIO.perlinSaiyan(layer, period, scaley);
							}
							

					EditorGUILayout.LabelField("Generate Sharp Terrain", EditorStyles.boldLabel);
					dsR = EditorGUILayout.IntField("Roughness", dsR);
					dsH = EditorGUILayout.IntField("Height", dsH);
					dsW = EditorGUILayout.IntField("Weight", dsW);
					if (GUILayout.Button("Apply"))
					{
                            MapIO.diamondSquareNoise(dsR, dsH, dsW);
					}
					
					
					EditorGUILayout.LabelField("Random Terracing", EditorStyles.boldLabel);
					
					EditorGUILayout.BeginHorizontal();
					trCount = EditorGUILayout.IntField("Terraces", trCount);
					trLow = EditorGUILayout.IntField("Smallest", trLow);
					EditorGUILayout.EndHorizontal();
					
					EditorGUILayout.BeginHorizontal();
					trZ = EditorGUILayout.IntField("Lowest", trZ);
					trHigh = EditorGUILayout.IntField("Tallest", trHigh);
					EditorGUILayout.EndHorizontal();
					
					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField("Weight",GUILayout.MaxWidth(50));
					trWeight = GUILayout.HorizontalSlider(trWeight, 0.0f, 1f);
					EditorGUILayout.EndHorizontal();
					
					
					
					
					EditorGUILayout.BeginHorizontal();
					
					trCirc = EditorGUILayout.Toggle("Smooth", trCirc);
					
					trPerlin = EditorGUILayout.Toggle("Banks", trPerlin);
					EditorGUILayout.EndHorizontal();
					
					EditorGUILayout.BeginHorizontal();
					trFlat = EditorGUILayout.Toggle("Basins", trFlat);
					if (trFlat)
					{
					trDescale = EditorGUILayout.IntField("Basin Flatness", trDescale);
					}
					EditorGUILayout.EndHorizontal();
					
					if (GUILayout.Button("Apply"))
					{
                            MapIO.bluffTerracing(trFlat, trPerlin, trCirc, trWeight, trZ, trLow, trHigh, trCount, trDescale);
					}
					
					
					//public void pucker(int radius, int gradient, float seafloor, int xOffset, int yOffset, bool perlin, int s)
					EditorGUILayout.LabelField("Puckering", EditorStyles.boldLabel);
					

					
					pDiam = EditorGUILayout.IntField("Island size", pDiam);
					pGradient = EditorGUILayout.IntField("Shore size", pGradient);
					seafloor = EditorGUILayout.IntField("Seafloor", seafloor);
					
					EditorGUILayout.BeginHorizontal();
					pXoff = EditorGUILayout.IntField("Z center", pXoff);
					pYoff = EditorGUILayout.IntField("Y center", pYoff);
					EditorGUILayout.EndHorizontal();
					
					EditorGUILayout.BeginHorizontal();
					channels = EditorGUILayout.ToggleLeft("Channels", channels);
					if (channels)
					{
						channelSize = EditorGUILayout.IntField("Channel Size", channelSize);
					}
					
					EditorGUILayout.EndHorizontal();
					
					if (GUILayout.Button("Apply"))
					{
                            MapIO.pucker(pDiam/2, pGradient, seafloor/1000f, pXoff, pYoff, channels, channelSize);
					}
										
                                        


										/* ????
										
										
                                        GUILayout.Label("Edge of Map Height", EditorStyles.boldLabel);
                                        EditorGUILayout.BeginHorizontal();
                                        sides[0] = EditorGUILayout.ToggleLeft("Top ", sides[0], GUILayout.MaxWidth(60));
                                        sides[3] = EditorGUILayout.ToggleLeft("Left ", sides[3], GUILayout.MaxWidth(60));
                                        sides[2] = EditorGUILayout.ToggleLeft("Bottom ", sides[2], GUILayout.MaxWidth(60));
                                        sides[1] = EditorGUILayout.ToggleLeft("Right ", sides[1], GUILayout.MaxWidth(60));
                                        EditorGUILayout.EndHorizontal();

                                        if (GUILayout.Button(new GUIContent("Set Edge Height", "Sets the very edge of the map to " + heightToSet.ToString() + " metres on any of the sides selected.")))
                                        {
                                            script.SetEdgePixel(heightToSet, sides);
                                        }
										*/
                                        EditorGUILayout.BeginHorizontal();
                                        /*
                                        if (GUILayout.Button(new GUIContent("Rescale", "Scales the heightmap by " + mapScale.ToString() + " %.")))
                                        {
                                            script.scaleHeightmap(mapScale);
                                        }*/
                                        
                                        EditorGUILayout.EndHorizontal();
                                        GUILayout.Label(new GUIContent("Normalise", "Moves the heightmap heights to between the two heights."), EditorStyles.boldLabel);
                                        EditorGUILayout.BeginHorizontal();
                                        EditorGUILayout.LabelField(new GUIContent("Low", "The lowest point on the map after being normalised."), GUILayout.MaxWidth(40));
                                        EditorGUI.BeginChangeCheck();
                                        normaliseLow = EditorGUILayout.Slider(normaliseLow, 0f, 1000f);
                                        EditorGUILayout.EndHorizontal();
                                        EditorGUILayout.BeginHorizontal();
                                        EditorGUILayout.LabelField(new GUIContent("High", "The highest point on the map after being normalised."), GUILayout.MaxWidth(40));
                                        normaliseHigh = EditorGUILayout.Slider(normaliseHigh, 0f, 1000f);
                                        EditorGUILayout.EndHorizontal();
                                        EditorGUILayout.BeginHorizontal();
                                        if (EditorGUI.EndChangeCheck() && autoUpdate == true)
                                        {
                            MapIO.NormaliseHeightmap(normaliseLow / 1000f, normaliseHigh / 1000f);
                                        }
                                        EditorGUILayout.EndHorizontal();
										
                                        EditorGUILayout.BeginHorizontal();
                                        if (GUILayout.Button(new GUIContent("Apply", "Normalises the heightmap between these heights.")))
                                        {
                            MapIO.NormaliseHeightmap(normaliseLow / 1000f, normaliseHigh / 1000f);
                                        }
                                        autoUpdate = EditorGUILayout.ToggleLeft(new GUIContent("Auto Update", "Automatically applies the changes to the heightmap on value change."), autoUpdate);
                                        EditorGUILayout.EndHorizontal();
										
										EditorGUILayout.Space();
                                        GUILayout.Label(new GUIContent("Smooth", "Smooth the entire terrain."), EditorStyles.boldLabel);
                                        EditorGUILayout.BeginHorizontal();
                                        EditorGUILayout.LabelField(new GUIContent("Strength", "The strength of the smoothing operation."), GUILayout.MaxWidth(85));
                                        filterStrength = EditorGUILayout.Slider(filterStrength, 0f, 1f);
                                        EditorGUILayout.EndHorizontal();
                                        EditorGUILayout.BeginHorizontal();
                                        EditorGUILayout.LabelField(new GUIContent("Blur Direction", "The direction the terrain should blur towards. Negative is down, " +
                                            "positive is up."), GUILayout.MaxWidth(85));
                                        blurDirection = EditorGUILayout.Slider(blurDirection, -1f, 1f);
                                        EditorGUILayout.EndHorizontal();
                                        if (GUILayout.Button(new GUIContent("Apply", "Smoothes the heightmap.")))
                                        {
                            MapIO.SmoothHeightmap(filterStrength, blurDirection);
                                        }
										EditorGUILayout.Space();
										
										GUILayout.Label("Crop Height", EditorStyles.boldLabel);
                                        EditorGUILayout.BeginHorizontal();
                                        heightToSet = EditorGUILayout.FloatField(heightToSet, GUILayout.MaxWidth(40));
                                        if (GUILayout.Button(new GUIContent("Floor", "Raises any of the land below " + heightToSet.ToString() + " metres to " + heightToSet.ToString() +
                                            " metres."), GUILayout.MaxWidth(130)))
                                        {
                            MapIO.SetMinimumHeight(heightToSet);
                                        }
                                        if (GUILayout.Button(new GUIContent("Ceiling", "Lowers any of the land above " + heightToSet.ToString() + " metres to " + heightToSet.ToString() +
                                            " metres."), GUILayout.MaxWidth(130)))
                                        {
                            MapIO.SetMaximumHeight(heightToSet);
                                        }
										
                                        EditorGUILayout.EndHorizontal();
										EditorGUILayout.Space();
										if (GUILayout.Button(new GUIContent("Flip", "Inverts the heightmap in on itself.")))
                                        {
                            MapIO.InvertHeightmap();
                                        }
					
                        break;
                    case 1:
					GUILayout.Label("Feature Placement", EditorStyles.boldLabel);
                            EditorGUILayout.BeginHorizontal();
                            GUILayout.Label("Slope Range: " + slopeLow.ToString() + "° - " + slopeHigh.ToString() + "°");
                            EditorGUILayout.EndHorizontal();
							
                            EditorGUILayout.MinMaxSlider(ref slopeLow, ref slopeHigh, 0f, 90f);
					
					rotations = EditorGUILayout.Vector3Field("Max rotation", rotations);
					
					cliffZ = EditorGUILayout.FloatField("Height Offset", cliffZ);
					cliffD = EditorGUILayout.IntField("Density", cliffD);
					cliffF = EditorGUILayout.IntField("Floor", cliffF);
					
					GUILayout.Label("Prefab Data", EditorStyles.boldLabel);
					
					cliffI = EditorGUILayout.IntField("Index", cliffI);
					
					

					
					//insertPrefabCliffs(WorldSerialization blob, Vector3 rotationRanges, int s1, int s2, int k, float zOffset, int density, float floor)
					
					EditorGUILayout.BeginHorizontal();
					if (GUILayout.Button("Browse",GUILayout.MaxWidth(55)))
					{
						loadFile = UnityEditor.EditorUtility.OpenFilePanel("Cliffsets", cliffFile, "cliffset.map");
					}
						
						loadFile = GUILayout.TextArea(loadFile);
					EditorGUILayout.EndHorizontal();
					
					if (GUILayout.Button("Apply"))
						{
							var blob = new WorldSerialization();
							blob.Load(cliffFile);
                            MapIO.insertPrefabCliffs(blob, rotations, (int)slopeLow, (int)slopeHigh, cliffI, cliffZ, cliffD, cliffF/1000f);
						}
						
						
                    break;
						
                case 2:
			
			if (GUILayout.Button("Switchyard Arena"))
			{
				ArenaHeightmap();
				ArenaTextures();
				WaterworldTopologies();
				WaterworldFinalizing();				
				
			}
			
			if (GUILayout.Button("Highway Arena"))
			{
				HighwayHeightmap();
				HighwayTextures();
				WaterworldTopologies();
				WaterworldFinalizing();				
				
			}
			
			if (GUILayout.Button("Forested Arena"))
			{
				ArenaHeightmap();
				ForestTextures();
				WaterworldTopologies();
				WaterworldFinalizing();				
				
			}
			
			if (GUILayout.Button("Rapa Nui Arena"))
			{
				WaterArenaHeightmap();
				ArenaTextures();
				WaterworldTopologies();
				WaterworldFinalizing();				
				
			}
			
			if (GUILayout.Button("Atoll heightmaps"))
			{
				WaterworldClassicHeightmap();
				
			}
			
			
			
			if (GUILayout.Button("Atoll textures monuments etc"))
			{
				WaterworldTextures();

				

				WaterworldClassicMonuments();
				
				WaterworldTopologies();	
				
				WaterworldFinalizing();
				//WaterworldCliffs();
				
			}
			
			if (GUILayout.Button("Small Island Map"))
			{
				WaterworldHeightmap();
				WaterworldTextures();

				WaterworldTopologies();	

				WaterworldMonuments();
				
				WaterworldFinalizing();
				WaterworldCliffs();
			}
			
						
			
			
			if (GUILayout.Button("Small Island Map Topos"))
			{

				WaterworldTopologies();
				WaterworldFinalizing();
			}
			


			if (GUILayout.Button("Small Island Monuments"))
			{
				WaterworldMonuments();
			}
			
			if (GUILayout.Button("Small Island Monuments bullshit"))
			{
				WaterworldTopologies();
				WaterworldFinalizing();
				WaterworldCliffs();
			}
			
			if (GUILayout.Button("Small Islands textures + topos"))
			{
				WaterworldTextures();
				WaterworldTopologies();
			}
		
		if (GUILayout.Button("Cliffs Heightmap"))
		{
                            MapIO.NewEmptyTerrain(4096);
                            //overly recursive perlin layer averaging madness
                            //           layers, scaling period, initial scale
                            MapIO.perlinSaiyan(5, 20, 67);

                            //script.diamondSquareNoise(30, 10, 10);
                            MapIO.zNudge(90f);
                            MapIO.pucker(600, 600, .40f, 200, 500, true, 200);
                            MapIO.zNudge(250f);




                            MapIO.punch(.503f, 200);

                            MapIO.bluffTerracing(true, true, true, .6f, 530, 30, 40, 10, 3);
                            //MapIO.bluffTerracing(false, true, true, .6f, 505, 20, 30, 10, 3);

                            MapIO.pucker(600, 600, .450f, 200, 500, true, 350);




                            MapIO.bluffTerracing(false, true, true, 1f, 499, 3, 3, 1, 0);
			
		}	

                break;
                    default:
                        prefabOptions = 0;
                        break;
                }
			break;
			
			
			
			case 4:
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Place monument.map", EditorStyles.boldLabel);
			strip = EditorGUILayout.ToggleLeft("Delete prefabs", strip, GUILayout.MaxWidth(200));
			EditorGUILayout.EndHorizontal();
			
		
			if (GUILayout.Button("Get Monument Height"))
			{
				y = (int)GameObject.Find("MonumentPlacer").transform.position.x;
				x = (int)GameObject.Find("MonumentPlacer").transform.position.z;
				loadFile = UnityEditor.EditorUtility.OpenFilePanel("Import Map File to Test", loadFile, "monument.map");
				var blob = new WorldSerialization();
				blob.Load(loadFile);
				z = MapIO.testMonumentHeight(blob, x, y);
				
			}
								
								
								z = EditorGUILayout.FloatField("Height:", z);
								
								if (GUILayout.Button("Place"))
								{
									y = (int)GameObject.Find("MonumentPlacer").transform.position.x;
									x = (int)GameObject.Find("MonumentPlacer").transform.position.z;
									loadFile = UnityEditor.EditorUtility.OpenFilePanel("Import Map File to Paste", loadFile, "monument.map");
									var blob = new WorldSerialization();
									
									if (strip)
									{
										blob.Load(loadFile);
                        MapIO.stripPrefabsUnderMonument(blob, x, y);
									}
									
									blob.Load(loadFile);
                    MapIO.pasteMonument(blob, x, y, z);
									
								}
								
		//hate the swamps?
		if (GUILayout.Button("Swamp Replacer"))
		{
			
			Terrain land = GameObject.FindGameObjectWithTag("Land").GetComponent<Terrain>();
			float ratio = land.terrainData.size.x / (land.terrainData.heightmapWidth-1);
			Debug.LogError (ratio);
			
			string fileA = "monuments/3300/SwampReplacerA.monument.map";
			string fileB = "monuments/3300/SwampReplacerB.monument.map";
			string fileC = "monuments/3300/SwampReplacerC.monument.map";
			int x5 = 0;
			int y5 = 0;
			float z5 = 0f;
			int offset = 0;
			
			PrefabDataHolder [] swampList = MapIO.swamps();
			foreach (PrefabDataHolder swamp in swampList)
			{
				x5 = (int)(swamp.prefabData.position.z + (land.terrainData.size.z /2f));
				y5 = (int)(swamp.prefabData.position.x + (land.terrainData.size.x /2f));
				z5 = (swamp.prefabData.position.y+3) / 1000f;
				
				if (swamp.prefabData.id == 873508118)
				{
					offset = 150;
					var blob = new WorldSerialization();
					blob.Load(fileA);
					MapIO.stripPrefabsUnderMonument(blob, x5-offset, y5-offset);
					blob.Load(fileA);
					MapIO.pasteMonument(blob, x5-offset, y5-offset, z5);
				}
				
				if (swamp.prefabData.id == 3530204693)
				{
					offset = 150;
					var blob = new WorldSerialization();
					blob.Load(fileB);
					MapIO.stripPrefabsUnderMonument(blob, x5-offset, y5-offset);
					blob.Load(fileB);
					MapIO.pasteMonument(blob, x5-offset, y5-offset, z5);
				}
				
				if (swamp.prefabData.id == 2563750503)
				{
					offset = 150;
					var blob = new WorldSerialization();
					blob.Load(fileC);
					MapIO.stripPrefabsUnderMonument(blob, x5-offset, y5-offset);
					blob.Load(fileC);
					MapIO.pasteMonument(blob, x5-offset, y5-offset, z5);
				}
				
				
			}
			
			
		}
								
						
																				EditorGUILayout.Space();
								EditorGUILayout.LabelField("Debugging", EditorStyles.boldLabel);
								
					
						EditorGUILayout.BeginHorizontal();
                                if (GUILayout.Button(new GUIContent("Debug Alpha", "Sets the ground texture to rock wherever the terrain is invisible. Prevents the floating grass effect.")))
                                {
                    MapIO.AlphaDebug();
                                }
                                if (GUILayout.Button(new GUIContent("Debug Water", "Raises the water heightmap to 500 metres if it is below.")))
                                {
                    MapIO.DebugWaterLevel();
                                }
								
                                EditorGUILayout.EndHorizontal();
					
                        if (GUILayout.Button(new GUIContent("Remove Broken Prefabs", "Removes any prefabs known to prevent maps from loading. Use this is you are having" +
                                    " errors loading a map on a server.")))
                        {
                            MapIO.RemoveBrokenPrefabs();
                        }
                        
						EditorGUILayout.Space();
						
						
						EditorGUILayout.LabelField("Modding", EditorStyles.boldLabel);
						
						EditorGUILayout.BeginHorizontal();
                        if (GUILayout.Button(new GUIContent("Export LootCrates", "Exports all lootcrates that don't yet respawn in Rust to a JSON for use with the LootCrateRespawn plugin." +
                            "If you don't delete them after export they will duplicate on first map load.")))
                        {
                            prefabSaveFile = EditorUtility.SaveFilePanel("Export LootCrates", prefabSaveFile, "LootCrateData", "json");
                            if (prefabSaveFile == "")
                            {
                                return;
                            }
                            MapIO.ExportLootCrates(prefabSaveFile, deletePrefabs);
                        }
                        if (GUILayout.Button(new GUIContent("Export Map Prefabs", "Exports all map prefabs to plugin data.")))
                        {
                            mapPrefabSaveFile = EditorUtility.SaveFilePanel("Export Map Prefabs", prefabSaveFile, "MapData", "json");
                            if (mapPrefabSaveFile == "")
                            {
                                return;
                            }
                            MapIO.ExportMapPrefabs(mapPrefabSaveFile, deletePrefabs);
                        }
                        EditorGUILayout.EndHorizontal();
						
						deletePrefabs = EditorGUILayout.ToggleLeft(new GUIContent("Delete on Export.", "Deletes the prefabs after exporting them."), deletePrefabs, GUILayout.MaxWidth(300));
                        
						EditorGUILayout.Space();
						
                        EditorGUILayout.BeginHorizontal();
                        if (GUILayout.Button(new GUIContent("Hide Prefabs in RustEdit", "Changes all the prefab categories to a semi-colon. Hides all of the prefabs from " +
                            "appearing in RustEdit. Use the break RustEdit Custom Prefabs button to undo.")))
                        {
                            MapIO.HidePrefabsInRustEdit();
                        }
                        if (GUILayout.Button(new GUIContent("Break RustEdit Custom Prefabs", "Breaks down all custom prefabs saved in the map file.")))
                        {
                            MapIO.BreakRustEditCustomPrefabs();
                        }
                        EditorGUILayout.EndHorizontal();
						
						
                        EditorGUILayout.BeginHorizontal();
                        if (GUILayout.Button(new GUIContent("Delete All Map Prefabs", "Removes all the prefabs from the map.")))
                        {
                            MapIO.RemoveMapObjects(true, false);
                        }
                        if (GUILayout.Button(new GUIContent("Delete All Map Paths", "Removes all the paths from the map.")))
                        {
                            MapIO.RemoveMapObjects(false, true);
                        }
                        EditorGUILayout.EndHorizontal();
						
						EditorGUILayout.Space();
						GUILayout.Label("Rotator", EditorStyles.boldLabel);
                                
                                EditorGUILayout.BeginHorizontal();
                                allLayers = EditorGUILayout.ToggleLeft("Rotate All", allLayers, GUILayout.MaxWidth(75));

                                if (GUILayout.Button("Rotate 90°", GUILayout.MaxWidth(90))) // Calls every rotate function from MapIO. Rotates 90 degrees.
                                {
                                    if (heightmap == true)
                                    {
                                        EditorUtility.DisplayProgressBar("Rotating Map", "Rotating Heightmap ", 0.05f);
                                        MapIO.RotateHeightmap(true);
                                    }
                                    if (paths == true)
                                    {
                                        EditorUtility.DisplayProgressBar("Rotating Map", "Rotating Paths ", 0.075f);
                                        MapIO.RotatePaths(true);
                                    }
                                    if (prefabs == true)
                                    {
                                        EditorUtility.DisplayProgressBar("Rotating Map", "Rotating Prefabs ", 0.1f);
                                        MapIO.RotatePrefabs(true);
                                    }
                                    if (ground == true)
                                    {
                                        EditorUtility.DisplayProgressBar("Rotating Map", "Rotating Ground Textures ", 0.15f);
                                        MapIO.RotateLayer("ground", true);
                                    }
                                    if (biome == true)
                                    {
                                        EditorUtility.DisplayProgressBar("Rotating Map", "Rotating Biome Textures ", 0.2f);
                                        MapIO.RotateLayer("biome", true);
                                    }
                                    if (alpha == true)
                                    {
                                        EditorUtility.DisplayProgressBar("Rotating Map", "Rotating Alpha Textures ", 0.25f);
                                        MapIO.RotateLayer("alpha", true);
                                    }
                                    if (topology == true)
                                    {
                                        MapIO.RotateAllTopologymap(true);
                                    };
                                    EditorUtility.DisplayProgressBar("Rotating Map", "Finished ", 1f);
                                    EditorUtility.ClearProgressBar();
                                }
                                if (GUILayout.Button("Rotate 270°", GUILayout.MaxWidth(90))) // Calls every rotate function from MapIO. Rotates 270 degrees.
                                {
                                    if (heightmap == true)
                                    {
                                        EditorUtility.DisplayProgressBar("Rotating Map", "Rotating Heightmap ", 0.05f);
                                        MapIO.RotateHeightmap(false);
                                    }
                                    if (paths == true)
                                    {
                                        EditorUtility.DisplayProgressBar("Rotating Map", "Rotating Paths ", 0.075f);
                                        MapIO.RotatePaths(false);
                                    }
                                    if (prefabs == true)
                                    {
                                        EditorUtility.DisplayProgressBar("Rotating Map", "Rotating Prefabs ", 0.1f);
                                        MapIO.RotatePrefabs(false);
                                    }
                                    if (ground == true)
                                    {
                                        EditorUtility.DisplayProgressBar("Rotating Map", "Rotating Ground Textures ", 0.15f);
                                        MapIO.RotateLayer("ground", false);
                                    }
                                    if (biome == true)
                                    {
                                        EditorUtility.DisplayProgressBar("Rotating Map", "Rotating Biome Textures ", 0.2f);
                                        MapIO.RotateLayer("biome", false);
                                    }
                                    if (alpha == true)
                                    {
                                        EditorUtility.DisplayProgressBar("Rotating Map", "Rotating Alpha Textures ", 0.25f);
                                        MapIO.RotateLayer("alpha", false);
                                    }
                                    if (topology == true)
                                    {
                                        MapIO.RotateAllTopologymap(false);
                                    };
                                    EditorUtility.DisplayProgressBar("Rotating Map", "Finished ", 1f);
                                    EditorUtility.ClearProgressBar();
                                }
                                EditorGUILayout.EndHorizontal();
								
								
                                if (allLayers == true)
                                {
                                    ground = true; biome = true; alpha = true; topology = true; heightmap = true; paths = true; prefabs = true;
                                }

								
                                EditorGUILayout.BeginHorizontal();
                                ground = EditorGUILayout.ToggleLeft("Ground", ground, GUILayout.MaxWidth(60));
                                biome = EditorGUILayout.ToggleLeft("Biome", biome, GUILayout.MaxWidth(60));
                                alpha = EditorGUILayout.ToggleLeft("Alpha", alpha, GUILayout.MaxWidth(60));
                                topology = EditorGUILayout.ToggleLeft("Topology", topology, GUILayout.MaxWidth(75));
                                EditorGUILayout.EndHorizontal();
                                EditorGUILayout.BeginHorizontal();
                                paths = EditorGUILayout.ToggleLeft("Paths", paths, GUILayout.MaxWidth(60));
                                prefabs = EditorGUILayout.ToggleLeft("Prefabs", prefabs, GUILayout.MaxWidth(60));
                                heightmap = EditorGUILayout.ToggleLeft("HeightMap", heightmap, GUILayout.MaxWidth(80));
                                EditorGUILayout.EndHorizontal();
								
								EditorGUILayout.Space();
						
						
						EditorGUILayout.Space();
						
                        GUILayout.Label(new GUIContent("Node Presets", "List of all the node presets in the project."), EditorStyles.boldLabel);
                        if (GUILayout.Button(new GUIContent("Refresh presets list.", "Refreshes the list of all the Node Presets in the project.")))
                        {
                            MapIO.RefreshAssetList();
                        }
                        presetScrollPos = GUILayout.BeginScrollView(presetScrollPos);
                        ReorderableListGUI.Title("Node Presets");
                        ReorderableListGUI.ListField(MapIO.generationPresetList, NodePresetDrawer, DrawEmpty);
                        GUILayout.EndScrollView();
						
			
			//here
			
			
			
			
			break;
			
			
			
			#endregion
            default:
                mainMenuOptions = 0;
                break;
        
		
		
		
		}
		
		void WaterworldHeightmap()
		{
            MapIO.NewEmptyTerrain(4096);
            MapIO.flattenWater(.5f);
            //This is the heightmap generator, alter this to change the hills, valleys, cliffs, and so on

            //This first step gives us the bumpy texture of our small islands. Scale it up to make it less bumpy

            //overly recursive perlin layer averaging madness
            //           layers, scaling period, initial scale
            MapIO.perlinSaiyan(4, 20, 45);
            MapIO.zNudge(9);

            //this adds a little micro roughness            
            MapIO.diamondSquareNoise(30, 10, 10);

            //now for some erosion to add a nice circular cutout for BEACHES

            //'perlin masking' should provide paths for players to make it up the cliffs

            // 'z initializing' is the baseline for where the erosion begins, and it will proceed upwards from there. 
            // Using the 'number of cliffs' the erosion algorithm will draw random numbers within the range (bottom height - top height) and erode cliffs that are that tall
            // By the 'downscaling factor' this script can flatten the terrain beneath the 'z-initializing' number. (downscaling must be true for this to have an effect).

            //downscaling, 
            //            perlin masking,
            //                     circular(t) or triangular(f),
            //										  opacity,
            //                                             z initializing
            //													bottom height randomnumber
            //                                                     top height randomnumber
            //                                                        number of cliffs
            //                                                            downscaling factor
            MapIO.bluffTerracing(true, true, true, 1f, 502, 3, 3, 1, 4);


            //add cliffs                         

            MapIO.bluffTerracing(false, true, true, 1f, 503, 6, 10, 1, 0);
            //script.bluffTerracing(false, true, false, .8f, 502, 4, 6, 2, 0);


            //this nudges the whole map down 1.5 units
            MapIO.zNudge(-1.5f);


            //Puckering punches down the edges of the map to the seafloor, creating a roughly ovular shape 

            //While the 'perlin cutouts' are true, that also punches a few deeper channels in between the islands

            //For a circular result zero out the offsets.

            //pucker(int radius, int gradient, float seafloor, int xOffset, yOffset, perlin cutouts, cutout scale
            MapIO.pucker(800, 300, .450f, 200, 600, true, 325);
				
			
			
		}

		void ArenaHeightmap()
		{
			MapIO.NewEmptyTerrain(2000);
			MapIO.flattenWater(.5f);
			MapIO.perlinSaiyan(3, 50, 120);
			
			
			MapIO.diamondSquareNoise(20, 20, 20);
			

			
			MapIO.pucker(400, 150, .450f, 100, 0, true, 200);
			//MapIO.unPucker(600,250, .495f, 300, 100, false, 0);
			MapIO.bluffTerracing(true, true, true, 1f, 500, 5, 5, 1, 2);
			MapIO.zNudge(1.5f);
			
			MapIO.SmoothHeightmap(1f, 1f);
			MapIO.SmoothHeightmap(1f, 1f);
			MapIO.SmoothHeightmap(1f, 1f);
			MapIO.SmoothHeightmap(1f, 1f);
			MapIO.SmoothHeightmap(1f, 1f);
			MapIO.SmoothHeightmap(1f, 1f);
			MapIO.SmoothHeightmap(1f, 1f);
			MapIO.SmoothHeightmap(1f, 1f);
			MapIO.SmoothHeightmap(1f, 1f);
			MapIO.SmoothHeightmap(1f, 1f);
			MapIO.SmoothHeightmap(1f, 1f);
			MapIO.SmoothHeightmap(1f, 1f);
			MapIO.SmoothHeightmap(1f, 1f);
			MapIO.SmoothHeightmap(1f, 1f);
			MapIO.SmoothHeightmap(1f, 1f);
			MapIO.SmoothHeightmap(1f, 1f);
			MapIO.SmoothHeightmap(1f, 1f);
			MapIO.SmoothHeightmap(1f, 1f);
			MapIO.SmoothHeightmap(1f, 1f);
			MapIO.SmoothHeightmap(1f, 1f);
			MapIO.SmoothHeightmap(1f, 1f);
			MapIO.SmoothHeightmap(1f, 1f);
			MapIO.SmoothHeightmap(1f, 1f);
			MapIO.SmoothHeightmap(1f, 1f);
			MapIO.SmoothHeightmap(1f, 1f);
			MapIO.SmoothHeightmap(1f, 1f);
			MapIO.SmoothHeightmap(1f, 1f);
			MapIO.SmoothHeightmap(1f, 1f);
			MapIO.SmoothHeightmap(1f, 1f);
			MapIO.SmoothHeightmap(1f, 1f);
			MapIO.SmoothHeightmap(1f, 1f);
			MapIO.SmoothHeightmap(1f, 1f);
			MapIO.SmoothHeightmap(1f, 1f);
			MapIO.SmoothHeightmap(1f, 1f);
			MapIO.SmoothHeightmap(1f, 1f);
			MapIO.SmoothHeightmap(1f, 1f);
			MapIO.SmoothHeightmap(1f, 1f);
			MapIO.SmoothHeightmap(1f, 1f);
			
			MapIO.SmoothHeightmap(1f, -1f);
			MapIO.SmoothHeightmap(1f, -1f);
			MapIO.SmoothHeightmap(1f, -1f);
			MapIO.SmoothHeightmap(1f, -1f);
			MapIO.SmoothHeightmap(1f, -1f);
			MapIO.SmoothHeightmap(1f, -1f);
			MapIO.SmoothHeightmap(1f, -1f);
			MapIO.SmoothHeightmap(1f, -1f);
			MapIO.SmoothHeightmap(1f, -1f);
			MapIO.SmoothHeightmap(1f, -1f);
			MapIO.SmoothHeightmap(1f, -1f);
			MapIO.SmoothHeightmap(1f, -1f);
			MapIO.SmoothHeightmap(1f, -1f);
			MapIO.SmoothHeightmap(1f, -1f);
			MapIO.SmoothHeightmap(1f, -1f);
			MapIO.SmoothHeightmap(1f, -1f);
			MapIO.SmoothHeightmap(1f, -1f);
			MapIO.SmoothHeightmap(1f, -1f);
			MapIO.SmoothHeightmap(1f, -1f);
			MapIO.SmoothHeightmap(1f, -1f);
			MapIO.SmoothHeightmap(1f, -1f);
			MapIO.SmoothHeightmap(1f, -1f);
			MapIO.SmoothHeightmap(1f, -1f);
			MapIO.SmoothHeightmap(1f, -1f);
			MapIO.SmoothHeightmap(1f, -1f);
			MapIO.SmoothHeightmap(1f, -1f);
			MapIO.SmoothHeightmap(1f, -1f);
			MapIO.SmoothHeightmap(1f, -1f);
			MapIO.SmoothHeightmap(1f, -1f);
			MapIO.SmoothHeightmap(1f, -1f);
			MapIO.SmoothHeightmap(1f, -1f);
			MapIO.SmoothHeightmap(1f, -1f);
			MapIO.SmoothHeightmap(1f, -1f);
			MapIO.SmoothHeightmap(1f, -1f);
			MapIO.SmoothHeightmap(1f, -1f);
			
			MapIO.ChangeLandLayer();
		}
		
		
		void WaterArenaHeightmap()
		{
			MapIO.NewEmptyTerrain(2000);
			MapIO.flattenWater(.5f);
			MapIO.perlinSaiyan(3, 50, 100);
			
			
			MapIO.diamondSquareNoise(20, 20, 20);
			

			
			MapIO.pucker(400, 150, .450f, 100, 0, true, 200);
			MapIO.unPucker(300,150, .495f, 300, 100, false, 0);
			MapIO.bluffTerracing(true, true, true, 1f, 500, 5, 5, 1, 2);
			MapIO.zNudge(1.5f);
			
			MapIO.SmoothHeightmap(1f, 1f);
			MapIO.SmoothHeightmap(1f, 1f);
			MapIO.SmoothHeightmap(1f, 1f);
			MapIO.SmoothHeightmap(1f, 1f);
			MapIO.SmoothHeightmap(1f, 1f);
			MapIO.SmoothHeightmap(1f, 1f);
			MapIO.SmoothHeightmap(1f, 1f);
			MapIO.SmoothHeightmap(1f, 1f);
			MapIO.SmoothHeightmap(1f, 1f);
			MapIO.SmoothHeightmap(1f, 1f);
			MapIO.SmoothHeightmap(1f, 1f);
			MapIO.SmoothHeightmap(1f, 1f);
			MapIO.SmoothHeightmap(1f, 1f);
			MapIO.SmoothHeightmap(1f, 1f);
			MapIO.SmoothHeightmap(1f, 1f);
			MapIO.SmoothHeightmap(1f, 1f);
			MapIO.SmoothHeightmap(1f, 1f);
			MapIO.SmoothHeightmap(1f, 1f);
			MapIO.SmoothHeightmap(1f, 1f);
			MapIO.SmoothHeightmap(1f, 1f);
			MapIO.SmoothHeightmap(1f, 1f);
			MapIO.SmoothHeightmap(1f, 1f);
			MapIO.SmoothHeightmap(1f, 1f);
			MapIO.SmoothHeightmap(1f, 1f);
			MapIO.SmoothHeightmap(1f, 1f);
			MapIO.SmoothHeightmap(1f, 1f);
			MapIO.SmoothHeightmap(1f, 1f);
			MapIO.SmoothHeightmap(1f, 1f);
			MapIO.SmoothHeightmap(1f, 1f);
			MapIO.SmoothHeightmap(1f, 1f);
			MapIO.SmoothHeightmap(1f, 1f);
			MapIO.SmoothHeightmap(1f, 1f);
			MapIO.SmoothHeightmap(1f, 1f);
			MapIO.SmoothHeightmap(1f, 1f);
			MapIO.SmoothHeightmap(1f, 1f);
			MapIO.SmoothHeightmap(1f, 1f);
			MapIO.SmoothHeightmap(1f, 1f);
			MapIO.SmoothHeightmap(1f, 1f);
			
			MapIO.SmoothHeightmap(1f, -1f);
			MapIO.SmoothHeightmap(1f, -1f);
			MapIO.SmoothHeightmap(1f, -1f);
			MapIO.SmoothHeightmap(1f, -1f);
			MapIO.SmoothHeightmap(1f, -1f);
			MapIO.SmoothHeightmap(1f, -1f);
			MapIO.SmoothHeightmap(1f, -1f);
			MapIO.SmoothHeightmap(1f, -1f);
			MapIO.SmoothHeightmap(1f, -1f);
			MapIO.SmoothHeightmap(1f, -1f);
			MapIO.SmoothHeightmap(1f, -1f);
			MapIO.SmoothHeightmap(1f, -1f);
			MapIO.SmoothHeightmap(1f, -1f);
			MapIO.SmoothHeightmap(1f, -1f);
			MapIO.SmoothHeightmap(1f, -1f);
			MapIO.SmoothHeightmap(1f, -1f);
			MapIO.SmoothHeightmap(1f, -1f);
			MapIO.SmoothHeightmap(1f, -1f);
			MapIO.SmoothHeightmap(1f, -1f);
			MapIO.SmoothHeightmap(1f, -1f);
			MapIO.SmoothHeightmap(1f, -1f);
			MapIO.SmoothHeightmap(1f, -1f);
			MapIO.SmoothHeightmap(1f, -1f);
			MapIO.SmoothHeightmap(1f, -1f);
			MapIO.SmoothHeightmap(1f, -1f);
			MapIO.SmoothHeightmap(1f, -1f);
			MapIO.SmoothHeightmap(1f, -1f);
			MapIO.SmoothHeightmap(1f, -1f);
			MapIO.SmoothHeightmap(1f, -1f);
			MapIO.SmoothHeightmap(1f, -1f);
			MapIO.SmoothHeightmap(1f, -1f);
			MapIO.SmoothHeightmap(1f, -1f);
			MapIO.SmoothHeightmap(1f, -1f);
			MapIO.SmoothHeightmap(1f, -1f);
			MapIO.SmoothHeightmap(1f, -1f);
			
			MapIO.ChangeLandLayer();
		}
		
		void HighwayHeightmap()
		{
			MapIO.NewEmptyTerrain(2000);
			MapIO.flattenWater(.5f);
			MapIO.perlinSaiyan(3, 50, 250);
			
			
			MapIO.diamondSquareNoise(20, 20, 20);
			

			
			MapIO.pucker(400, 150, .450f, 200, 0, true, 400);
			//MapIO.unPucker(600,250, .495f, 300, 100, false, 0);
			MapIO.bluffTerracing(true, true, true, 1f, 500, 5, 5, 1, 2);
			MapIO.zNudge(1.5f);
			
			MapIO.SmoothHeightmap(1f, 1f);
			MapIO.SmoothHeightmap(1f, 1f);
			MapIO.SmoothHeightmap(1f, 1f);
			MapIO.SmoothHeightmap(1f, 1f);
			MapIO.SmoothHeightmap(1f, 1f);
			MapIO.SmoothHeightmap(1f, 1f);
			MapIO.SmoothHeightmap(1f, 1f);
			MapIO.SmoothHeightmap(1f, 1f);
			MapIO.SmoothHeightmap(1f, 1f);
			MapIO.SmoothHeightmap(1f, 1f);
			MapIO.SmoothHeightmap(1f, 1f);
			MapIO.SmoothHeightmap(1f, 1f);
			MapIO.SmoothHeightmap(1f, 1f);
			MapIO.SmoothHeightmap(1f, 1f);
			MapIO.SmoothHeightmap(1f, 1f);
			MapIO.SmoothHeightmap(1f, 1f);
			MapIO.SmoothHeightmap(1f, 1f);
			MapIO.SmoothHeightmap(1f, 1f);
			MapIO.SmoothHeightmap(1f, 1f);
			MapIO.SmoothHeightmap(1f, 1f);
			MapIO.SmoothHeightmap(1f, 1f);
			MapIO.SmoothHeightmap(1f, 1f);
			MapIO.SmoothHeightmap(1f, 1f);
			MapIO.SmoothHeightmap(1f, 1f);
			MapIO.SmoothHeightmap(1f, 1f);
			MapIO.SmoothHeightmap(1f, 1f);
			MapIO.SmoothHeightmap(1f, 1f);
			MapIO.SmoothHeightmap(1f, 1f);
			MapIO.SmoothHeightmap(1f, 1f);
			MapIO.SmoothHeightmap(1f, 1f);
			MapIO.SmoothHeightmap(1f, 1f);
			MapIO.SmoothHeightmap(1f, 1f);
			MapIO.SmoothHeightmap(1f, 1f);
			MapIO.SmoothHeightmap(1f, 1f);
			MapIO.SmoothHeightmap(1f, 1f);
			MapIO.SmoothHeightmap(1f, 1f);
			MapIO.SmoothHeightmap(1f, 1f);
			MapIO.SmoothHeightmap(1f, 1f);
			
			MapIO.SmoothHeightmap(1f, -1f);
			MapIO.SmoothHeightmap(1f, -1f);
			MapIO.SmoothHeightmap(1f, -1f);
			MapIO.SmoothHeightmap(1f, -1f);
			MapIO.SmoothHeightmap(1f, -1f);
			MapIO.SmoothHeightmap(1f, -1f);
			MapIO.SmoothHeightmap(1f, -1f);
			MapIO.SmoothHeightmap(1f, -1f);
			MapIO.SmoothHeightmap(1f, -1f);
			MapIO.SmoothHeightmap(1f, -1f);
			MapIO.SmoothHeightmap(1f, -1f);
			MapIO.SmoothHeightmap(1f, -1f);
			MapIO.SmoothHeightmap(1f, -1f);
			MapIO.SmoothHeightmap(1f, -1f);
			MapIO.SmoothHeightmap(1f, -1f);
			MapIO.SmoothHeightmap(1f, -1f);
			MapIO.SmoothHeightmap(1f, -1f);
			MapIO.SmoothHeightmap(1f, -1f);
			MapIO.SmoothHeightmap(1f, -1f);
			MapIO.SmoothHeightmap(1f, -1f);
			MapIO.SmoothHeightmap(1f, -1f);
			MapIO.SmoothHeightmap(1f, -1f);
			MapIO.SmoothHeightmap(1f, -1f);
			MapIO.SmoothHeightmap(1f, -1f);
			MapIO.SmoothHeightmap(1f, -1f);
			MapIO.SmoothHeightmap(1f, -1f);
			MapIO.SmoothHeightmap(1f, -1f);
			MapIO.SmoothHeightmap(1f, -1f);
			MapIO.SmoothHeightmap(1f, -1f);
			MapIO.SmoothHeightmap(1f, -1f);
			MapIO.SmoothHeightmap(1f, -1f);
			MapIO.SmoothHeightmap(1f, -1f);
			MapIO.SmoothHeightmap(1f, -1f);
			MapIO.SmoothHeightmap(1f, -1f);
			MapIO.SmoothHeightmap(1f, -1f);
			
			MapIO.ChangeLandLayer();
		}


		void WaterworldClassicHeightmap()
		{
			/*
						script.flattenWater(.5f);
			
			//overly recursive perlin layer averaging madness
			//           layers, scaling period, initial scale
			script.perlinSaiyan(5, 20, 67);
						          
			//script.diamondSquareNoise(30, 10, 10);
			script.zNudge(50f);
			script.pucker(600, 600, .40f, 200, 500, true, 200);
			
			script.zNudge(200f);

			script.punch(.480f, 90);
			
			//dunes
			script.bluffTerracing(true, true, true, .9f, 505, 15, 20, 4, 7);
			//cliffs
			script.bluffTerracing(true, true, true, .9f, 565, 30, 40, 10, 2);
			
			script.punch(.495f, 120);
			
			script.pucker(600, 600, .475f, 200, 500, false, 0);
			
			script.zNudge(10f);
			script.unPucker(500,250, .495f, 450, 0, false, 0);
			//script.unPucker(200,100, .485f, 0, 0, false, 0);
			
			script.bluffTerracing(false, true, true, 1f, 497, 10, 10, 1, 0);
			
			*/
			MapIO.NewEmptyTerrain(3000);
			MapIO.flattenWater(.5f);
			MapIO.perlinSaiyan(3, 50, 120);
			
			
			MapIO.diamondSquareNoise(20, 20, 20);
			

			
			MapIO.pucker(800, 300, .450f, 0, 0, true, 200);
			MapIO.unPucker(600,250, .495f, 300, 100, false, 0);
			MapIO.bluffTerracing(true, true, true, 1f, 500, 5, 5, 1, 2);
			MapIO.zNudge(1.5f);
			
			
						//awesome I know
			MapIO.SmoothHeightmap(1f, 0f);
			MapIO.SmoothHeightmap(1f, 0f);
			MapIO.SmoothHeightmap(1f, 0f);
			MapIO.SmoothHeightmap(1f, 0f);
			MapIO.SmoothHeightmap(1f, 0f);
			MapIO.SmoothHeightmap(1f, 0f);
			MapIO.SmoothHeightmap(1f, 0f);
			MapIO.SmoothHeightmap(1f, 0f);
			MapIO.SmoothHeightmap(1f, 0f);
			MapIO.SmoothHeightmap(1f, 0f);
			MapIO.SmoothHeightmap(1f, 0f);
			MapIO.SmoothHeightmap(1f, 0f);
			MapIO.SmoothHeightmap(1f, 0f);
			MapIO.SmoothHeightmap(1f, 0f);
			MapIO.SmoothHeightmap(1f, 0f);
			MapIO.SmoothHeightmap(1f, 0f);
			MapIO.SmoothHeightmap(1f, 0f);
			MapIO.SmoothHeightmap(1f, 0f);
			MapIO.SmoothHeightmap(1f, 0f);
			MapIO.SmoothHeightmap(1f, 0f);
			MapIO.SmoothHeightmap(1f, 0f);
			MapIO.SmoothHeightmap(1f, 0f);
			MapIO.SmoothHeightmap(1f, 0f);
			MapIO.SmoothHeightmap(1f, 0f);
			MapIO.SmoothHeightmap(1f, 0f);
			MapIO.SmoothHeightmap(1f, 0f);
			MapIO.SmoothHeightmap(1f, 0f);
			MapIO.SmoothHeightmap(1f, 0f);
			MapIO.SmoothHeightmap(1f, 0f);
			MapIO.SmoothHeightmap(1f, 0f);
			MapIO.SmoothHeightmap(1f, 0f);
			MapIO.SmoothHeightmap(1f, 0f);
			MapIO.SmoothHeightmap(1f, 0f);
			MapIO.SmoothHeightmap(1f, 0f);
			MapIO.SmoothHeightmap(1f, 0f);
			MapIO.SmoothHeightmap(1f, 0f);
			MapIO.SmoothHeightmap(1f, 0f);
			MapIO.SmoothHeightmap(1f, 0f);
			MapIO.SmoothHeightmap(1f, 0f);
			MapIO.SmoothHeightmap(1f, 0f);
			MapIO.SmoothHeightmap(1f, 0f);
			MapIO.SmoothHeightmap(1f, 0f);
			MapIO.SmoothHeightmap(1f, 0f);
			MapIO.SmoothHeightmap(1f, 0f);
			MapIO.SmoothHeightmap(1f, 0f);
			MapIO.SmoothHeightmap(1f, 0f);
			MapIO.SmoothHeightmap(1f, 0f);
			MapIO.SmoothHeightmap(1f, 0f);
			MapIO.SmoothHeightmap(1f, 0f);
			MapIO.SmoothHeightmap(1f, 0f);
			MapIO.SmoothHeightmap(1f, 0f);
			MapIO.SmoothHeightmap(1f, 0f);
			MapIO.SmoothHeightmap(1f, 0f);
			MapIO.SmoothHeightmap(1f, 0f);
			MapIO.SmoothHeightmap(1f, 0f);
			MapIO.SmoothHeightmap(1f, 0f);
			MapIO.SmoothHeightmap(1f, 0f);
			MapIO.SmoothHeightmap(1f, 0f);
			MapIO.SmoothHeightmap(1f, 0f);
			MapIO.SmoothHeightmap(1f, 0f);
			MapIO.SmoothHeightmap(1f, 0f);
			MapIO.SmoothHeightmap(1f, 0f);
			MapIO.SmoothHeightmap(1f, 0f);
			MapIO.SmoothHeightmap(1f, 0f);
			MapIO.SmoothHeightmap(1f, 0f);
			MapIO.SmoothHeightmap(1f, 0f);
			MapIO.SmoothHeightmap(1f, 0f);
			MapIO.SmoothHeightmap(1f, 0f);
			MapIO.SmoothHeightmap(1f, 0f);
			MapIO.SmoothHeightmap(1f, 0f);
			MapIO.SmoothHeightmap(1f, 0f);
			MapIO.SmoothHeightmap(1f, 0f);
			
			
			
			
		}

		void WaterworldTopologies()
			{
			
			//mmmmmmmm
			
				MapIO.landLayer = "topology";
				
				//draw cliffs on rock
				MapIO.targetTerrainLayer = TerrainSplat.Enum.Rock;
				MapIO.topologyLayer = TerrainTopology.Enum.Cliff;
				MapIO.ChangeLandLayer();
            MapIO.terrainToTopology(.6f);

				
				
				//draw fields on grass
				MapIO.targetTerrainLayer = TerrainSplat.Enum.Grass;
				MapIO.topologyLayer = TerrainTopology.Enum.Field;
				MapIO.ChangeLandLayer();
            MapIO.terrainToTopology(.6f);

				
				
				//draw junkpiles on gravel
				MapIO.targetTerrainLayer = TerrainSplat.Enum.Gravel;
				MapIO.topologyLayer = TerrainTopology.Enum.Roadside;
				MapIO.ChangeLandLayer();
            MapIO.terrainToTopology(.6f);

				
				//outline junkpiles with forest
				MapIO.targetTopologyLayer = TerrainTopology.Enum.Roadside;
				MapIO.topologyLayer = TerrainTopology.Enum.Forest;
				MapIO.ChangeLandLayer();
            MapIO.paintTopologyOutline(5);

				
				
				//forest on forest
				MapIO.targetTerrainLayer = TerrainSplat.Enum.Forest;
				MapIO.topologyLayer = TerrainTopology.Enum.Forest;
				MapIO.ChangeLandLayer();
            MapIO.terrainToTopology(.7f);

				MapIO.topologyLayer = TerrainTopology.Enum.Forest;
				MapIO.ChangeLandLayer();
            MapIO.paintHeight(497,500);
				
				//mangroves
				MapIO.topologyLayer = TerrainTopology.Enum.Swamp;
				MapIO.ChangeLandLayer();
				MapIO.paintHeight(499,500);
				
				MapIO.topologyLayer = TerrainTopology.Enum.Forest;
				MapIO.ChangeLandLayer();
				MapIO.paintHeight(499,500);
				
				
				//draw junkpiles on stones
				MapIO.targetTerrainLayer = TerrainSplat.Enum.Stones;
				MapIO.topologyLayer = TerrainTopology.Enum.Roadside;
				MapIO.ChangeLandLayer();
            MapIO.terrainToTopology(.6f);

				
				//draw junkpiles on sand
				MapIO.targetTerrainLayer = TerrainSplat.Enum.Sand;
				MapIO.topologyLayer = TerrainTopology.Enum.Roadside;
				MapIO.ChangeLandLayer();
            MapIO.terrainToTopology(.6f);

				
				//draw junkpiles on dirt
				MapIO.targetTerrainLayer = TerrainSplat.Enum.Dirt;
				MapIO.topologyLayer = TerrainTopology.Enum.Roadside;
				MapIO.ChangeLandLayer();
            MapIO.terrainToTopology(.65f);
				
				
				//paint riversides
				MapIO.topologyLayer = TerrainTopology.Enum.Riverside;
				MapIO.ChangeLandLayer();
            MapIO.paintHeight(502,503);
				
				
				//erase junkpiles on beach or underwater
				MapIO.topologyLayer = TerrainTopology.Enum.Roadside;
				MapIO.ChangeLandLayer();
            MapIO.eraseHeight(0, 502);
				
				//erase underwater/beach forests
				MapIO.topologyLayer = TerrainTopology.Enum.Forest;
				MapIO.ChangeLandLayer();
            MapIO.eraseHeight(0, 502);
				
				//outline forest with forestside
				MapIO.targetTopologyLayer = TerrainTopology.Enum.Forest;
				MapIO.topologyLayer = TerrainTopology.Enum.Forestside;
				MapIO.ChangeLandLayer();
            MapIO.paintTopologyOutline(2);
				
				//outline cliffs with nodes
				MapIO.targetTopologyLayer = TerrainTopology.Enum.Cliff;
				MapIO.topologyLayer = TerrainTopology.Enum.Clutter;
				MapIO.ChangeLandLayer();
            MapIO.paintTopologyOutline(6);
				
				//erase nodes on beach or underwater
				MapIO.topologyLayer = TerrainTopology.Enum.Clutter;
				MapIO.ChangeLandLayer();
            MapIO.eraseHeight(0, 501);
				
				//minicopter spawns
				MapIO.topologyLayer = TerrainTopology.Enum.Road;
				MapIO.ChangeLandLayer();
            MapIO.paintHeight(510,515);
				
				//paint beaches
				MapIO.topologyLayer = TerrainTopology.Enum.Beach;
				MapIO.ChangeLandLayer();
            MapIO.paintHeight(500,502);
				
				//paint oceansides
				MapIO.targetTopologyLayer = TerrainTopology.Enum.Beach;
				MapIO.topologyLayer = TerrainTopology.Enum.Oceanside;
				MapIO.ChangeLandLayer();
            MapIO.copyTopologyLayer();
				
				//conflict resolutions
				
				//deletions for nodes
				//Cliff, Summit, Beachside, Beach, Ocean, Oceanside, Monument, Road, Roadside, Swamp, River, Riverside, Lake, Lakeside, Runway, Building
				MapIO.targetTopologyLayer = TerrainTopology.Enum.Clutter;
				MapIO.topologyLayer = TerrainTopology.Enum.Roadside;
				MapIO.ChangeLandLayer();
            MapIO.notTopologyLayer();
				
				MapIO.targetTopologyLayer = TerrainTopology.Enum.Clutter;
				MapIO.topologyLayer = TerrainTopology.Enum.Road;
				MapIO.ChangeLandLayer();
            MapIO.notTopologyLayer();
				
				MapIO.targetTopologyLayer = TerrainTopology.Enum.Clutter;
				MapIO.topologyLayer = TerrainTopology.Enum.Riverside;
				MapIO.ChangeLandLayer();
            MapIO.notTopologyLayer();
				
				MapIO.targetTopologyLayer = TerrainTopology.Enum.Clutter;
				MapIO.topologyLayer = TerrainTopology.Enum.Beach;
				MapIO.ChangeLandLayer();
            MapIO.notTopologyLayer();
				
				MapIO.targetTopologyLayer = TerrainTopology.Enum.Clutter;
				MapIO.topologyLayer = TerrainTopology.Enum.Oceanside;
				MapIO.ChangeLandLayer();
            MapIO.notTopologyLayer();
				
				//deletions for forest
				//Blocked Topology: Field, Cliff, Summit, Beachside, Beach, Forestside, Ocean, Road, Swamp, River, Lake, Offshore, Powerline, Runway, Building, Alt
				MapIO.targetTopologyLayer = TerrainTopology.Enum.Forest;
				MapIO.topologyLayer = TerrainTopology.Enum.Field;
				MapIO.ChangeLandLayer();
            MapIO.notTopologyLayer();
				
				MapIO.targetTopologyLayer = TerrainTopology.Enum.Forest;
				MapIO.topologyLayer = TerrainTopology.Enum.Road;
				MapIO.ChangeLandLayer();
            MapIO.notTopologyLayer();
				
				//deletions for junkpiles
				//Blocked Topology: Cliff, Summit, Beach, Ocean, Oceanside, Decor, Monument, Road, River, Lake, Offshore, Building, Cliffside, Mountain, Clutter
				MapIO.targetTopologyLayer = TerrainTopology.Enum.Roadside;
				MapIO.topologyLayer = TerrainTopology.Enum.Road;
				MapIO.ChangeLandLayer();
            MapIO.notTopologyLayer();
				
				MapIO.targetTopologyLayer = TerrainTopology.Enum.Roadside;
				MapIO.topologyLayer = TerrainTopology.Enum.Forest;
				MapIO.ChangeLandLayer();
            MapIO.notTopologyLayer();
				
				//copy nodes
				MapIO.targetTopologyLayer = TerrainTopology.Enum.Clutter;
				MapIO.topologyLayer = TerrainTopology.Enum.Cliffside;
				MapIO.ChangeLandLayer();
            MapIO.copyTopologyLayer();
				
				//copy nodes
				MapIO.targetTopologyLayer = TerrainTopology.Enum.Clutter;
				MapIO.topologyLayer = TerrainTopology.Enum.Decor;
				MapIO.ChangeLandLayer();
            MapIO.copyTopologyLayer();
				
				//copy junkpiles
				MapIO.targetTopologyLayer = TerrainTopology.Enum.Roadside;
				MapIO.topologyLayer = TerrainTopology.Enum.Powerline;
				MapIO.ChangeLandLayer();
            MapIO.copyTopologyLayer();
				
				//copy minicopter spawns
				MapIO.targetTopologyLayer = TerrainTopology.Enum.Road;
				MapIO.topologyLayer = TerrainTopology.Enum.Runway;
				MapIO.ChangeLandLayer();
            MapIO.copyTopologyLayer();
				
				//paint oceans
				MapIO.topologyLayer = TerrainTopology.Enum.Ocean;
				MapIO.ChangeLandLayer();
            MapIO.paintHeight(0,500);
				
				//paint offshore loot
				MapIO.topologyLayer = TerrainTopology.Enum.Offshore;
				MapIO.ChangeLandLayer();
            MapIO.paintHeight(480,493);
				
				//paint mainlands
				MapIO.topologyLayer = TerrainTopology.Enum.Mainland;
				MapIO.ChangeLandLayer();
            MapIO.paintHeight(500,1000);
				
				//paint t0
				MapIO.topologyLayer = TerrainTopology.Enum.Tier0;
				MapIO.ChangeLandLayer();
            MapIO.invertTopologyLayer();
				
				MapIO.oldTopologyLayer = MapIO.topologyLayer;
				
		}
		
		void ArenaTextures()
		{
			MapIO.landLayer ="biome";
			MapIO.PaintLayer("Biome", 0);
            //MapIO.biomeGradients(100,0,0,0);
			MapIO.landLayer ="ground";
				//paint Dirt on temperate
				MapIO.terrainLayer = TerrainSplat.Enum.Dirt;
				MapIO.targetBiomeLayer = TerrainBiome.Enum.Temperate;
            MapIO.paintPerlin(60, 1f, false, true);
				
				//paint stones on tundra
				MapIO.terrainLayer = TerrainSplat.Enum.Stones;
				MapIO.targetBiomeLayer = TerrainBiome.Enum.Tundra;
            MapIO.paintPerlin(30, 1f, false, true);
				
				//paint Sand on arid
				MapIO.terrainLayer = TerrainSplat.Enum.Sand;
				MapIO.targetBiomeLayer = TerrainBiome.Enum.Arid;
            MapIO.paintPerlin(40, 1f, false, true);
				
				//paint Forests
				MapIO.terrainLayer = TerrainSplat.Enum.Forest;
            //z is number of random zones, a is min size, a1 is max
            MapIO.paintCrazing(100, 500, 1000);
            MapIO.paintTerrainOutline(4, .667f);
				
				//paint Snow on arctic
				MapIO.terrainLayer = TerrainSplat.Enum.Snow;
				MapIO.targetBiomeLayer = TerrainBiome.Enum.Arctic;
            MapIO.paintPerlin(80, 3f, false, true);
				
				//paint gravel on arctic
				MapIO.terrainLayer = TerrainSplat.Enum.Gravel;
				MapIO.targetBiomeLayer = TerrainBiome.Enum.Arctic;
            MapIO.paintPerlin(40, 1f, false, true);
				
				//paint stones on steep areas
				MapIO.terrainLayer = TerrainSplat.Enum.Stones;
            MapIO.paintTerrainSlope(35,90);
				
				//paint Rock on less steep areas
				MapIO.terrainLayer = TerrainSplat.Enum.Rock;
            MapIO.paintTerrainSlope(38,90);
				
				//paint Sand under water
				MapIO.terrainLayer = TerrainSplat.Enum.Sand;
            MapIO.paintSplatHeight(0,500);
            MapIO.paintTerrainOutline(2, .667f);

            //expand beaches a bit
            MapIO.paintTerrainOutline(2, .66f);
			
			
		}
		
		void HighwayTextures()
		{
			MapIO.landLayer ="biome";
			MapIO.PaintLayer("Biome", 1);
            //MapIO.biomeGradients(100,0,0,0);
			MapIO.landLayer ="ground";
				//paint Dirt on temperate
				MapIO.terrainLayer = TerrainSplat.Enum.Dirt;
				MapIO.targetBiomeLayer = TerrainBiome.Enum.Temperate;
            MapIO.paintPerlin(60, 1f, false, true);
				
				//paint stones on tundra
				MapIO.terrainLayer = TerrainSplat.Enum.Stones;
				MapIO.targetBiomeLayer = TerrainBiome.Enum.Tundra;
            MapIO.paintPerlin(30, 1f, false, true);
				
				//paint Sand on arid
				MapIO.terrainLayer = TerrainSplat.Enum.Sand;
				MapIO.targetBiomeLayer = TerrainBiome.Enum.Arid;
            MapIO.paintPerlin(40, 1f, false, true);
				
				//paint Forests
				MapIO.terrainLayer = TerrainSplat.Enum.Forest;
            //z is number of random zones, a is min size, a1 is max
            MapIO.paintCrazing(100, 500, 1000);
            MapIO.paintTerrainOutline(4, .667f);
				
				//paint Snow on arctic
				MapIO.terrainLayer = TerrainSplat.Enum.Snow;
				MapIO.targetBiomeLayer = TerrainBiome.Enum.Arctic;
            MapIO.paintPerlin(80, 3f, false, true);
				
				//paint gravel on arctic
				MapIO.terrainLayer = TerrainSplat.Enum.Gravel;
				MapIO.targetBiomeLayer = TerrainBiome.Enum.Arctic;
            MapIO.paintPerlin(40, 1f, false, true);
				
				//paint stones on steep areas
				MapIO.terrainLayer = TerrainSplat.Enum.Stones;
            MapIO.paintTerrainSlope(35,90);
				
				//paint Rock on less steep areas
				MapIO.terrainLayer = TerrainSplat.Enum.Rock;
            MapIO.paintTerrainSlope(38,90);
				
				//paint Sand under water
				MapIO.terrainLayer = TerrainSplat.Enum.Sand;
            MapIO.paintSplatHeight(0,500);
            MapIO.paintTerrainOutline(2, .667f);

            //expand beaches a bit
            MapIO.paintTerrainOutline(2, .66f);
			
			
		}
		
				
		void ForestTextures()
		{
			MapIO.landLayer ="biome";
			MapIO.PaintLayer("Biome", 1);
            //MapIO.biomeGradients(100,0,0,0);
			MapIO.landLayer ="ground";
				//paint Dirt on temperate
				MapIO.terrainLayer = TerrainSplat.Enum.Dirt;
				MapIO.targetBiomeLayer = TerrainBiome.Enum.Temperate;
            MapIO.paintPerlin(60, 1f, false, true);
				
				//paint stones on tundra
				MapIO.terrainLayer = TerrainSplat.Enum.Stones;
				MapIO.targetBiomeLayer = TerrainBiome.Enum.Tundra;
            MapIO.paintPerlin(30, 1f, false, true);
				
				//paint Sand on arid
				MapIO.terrainLayer = TerrainSplat.Enum.Sand;
				MapIO.targetBiomeLayer = TerrainBiome.Enum.Arid;
            MapIO.paintPerlin(40, 1f, false, true);
				
				//paint Forests
				MapIO.terrainLayer = TerrainSplat.Enum.Forest;
            //z is number of random zones, a is min size, a1 is max
            MapIO.paintCrazing(800, 700, 1000);
            MapIO.paintTerrainOutline(4, .667f);
				
				//paint Snow on arctic
				MapIO.terrainLayer = TerrainSplat.Enum.Snow;
				MapIO.targetBiomeLayer = TerrainBiome.Enum.Arctic;
            MapIO.paintPerlin(80, 3f, false, true);
				
				//paint gravel on arctic
				MapIO.terrainLayer = TerrainSplat.Enum.Gravel;
				MapIO.targetBiomeLayer = TerrainBiome.Enum.Arctic;
            MapIO.paintPerlin(40, 1f, false, true);
				
				//paint stones on steep areas
				MapIO.terrainLayer = TerrainSplat.Enum.Stones;
            MapIO.paintTerrainSlope(35,90);
				
				//paint Rock on less steep areas
				MapIO.terrainLayer = TerrainSplat.Enum.Rock;
            MapIO.paintTerrainSlope(38,90);
				
				//paint Sand under water
				MapIO.terrainLayer = TerrainSplat.Enum.Sand;
            MapIO.paintSplatHeight(0,500);
            MapIO.paintTerrainOutline(2, .667f);

            //expand beaches a bit
            MapIO.paintTerrainOutline(2, .66f);
			
			
		}
		
		void WaterworldTextures()
		{
			MapIO.landLayer ="biome";
            MapIO.biomeGradients(25,25,40,40);
			MapIO.landLayer ="ground";
				//paint Dirt on temperate
				MapIO.terrainLayer = TerrainSplat.Enum.Dirt;
				MapIO.targetBiomeLayer = TerrainBiome.Enum.Temperate;
            MapIO.paintPerlin(60, 1f, false, true);
				
				//paint stones on tundra
				MapIO.terrainLayer = TerrainSplat.Enum.Stones;
				MapIO.targetBiomeLayer = TerrainBiome.Enum.Tundra;
            MapIO.paintPerlin(30, 1f, false, true);
				
				//paint Sand on arid
				MapIO.terrainLayer = TerrainSplat.Enum.Sand;
				MapIO.targetBiomeLayer = TerrainBiome.Enum.Arid;
            MapIO.paintPerlin(40, 1f, false, true);
				
				//paint Forests
				MapIO.terrainLayer = TerrainSplat.Enum.Forest;
            //z is number of random zones, a is min size, a1 is max
            MapIO.paintCrazing(1500, 1000, 2000);
            MapIO.paintTerrainOutline(4, .667f);
				
				//paint Snow on arctic
				MapIO.terrainLayer = TerrainSplat.Enum.Snow;
				MapIO.targetBiomeLayer = TerrainBiome.Enum.Arctic;
            MapIO.paintPerlin(80, 3f, false, true);
				
				//paint gravel on arctic
				MapIO.terrainLayer = TerrainSplat.Enum.Gravel;
				MapIO.targetBiomeLayer = TerrainBiome.Enum.Arctic;
            MapIO.paintPerlin(40, 1f, false, true);
				
				//paint stones on steep areas
				MapIO.terrainLayer = TerrainSplat.Enum.Stones;
            MapIO.paintTerrainSlope(35,90);
				
				//paint Rock on less steep areas
				MapIO.terrainLayer = TerrainSplat.Enum.Rock;
            MapIO.paintTerrainSlope(38,90);
				
				//paint Sand under water
				MapIO.terrainLayer = TerrainSplat.Enum.Sand;
            MapIO.paintSplatHeight(0,500);
            MapIO.paintTerrainOutline(2, .667f);

            //expand beaches a bit
            MapIO.paintTerrainOutline(2, .66f);
			
			
		}
		
		void newWaterworldMonuments()
		{
			MapIO.landLayer = "topology";
			MapIO.topologyLayer = TerrainTopology.Enum.Monument;
			MapIO.ChangeLandLayer();
			
			var blob = new WorldSerialization();
			//public static void randomMonument(WorldSerialization blob, int xMin, int xMax, int yMin, int yMax, int zMin, int zMax, bool water)
			blob.Load("Monuments/4096/outpost.monument.map");
            MapIO.randomMonument(blob, 0, 3000, 0, 3000, 400, 1000, true);
		}
		
		void WaterworldClassicMonuments()
		{
			MapIO.landLayer = "topology";
			MapIO.topologyLayer = TerrainTopology.Enum.Monument;
			MapIO.ChangeLandLayer();
			
			var blob = new WorldSerialization();
			//public static void randomMonument(WorldSerialization blob, int xMin, int xMax, int yMin, int yMax, int zMin, int zMax, bool water)
			blob.Load("Monuments/3000/ship.monument.map");
            MapIO.randomMonument(blob, 1000, 2000, 1000, 2000, 400, 500, true);
			
			blob.Load("Monuments/3000/oilrig.monument.map");
            MapIO.randomMonument(blob, 1000, 2000, 1000, 2000, 400, 500, true);
			
			blob.Load("Monuments/3000/sunkenDome.monument.map");
            MapIO.randomMonument(blob, 1000, 2000, 1000, 2000, 400, 500, true);
			
			blob.Load("Monuments/3000/artilleryTower.monument.map");
            MapIO.randomMonument(blob, 1000, 2000, 1000, 2000, 400, 500, true);
			
			blob.Load("Monuments/3000/artilleryTower.monument.map");
            MapIO.randomMonument(blob, 1000, 2000, 1000, 2000, 400, 500, true);
			
			blob.Load("Monuments/3000/seaLand.monument.map");
            MapIO.randomMonument(blob, 1000, 2000, 1000, 2000, 400, 500, true);
			
			blob.Load("Monuments/3000/seaLand.monument.map");
            MapIO.randomMonument(blob, 1000, 2000, 1000, 2000, 400, 500, true);
			
			blob.Load("Monuments/3000/seaLand.monument.map");
            MapIO.randomMonument(blob, 1000, 2000, 1000, 2000, 400, 500, true);
			
			blob.Load("Monuments/3000/texasTower.monument.map");
            MapIO.randomMonument(blob, 1000, 2000, 1000, 2000, 400, 500, true);
			
			blob.Load("Monuments/3000/texasTower.monument.map");
            MapIO.randomMonument(blob, 1000, 2000, 1000, 2000, 400, 500, true);
						
			blob.Load("Monuments/3000/texasTower.monument.map");
            MapIO.randomMonument(blob, 1000, 2000, 1000, 2000, 400, 500, true);
			
			blob.Load("Monuments/3000/easterIsland.monument.map");
            MapIO.randomMonument(blob, 2000, 2500, 0000, 2000, 498, 505, false);
			
			blob.Load("Monuments/3000/easterIsland.monument.map");
            MapIO.randomMonument(blob, 2000, 2500, 0000, 2000, 498, 505, false);
			
			blob.Load("Monuments/3000/easterIsland.monument.map");
            MapIO.randomMonument(blob, 1000, 2500, 0000, 2000, 498, 505, false);
			
			blob.Load("Monuments/3000/easterIsland.monument.map");
            MapIO.randomMonument(blob, 1000, 2500, 0000, 2000, 498, 505, false);
			
			blob.Load("Monuments/3000/easterIsland.monument.map");
            MapIO.randomMonument(blob, 0000, 1000, 0000, 2000, 498, 505, false);
			
			blob.Load("Monuments/3000/easterIsland.monument.map");
            MapIO.randomMonument(blob, 0000, 1000, 1000, 2500, 498, 505, false);
			
			blob.Load("Monuments/3000/easterIsland.monument.map");
            MapIO.randomMonument(blob, 000, 1000, 1000, 2500, 498, 505, false);
			
			blob.Load("Monuments/3000/easterIsland.monument.map");
            MapIO.randomMonument(blob, 000, 2000, 1000, 2500, 498, 505, false);
			
			blob.Load("Monuments/3000/easterIsland.monument.map");
            MapIO.randomMonument(blob, 000, 2000, 1000, 2500, 498, 505, false);
		}
		
		void WaterworldMonuments()
		{
			MapIO.landLayer = "topology";
			MapIO.topologyLayer = TerrainTopology.Enum.Monument;
			MapIO.ChangeLandLayer();
			
			var blob = new WorldSerialization();
			
			//npc monuments
			blob.Load("Monuments/4096/outpost.monument.map");
            MapIO.randomMonument(blob, 1500, 2500, 1500, 2500, 500, 1000, false);
			
			blob.Load("Monuments/4096/banditcamp.monument.map");
            MapIO.randomMonument(blob, 1000,3000,1000,3000,480,500, true);
			

			
			//tier two
			blob.Load("Monuments/4096/trainyard.monument.map");
            MapIO.randomMonument(blob, 750, 3100, 750, 3100, 480, 1000, true);
			
			blob.Load("Monuments/4096/watertreatment.monument.map");
            MapIO.randomMonument(blob, 750, 3100, 750, 3100, 480, 1000, true);
			
			blob.Load("Monuments/4096/sunkendome.monument.map");
            MapIO.randomMonument(blob, 750, 3100, 750, 3100, 400, 510, true);		
			
			//tier three
			blob.Load("Monuments/4096/launch.monument.map");
            MapIO.randomMonument(blob, 750, 3000, 750, 3000, 480, 1000, true);
			
			blob.Load("Monuments/4096/oilriglarge.monument.map");
            MapIO.randomMonument(blob, 3700, 3800, 3700, 3800, 0, 500, true);
			
			blob.Load("Monuments/4096/oilrigsmall.monument.map");
            MapIO.randomMonument(blob, 0, 300, 0, 300, 0, 500, true);

			//tier one			
			blob.Load("Monuments/4096/oxum.monument.map");
            MapIO.randomMonument(blob, 2000, 3000, 2000, 3000, 500, 1000, false);
						
			blob.Load("Monuments/4096/oxum.monument.map");
            MapIO.randomMonument(blob, 1000, 2000, 1000, 2000, 500, 1000, false);
			
			blob.Load("Monuments/4096/supermarket.monument.map");
            MapIO.randomMonument(blob, 2000, 3000, 2000, 3000, 500, 1000, false);
			
			blob.Load("Monuments/4096/supermarket.monument.map");
            MapIO.randomMonument(blob, 1000, 2000, 2000, 3000, 500, 1000, false);
			
			blob.Load("Monuments/4096/supermarket.monument.map");
            MapIO.randomMonument(blob, 2000, 3000, 1000, 2000, 500, 1000, false);
			
			//water pumper
						
			blob.Load("Monuments/4096/waterpumper.monument.map");
            MapIO.randomMonument(blob, 500,2000,500,3100,500,1000, false);
		
			blob.Load("Monuments/4096/waterpumper.monument.map");
            MapIO.randomMonument(blob, 2000,3000,500,3000,500,1000, false);
			
			
			blob.Load("Monuments/4096/tiltlighthouse.monument.map");
            MapIO.randomMonument(blob, 2000, 3000, 2000, 3000, 480, 520, true);
			blob.Load("Monuments/4096/tiltlighthouse.monument.map");
            MapIO.randomMonument(blob, 1000, 2000, 2000, 3000, 480, 520, true);
			blob.Load("Monuments/4096/tiltlighthouse.monument.map");
            MapIO.randomMonument(blob, 2000, 3000, 1000, 2000, 480, 520, true);
			
			blob.Load("Monuments/4096/easterislandbubbler.monument.map");
            MapIO.randomMonument(blob, 2000, 3000, 2000, 3000, 500, 1000, false);
			blob.Load("Monuments/4096/easterislandbubbler.monument.map");
            MapIO.randomMonument(blob, 1000, 2000, 2000, 3000, 500, 1000, false);
			blob.Load("Monuments/4096/easterislandbubbler.monument.map");
            MapIO.randomMonument(blob, 2000, 3000, 1000, 2000, 500, 1000, false);
			blob.Load("Monuments/4096/easterislandbubbler.monument.map");
            MapIO.randomMonument(blob, 2000, 3000, 1000, 2000, 500, 1000, false);
			blob.Load("Monuments/4096/easterislandbubbler.monument.map");
            MapIO.randomMonument(blob, 2000, 3000, 1000, 2000, 500, 1000, false);
		
			
		
		}
		
		void WaterworldFinalizing()
		{
			
						//select topology layers
			MapIO.landLayer = "topology";	MapIO.ChangeLandLayer(); Repaint(); //rreeeee paint
		
			//erase underwater monument topology for better cliff prefabs
			MapIO.topologyLayer = TerrainTopology.Enum.Monument;
			MapIO.ChangeLandLayer();
            MapIO.eraseHeight(0, 500);
			
			//mapbreaking if any nodes spawn underwater
			MapIO.topologyLayer = TerrainTopology.Enum.Cliffside;
			MapIO.ChangeLandLayer();
            MapIO.eraseHeight(0, 500);
			
			MapIO.topologyLayer = TerrainTopology.Enum.Clutter;
			MapIO.ChangeLandLayer();
            MapIO.eraseHeight(0, 500);
			
			MapIO.topologyLayer = TerrainTopology.Enum.Decor;
			MapIO.ChangeLandLayer();
            MapIO.eraseHeight(0, 500);
			
			
			//make sure the weeds aint too high
			MapIO.topologyLayer = TerrainTopology.Enum.Offshore;
			MapIO.ChangeLandLayer();
            MapIO.eraseHeight(493, 1000);
			
			
			//make sure the fields dont get underwater
			MapIO.topologyLayer = TerrainTopology.Enum.Field;
			MapIO.ChangeLandLayer();
            MapIO.eraseHeight(0, 501);
			
			//make sure the junkpiles dont get underwater
			MapIO.topologyLayer = TerrainTopology.Enum.Roadside;
			MapIO.ChangeLandLayer();
            MapIO.eraseHeight(0, 501);
			
			MapIO.topologyLayer = TerrainTopology.Enum.Powerline;
			MapIO.ChangeLandLayer();
            MapIO.eraseHeight(0, 501);
			
			//no underwater minicopters
			MapIO.topologyLayer = TerrainTopology.Enum.Road;
			MapIO.ChangeLandLayer();
            MapIO.eraseHeight(0, 501);
			
			MapIO.oldTopologyLayer = MapIO.topologyLayer;
		
		}
		
		void WaterworldCliffs()
		{
			//To customize cliff prefabs, create a cliffset.map file, map size 4096
			
			//Place all the cliffs you wish to use with their origin point directly on the southwest origin.
			
			//Scale and rotate as you see fit
			
			//The 'cliffset index' match the order in which the prefabs are placed, beginning with 0
			//A good place to start for the 'z Offset' is -1/2 of the total height of the prefab, this may take fine tuning
			
			//The specified cliff is then placed within the specified range of slopes, automatically avoid 'monument' topologies, only above the 'lowest height boundary' & the density can be tuned.
			//If you are getting null results or only a few cliffs that probably means your density is too low.
			
			//NB: cliffset sizes lower than 4096 are fine, but cliffsets MUST match the initial map size or else the cliffs will be off register.
			
			MapIO.topologyLayer = TerrainTopology.Enum.Monument;
			MapIO.ChangeLandLayer();
			
			//this controls randomized rotations
			Vector3 rotters = new Vector3(10, 300, 10);
			var blob = new WorldSerialization();
			
			blob.Load("monuments/3000/cliffset.monument.map");

            //blob, random rotation ranges, slope lower bound, slope higher bound, cliffset index,  prefab z Offset,
            //cliff density from 0-100, lowest height boundary 0-1000

            MapIO.insertPrefabCliffs(blob,  rotters, 65, 90, 1, -16.4f,50,480f);
            MapIO.insertPrefabCliffs(blob,  rotters, 40, 65, 1, -16.4f,55,495f);
            MapIO.insertPrefabCliffs(blob,  rotters, 30, 40, 1, -16.4f,60,495f);
            MapIO.insertPrefabCliffs(blob,  rotters, 25, 30, 0, -6.4f,65,495f);
		}
		
        #endregion
        #region InspectorGUIInput
        Event e = Event.current;
        #endregion
        EditorGUILayout.EndScrollView();
    }
    #region OtherMenus
    [MenuItem("Rust Map Editor/Terrain Tools", false, 1)]
    static void OpenTerrainTools()
    {
        Selection.activeGameObject = GameObject.FindGameObjectWithTag("Land");
    }
    [MenuItem("Rust Map Editor/Wiki", false, 2)]
    static void OpenWiki()
    {
        Application.OpenURL("https://github.com/RustMapMaking/Rust-Map-Editor-Unity/wiki");
    }
    [MenuItem("Rust Map Editor/Discord", false, 3)]
    static void OpenDiscord()
    {
        Application.OpenURL("https://discord.gg/HPmTWVa");
    }
    #endregion
    #region Methods
    private string NodePresetDrawer(Rect position, string itemValue)
    {
        position.width -= 39;
        GUI.Label(position, itemValue);
        position.x = position.xMax;
        position.width = 39;
        if (GUI.Button(position, new GUIContent("Open", "Opens the Node Editor for the preset.")))
        {
            MapIO.RefreshAssetList();
            MapIO.nodePresetLookup.TryGetValue(itemValue, out Object preset);
            if (preset != null)
            {
                AssetDatabase.OpenAsset(preset.GetInstanceID());
            }
            else
            {
                Debug.LogError("The preset you are trying to open is null.");
            }
        }
        /*
        position.x = position.x + 40;
        position.width = 30;
        if (GUI.Button(position, "Run"))
        {
            MapIO.nodePresetLookup.TryGetValue(itemValue, out Object preset);
            if (preset != null)
            {
                var graph = (XNode.NodeGraph)AssetDatabase.LoadAssetAtPath(assetDirectory + itemValue + ".asset", typeof(XNode.NodeGraph));
                MapIO.ParseNodeGraph(graph);
            }
        }
        */
        return itemValue;
    }
    private void DrawEmpty()
    {
        GUILayout.Label("No presets in list.", EditorStyles.miniLabel);
    }
    #endregion
	
	void DoToolbar()
    {
        GUILayout.BeginHorizontal(EditorStyles.toolbar);
        GUILayout.Space(100);
        GUILayout.FlexibleSpace();
        m_TreeView.searchString = m_SearchField.OnToolbarGUI(m_TreeView.searchString);
        GUILayout.EndHorizontal();
    }
    void DoTreeView()
    {
        Rect rect = GUILayoutUtility.GetRect(0, 100000, 0, 100000);
        m_TreeView.OnGUI(rect);
    }
	
	
	
}
public class PrefabHierachyEditor : EditorWindow
{
    [SerializeField] TreeViewState m_TreeViewState;

    PrefabHierachy m_TreeView;
    SearchField m_SearchField;

    void OnEnable()
    {
        if (m_TreeViewState == null)
            m_TreeViewState = new TreeViewState();

        m_TreeView = new PrefabHierachy(m_TreeViewState);
        m_SearchField = new SearchField();
        m_SearchField.downOrUpArrowKeyPressed += m_TreeView.SetFocusAndEnsureSelectedItem;
    }
    void OnGUI()
    {
        DoToolbar();
        DoTreeView();
    }
    void DoToolbar()
    {
        GUILayout.BeginHorizontal(EditorStyles.toolbar);
        GUILayout.Space(100);
        GUILayout.FlexibleSpace();
        m_TreeView.searchString = m_SearchField.OnToolbarGUI(m_TreeView.searchString);
        GUILayout.EndHorizontal();
    }
    void DoTreeView()
    {
        Rect rect = GUILayoutUtility.GetRect(0, 100000, 0, 100000);
        m_TreeView.OnGUI(rect);
    }
    public static void ShowWindow()
    {
        var window = GetWindow<PrefabHierachyEditor>();
        window.titleContent = new GUIContent("Prefabs");
        window.Show();
    }
}
