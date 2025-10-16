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
    public GameObject ShopUI;
    // public ParticleSystem SetTrapEffect;
    public GameObject SelectedTower;
    public int Gold;
    public int InitialGold = 100;
    public List<GameObject> Towers = new();
    private float _goldTimer = 0f;
    private bool _isMenuOpen = false;

    // Battle
    public GameObject BulletPrefab;
    public TextMeshProUGUI RespawnTimerText;
    public Camera ObserverCamera;
    public int MaxHp = 100;
    public float AttackCooldown = 0.5f;
    public bool IsDead { get; private set; } = false;
    private float _attackTimer = 0f;
    public float RespawnTime = 10f;
    private float _recoverTimer = 0f;
    private float _respawnTimer = 0f;

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
        RespawnTimerText.gameObject.SetActive(false);
        ObserverCamera.gameObject.SetActive(false);

        Gold = InitialGold;
        Health = MaxHp;
    }

    void Update()
    {
        // Respawn logic
        if (IsDead)
        {
            RespawnTimerText.gameObject.SetActive(true);
            RespawnTimerText.text = $"Respawning in {Mathf.Ceil(RespawnTime - _respawnTimer)}s";
            ObserverCamera.gameObject.SetActive(true);
            _respawnTimer += Time.deltaTime;
            if (_respawnTimer >= RespawnTime)
            {
                IsDead = false;
                Health = MaxHp;
                ResetVelocity();
                _controller.enabled = false;
                transform.position = RespawnPoint.position;
                _controller.enabled = true;
                _playerMesh.enabled = true;
                RespawnTimerText.gameObject.SetActive(false);
                ObserverCamera.gameObject.SetActive(false);
                _respawnTimer = 0f;
            }
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
                Health += 3;
                _recoverTimer = 0f;
            }
        }
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

        if (_controller.isGrounded)
        {
            LastGroundedPosition = transform.position;
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
            OpenMenu();
        }
        if (Input.GetKeyDown(KeyCode.Escape) && _isMenuOpen)
        {
            CloseMenu();
        }
        if (SelectedTower != null)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 10, LayerMask.GetMask("Ground")) && hit.collider.gameObject.CompareTag("Brick"))
            {
                GridBrick brick = hit.collider.gameObject.GetComponent<GridBrick>();
                Tower towerComponent = SelectedTower.GetComponent<Tower>();
                var coords = _gridManager.CheckEmpty(brick.Coordinate, towerComponent.SizeType, out bool isValid);
                if (isValid && coords != null)
                {
                    Vector3 centerPos = brick.transform.position;
                    foreach (var coord in coords)
                    {
                        centerPos = Vector3.Lerp(centerPos, _gridManager.GetBrickAt(coord).transform.position, 1f / coords.Count);
                    }
                    SelectedTower.transform.position = centerPos + new Vector3(0, 1f, 0);
                }
                else
                {
                    SelectedTower.transform.position = transform.position + transform.forward * 2f;
                }

                if (Input.GetMouseButtonDown(0) && isValid)
                {
                    Gold -= towerComponent.Cost;
                    int towerNum = towerComponent.Number;
                    var realTower = Instantiate(Towers.Find(t => t.GetComponent<Tower>().Number == towerNum), SelectedTower.transform.position, SelectedTower.transform.rotation);
                    foreach (var coord in coords)
                    {
                        _gridManager.GetBrickAt(coord).TowerPrefab = realTower;
                    }
                    Destroy(SelectedTower);
                    SelectedTower = null;
                }
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Destroy(SelectedTower);
                SelectedTower = null;
            }
        }
    }

    private void HandleAttack()
    {

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

        Health -= msg.DamageAmount;
        if (Health <= 0)
        {
            Health = 0;
            RespawnTimerText.gameObject.SetActive(true);
            ObserverCamera.gameObject.SetActive(true);
            _playerMesh.enabled = false;
            IsDead = true;
            _respawnTimer = 0f;
        }
    }

    public void ResetVelocity()
    {
        _horizontalVelocity = Vector3.zero;
        _verticalVelocity = 0f;
    }
}
