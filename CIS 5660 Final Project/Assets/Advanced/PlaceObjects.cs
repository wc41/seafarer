using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst.CompilerServices;
using UnityEngine;

[RequireComponent(typeof(GenerateMesh))]
public class PlaceObjects : MonoBehaviour {

    public TerrainController TerrainController { get; set; }

    public void Place() {
        // random num objects btwn the min and max number of objects per tile 
        int numObjects = Random.Range(TerrainController.MinObjectsPerTile, TerrainController.MaxObjectsPerTile);
        for (int i = 0; i < numObjects; i++) {
            // pick random prefab stored in pleaceable objects in terrain controller
            int prefabType = Random.Range(0, TerrainController.PlaceableObjects.Length);
            Vector3 startPoint = RandomPointAboveTerrain();
            Vector3 startPoint1 = new Vector3(startPoint.x + 5, startPoint.y, startPoint.z);
            Vector3 startPoint2 = new Vector3(startPoint.x + 5, startPoint.y, startPoint.z + 5);
            Vector3 startPoint3 = new Vector3(startPoint.x, startPoint.y, startPoint.z + 5);
            Vector3 startPoint4 = new Vector3(startPoint.x + 5, startPoint.y, startPoint.z - 5);
            Vector3 startPoint5 = new Vector3(startPoint.x - 5, startPoint.y, startPoint.z);
            Vector3 startPoint6 = new Vector3(startPoint.x - 5, startPoint.y, startPoint.z + 5);
            Vector3 startPoint7 = new Vector3(startPoint.x, startPoint.y, startPoint.z - 5);
            Vector3 startPoint8 = new Vector3(startPoint.x - 5, startPoint.y, startPoint.z - 5);

            RaycastHit hit;
            RaycastHit hit1;

            RaycastHit hit2;
            RaycastHit hit3;
            RaycastHit hit4;
            RaycastHit hit5;
            RaycastHit hit6;
            RaycastHit hit7;
            RaycastHit hit8;

            if (Physics.Raycast(startPoint, Vector3.down, out hit) 
                && Physics.Raycast(startPoint1, Vector3.down, out hit1)
                && Physics.Raycast(startPoint2, Vector3.down, out hit2)
                && Physics.Raycast(startPoint3, Vector3.down, out hit3)
                && Physics.Raycast(startPoint4, Vector3.down, out hit4)
                && Physics.Raycast(startPoint5, Vector3.down, out hit5)
                && Physics.Raycast(startPoint6, Vector3.down, out hit6)
                && Physics.Raycast(startPoint7, Vector3.down, out hit7)
                && Physics.Raycast(startPoint8, Vector3.down, out hit8)
                && hit.point.y > TerrainController.Water.transform.position.y && hit.collider.CompareTag("Terrain")
                && hit1.point.y > TerrainController.Water.transform.position.y && hit1.collider.CompareTag("Terrain")
                && hit2.point.y > TerrainController.Water.transform.position.y && hit2.collider.CompareTag("Terrain")
                && hit3.point.y > TerrainController.Water.transform.position.y && hit3.collider.CompareTag("Terrain")
                && hit4.point.y > TerrainController.Water.transform.position.y && hit4.collider.CompareTag("Terrain")
                && hit5.point.y > TerrainController.Water.transform.position.y && hit5.collider.CompareTag("Terrain")
                && hit6.point.y > TerrainController.Water.transform.position.y && hit6.collider.CompareTag("Terrain")
                && hit7.point.y > TerrainController.Water.transform.position.y && hit7.collider.CompareTag("Terrain")
                && hit8.point.y > TerrainController.Water.transform.position.y && hit8.collider.CompareTag("Terrain")) {

                Quaternion orientation = Quaternion.Euler(Vector3.up * Random.Range(0f, 360f));
                RaycastHit boxHit;
                // get object size 
                GameObject objectToPlace = TerrainController.PlaceableObjects[prefabType];
                Vector3 objectSize = objectToPlace.transform.localScale; 

                if (Physics.BoxCast(startPoint, objectSize, Vector3.down, out boxHit, orientation) && boxHit.collider.CompareTag("Terrain"))
                {
                    // check object tags
                    if (objectToPlace.tag == "Pine Tree" || objectToPlace.tag == "Mushroom Tree" || objectToPlace.tag == "Swirl Tree" || objectToPlace.tag == "Palm Tree")
                    {
                        // change orientation for trees
                        Vector3 fixRotation = new Vector3(-90, 0, 0);
                        orientation = Quaternion.Euler(fixRotation);
                    }

                    // instantiating rocks 
                    if (objectToPlace.tag == "Rock")
                    {
                        // Instantiate(TerrainController.PlaceableObjects[prefabType], new Vector3(startPoint.x - 3, hit.point.y - 12, startPoint.z - 5), orientation, transform);
                        Instantiate(TerrainController.PlaceableObjects[prefabType], new Vector3(startPoint.x, hit.point.y - 5, startPoint.z), orientation, transform);
                    }

                    // cliff trees 
                    if (objectToPlace.tag == "Pine Tree" || objectToPlace.tag == "Mushroom Tree" || objectToPlace.tag == "Swirl Tree")
                    {
                        // check if above rock coast line 
                        if (hit.point.y > 20.0f)
                        {
                            Instantiate(TerrainController.PlaceableObjects[prefabType], new Vector3(startPoint.x, hit.point.y, startPoint.z), orientation, transform);
                        }
                    }

                    // instantiating palm tree for beach biome 
                    if (objectToPlace.tag == "Palm Tree")
                    {
                        Instantiate(TerrainController.PlaceableObjects[prefabType], new Vector3(startPoint.x, hit.point.y, startPoint.z), orientation, transform);
                    }

                }

                //Debug code. To use, uncomment the giant thingy below
                //Debug.DrawRay(startPoint, Vector3.down * 10000, Color.blue);
                //DrawBoxCastBox(startPoint, TerrainController.PlaceableObjectSizes[prefabType], orientation, Vector3.down, 10000, Color.red);
                //UnityEditor.EditorApplication.isPaused = true;
            }

        }
    }

    // get a position above terrain 
    private Vector3 RandomPointAboveTerrain() {
        return new Vector3(
            Random.Range(transform.position.x - TerrainController.TerrainSize.x / 2, transform.position.x + TerrainController.TerrainSize.x / 2),
            transform.position.y + TerrainController.TerrainSize.y * 2,
            Random.Range(transform.position.z - TerrainController.TerrainSize.z / 2, transform.position.z + TerrainController.TerrainSize.z / 2)
        );
    }

    //code to help visualize the boxcast
    //source: https://answers.unity.com/questions/1156087/how-can-you-visualize-a-boxcast-boxcheck-etc.html
    /*
    //Draws just the box at where it is currently hitting.
    public static void DrawBoxCastOnHit(Vector3 origin, Vector3 halfExtents, Quaternion orientation, Vector3 direction, float hitInfoDistance, Color color) {
        origin = CastCenterOnCollision(origin, direction, hitInfoDistance);
        DrawBox(origin, halfExtents, orientation, color);
    }

    //Draws the full box from start of cast to its end distance. Can also pass in hitInfoDistance instead of full distance
    public static void DrawBoxCastBox(Vector3 origin, Vector3 halfExtents, Quaternion orientation, Vector3 direction, float distance, Color color) {
        direction.Normalize();
        Box bottomBox = new Box(origin, halfExtents, orientation);
        Box topBox = new Box(origin + (direction * distance), halfExtents, orientation);

        Debug.DrawLine(bottomBox.backBottomLeft, topBox.backBottomLeft, color);
        Debug.DrawLine(bottomBox.backBottomRight, topBox.backBottomRight, color);
        Debug.DrawLine(bottomBox.backTopLeft, topBox.backTopLeft, color);
        Debug.DrawLine(bottomBox.backTopRight, topBox.backTopRight, color);
        Debug.DrawLine(bottomBox.frontTopLeft, topBox.frontTopLeft, color);
        Debug.DrawLine(bottomBox.frontTopRight, topBox.frontTopRight, color);
        Debug.DrawLine(bottomBox.frontBottomLeft, topBox.frontBottomLeft, color);
        Debug.DrawLine(bottomBox.frontBottomRight, topBox.frontBottomRight, color);

        DrawBox(bottomBox, color);
        DrawBox(topBox, color);
    }

    public static void DrawBox(Vector3 origin, Vector3 halfExtents, Quaternion orientation, Color color) {
        DrawBox(new Box(origin, halfExtents, orientation), color);
    }
    public static void DrawBox(Box box, Color color) {
        Debug.DrawLine(box.frontTopLeft, box.frontTopRight, color);
        Debug.DrawLine(box.frontTopRight, box.frontBottomRight, color);
        Debug.DrawLine(box.frontBottomRight, box.frontBottomLeft, color);
        Debug.DrawLine(box.frontBottomLeft, box.frontTopLeft, color);

        Debug.DrawLine(box.backTopLeft, box.backTopRight, color);
        Debug.DrawLine(box.backTopRight, box.backBottomRight, color);
        Debug.DrawLine(box.backBottomRight, box.backBottomLeft, color);
        Debug.DrawLine(box.backBottomLeft, box.backTopLeft, color);

        Debug.DrawLine(box.frontTopLeft, box.backTopLeft, color);
        Debug.DrawLine(box.frontTopRight, box.backTopRight, color);
        Debug.DrawLine(box.frontBottomRight, box.backBottomRight, color);
        Debug.DrawLine(box.frontBottomLeft, box.backBottomLeft, color);
    }

    public struct Box {
        public Vector3 localFrontTopLeft { get; private set; }
        public Vector3 localFrontTopRight { get; private set; }
        public Vector3 localFrontBottomLeft { get; private set; }
        public Vector3 localFrontBottomRight { get; private set; }
        public Vector3 localBackTopLeft { get { return -localFrontBottomRight; } }
        public Vector3 localBackTopRight { get { return -localFrontBottomLeft; } }
        public Vector3 localBackBottomLeft { get { return -localFrontTopRight; } }
        public Vector3 localBackBottomRight { get { return -localFrontTopLeft; } }

        public Vector3 frontTopLeft { get { return localFrontTopLeft + origin; } }
        public Vector3 frontTopRight { get { return localFrontTopRight + origin; } }
        public Vector3 frontBottomLeft { get { return localFrontBottomLeft + origin; } }
        public Vector3 frontBottomRight { get { return localFrontBottomRight + origin; } }
        public Vector3 backTopLeft { get { return localBackTopLeft + origin; } }
        public Vector3 backTopRight { get { return localBackTopRight + origin; } }
        public Vector3 backBottomLeft { get { return localBackBottomLeft + origin; } }
        public Vector3 backBottomRight { get { return localBackBottomRight + origin; } }

        public Vector3 origin { get; private set; }

        public Box(Vector3 origin, Vector3 halfExtents, Quaternion orientation) : this(origin, halfExtents) {
            Rotate(orientation);
        }
        public Box(Vector3 origin, Vector3 halfExtents) {
            this.localFrontTopLeft = new Vector3(-halfExtents.x, halfExtents.y, -halfExtents.z);
            this.localFrontTopRight = new Vector3(halfExtents.x, halfExtents.y, -halfExtents.z);
            this.localFrontBottomLeft = new Vector3(-halfExtents.x, -halfExtents.y, -halfExtents.z);
            this.localFrontBottomRight = new Vector3(halfExtents.x, -halfExtents.y, -halfExtents.z);

            this.origin = origin;
        }


        public void Rotate(Quaternion orientation) {
            localFrontTopLeft = RotatePointAroundPivot(localFrontTopLeft, Vector3.zero, orientation);
            localFrontTopRight = RotatePointAroundPivot(localFrontTopRight, Vector3.zero, orientation);
            localFrontBottomLeft = RotatePointAroundPivot(localFrontBottomLeft, Vector3.zero, orientation);
            localFrontBottomRight = RotatePointAroundPivot(localFrontBottomRight, Vector3.zero, orientation);
        }
    }

    //This should work for all cast types
    static Vector3 CastCenterOnCollision(Vector3 origin, Vector3 direction, float hitInfoDistance) {
        return origin + (direction.normalized * hitInfoDistance);
    }

    static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Quaternion rotation) {
        Vector3 direction = point - pivot;
        return pivot + rotation * direction;
    }
    */
}