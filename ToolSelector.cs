using TMPro;
using UnityEngine;

public class ToolSelector : MonoBehaviour
{

    private bool toolWasSelectedOnce = false;
    private GameObject selectedTool;
    public Camera mainCamera;
    private CameraController cameraControllerScript;
    private ToolInitialTransform selectedToolInitial;

    public GameObject warningButton;
    public TMP_Text warningText;
    public bool isPaused = false;

    public float moveSpeed = 2f;
    public float rotationSpeed = 100f;
    public Vector3 frontOffset = new Vector3(0, 1.5f, -3f); // önden bakış
    public Vector3 backOffset = new Vector3(0, 1.5f, -3f);   // arkadan bakış
    public float cameraFollowSpeed = 5f;

    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private Rigidbody selectedRigidbody;

    private bool rearView = false;
    float pitch = 0f;
    float yaw = 0f;

    void Start()
    {

        if (warningButton != null)
            warningButton.SetActive(false);

        if (mainCamera != null)
            cameraControllerScript = mainCamera.GetComponent<CameraController>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // Alet seçme
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit) && hit.collider.CompareTag("Tool"))
            {
                selectedTool = hit.collider.gameObject;
                Debug.Log("Seçildi: " + selectedTool.name);

                selectedRigidbody = selectedTool.GetComponent<Rigidbody>();

                // Sadece ilk kez seçiliyorsa pozisyon kaydet
                if (!toolWasSelectedOnce)
                {
                    initialPosition = selectedTool.transform.position;
                    initialRotation = selectedTool.transform.rotation;
                    toolWasSelectedOnce = true;
                }

                if (cameraControllerScript != null)
                    cameraControllerScript.enabled = false;
            }
        }

        // ESC ile aleti bırak ve geri döndür
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (selectedTool != null)
            {
                if (selectedRigidbody != null)
                {
                    selectedRigidbody.linearVelocity = Vector3.zero;
                    selectedRigidbody.angularVelocity = Vector3.zero;
                }

                selectedTool.transform.position = initialPosition;
                selectedTool.transform.rotation = initialRotation;

                selectedTool = null;
                selectedRigidbody = null;
                toolWasSelectedOnce = false; // <- burası önemli!

                if (cameraControllerScript != null)
                    cameraControllerScript.enabled = true;
            }
        }

        // Görüş açısını değiştir
        if (Input.GetKeyDown(KeyCode.R))
        {
            rearView = !rearView;
        }

        // Aleti kontrol et
        if (selectedTool != null)
        {
            // Klavye ile pozisyon
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");
            float y = 0f;

            if (Input.GetKey(KeyCode.E)) y = 1f;
            if (Input.GetKey(KeyCode.Q)) y = -1f;

            Vector3 moveDir = new Vector3(h, y, v);
            selectedTool.transform.Translate(moveDir * moveSpeed * Time.deltaTime, Space.Self);

            // Mouse ile dönüş
            float mouseX = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;

            yaw += mouseX;
            pitch -= mouseY;
            pitch %= 360f;

            Quaternion targetRotation = Quaternion.Euler(pitch, yaw, 0f);
            selectedTool.transform.rotation = Quaternion.Slerp(selectedTool.transform.rotation, targetRotation, Time.deltaTime * 5f);

            // Kamera takip
            Vector3 offset = rearView ? backOffset : frontOffset;
            Vector3 targetPosition = selectedTool.transform.position + selectedTool.transform.TransformDirection(offset);
            mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, targetPosition, Time.deltaTime * cameraFollowSpeed);
            mainCamera.transform.LookAt(selectedTool.transform);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Tooth"))
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.isKinematic = true;
            }
        }
    }
    public void ResumeAfterWarning()
    {
        if (warningButton != null)
            warningButton.SetActive(false);

        isPaused = false;
        Time.timeScale = 1f;
    }
    public void ResetSelectedToolTransform()
    {
        if (selectedTool != null)
        {
            selectedTool.transform.position = initialPosition;
            selectedTool.transform.rotation = initialRotation;
        }
    }


    public void ReturnToolToInitialState()
    {
        if (selectedTool != null)
        {
            selectedTool.transform.position = initialPosition;
            selectedTool.transform.rotation = initialRotation;

            if (selectedRigidbody != null)
            {
                selectedRigidbody.linearVelocity = Vector3.zero;
                selectedRigidbody.angularVelocity = Vector3.zero;
            }

            selectedTool = null;
            selectedRigidbody = null;

            if (cameraControllerScript != null)
                cameraControllerScript.enabled = true;
        }
    }
    public void DeselectToolWithoutReset()
    {
        if (selectedTool != null)
        {
            if (selectedRigidbody != null)
            {
                selectedRigidbody.linearVelocity = Vector3.zero;
                selectedRigidbody.angularVelocity = Vector3.zero;
            }

            selectedTool = null;
            selectedRigidbody = null;

            if (cameraControllerScript != null)
                cameraControllerScript.enabled = true;
        }
    }
}