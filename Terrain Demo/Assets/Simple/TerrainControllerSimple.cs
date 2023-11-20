using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TerrainControllerSimple : MonoBehaviour {

    /*
     * Variables that can be controlled by editor 
     */ 
    [SerializeField]
    private GameObject terrainTilePrefab = null;
    [SerializeField]
    private Vector3 terrainSize = new Vector3(20, 1, 20);
    [SerializeField]
    private Gradient gradient;
    [SerializeField]
    private float noiseScale = 3, cellSize = 1;
    [SerializeField]
    private int radiusToRender = 5;
    [SerializeField]
    private Transform[] gameTransforms;
    [SerializeField]
    private Transform playerTransform;

    // startOffset = where on perlin noise that it needs to start 
    // pick a coordinate -> get value btwn 0-1 in that place to get height of terrain at the place  
    private Vector2 startOffset;
    // when you input a coordinate -> you get a tile 
    private Dictionary<Vector2, GameObject> terrainTiles = new Dictionary<Vector2, GameObject>();
    
    // these two are here for performance reasons 
    // prev center tiles = center tiles from the frame before 
    private Vector2[] previousCenterTiles;
    // all the tile objects that should be enabled from the frame before 
    private List<GameObject> previousTileObjects = new List<GameObject>();

    private void Start() {
        InitialLoad();
    }

    /*
     * InitialLoad(): 
     * destroy terrain if exists
     * pick value btwn 0, 256 to pick in perlin noise 
     */  
    public void InitialLoad() {
        DestroyTerrain();

        //choose a place on perlin noise (which loops after 256)
        startOffset = new Vector2(Random.Range(0f, 256f), Random.Range(0f, 256f));
    }

    private void Update() {
        //save the tile the player is on
        Vector2 playerTile = TileFromPosition(playerTransform.position);
        //save the tiles of all tracked objects in gameTransforms (including the player)
        List<Vector2> centerTiles = new List<Vector2>();
        centerTiles.Add(playerTile);
        // physics based objects w radius 1 that you don't want to fall in the world 
        // this includes player tile 
        foreach (Transform t in gameTransforms)
            centerTiles.Add(TileFromPosition(t.position));

        //if no tiles exist yet or tiles should change
        if (previousCenterTiles == null || HaveTilesChanged(centerTiles)) {
            List<GameObject> tileObjects = new List<GameObject>();
            //activate new tiles
            foreach (Vector2 tile in centerTiles) {
                bool isPlayerTile = tile == playerTile;
                // if is player -> use radius to render
                // if not player -> use radius of 1 
                int radius = isPlayerTile ? radiusToRender : 1;
                // range of -radius to radius 
                // get a square of tiles around the player based on radius input 
                for (int i = -radius; i <= radius; i++)
                    for (int j = -radius; j <= radius; j++)
                        // activates created tile or creates if not existing already 
                        ActivateOrCreateTile((int)tile.x + i, (int)tile.y + j, tileObjects);
            }
            //deactivate old tiles
            foreach (GameObject g in previousTileObjects)
                if (!tileObjects.Contains(g))
                    g.SetActive(false);

            previousTileObjects = new List<GameObject>(tileObjects);
        }

        previousCenterTiles = centerTiles.ToArray();
    }

    //Helper methods below
    // takes in list of tile objects to activate 
    // if tile doesn't already exist in dictionary, we add it 
    private void ActivateOrCreateTile(int xIndex, int yIndex, List<GameObject> tileObjects) {
        if (!terrainTiles.ContainsKey(new Vector2(xIndex, yIndex))) {
            tileObjects.Add(CreateTile(xIndex, yIndex));
        } else {
            GameObject t = terrainTiles[new Vector2(xIndex, yIndex)];
            tileObjects.Add(t);
            if (!t.activeSelf)
                t.SetActive(true);
        }
    }
    
    // instantiates object (tile prefab) at the input position 
    private GameObject CreateTile(int xIndex, int yIndex) {
        GameObject terrain = Instantiate(
            terrainTilePrefab,
            new Vector3(terrainSize.x * xIndex, terrainSize.y, terrainSize.z * yIndex),
            Quaternion.identity
        );
        terrain.name = TrimEnd(terrain.name, "(Clone)") + " [" + xIndex + " , " + yIndex + "]";

        terrainTiles.Add(new Vector2(xIndex, yIndex), terrain);

        GenerateMeshSimple gm = terrain.GetComponent<GenerateMeshSimple>();
        gm.TerrainSize = terrainSize;
        gm.Gradient = gradient;
        gm.NoiseScale = noiseScale;
        gm.CellSize = cellSize;
        gm.NoiseOffset = NoiseOffset(xIndex, yIndex);
        gm.Generate();

        return terrain;
    }

    // pick a place on perlin noise given input indices
    private Vector2 NoiseOffset(int xIndex, int yIndex) {
        Vector2 noiseOffset = new Vector2(
            (xIndex * noiseScale + startOffset.x) % 256,
            (yIndex * noiseScale + startOffset.y) % 256
        );
        //account for negatives (ex. -1 % 256 = -1). needs to loop around to 255
        if (noiseOffset.x < 0)
            noiseOffset = new Vector2(noiseOffset.x + 256, noiseOffset.y);
        if (noiseOffset.y < 0)
            noiseOffset = new Vector2(noiseOffset.x, noiseOffset.y + 256);
        return noiseOffset;
    }

    private Vector2 TileFromPosition(Vector3 position) {
        return new Vector2(Mathf.FloorToInt(position.x / terrainSize.x + .5f), Mathf.FloorToInt(position.z / terrainSize.z + .5f));
    }

    // compares lists
    private bool HaveTilesChanged(List<Vector2> centerTiles) {
        if (previousCenterTiles.Length != centerTiles.Count)
            return true;
        for (int i = 0; i < previousCenterTiles.Length; i++)
            if (previousCenterTiles[i] != centerTiles[i])
                return true;
        return false;
    }

    public void DestroyTerrain() {
        foreach (KeyValuePair<Vector2, GameObject> kv in terrainTiles)
            Destroy(kv.Value);
        terrainTiles.Clear();
    }

    private static string TrimEnd(string str, string end) {
        if (str.EndsWith(end))
            return str.Substring(0, str.LastIndexOf(end));
        return str;
    }

}