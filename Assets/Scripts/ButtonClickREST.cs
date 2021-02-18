using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Net;
using System;
using System.IO;
using UnityEngine.Events;
using UnityEngine.Networking;

// sources: 
// https://stackoverflow.com/questions/36239705/serialize-and-deserialize-json-and-json-array-in-unity
// https://stackoverflow.com/questions/45229260/unity-json-de-serializing-nested-data

public class ButtonClickREST : MonoBehaviour
{
    // Public variables
    public GameObject houseBase;
    public VillageInfo[] village;

    // Private variables
    Vector3 coordinates = new Vector3(0,0,0);

    // Update is called once per frame
    public void OnMouseUp(string text)
    {
        Text txt = transform.Find("Text").GetComponent<Text>();
        txt.text = text;
        Debug.Log("aasdad");
        print("click0");
    }

    public void PopulateHouses()
    {
        // Querry JSON data
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(String.Format("https://jsonplaceholder.typicode.com/users"));
        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        StreamReader reader = new StreamReader(response.GetResponseStream());
        string jsonResponse = JsonHelper.fixJson(reader.ReadToEnd());
               
        // Deserialize data to lists
        village = JsonHelper.FromJson<VillageInfo>(jsonResponse);

        GameObject[] houseDescription = GameObject.FindGameObjectsWithTag("HouseTag");

        // Prevent additional calls
        if (houseDescription.Length == 0)
        { 
            foreach (VillageInfo house in village)
            {
                coordinates = houseBase.transform.position;
                coordinates.x = float.Parse(house.address.geo.lat);
                coordinates.z = float.Parse(house.address.geo.lng);
            
                GameObject newHouse = Instantiate(houseBase, coordinates, Quaternion.identity) as GameObject;

                // Ta klica delata v Imediate windowu, v kodi pa ne
                //Text houseName = newHouse.Find("TextName").GetComponent<Text>() as Text;
                //houseName.text = house.name;
                // Dirty!
                Text houseName = newHouse.transform.GetChild(1).GetChild(0).GetComponent<Text>();
                houseName.text = house.name;
            }
        }
    }

    public static class JsonHelper
    {
        public static string fixJson(string value)
        {
            value = "{\"Items\":" + value + "}";
            return value;
        }

        public static T[] FromJson<T>(string json)
        {
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
            return wrapper.Items;
        }

        public static string ToJson<T>(T[] array)
        {
            Wrapper<T> wrapper = new Wrapper<T>();
            wrapper.Items = array;
            return JsonUtility.ToJson(wrapper);
        }

        public static string ToJson<T>(T[] array, bool prettyPrint)
        {
            Wrapper<T> wrapper = new Wrapper<T>();
            wrapper.Items = array;
            return JsonUtility.ToJson(wrapper, prettyPrint);
        }

        [Serializable]
        private class Wrapper<T>
        {
            public T[] Items;
        }
    }
}

// Kopiraj Json vsebino -> Edit -> Paste special -> Paste JSON as classes
//public class Rootobject
//{
//    public VillageInfo[] Property1 { get; set; }
//}

[Serializable]
public class VillageInfo
{
    public int id;
    public string name;
    public string username;
    public string email;
    public Address address;
    public string phone;
    public string website;
    public Company company;
}

[Serializable]
public class Address
{
    public string street;
    public string suite;
    public string city;
    public string zipcode;
    public Geo geo;
}

[Serializable]
public class Geo
{
    public string lat;
    public string lng;
}

[Serializable]
public class Company
{
    public string name;
    public string catchPhrase;
    public string bs;
}
