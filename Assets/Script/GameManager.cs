using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    public Tile[] highlightTile = new Tile[6];
    public Tilemap tilemap;
    private Vector3Int tilemapSize = new Vector3Int(27, 17, 1);

    public GameObject player;
    public List<GameObject> playerList = new List<GameObject>();
    private Vector3Int playerDirection = new Vector3Int(-1, 0, 0);

    public GameObject fruit;

    private bool dead = false;

    private void Start()
    {
        createMap();

        createAddPlayer();
        playerList[0].name += "Head";
        playerList[0].transform.position = new Vector3(0.5f, 0.5f);

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
                if (h == tilemap.cellBounds.yMin ||
                    w == tilemap.cellBounds.xMin ||
                    h == tilemap.cellBounds.yMax - 1 ||
                    w == tilemap.cellBounds.xMax - 1)
                {
                    tilemap.SetTile(new Vector3Int(w, h, 0), highlightTile[2]);
                }
                else
                {
                    tilemap.SetTile(new Vector3Int(w, h, 0), highlightTile[1]);
                }
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


        if (newPosition.x > 14.5f)
        {
            newPosition.x = -9.5f;
        }
        else if (newPosition.x < -9.5f)
        {
            newPosition.x = 14.5f;
        }
        if (newPosition.y > 7.5f)
        {
            newPosition.y = -6.5f;
        }
        else if (newPosition.y < -6.5f)
        {
            newPosition.y = 7.5f;
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
        float x = Random.Range(-10, 15) + 0.5f;
        float y = Random.Range(-7, 7) + 0.5f;

        Vector3 newLocation = new Vector3(x, y);

        return newLocation;
    }
}
