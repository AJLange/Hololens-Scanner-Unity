using System;
using System.Net;
using System.IO;
using UnityEngine;
using System.Collections;
using SocketIO;
using Debug = UnityEngine.Debug;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;




public class WWWFormImage : MonoBehaviour
{
    private TestSocketIO _testSocketScript;
    private SocketIOComponent _socket;

    //public AuthenticationHeaderValue Authorization { get; set; }

    private void OnMouseDown()
    {
        // Get sockets script
        GameObject go = GameObject.Find("TestSocketObj");
        _testSocketScript = go.GetComponent<TestSocketIO>();
        _socket = _testSocketScript.socket;

        ServicePointManager.ServerCertificateValidationCallback = MyRemoteCertificateValidationCallback;


        string url = "https://IoTBioband.azure-devices.net/devices/myFirstNodeDevice/messages/events?api-version=2016-02-03";

        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
        request.Headers.Add("Authorization", "SharedAccessSignature sr=IoTBioband.azure-devices.net%2Fdevices%2FmyFirstNodeDevice&sig=91l2KDTlOuAvyZpp3r6VRX5mTWip1OmKOtd8R7UD93k%3D&se=1482068965");
        request.ContentType = "application/atom+xml";
        request.Method = "POST";
        HttpWebResponse response = (HttpWebResponse)request.GetResponse();



            Stream resStream = response.GetResponseStream();
        Debug.Log(resStream);


        // StartCoroutine(UploadPNG());
        // StartCoroutine(UploadData());
    }



    public bool MyRemoteCertificateValidationCallback(System.Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
    {
        bool isOk = true;
        // If there are errors in the certificate chain, look at each error to determine the cause.
        if (sslPolicyErrors != SslPolicyErrors.None)
        {
            for (int i = 0; i < chain.ChainStatus.Length; i++)
            {
                if (chain.ChainStatus[i].Status != X509ChainStatusFlags.RevocationStatusUnknown)
                {
                    chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;
                    chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
                    chain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan(0, 1, 0);
                    chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllFlags;
                    bool chainIsValid = chain.Build((X509Certificate2)certificate);
                    if (!chainIsValid)
                    {
                        isOk = false;
                    }
                }
            }
        }
        return isOk;
    }


    IEnumerator UploadData()
    {

        yield return new WaitForEndOfFrame();

        //these are just blank values to send basic values over
        int temp = 50;
        int lit = 60;

        Debug.Log("Temp: " + temp + "Light: " + lit);

    }



    IEnumerator UploadPNG()
    {
        // We should only read the screen after all rendering is complete
        yield return new WaitForEndOfFrame();

        // Create a texture the size of the screen, RGB24 format
        int width  = Screen.width;
        int height = Screen.height;
        var tex    = new Texture2D(width, height, TextureFormat.RGB24, false);

        // Read screen contents into the texture
        tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        tex.Apply();

        // Encode texture into PNG & apply it to cube
        byte[] bytes                        = tex.EncodeToPNG();
        var cubeRender                      = GameObject.Find("Cube").GetComponent<Renderer>();
            cubeRender.material.mainTexture = tex;

        // Prefefix is necessary to draw image on canvas.
        // Convert || img -> Byte Array -> String -> JSON obj
        string prefix       = "data:image/png;base64,";
        string tempInBase64 = Convert.ToBase64String(bytes);
        string payload      = prefix + tempInBase64;
        var jsonPayload     = JSONObject.StringObject(payload);

        Debug.Log("jsonPayload:" + jsonPayload);
        _socket.Emit("msgFromUnity", jsonPayload);

        //TODO: May not need to destroy this....
        // Destroy(tex);        
    }



}
