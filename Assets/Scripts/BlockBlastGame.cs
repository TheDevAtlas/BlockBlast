using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BlockBlastGame : MonoBehaviour
{
    public int gridWidth = 10;
    public int gridHeight = 10;
    public float squareSize = 1f;
    public GameObject squarePrefab;
    public Transform gridParent;
    public Transform pieceParent;
    public Collider2D spawnArea; // Invisible 2D object defining spawn area

    private Transform[,] grid;
    private List<GameObject> pieceShapes = new List<GameObject>();
    private List<GameObject> spawnedPieces = new List<GameObject>();

    public Sprite[] squares;

    private void Start()
    {
        InitializeGrid();
        GeneratePieceShapes();
        SpawnPiecesInArea();
    }

    private void InitializeGrid()
    {
        grid = new Transform[gridWidth, gridHeight];
    }

    private void GeneratePieceShapes()
    {
        // Define basic shapes with rotations (L, T, Square, Line, Z, S, J)
        List<List<Vector2Int[]>> shapes = new List<List<Vector2Int[]>>();

        // Single Piece
        shapes.Add(new List<Vector2Int[]> {
            new Vector2Int[] { new Vector2Int(0, 0)}
        });

        // Double Piece
        shapes.Add(new List<Vector2Int[]> {
            new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(1, 0)},
            new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(0, 1)}
        });

        // L Shape Rotations
        shapes.Add(new List<Vector2Int[]> {
            new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(2, 0), new Vector2Int(2, 1) },
            new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(0, 2), new Vector2Int(1, 0) },
            new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(2, 0), new Vector2Int(0, -1) },
            new Vector2Int[] { new Vector2Int(0, 1), new Vector2Int(1, 1), new Vector2Int(1, 0), new Vector2Int(1, 2) }
        });

        // T Shape Rotations
        shapes.Add(new List<Vector2Int[]> {
            new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(2, 0), new Vector2Int(1, 1) },
            new Vector2Int[] { new Vector2Int(0, 1), new Vector2Int(1, 1), new Vector2Int(2, 1), new Vector2Int(1, 0) },
            new Vector2Int[] { new Vector2Int(1, 0), new Vector2Int(1, 1), new Vector2Int(1, 2), new Vector2Int(0, 1) },
            new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(2, 0), new Vector2Int(1, -1) }
        });

        // Square Shape (No rotation needed, only one state)
        shapes.Add(new List<Vector2Int[]> {
            new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(0, 1), new Vector2Int(1, 1) }
        });

        // Line Shape Rotations
        shapes.Add(new List<Vector2Int[]> {
            new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(2, 0), new Vector2Int(3, 0) },
            new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(0, 2), new Vector2Int(0, 3) }
        });

        // Z Shape Rotations
        shapes.Add(new List<Vector2Int[]> {
            new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(1, 1), new Vector2Int(2, 1) },
            new Vector2Int[] { new Vector2Int(1, 0), new Vector2Int(0, 1), new Vector2Int(1, 1), new Vector2Int(0, 2) }
        });

        // S Shape Rotations
        shapes.Add(new List<Vector2Int[]> {
            new Vector2Int[] { new Vector2Int(0, 1), new Vector2Int(1, 1), new Vector2Int(1, 0), new Vector2Int(2, 0) },
            new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(1, 1), new Vector2Int(1, 2) }
        });

        // J Shape Rotations
        shapes.Add(new List<Vector2Int[]> {
            new Vector2Int[] { new Vector2Int(0, 1), new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(2, 0) },
            new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(1, 1), new Vector2Int(2, 1) },
            new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(2, 0), new Vector2Int(2, -1) },
            new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(1, 1), new Vector2Int(1, 2) }
        });

        foreach (var shapeRotations in shapes)
        {
            foreach (var shape in shapeRotations)
            {
                GameObject piece = new GameObject("Piece");
                foreach (var square in shape)
                {
                    Vector2 fPos = new Vector2(square.x, square.y);
                    Instantiate(squarePrefab, fPos * squareSize, Quaternion.identity, piece.transform);
                }
                piece.SetActive(false);
                pieceShapes.Add(piece);
            }
        }
    }


    private void SpawnPiecesInArea()
    {
        for (int i = 0; i < 4; i++)
        {
            int randomIndex = Random.Range(0, pieceShapes.Count);
            GameObject newPiece = Instantiate(pieceShapes[randomIndex], pieceParent);
            newPiece.SetActive(true);

            Sprite pieceType = squares[i%(squares.Length-1)];//squares[Random.Range(0, squares.Length-1)];

            // Add Rigidbody2D to the parent piece
            Rigidbody2D rb = newPiece.AddComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Kinematic; // Prevent physics simulation

            // Add CompositeCollider2D to the parent piece
            CompositeCollider2D compositeCollider = newPiece.AddComponent<CompositeCollider2D>();
            compositeCollider.geometryType = CompositeCollider2D.GeometryType.Polygons;

            // Add BoxCollider2D to each child square and configure them to work with the composite collider
            foreach (Transform child in newPiece.transform)
            {
                BoxCollider2D boxCollider = child.gameObject.AddComponent<BoxCollider2D>();
                boxCollider.usedByComposite = true; // Enable usage by the composite collider

                child.GetComponent<SpriteRenderer>().sprite = pieceType;
            }

            // Set spawn position
            // Vector2 spawnPosition = GetRandomPositionInArea();
            // newPiece.transform.position = spawnPosition;
            newPiece.transform.position = new Vector2((i % 2) * 3f - 10f, ((((float)i/2f) % 2) * 3f) + 2f);

            // Add the DraggablePiece script
            newPiece.AddComponent<DraggablePiece>();

            spawnedPieces.Add(newPiece);
        }
    }


    private Vector2 GetRandomPositionInArea()
    {
        Bounds bounds = spawnArea.bounds;
        float x = Random.Range(bounds.min.x, bounds.max.x);
        float y = Random.Range(bounds.min.y, bounds.max.y);
        return new Vector2(x, y);
    }

    public bool PlacePiece(GameObject piece, Vector2Int gridPosition, Sprite sp)
    {
        List<Vector2Int> occupiedPositions = new List<Vector2Int>();

        foreach (Transform square in piece.transform)
        {
            Vector2Int gridPos = gridPosition + Vector2Int.RoundToInt(square.localPosition / squareSize);

            if (gridPos.x < 0 || gridPos.x >= gridWidth || gridPos.y < 0 || gridPos.y >= gridHeight || grid[gridPos.x, gridPos.y] != null)
                return false; // Invalid placement

            occupiedPositions.Add(gridPos);
        }

        foreach (var gridPos in occupiedPositions)
        {
            Transform square = Instantiate(squarePrefab, new Vector3(gridPos.x * squareSize, gridPos.y * squareSize, 0), Quaternion.identity, gridParent).transform;
            square.gameObject.GetComponent<SpriteRenderer>().sprite = sp;
            grid[gridPos.x, gridPos.y] = square;
        }

        Destroy(piece);
        CheckAndClearRowsAndColumns();

        return true;
    }

    private void CheckAndClearRowsAndColumns()
    {
        // return;

        List<int> rowsToClear = new List<int>();
        List<int> columnsToClear = new List<int>();

        for (int y = 0; y < gridHeight; y++)
        {
            bool fullRow = true;
            for (int x = 0; x < gridWidth; x++)
            {
                if (grid[x, y] == null)
                {
                    fullRow = false;
                    break;
                }
            }
            if (fullRow) rowsToClear.Add(y);
        }

        for (int x = 0; x < gridWidth; x++)
        {
            bool fullColumn = true;
            for (int y = 0; y < gridHeight; y++)
            {
                if (grid[x, y] == null)
                {
                    fullColumn = false;
                    break;
                }
            }
            if (fullColumn) columnsToClear.Add(x);
        }

        foreach (int y in rowsToClear)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                Destroy(grid[x, y].gameObject);
                grid[x, y] = null;
            }
        }

        foreach (int x in columnsToClear)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                Destroy(grid[x, y].gameObject);
                grid[x, y] = null;
            }
        }
    }

    public Vector2Int GetNearestGridPosition(Vector3 worldPosition)
    {
        int x = Mathf.RoundToInt(worldPosition.x / squareSize);
        int y = Mathf.RoundToInt(worldPosition.y / squareSize);
        return new Vector2Int(x, y);
    }

    public void SpawnReplacementPiece()
    {
        //Debug.Log(pieceParent.GetComponentsInChildren<DraggablePiece>().Length);
        if(pieceParent.GetComponentsInChildren<DraggablePiece>().Length <= 1)
        {
            SpawnPiecesInArea();
        }

        // int randomIndex = Random.Range(0, pieceShapes.Count);
        // GameObject newPiece = Instantiate(pieceShapes[randomIndex], pieceParent);
        // newPiece.SetActive(true);

        // Sprite pieceType = squares[Random.Range(0, squares.Length-1)];

        // Vector2 spawnPosition = GetRandomPositionInArea();
        // newPiece.transform.position = spawnPosition;

        // // Add Rigidbody2D to the parent piece
        // Rigidbody2D rb = newPiece.AddComponent<Rigidbody2D>();
        // rb.bodyType = RigidbodyType2D.Kinematic; // Prevent physics simulation

        // // Add CompositeCollider2D to the parent piece
        // CompositeCollider2D compositeCollider = newPiece.AddComponent<CompositeCollider2D>();
        // compositeCollider.geometryType = CompositeCollider2D.GeometryType.Polygons;

        // // Add BoxCollider2D to each child square and configure them to work with the composite collider
        // foreach (Transform child in newPiece.transform)
        // {
        //     BoxCollider2D boxCollider = child.gameObject.AddComponent<BoxCollider2D>();
        //     boxCollider.usedByComposite = true; // Enable usage by the composite collider

        //     child.GetComponent<SpriteRenderer>().sprite = pieceType;
        // }

        // newPiece.AddComponent<DraggablePiece>();
        // spawnedPieces.Add(newPiece);
    }


}

public class DraggablePiece : MonoBehaviour
{
    private bool isDragging = false;
    private Vector3 offset;
    private Camera mainCamera;
    private BlockBlastGame gameManager;

    private Vector2 startPos;

    void Start()
    {
        mainCamera = Camera.main;
        gameManager = FindObjectOfType<BlockBlastGame>();
    }

    void OnMouseDown()
    {
        isDragging = true;
        Vector3 mouseWorldPosition = GetMouseWorldPosition();
        offset = transform.position - mouseWorldPosition;

        startPos = transform.position;
    }

    void OnMouseUp()
    {
        isDragging = false;

        // Try to place the piece in the grid
        Vector2Int gridPosition = gameManager.GetNearestGridPosition(transform.position);

        if (gameManager.PlacePiece(gameObject, gridPosition, GetComponentInChildren<SpriteRenderer>().sprite))
        {
            // Successfully placed, lock the piece and spawn a new one
            Destroy(this); // Remove the DraggablePiece script
            gameManager.SpawnReplacementPiece();
        }
        else
        {
            StartCoroutine(SmoothReturnToStart());
        }
    }


    void Update()
    {
        if (isDragging)
        {
            Vector3 mouseWorldPosition = GetMouseWorldPosition();
            transform.position = mouseWorldPosition + offset;
        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mouseScreenPosition = Input.mousePosition;
        mouseScreenPosition.z = Mathf.Abs(mainCamera.transform.position.z - transform.position.z);
        return mainCamera.ScreenToWorldPoint(mouseScreenPosition);
    }

    private IEnumerator SmoothReturnToStart()
    {
        float duration = 0.333f; // 1/3 second
        float elapsed = 0f;
        Vector3 initialPosition = transform.position;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            transform.position = Vector3.Lerp(initialPosition, startPos, elapsed / duration);
            yield return null;
        }

        // Ensure the piece snaps exactly to the start position at the end
        transform.position = startPos;
    }

}