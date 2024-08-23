using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class GridManager : MonoBehaviour
{
    public GameObject[] cubes;

    void Start()
    {
        GameObject[] cubeGameObjects = GameObject.FindGameObjectsWithTag("Cube");
        cubes = new GameObject[cubeGameObjects.Length];

        for (int i = 0; i < cubeGameObjects.Length; i++)
        {
            cubes[i] = cubeGameObjects[i];
            InitializeCube(cubes[i]);
        }
    }

    void InitializeCube(GameObject cube)
    {
        CubeController cubeController = cube.GetComponent<CubeController>();
        cubeController.AssignRandomLetters();
    }
}
