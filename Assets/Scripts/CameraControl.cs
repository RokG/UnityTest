using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;


public class CameraControl : MonoBehaviour
{
    // Public variables
    public Transform target;
    public GameObject DatabaseObject;

    // Private variables
    private ButtonClickREST databaseScript;

    // Translation speed
    public int translationSpeed = 40;
    // Rotation speed
    public float rotationSpeed = 1.5f;
    // Dead zone for mouse rotations
    int deadzone = 1;
    // Speed limit for rotations
    int speedlimit = 5;
    // Limit movement area
    int maxPosition = 100;
    // timer for mouseover
    float timer = 0;
    float timeLimit = 5;

    // Placeholders
    float drX, drY = 0f;
    Vector3 localY = new Vector3(0, 0, 0);
    Vector3 currentRotation = new Vector3(0, 0, 0);
    Vector3 currentPosition = new Vector3(0, 0, 0);

    // Update is called once per frame
    void FixedUpdate()
    {
        // Controller definitions
        var kbd = Keyboard.current;
        var mouse = Mouse.current;

        // If left arrow key is pressed
        if (kbd.leftArrowKey.isPressed)
        {
            // move camera left
            transform.Translate(Vector3.left * Time.deltaTime * translationSpeed);
        }

        // If right arrow key is pressed
        if (kbd.rightArrowKey.isPressed)
        {
            // move camera right
            transform.Translate(Vector3.right * Time.deltaTime * translationSpeed);
        }

        // If up arrow key is presed
        if (kbd.upArrowKey.isPressed)
        {
            // Get camera forward direction without "UP" component
            localY = transform.forward;
            localY.y = 0;
            // Move camera forward
            transform.Translate(localY * Time.deltaTime * translationSpeed, Space.World);
        }

        // if down arrow key is pressed
        if (kbd.downArrowKey.isPressed)
        {
            // Get forward direction without "UP" component
            localY = transform.forward;
            localY.y = 0;
            // Move camera backward
            transform.Translate(-localY * Time.deltaTime * translationSpeed, Space.World);
        }

        // Limit camera position
        if (kbd.anyKey.isPressed)
        {
            transform.position = new Vector3(Mathf.Clamp(transform.position.x, -maxPosition, maxPosition), transform.position.y, Mathf.Clamp(transform.position.z, -maxPosition, maxPosition));
        }


        // Do rotations if RMB is pressed
        if (mouse.rightButton.isPressed)
        {
            // read mouse movement
            drX = Mathf.Clamp(mouse.delta.x.ReadValue(), -speedlimit, speedlimit);
            drY = Mathf.Clamp(mouse.delta.y.ReadValue(), -speedlimit, speedlimit);

            // If mouse is moved verticaly
            if (Mathf.Abs(drY) >= deadzone)
            {
                // Do this in world space to prevent tilts
                transform.Rotate(-drY * rotationSpeed, 0, 0);
                // Limit rotations
                currentRotation = transform.localRotation.eulerAngles;
                if (currentRotation.x < 180)
                {
                    currentRotation.x = Mathf.Clamp(currentRotation.x, 0, 45);
                }
                else
                {
                    currentRotation.x = Mathf.Clamp(currentRotation.x, 355, 360);
                }
                transform.localRotation = Quaternion.Euler(currentRotation);
            }

            // If mouse is moved horizontaly
            if (Mathf.Abs(drX) >= deadzone)
            {
                // Do this in world space to prevent tilts
                transform.Rotate(0, drX * rotationSpeed, 0, Space.World);
                // No limit for Y axis rotation
            }
        }


        // Make all canvases look at camera
        GameObject[] houseDescription = GameObject.FindGameObjectsWithTag("HouseCanvas");
        foreach (GameObject tag in houseDescription)
        {
            tag.transform.LookAt(target);
        }

        // On click add description
        if (mouse.leftButton.isPressed)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(mouse.position.ReadValue());

            if (Physics.Raycast(ray, out hit))
            {
                // If house is clicked
                if (hit.transform.tag == "HouseTag")
                {
                    // Probably not the best way to add descriptions...
                    Text houseDescr = hit.transform.GetChild(1).GetChild(1).GetComponent<Text>();
                    Text houseName = hit.transform.GetChild(1).GetChild(0).GetComponent<Text>();

                    // Only add description if none exists
                    if (houseDescr.text == "")
                    {
                        // Acces database
                        databaseScript = DatabaseObject.GetComponent<ButtonClickREST>();
                        foreach (VillageInfo house in databaseScript.village)
                        {
                            if (house.name.Equals(houseName.text))
                            {
                                houseDescr.text = house.company.catchPhrase;
                            }
                        }

                    }
                }
            }
        }

        // If mouse is still
        if (mouse.delta.x.ReadValue() == 0 && mouse.delta.y.ReadValue() == 0)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(mouse.position.ReadValue());

            if (Physics.Raycast(ray, out hit))
            {
                // If its hovering over a house
                if (hit.transform.tag == "HouseTag")
                {
                    timer += Time.deltaTime;
                    if (timer >= timeLimit)
                    {
                        // Probably not the best way to add descriptions...
                        Text houseDescr = hit.transform.GetChild(1).GetChild(1).GetComponent<Text>();
                        Text houseName = hit.transform.GetChild(1).GetChild(0).GetComponent<Text>();

                        // Only add description if none exists
                        if (houseDescr.text == "")
                        {
                            // Acces database
                            databaseScript = DatabaseObject.GetComponent<ButtonClickREST>();
                            foreach (VillageInfo house in databaseScript.village)
                            {
                                if (house.name.Equals(houseName.text))
                                {
                                    houseDescr.text = house.company.catchPhrase;
                                }
                            }

                        }
                    }

                }
            }
        }
        else
        {
            timer = 0;
        }

    }
}
