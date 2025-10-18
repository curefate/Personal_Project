using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerController : BattleBase
{
    private CharacterController _controller;
    private Camera _camera;
    private GridManager _gridManager;
    private MeshRenderer _playerMesh;

    // Control
    [Header("Movement Settings")]
    public Transform RespawnPoint;
    public float MoveSpeed = 5f;
    public float SprintSpeed = 8f;
    public float JumpHeight = 6f;
    public float MouseSensitivity = .8f;
    public Vector3 LastGroundedPosition { get; private set; }
    private float _gravityMultiplier = 1.2f;
    private float _cameraAngleLimit = 60f;
    private float _verticalVelocity = 0f;
    private Vector3 _horizontalVelocity = Vector3.zero;

    // UI & Tower Set
    [Header("UI & Tower Settings")]
    public GameObject ShopUI;
    // public ParticleSystem SetTrapEffect;
    public int Gold;
    public int InitialGold = 100;
    public List<GameObject> Towers = new();
    private GameObject _selectedTower;
    private Quaternion _initialTowerRotation;
    private Vector3 _initialTowerOffset;
    private float _goldTimer = 0f;
    private bool _isMenuOpen = false;
    private bool _isFromHorizontal = true;

    // Battle
    [Header("Battle Settings")]
    public GameObject BulletPrefab;
    public Camera ObserverCamera;
    public int MaxHp = 100;
    public float AttackCooldown = 0.5f;
    public bool IsDead { get; private set; } = false;
    public int RemainRespawnTime => Mathf.CeilToInt(RespawnTime - _respawnTimer);
    private float _attackTimer = 0f;
    public float RespawnTime = 10f;
    private float _recoverTimer = 0f;
    private float _respawnTimer = 0f;
    private float _damagedTimer = 0f;

    void Start()
    {
        transform.position = RespawnPoint.position;

        _controller = GetComponent<CharacterController>();
        _camera = Camera.main;
        _gridManager = FindFirstObjectByType<GridManager>();
        _playerMesh = GetComponent<MeshRenderer>();

        ShopUI.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        ObserverCamera.gameObject.SetActive(false);

        Gold = InitialGold;
        Health = MaxHp;
    }

    void Update()
    {
        // Respawn logic
        if (IsDead)
        {
            Respawning();
            return;
        }

        // Control logic
        if (!_isMenuOpen)
        {
            HandleMovement();
            HandleRotation();
            HandleAttack();
        }
        HandleShopAndTower();

        // gold increment
        _goldTimer += Time.deltaTime;
        if (_goldTimer >= 1f)
        {
            Gold += 1;
            _goldTimer = 0f;
        }

        // health recover
        if (Health < MaxHp)
        {
            _recoverTimer += Time.deltaTime;
            if (_recoverTimer >= 5f)
            {
                Heal(3);
                _recoverTimer = 0f;
            }
        }

        // other timers
        _attackTimer += Time.deltaTime;
        _damagedTimer += Time.deltaTime;
    }

    private void HandleMovement()
    {
        // vertical
        if (_controller.isGrounded)
        {
            if (_verticalVelocity < 0)
            {
                _verticalVelocity = -2f;
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                _verticalVelocity = JumpHeight;
            }
        }
        else
        {
            _verticalVelocity += Physics.gravity.y * _gravityMultiplier * Time.deltaTime;
            _verticalVelocity = Mathf.Max(_verticalVelocity, -10f);
        }

        // horizontal
        Vector3 inputDir = new(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        Vector3 moveDir = (transform.right * inputDir.x + transform.forward * inputDir.z).normalized;
        float targetSpeed = Input.GetKey(KeyCode.LeftShift) ? SprintSpeed : MoveSpeed;
        Vector3 targetVelocity = moveDir * targetSpeed;
        if (_controller.isGrounded)
        {
            if (moveDir.magnitude > 0.1f)
            {
                _horizontalVelocity = targetVelocity;
            }
            else
            {
                _horizontalVelocity = Vector3.Lerp(_horizontalVelocity, Vector3.zero, 10f * Time.deltaTime);
            }
        }
        else
        {
            _horizontalVelocity = Vector3.Lerp(_horizontalVelocity, targetVelocity, Time.deltaTime);
        }


        // final
        Vector3 finalVelocity = new(_horizontalVelocity.x, _verticalVelocity, _horizontalVelocity.z);

        // apply
        _controller.Move(finalVelocity * Time.deltaTime);
        _camera.fieldOfView = Mathf.Lerp(_camera.fieldOfView, Input.GetKey(KeyCode.LeftShift) ? 70f : 60f, 5f * Time.deltaTime);

        // Update last grounded position
        if (_controller.isGrounded)
        {
            LastGroundedPosition = transform.position;
        }

        // Cheat gold
        if (Input.GetKeyDown(KeyCode.G) && Input.GetKey(KeyCode.LeftControl))
        {
            Gold += 10000;
        }
    }

    private void HandleRotation()
    {
        var mouseX = Input.GetAxis("Mouse X");
        var mouseY = Input.GetAxis("Mouse Y");
        transform.Rotate(MouseSensitivity * mouseX * Vector3.up);
        _camera.transform.Rotate(MouseSensitivity * mouseY * Vector3.left);
        var currentAngle = _camera.transform.eulerAngles.x;
        if (currentAngle > _cameraAngleLimit && currentAngle < 180)
        {
            _camera.transform.eulerAngles = new Vector3(_cameraAngleLimit, _camera.transform.eulerAngles.y, 0);
        }
        else if (currentAngle > 180 && currentAngle < 360 - _cameraAngleLimit)
        {
            _camera.transform.eulerAngles = new Vector3(360 - _cameraAngleLimit, _camera.transform.eulerAngles.y, 0);
        }
    }

    private void HandleShopAndTower()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            _selectedTower = null;
            Destroy(_selectedTower);
            OpenMenu();
        }
        if (Input.GetKeyDown(KeyCode.Escape) && _isMenuOpen)
        {
            CloseMenu();
        }
        // 若有选中的塔，处理放置逻辑
        if (_selectedTower != null)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 10, LayerMask.GetMask("Ground")) && hit.collider.gameObject.CompareTag("Brick"))
            {
                GridBrick brick = hit.collider.gameObject.GetComponent<GridBrick>();
                Tower towerComponent = _selectedTower.GetComponent<Tower>();

                if (Input.GetKeyDown(KeyCode.R))
                {
                    _isFromHorizontal = !_isFromHorizontal;
                }

                if (_gridManager.TryMatchPlacement(brick.Coordinate, towerComponent.SizeType, out List<Vector3Int> matchCoords, _isFromHorizontal) && matchCoords != null)
                {
                    Vector3 centerPos = brick.transform.position;
                    foreach (var coord in matchCoords)
                    {
                        centerPos = Vector3.Lerp(centerPos, _gridManager.GetBrickAt(coord).transform.position, 1f / matchCoords.Count);
                    }
                    _selectedTower.transform.position = centerPos + new Vector3(0, 1f, 0) + _initialTowerOffset;
                    _selectedTower.transform.rotation = _isFromHorizontal ? _initialTowerRotation : _initialTowerRotation * Quaternion.Euler(0, -90f, 0);

                    // place tower
                    if (Input.GetMouseButtonDown(0))
                    {
                        Gold -= towerComponent.Cost;
                        var realTower = Instantiate(Towers.Find(t => t.GetComponent<Tower>().Number == towerComponent.Number), _selectedTower.transform.position, _selectedTower.transform.rotation);
                        foreach (var coord in matchCoords)
                        {
                            _gridManager.GetBrickAt(coord).TowerPrefab = realTower;
                        }
                        Destroy(_selectedTower);
                        _selectedTower = null;
                    }
                }
                else
                {
                    _selectedTower.transform.position = transform.position + _camera.transform.forward * 4f + _initialTowerOffset;
                }
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Destroy(_selectedTower);
                _selectedTower = null;
            }
        }
    }

    private void HandleAttack()
    {

    }

    private void Respawning()
    {
        _respawnTimer += Time.deltaTime;
        if (_respawnTimer >= RespawnTime)
        {
            IsDead = false;
            Health = MaxHp;
            Transport(RespawnPoint.position);
            _playerMesh.enabled = true;
            ObserverCamera.gameObject.SetActive(false);
            _respawnTimer = 0f;
        }
    }

    public void OpenMenu()
    {
        _isMenuOpen = true;
        ShopUI.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void CloseMenu()
    {
        _isMenuOpen = false;
        ShopUI.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public override void TakeDamage(DamageMessage msg)
    {
        if (IsDead) return;
        if (_damagedTimer < _damagedInterval) return;

        _damagedTimer = 0f;

        Health -= msg.DamageAmount;
        if (Health <= 0)
        {
            Dead();
        }
    }

    private void Dead()
    {
        Health = 0;
        ObserverCamera.gameObject.SetActive(true);
        _playerMesh.enabled = false;
        IsDead = true;
        _respawnTimer = 0f;
    }

    private void ResetVelocity()
    {
        _horizontalVelocity = Vector3.zero;
        _verticalVelocity = 0f;
    }

    public void Transport(Vector3 position)
    {
        _controller.enabled = false;
        transform.position = position;
        _controller.enabled = true;
        ResetVelocity();
    }

    public void Heal(int amount)
    {
        Health += amount;
        if (Health > MaxHp)
        {
            Health = MaxHp;
        }
    }

    public void SelectTower(GameObject towerPrefab)
    {
        _selectedTower = towerPrefab;
        _initialTowerRotation = _selectedTower.transform.rotation;
        _initialTowerOffset = _selectedTower.transform.position;
    }
}
