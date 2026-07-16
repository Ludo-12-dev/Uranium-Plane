using UnityEngine;
using TMPro;

public class CameraViewDropdownController : MonoBehaviour
{
    [Header("UI")]
    public TMP_Dropdown dropdown;
    public AvionMovementPrevious avionMovement;

    [Header("Scene Objects")]
    public Camera mainCamera;
    public Transform avion;
    public Transform planeObject;

    [Header("View 1 - First Person")]
    public Vector3 firstPersonLocalOffset = new Vector3(-0.8f, 2.1f, 0f);

    [Header("View 2 - Initial Camera View")]
    public Vector3 initialCameraPosition = new Vector3(-70f, 8f, 0f);
    public Vector3 initialCameraEuler = new Vector3(2f, 90f, 0f);

    [Header("View 3 - Trailing View")]
    public float trailingDistanceBehind = 12f;
    public float trailingHeight = 0.8f;
    public float trailingLookAhead = 10f;
    public float trailingLookHeight = 0.5f;

    [Header("Mouse Orbit Around Avion")]
    public bool allowMouseOrbit = true;
    public float orbitSensitivity = 4f;
    public float orbitDistance = 70f;
    public float orbitMinPitch = -20f;
    public float orbitMaxPitch = 80f;

    private float orbitYaw;
    private float orbitPitch = 15f;

    private int currentView = 0;
    private const int numberOfViews = 3;

    private void Start()
    {
        if (dropdown == null)
            dropdown = GetComponent<TMP_Dropdown>();

        if (mainCamera == null)
            mainCamera = Camera.main;

        if (dropdown != null)
        {
            currentView = dropdown.value;
            dropdown.onValueChanged.AddListener(ChangeView);
        }

        InitOrbitFromCurrentCamera();
        ChangeView(currentView);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            currentView++;

            if (currentView >= numberOfViews)
                currentView = 0;

            if (dropdown != null)
                dropdown.SetValueWithoutNotify(currentView);

            ChangeView(currentView);
        }

        HandleMouseOrbitInput();
    }

    private void LateUpdate()
    {
        if (mainCamera == null || avion == null)
            return;

        if (allowMouseOrbit && Input.GetMouseButton(1))
        {
            ApplyOrbitView();
            return;
        }

        switch (currentView)
        {
            case 0:
                ApplyFirstPersonView();
                break;

            case 2:
                ApplyTrailingView();
                break;
        }
    }

    private void HandleMouseOrbitInput()
    {
        if (!allowMouseOrbit || avion == null || mainCamera == null)
            return;

        if (Input.GetMouseButtonDown(1))
            InitOrbitFromCurrentCamera();

        if (Input.GetMouseButton(1))
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            orbitYaw += mouseX * orbitSensitivity;
            orbitPitch -= mouseY * orbitSensitivity;
            orbitPitch = Mathf.Clamp(orbitPitch, orbitMinPitch, orbitMaxPitch);
        }
    }

    private void InitOrbitFromCurrentCamera()
    {
        if (mainCamera == null || avion == null)
            return;

        Vector3 direction = mainCamera.transform.position - avion.position;

        if (direction.sqrMagnitude < 0.001f)
            direction = new Vector3(-orbitDistance, 10f, 0f);

        orbitDistance = direction.magnitude;

        Vector3 flatDirection = new Vector3(direction.x, 0f, direction.z);

        orbitYaw = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        orbitPitch = Mathf.Atan2(direction.y, flatDirection.magnitude) * Mathf.Rad2Deg;
        orbitPitch = Mathf.Clamp(orbitPitch, orbitMinPitch, orbitMaxPitch);
    }

    private void ApplyOrbitView()
    {
        Quaternion rotation = Quaternion.Euler(orbitPitch, orbitYaw, 0f);

        Vector3 offset = rotation * Vector3.forward * orbitDistance;

        mainCamera.transform.position = avion.position + offset;
        mainCamera.transform.LookAt(avion.position, Vector3.up);
    }

    private void ChangeView(int value)
    {
        currentView = value;

        if (avionMovement != null)
        {
            avionMovement.firstPersonView = false;
            avionMovement.trailingView = false;
        }

        switch (currentView)
        {
            case 0:
                ApplyFirstPersonView();

                if (avionMovement != null)
                    avionMovement.firstPersonView = true;
                break;

            case 1:
                ApplyInitialCameraView();

                if (avionMovement != null)
                {
                    avionMovement.firstPersonView = false;
                    avionMovement.trailingView = false;
                }

                InitOrbitFromCurrentCamera();
                break;

            case 2:
                ApplyTrailingView();

                if (avionMovement != null)
                    avionMovement.trailingView = true;
                break;
        }
    }

    private void ApplyFirstPersonView()
    {
        if (avion == null || mainCamera == null)
            return;

        mainCamera.transform.position = avion.TransformPoint(firstPersonLocalOffset);

        Vector3 forwardDirection = avion.TransformDirection(Vector3.left);

        mainCamera.transform.rotation =
            Quaternion.LookRotation(forwardDirection, Vector3.up);
    }

    private void ApplyInitialCameraView()
    {
        if (mainCamera == null)
            return;

        mainCamera.transform.position = initialCameraPosition;
        mainCamera.transform.rotation = Quaternion.Euler(initialCameraEuler);
    }

    private void ApplyTrailingView()
    {
        if (avion == null || mainCamera == null)
            return;

        // Update orbit angles while RMB is held
        if (Input.GetMouseButton(1))
        {
            orbitYaw += Input.GetAxis("Mouse X") * orbitSensitivity;
            orbitPitch -= Input.GetAxis("Mouse Y") * orbitSensitivity;
            orbitPitch = Mathf.Clamp(orbitPitch, orbitMinPitch, orbitMaxPitch);
        }
        else
        {
            // Gradually return behind the aircraft
            Vector3 aircraftForward = -avion.right;

            float desiredYaw = Mathf.Atan2(
                -aircraftForward.x,
                -aircraftForward.z) * Mathf.Rad2Deg;

            orbitYaw = Mathf.LerpAngle(orbitYaw, desiredYaw, Time.deltaTime * 3f);

            orbitPitch = Mathf.Lerp(orbitPitch, 10f, Time.deltaTime * 3f);
        }

        Quaternion rotation = Quaternion.Euler(orbitPitch, orbitYaw, 0f);

        Vector3 offset = rotation * new Vector3(0f, 0f, -trailingDistanceBehind);

        Vector3 cameraPos =
            avion.position +
            offset +
            Vector3.up * trailingHeight;

        mainCamera.transform.position = cameraPos;
        mainCamera.transform.LookAt(
            avion.position + Vector3.up * trailingLookHeight,
            Vector3.up);
    }
}