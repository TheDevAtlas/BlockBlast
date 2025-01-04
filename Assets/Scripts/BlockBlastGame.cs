using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BlockBlastGame : MonoBehaviour
{
    public GameObject particleEffectPrefab;
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

    public SoundManager soundManager;

    public List<Color[]> particleColors = new List<Color[]>
    {
        new Color[] { Color.blue, Color.cyan },
        new Color[] { Color.red, Color.yellow },
        new Color[] { Color.green, Color.cyan }
    };


    private void Start()
    {
        soundManager = FindObjectOfType<SoundManager>();

        InitializeGrid();

        // Start the game after a 10-second delay
        StartCoroutine(StartGameWithDelay());

        StartCoroutine(SpawnPieces());

        StartCoroutine(CreateUniqueStartingBoard());

    }

    private IEnumerator StartGameWithDelay()
    {
        yield return new WaitForSeconds(2f); // Wait for 10 seconds
        GeneratePieceShapes();
    }

    private IEnumerator SpawnPieces()
    {
        yield return new WaitForSeconds(2f); // Wait for 10 seconds
        SpawnPiecesInArea();
    }

    private IEnumerator CreateUniqueStartingBoard()
    {
        float fillDuration = 1f; // Time to fill the grid
        float timePerPiece = fillDuration / (gridHeight); // Time between each piece placement

        // Fill the grid with single pieces
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                if(grid[x,y] == null)
                {
                    Vector3 position = new Vector3(x * squareSize, y * squareSize, 0);
                    Transform square = Instantiate(squarePrefab, position, Quaternion.identity, gridParent).transform;

                    // Randomize sprite for the single piece
                    Sprite randomSprite = squares[Random.Range(0, squares.Length)];
                    square.gameObject.GetComponent<SpriteRenderer>().sprite = randomSprite;

                    // Store the square in the grid
                    grid[x, y] = square;
                }
                

            }
            yield return new WaitForSeconds(timePerPiece);
        }

        // Remove pieces at random to create a unique starting board
        int totalPieces = gridWidth * gridHeight;
        int piecesToRemove = (int)Random.Range(totalPieces / 1.25f, totalPieces / 1.5f); // Remove 25-50% of pieces

        for (int i = 0; i < piecesToRemove; i++)
        {
            // Choose a random piece to remove
            int randomX, randomY;
            do
            {
                randomX = Random.Range(0, gridWidth);
                randomY = Random.Range(0, gridHeight);
            } while (grid[randomX, randomY] == null); // Ensure the piece exists

            // Remove the piece
            Destroy(grid[randomX, randomY].gameObject);
            grid[randomX, randomY] = null;

            yield return new WaitForSeconds(1f / piecesToRemove); // Small delay for the removal effect
        }
    }

    private void InitializeGrid()
    {
        grid = new Transform[gridWidth, gridHeight];
    }

    private void GeneratePieceShapes()
    {
        // Define basic shapes with rotations
        List<List<Vector2Int[]>> shapes = new List<List<Vector2Int[]>>();

        // 1x1 Piece
        shapes.Add(new List<Vector2Int[]> {
            new Vector2Int[] { new Vector2Int(0, 0) }
        });

        // 2x2 Piece
        shapes.Add(new List<Vector2Int[]> {
            new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(0, 1), new Vector2Int(1, 1) }
        });

        // 3x3 Piece
        shapes.Add(new List<Vector2Int[]> {
            new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(2, 0),
                            new Vector2Int(0, 1), new Vector2Int(1, 1), new Vector2Int(2, 1),
                            new Vector2Int(0, 2), new Vector2Int(1, 2), new Vector2Int(2, 2) }
        });

        // 2x3 Piece
        shapes.Add(new List<Vector2Int[]> {
            new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(0, 1), new Vector2Int(1, 1), new Vector2Int(0, 2), new Vector2Int(1, 2) }
        });

        // 3x2 Piece
        shapes.Add(new List<Vector2Int[]> {
            new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(2, 0),
                            new Vector2Int(0, 1), new Vector2Int(1, 1), new Vector2Int(2, 1) }
        });

        // 1x4 and 4x1 Pieces
        shapes.Add(new List<Vector2Int[]> {
            new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(0, 2), new Vector2Int(0, 3) },
            new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(2, 0), new Vector2Int(3, 0) }
        });

        // 1x3 and 3x1 Pieces
        shapes.Add(new List<Vector2Int[]> {
            new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(0, 2) },
            new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(2, 0) }
        });

        // 1x5 and 5x1 Pieces
        shapes.Add(new List<Vector2Int[]> {
            new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(0, 2), new Vector2Int(0, 3), new Vector2Int(0, 4) },
            new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(2, 0), new Vector2Int(3, 0), new Vector2Int(4, 0) }
        });

        // L Shape Rotations
        shapes.Add(new List<Vector2Int[]> {
            new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(2, 0), new Vector2Int(2, 1) },
            new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(0, 2), new Vector2Int(1, 0) },
            new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(2, 0), new Vector2Int(0, -1) },
            new Vector2Int[] { new Vector2Int(0, 1), new Vector2Int(1, 1), new Vector2Int(1, 0), new Vector2Int(1, 2) }
        });

        // J Shape Rotations
        shapes.Add(new List<Vector2Int[]> {
            new Vector2Int[] { new Vector2Int(0, 1), new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(2, 0) },
            new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(1, 1), new Vector2Int(2, 1) },
            new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(2, 0), new Vector2Int(2, -1) },
            new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(1, 1), new Vector2Int(1, 2) }
        });

        // Big L Shape
        shapes.Add(new List<Vector2Int[]> {
            new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(0, 2),
                            new Vector2Int(1, 0), new Vector2Int(2, 0) },
            new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(2, 0),
                            new Vector2Int(2, 1), new Vector2Int(2, 2) },
            new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(0, 2),
                            new Vector2Int(1, 2), new Vector2Int(2, 2) },
            new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(2, 0),
                            new Vector2Int(0, 1), new Vector2Int(0, 2)}
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

        soundManager.PlayRandomSound("PiecePlaced");

        return true;
    }

    private void CheckAndClearRowsAndColumns()
    {
        HashSet<Vector2Int> positionsToClear = new HashSet<Vector2Int>();

        // Check for full rows
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
            if (fullRow)
            {
                // Generate explosion effect for the row
                Vector3 effectPosition = new Vector3(gridWidth * squareSize / 2, y * squareSize, 0);
                GenerateExplosionEffect(effectPosition);

                for (int x = 0; x < gridWidth; x++)
                {
                    positionsToClear.Add(new Vector2Int(x, y));
                }
            }
        }

        // Check for full columns
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
            if (fullColumn)
            {
                // Generate explosion effect for the column
                Vector3 effectPosition = new Vector3(x * squareSize, gridHeight * squareSize / 2, 0);
                GenerateExplosionEffect(effectPosition, true);

                for (int y = 0; y < gridHeight; y++)
                {
                    positionsToClear.Add(new Vector2Int(x, y));
                }
            }
        }

        // Clear all positions in the HashSet
        foreach (var position in positionsToClear)
        {
            if (grid[position.x, position.y] != null)
            {
                Destroy(grid[position.x, position.y].gameObject);
                grid[position.x, position.y] = null;
            }
        }

        // Camera shake and sound
        if (positionsToClear.Count > 0)
        {
            StartCoroutine(ShakeCamera(0.2f, 0.1f));
            soundManager.PlayRandomSound("ClearLine");
        }

        if (!CanAnyPieceFit())
        {
            Debug.Log("No valid moves left! Resetting the board.");
            HandleNoValidMoves();
        }
    }

    private void GenerateExplosionEffect(Vector3 position, bool isColumn = false)
    {
        GameObject effect = Instantiate(particleEffectPrefab, position, isColumn ? Quaternion.Euler(0, 0, 90) : Quaternion.identity);
        ParticleSystem particleSystem = effect.GetComponent<ParticleSystem>();
        if (particleSystem != null && particleColors.Count > 0)
        {
            int randomIndex = Random.Range(0, particleColors.Count);
            var main = particleSystem.main;
            main.startColor = new ParticleSystem.MinMaxGradient(particleColors[randomIndex][0], particleColors[randomIndex][1]);
        }
        Destroy(effect, 1f); // Destroy after 1 second
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
    }

    public IEnumerator ShakeCamera(float duration, float magnitude)
    {
        Vector3 originalPosition = Camera.main.transform.position;
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            float offsetX = Random.Range(-1f, 1f) * magnitude;
            float offsetY = Random.Range(-1f, 1f) * magnitude;

            Camera.main.transform.position = new Vector3(originalPosition.x + offsetX, originalPosition.y + offsetY, originalPosition.z);

            elapsed += Time.deltaTime;
            yield return null;
        }

        Camera.main.transform.position = originalPosition;
    }

    // Method to check if any piece can be placed
    public bool CanAnyPieceFit()
    {
        // Remove any destroyed objects from the list
        spawnedPieces.RemoveAll(piece => piece == null);

        foreach (var piece in spawnedPieces)
        {
            if (piece == null) continue; // Skip null references

            foreach (Transform square in piece.transform)
            {
                Vector2Int piecePosition = GetNearestGridPosition(square.position);

                // Iterate through possible placements on the grid
                for (int x = 0; x < gridWidth; x++)
                {
                    for (int y = 0; y < gridHeight; y++)
                    {
                        if (CanPlacePieceAtPosition(piece, new Vector2Int(x, y)))
                        {
                            return true; // At least one piece fits
                        }
                    }
                }
            }
        }

        return false; // No piece fits
    }


    // Helper method to check if a piece can fit at a specific grid position
    private bool CanPlacePieceAtPosition(GameObject piece, Vector2Int gridPosition)
    {
        foreach (Transform square in piece.transform)
        {
            Vector2Int gridPos = gridPosition + Vector2Int.RoundToInt(square.localPosition / squareSize);

            // Check if position is out of bounds or already occupied
            if (gridPos.x < 0 || gridPos.x >= gridWidth || gridPos.y < 0 || gridPos.y >= gridHeight || grid[gridPos.x, gridPos.y] != null)
            {
                return false;
            }
        }
        return true;
    }

    public void HandleNoValidMoves()
    {
        // Destroy all current player pieces
        foreach (var piece in spawnedPieces)
        {
            if (piece != null)
            {
                Destroy(piece);
            }
        }
        spawnedPieces.Clear();

        StartCoroutine(CreateUniqueStartingBoard());
        StartCoroutine(SpawnPieces());
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

        gameManager.soundManager.PlayRandomSound("ReturnPiece");

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