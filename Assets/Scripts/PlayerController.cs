using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private CharacterController _controller;
    private Camera _camera;

    public float MoveSpeed = 5f;
    public float SprintSpeed = 8f;
    public float JumpHeight = 6f;
    public float MouseSensitivity = .8f;

    private float _gravityMultiplier = 1.2f;
    private float _cameraAngleLimit = 60f;
    private float _verticalVelocity = 0f;
    private Vector3 _horizontalVelocity = Vector3.zero;

    public GameObject ShopUI;
    // public ParticleSystem SetTrapEffect;
    private bool _isMenuOpen = false;
    public GameObject SelectedTower;
    private GridManager _gridManager;
    public int Gold;

    public List<GameObject> Towers = new();

    void Start()
    {
        _controller = GetComponent<CharacterController>();
        _camera = Camera.main;
        _gridManager = FindFirstObjectByType<GridManager>();

        ShopUI.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Gold = 100;
    }

    void Update()
    {
        if (!_isMenuOpen)
        {
            HandleMovement();
            HandleRotation();
            HandleAttack();
        }
        HandleShopAndTower();
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
}
