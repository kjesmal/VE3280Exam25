
using UnityEngine;
using TMPro;
using System;

public class popupSensor : MonoBehaviour
{

    public GameObject Popup;

    public GameObject TMP_sensor_name;
    public GameObject TMP_sensor_value;

    TextMeshProUGUI sensorNameText;
    TextMeshProUGUI sensorValueText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Popup.SetActive(false);
        sensorNameText = TMP_sensor_name.GetComponent<TextMeshProUGUI>();
        sensorValueText = TMP_sensor_value.GetComponent<TextMeshProUGUI>();
    }

    async void OnTriggerEnter(Collider other)
    {
        if (Popup == null || sensorNameText == null || sensorValueText == null)
        {
            Debug.LogError("UI references not assigned.");
            return;
        }

        // Wait for MySQLManagement to be ready (up to 5 seconds)
        float waitTime = 0f;
        while (MySQLManagement.Instance == null && waitTime < 5f)
        {
            await System.Threading.Tasks.Task.Delay(100);
            waitTime += 0.1f;
        }

        if (MySQLManagement.Instance == null)
        {
            Debug.LogError("MySQLManagement instance not found after waiting.");
            sensorValueText.text = "N/A";
            return;
        }
        
        Popup.SetActive(true);
        sensorNameText.text = await MySQLManagement.Instance.GetSensorTypeAsync(name);
        sensorValueText.text = await MySQLManagement.Instance.GetSensorValueAsync(name);
    }

    void OnTriggerExit(Collider other)
    {
        Popup.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }
}