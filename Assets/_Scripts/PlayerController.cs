using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.IO;
using TMPro;

public class PlayerController : MonoBehaviour
{
    public float speed;
    public TextMeshProUGUI countText, errorText; 
    public GameObject instructionTextObject,instructionPanel, winTextObject, deathByDamageObject, deathByMonsterObject;
    private Rigidbody rb;
    private int count, error;
    private bool cubeOnGround;
    private float movementX, movementY;
    private StreamWriter sw;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        speed = 2;
        count = 0;
        error = 0;
        cubeOnGround = true;

        SetCountText();

        instructionTextObject.SetActive(true);
        instructionPanel.SetActive(true);
        winTextObject.SetActive(false);
        deathByDamageObject.SetActive(false);
        deathByMonsterObject.SetActive(false);

        // If output data file exists, clear the previous data in the file.
        if(File.Exists("OutputData.txt"))
        {
            File.Delete("OutputData.txt");
        }
    }

    void OnMove(InputValue movementValue)
    {
        Vector2 movementVector = movementValue.Get<Vector2>();

        movementX = movementVector.x;
        movementY = movementVector.y; 
    }

    void SetCountText()
    {
        countText.text = "Power: " + count.ToString();
        errorText.text = "Damage: " + error.ToString();
    }

    void ReadFile()
    {
        StreamReader read = new StreamReader("OutputData.txt");
        string allLines = read.ReadToEnd();

        if(allLines.Contains("Damage count: 4"))
        {
            deathByDamageObject.SetActive(true);
            Debug.Log("You died of self-damage!");   
        }
        else if(allLines.Contains("Power count: 20") && allLines.Contains("You encountered a monster!"))
        {
            winTextObject.SetActive(true);
            Debug.Log("You win!");
        }
        else if(allLines.Contains("You encountered a monster!"))
        {
            deathByMonsterObject.SetActive(true);
            Debug.Log("The monster ate you!");
        }
        read.Close();
    }

    void FixedUpdate()
    {
        Vector3 movement = new Vector3(movementX, 0.0f, movementY);
        rb.AddForce(movement * speed);

        if(Input.GetKey(KeyCode.Space) && cubeOnGround)
        {
            Vector3 jump = new Vector3(0.0f, 300.0f, 0.0f);
            rb.AddForce(jump);
            cubeOnGround = false;
        }
        else if(Input.GetKey(KeyCode.Return))
        {
            instructionTextObject.SetActive(false);
            instructionPanel.SetActive(false);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Ground"))
        {
            cubeOnGround = true;
        }
    }
    void OnTriggerEnter(Collider other)
    {
        sw = new StreamWriter("OutputData.txt", true);
        
        if(other.gameObject.CompareTag("PickUp")) 
        {
            other.gameObject.SetActive(false);
            count++;
            sw.Write("Power count: " + count.ToString() + "\n");
            sw.Flush();
        }
        else if(other.gameObject.CompareTag("PickUpError"))
        {
            other.gameObject.SetActive(false);
            error++;
            sw.Write("Damage count: " + error.ToString() + "\n");
            sw.Flush();
        }
        else if(other.gameObject.CompareTag("PickUpFinal"))
        {
            other.gameObject.SetActive(false);
            sw.Write("You encountered a monster!" + "\n");
            sw.Flush();
        }

        sw.Close();
        SetCountText();
        ReadFile();     
    }

}
