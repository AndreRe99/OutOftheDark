using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraWalls : MonoBehaviour
{
    public Camera cam;
    public float wallThickness = 10f;
    public float wallHeight = 10f;

    void Start()
    {
        float height = 2f * cam.orthographicSize;
        float width = height * cam.aspect;

        CreateWall("LeftWall",
            new Vector3(-width / 2f - wallThickness / 2f, 0, 0),
            new Vector3(wallThickness, wallHeight, height));

        CreateWall("RightWall",
            new Vector3(width / 2f + wallThickness / 2f, 0, 0),
            new Vector3(wallThickness, wallHeight, height));

        CreateWall("TopWall",
            new Vector3(0, 0, height / 2f + wallThickness / 2f),
            new Vector3(width, wallHeight, wallThickness));

        CreateWall("BottomWall",
            new Vector3(0, 0, -height / 2f - wallThickness / 2f),
            new Vector3(width, wallHeight, wallThickness));
    }

    void CreateWall(string name, Vector3 pos, Vector3 scale)
    {
        GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall.name = name;
        wall.transform.position = pos;
        wall.transform.localScale = scale;
    }
}