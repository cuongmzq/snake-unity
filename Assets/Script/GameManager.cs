﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    public Tile[] highlightTile = new Tile[6];
    public Tilemap tilemap;
    private Vector3Int tilemapSize;
    public List<Vector3Int> tilePositionList;

    public GameObject player;
    public List<GameObject> playerList = new List<GameObject>();
    private Vector3Int playerDirection = new Vector3Int(-1, 0, 0);

    private Vector2Int tilemapBoundMax;
    private Vector2Int tilemapBoundMin;

    private Vector2 playTilemapMax;
    private Vector2 playTilemapMin;

    public GameObject fruit;



    private bool dead = false;

    private void Start()
    {
        Vector3Int size = aspectRatio();
        tilemapSize = size;

        createMap();

        Camera cam = Camera.main;

        Vector3Int centerTilemap = new Vector3Int((int)tilemap.cellBounds.center.x,
                                                  (int)tilemap.cellBounds.center.y,
                                                  (int)tilemap.cellBounds.center.z);

        Vector3 newPosition = tilemap.CellToWorld(centerTilemap);

        newPosition.z = -10;

        cam.transform.position = newPosition;

        tilemapBoundMax = new Vector2Int(tilemap.cellBounds.xMax, tilemap.cellBounds.yMax);
        tilemapBoundMin = new Vector2Int(tilemap.cellBounds.xMin, tilemap.cellBounds.yMin);

        Debug.Log(tilemapBoundMax);
        Debug.Log(tilemapBoundMin);

        playTilemapMax = tilemapBoundMax - new Vector2(1.5f, 1.5f);
        playTilemapMin = tilemapBoundMin + new Vector2(1.5f, 1.5f);

        createAddPlayer();

        playerList[0].name += "Head";
        playerList[0].transform.position = centerTilemap + new Vector3(0.5f, 0.5f);

        fruit = Instantiate(fruit, newFruitLocation(), Quaternion.identity);
        
        StartCoroutine(intervalUpdate());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            playerDirection = new Vector3Int(-1, 0, 0);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            playerDirection = new Vector3Int(1, 0, 0);
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            playerDirection = new Vector3Int(0, 1, 0);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            playerDirection = new Vector3Int(0, -1, 0);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            createAddPlayer();
        }
    }

    private void createMap()
    {
        tilemap.size = tilemapSize;

        for (int h = tilemap.cellBounds.yMin; h < tilemap.cellBounds.yMax; ++h)
        {
            for (int w = tilemap.cellBounds.xMin; w < tilemap.cellBounds.xMax; ++w)
            {
                Vector3Int tilePosition = new Vector3Int(w, h, 0);

                if (h == tilemap.cellBounds.yMin ||
                    w == tilemap.cellBounds.xMin ||
                    h == tilemap.cellBounds.yMax - 1 ||
                    w == tilemap.cellBounds.xMax - 1)
                {
                    tilemap.SetTile(tilePosition, highlightTile[2]);
                }
                else
                {
                    tilemap.SetTile(tilePosition, highlightTile[1]);
                }

                tilePositionList.Add(tilePosition);
            }
        }
    }

    private void createAddPlayer()
    {
        GameObject newPlayer = Instantiate(player);
        newPlayer.SetActive(false);
        playerList.Add(newPlayer);
    }

    private void updatePlayerPosition()
    {
        if (playerList[0].activeSelf == false)
        {
            playerList[0].SetActive(true);
        }

        Vector3 newPosition = playerList[0].transform.position + playerDirection;

        if (playerList.Count > 1 && newPosition == playerList[1].transform.position)
        {
            return;
        }

        for (int i = playerList.Count - 1; i > 0; --i)
        {
            playerList[i].transform.position = playerList[i - 1].transform.position;

            if (playerList[i].activeSelf == false)
            {
                playerList[i].SetActive(true);
            }
        }


        if (newPosition.x > playTilemapMax.x)
        {
            newPosition.x = playTilemapMin.x;
        }
        else if (newPosition.x < playTilemapMin.x)
        {
            newPosition.x = playTilemapMax.x;
        }
        if (newPosition.y > playTilemapMax.y)
        {
            newPosition.y = playTilemapMin.y;
        }
        else if (newPosition.y < playTilemapMin.y)
        {
            newPosition.y = playTilemapMax.y;
        }

        playerList[0].transform.position = newPosition;
    }

    void fruitHandle()
    {
        if (playerList[0].transform.position == fruit.transform.position)
        {
            createAddPlayer();
            fruit.transform.position = newFruitLocation();
        }
    }

    IEnumerator intervalUpdate()
    {
        while (!dead)
        {
            yield return new WaitForSeconds(0.2f);
            fruitHandle();
            updatePlayerPosition();
        }
    }

    Vector3 newFruitLocation()
    {
        float x = (int)Random.Range(playTilemapMin.x, playTilemapMax.x) + 0.5f;
        float y = (int)Random.Range(playTilemapMin.y, playTilemapMax.y) + 0.5f;


        Vector3 newLocation = new Vector3(x, y);

        return newLocation;
    }

    Vector3Int aspectRatio()
    {
        Vector3Int myAspect = new Vector3Int();
        Camera cam = Camera.main;

        myAspect.y = Mathf.RoundToInt(2.0f * cam.orthographicSize);
        myAspect.x = Mathf.RoundToInt(myAspect.y * cam.aspect);
        Debug.Log(myAspect.y * cam.aspect);
        return myAspect;
    }
}
