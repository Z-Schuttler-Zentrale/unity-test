using System;
using System.Collections.Generic;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour, IDamageable
{
    [SerializeField] private CharacterController controller;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float jumpHeight = 1.5f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundDistance = 0.4f;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private Canvas inventoryCanvas;
    
    public List<Item> inventory = new List<Item>();

    public float maxHp = 100f;
    public float hp;
    
    // TODO: rename?
    public GameObject mainWeapon;
    public GameObject secondaryWeapon;
    public GameObject helmetSlot;
    public GameObject chestSlot;
    public GameObject pantSlot;
    public GameObject bootSlot;
    
    private Text _mainText;
    private Text _secondaryText;
    private Text _helmetText;
    private Text _chestText;
    private Text _pantText;
    private Text _bootsText;
    private Text _inventoryListText;
    
    private Vector3 velocity;
    private bool isGrounded;
    private float currentSpeed;
    private bool _isAlive = true;

    private void Awake()
    {
        hp = maxHp;
    }

    private void Start()
    {
        _mainText = inventoryCanvas.transform.Find("Main").GetComponent<Text>();
        _secondaryText = inventoryCanvas.transform.Find("Secondary").GetComponent<Text>();
        _helmetText = inventoryCanvas.transform.Find("Helmet").GetComponent<Text>();
        _chestText = inventoryCanvas.transform.Find("Chest").GetComponent<Text>();
        _pantText = inventoryCanvas.transform.Find("Pants").GetComponent<Text>();
        _bootsText = inventoryCanvas.transform.Find("Boots").GetComponent<Text>();
        _inventoryListText = inventoryCanvas.transform.Find("List").GetComponent<Text>();

        
        // zu testzwecken
        for (int i = 0; i < 30; i++)
        {
            inventory.Add(new Magazine());
        }
    }
    
    public void Damage(float amount)
    {
        if (!_isAlive)
        {
            Debug.Log("Player is already dead");
            return;
        }
        if (amount >= hp)
        {
            Debug.Log("Eliminated");
            hp = 0;
            _isAlive = false;
            return;
        }
        hp -= amount;
        Debug.Log($"Damaged Player by {amount}");
    }

    void Update()
    {
        UpdateInventoryDisplay();
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        // Wenn auf dem boden
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
            currentSpeed = speed;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        
        Vector3 moveVector = transform.right * x + transform.forward * z;
        
        controller.Move(moveVector * (currentSpeed * Time.deltaTime));
        
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
        }
        
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("Aufhebbar"))
        {
            int itemID = hit.gameObject.GetInstanceID();
            bool alreadyAdded = inventory.Exists(item => item.GetInstanceID() == itemID);

            if (!alreadyAdded)
            {
                inventory.Add(hit.gameObject.GetComponent<Item>());
                hit.gameObject.SetActive(false);
            }
        }
    }

    private void UpdateInventoryDisplay()
    {
        _mainText.text = $"Main: {(mainWeapon != null ? mainWeapon.name : "")}";
        _secondaryText.text = $"Secondary: {(secondaryWeapon != null ? secondaryWeapon.name : "")}";
        _helmetText.text = $"Helmet: {(helmetSlot != null ? helmetSlot.name : "")}";
        _chestText.text = $"Chest: {(chestSlot != null ? chestSlot.name : "")}";
        _pantText.text = $"Pants: {(pantSlot != null ? pantSlot.name : "")}";
        _bootsText.text = $"Boots: {(bootSlot != null ? bootSlot.name : "")}";

        _inventoryListText.text = ""; // Clear the text first
        foreach (Item item in inventory)
        {
            _inventoryListText.text += $"{item.name}\n";
        }
    }
}