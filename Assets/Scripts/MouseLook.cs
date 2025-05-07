using UnityEngine;
using UnityEngine.Serialization;

public class MouseLook : MonoBehaviour
{
    [SerializeField] private float mouseSensitivity = 500f;
    [SerializeField] private Transform playerBody;
    [SerializeField] private GameObject rightHand;
    private float xRotation;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        Quaternion cameraRotation = Quaternion.Euler(xRotation, 0f, 0f);
        Quaternion weaponRotation = Quaternion.Euler(-xRotation, -180f, 0f);
        
        transform.localRotation = cameraRotation;
        rightHand.transform.localRotation = weaponRotation;
        
        playerBody.Rotate(Vector3.up * mouseX);
    }
}
