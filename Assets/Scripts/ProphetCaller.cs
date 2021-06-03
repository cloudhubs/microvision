using System.Collections;
using System.Collections.Generic;
using System.Net;
using System;
using System.IO;
using UnityEngine;
using Assets.Scripts.model;
using TMPro;

public class ProphetCaller : MonoBehaviour
{
    public TextMeshProUGUI testText;

    // Start is called before the first frame update
    void Start()
    {
        ProphetData data = CallProphet();
        for (int i = 0; i < data.communication.nodes.Count; i++)
        {
            // create the cube (node), and a child holder for the node label
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            GameObject textHolder = new GameObject();
            textHolder.transform.parent = cube.transform;
            TextMeshPro text = textHolder.AddComponent<TextMeshPro>();

            // position the cube
            cube.transform.position = new Vector3(0 + 0.5f * i, 0f, 1.5f);
            cube.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);

            // position the label holder
            textHolder.transform.localPosition = new Vector3(0, 1f, 0);
            //text.transform.position = text.transform.parent.forward * 0.5f;
            //text.transform.rotation.localEulerAngles.y = 180;
            //text.transform.Translate(new Vector3(0, 0.5f, 0));

            // set up the text
            text.SetText(data.communication.nodes[i].label);
            text.fontSize = 100;
            float scaler = getTextScale(0.25f);
            text.transform.localScale = new Vector3(scaler, scaler, scaler);
        }
    }

    private float getTextScale(float parentScale)
    {
        return 0.005f / parentScale;
    }

    private MsNode test()
    {
        string json = "{\"id\": null,\"label\": \"cms\",\"shape\": \"box\"}";
        MsNode node = JsonUtility.FromJson<MsNode>(json);
        return node;
    }

    private ProphetData CallProphet()
    {
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(String.Format("http://demo1986600.mockable.io/"));
        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        StreamReader reader = new StreamReader(response.GetResponseStream());
        string jsonResponse = reader.ReadToEnd();
        ProphetData info = JsonUtility.FromJson<ProphetData>(jsonResponse);
        return info;
    }
}
